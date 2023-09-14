
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Common;

namespace VisionProcessing
{
    public class UnitPresent
    {
        #region Struct

        class BlobObject
        {
            // Object ID
            public int intNoID;          
            public bool blnSelected;

            // Blob Data
            public int intObjNo = -1;
            public int intArea;
            public float fStartX;
            public float fStartY;
            public float fEndX;
            public float fEndY;
            public float fLimitCenterX;
            public float fLimitCenterY;
            public float fWidth;
            public float fHeight;

            public ROI objUnitROI = new ROI();

            // Settings
            public float fMinArea;
            public float fMinOffSet;

            // Matching
            public int intMatchSampleIndex = -1;
            public float fDistanceToSampleObjects;
        }

        #endregion

        #region Member Variables

        private bool m_blnWhiteOnBlack = false;
        private bool m_bFinalResult = false;
        private bool m_blnFailExtraObjects = false;
        private bool m_blnFollowFirstROISize = false;
        private bool m_blnAdjustBasedOnCornerROI = false;
        private int m_intThresholdValue = 127;
        private int m_intUnitPresentFailMask = -1;
        private int m_intUnitOffSetFailMask = -1;
        private int m_intUnitROICountX = 0;
        private int m_intUnitROICountY = 0;
        private int m_intDefineUnitMethod = 0;  // 0=Use Unit ROI, 1= Use Blob object   (Currently fix it to method 0)
        private float m_fHalfPitch = 10;
        private float m_fFilterMinArea = 10;
        private float m_fFilterMaxArea = 250000;
        private float m_fFirstObjectOffSetX = 0;
        private float m_fFirstObjectOffSetY = 0;
        private string m_strErrorMessage = "";
        private List<int> m_arrSampleArea = new List<int>();
        private List<BlobObject> m_arrTemporaryObjects = new List<BlobObject>();
        private List<BlobObject> m_arrTemplateObjects = new List<BlobObject>();
        private List<BlobObject> m_arrSampleObjects = new List<BlobObject>();

        private EBlobs m_objEBlobs = new EBlobs();

        Line objUpLine = new Line();
        Line objLeftLine = new Line();
        Line objRightLine = new Line();
        Line objBottomLine = new Line();

        #endregion

        #region Properties

        public bool ref_blnWhiteOnBlack { get { return m_blnWhiteOnBlack; } set { m_blnWhiteOnBlack = value; } }
        public bool ref_bFinalResult { get { return m_bFinalResult; } set { m_bFinalResult = value; } }
        public bool ref_blnFollowFirstROISize { get { return m_blnFollowFirstROISize; } set { m_blnFollowFirstROISize = value; } }
        public bool ref_blnAdjustBasedOnCornerROI { get { return m_blnAdjustBasedOnCornerROI; } set { m_blnAdjustBasedOnCornerROI = value; } }
        public int ref_intThresholdValue { get { return m_intThresholdValue; } set { m_intThresholdValue = value; } }
        public int ref_intTemplateObjectsCount { get { return m_arrTemplateObjects.Count; }}
        public int ref_intUnitPresentFailMask { get { return m_intUnitPresentFailMask; } set { m_intUnitPresentFailMask = value; } }
        public int ref_intUnitOffSetFailMask { get { return m_intUnitOffSetFailMask; } set { m_intUnitOffSetFailMask = value; } }
        public int ref_intUnitROICountX { get { return m_intUnitROICountX; } set { m_intUnitROICountX = value; } }
        public int ref_intUnitROICountY { get { return m_intUnitROICountY; } set { m_intUnitROICountY = value; } }
        public int ref_intDefineUnitMethod { get { return m_intDefineUnitMethod; } set { m_intDefineUnitMethod = value; } }
        public float ref_fFilterMinArea { get { return m_fFilterMinArea; } set { m_fFilterMinArea = value; } }
        public float ref_fFilterMaxArea { get { return m_fFilterMaxArea; } set { m_fFilterMaxArea = value; } }

        public string ref_strErrorMessage { get { return m_strErrorMessage; } set { m_strErrorMessage = value; } }

        #endregion

        public UnitPresent()
        {

        }

        public int BuildObjects(ROI objROI, bool blnRemoveBorder)
        {
            //if (m_blnWhiteOnBlack)
            //    m_objBlobs.SetClassSelection(2);
            //else
            //    m_objBlobs.SetClassSelection(1);

            //m_objBlobs.ref_intThreshold = m_intThresholdValue;
            //m_objBlobs.SetConnexity(4);
            //m_objBlobs.SetObjectAreaRange(Convert.ToInt32(Math.Floor(m_fFilterMinArea)), Convert.ToInt32(Math.Floor(m_fFilterMaxArea)));
            //m_objBlobs.ref_intFeature = 0x0F;
            //m_objBlobs.BuildObjects(objROI, blnRemoveBorder);  // True = remove border

            ////objROI.SaveImage("D:\\TS\\ObjROI.bmp");
            //return m_objBlobs.ref_intNumSelectedObject;


            m_objEBlobs.BuildObjects_Filter_GetElement(objROI, !m_blnWhiteOnBlack, true, 0, m_intThresholdValue,
                Convert.ToInt32(Math.Floor(m_fFilterMinArea)), Convert.ToInt32(Math.Floor(m_fFilterMaxArea)), blnRemoveBorder, 0x0F);

            return m_objEBlobs.ref_intNumSelectedObject;
        }

        public int BuildObjectsUnitROI(ROI objROI, bool blnRemoveBorder)
        {
            //if (m_blnWhiteOnBlack)
            //    m_objBlobs.SetClassSelection(2);
            //else
            //    m_objBlobs.SetClassSelection(1);

            //m_objBlobs.ref_intThreshold = m_intThresholdValue;
            //m_objBlobs.SetConnexity(4);
            //m_objBlobs.SetObjectAreaRange(Convert.ToInt32(Math.Floor(m_fFilterMinArea)), objROI.ref_ROIWidth * objROI.ref_ROIHeight);
            //m_objBlobs.ref_intFeature = 0x0F;
            //m_objBlobs.BuildObjects(objROI, blnRemoveBorder);  // True = remove border

            ////objROI.SaveImage("D:\\TS\\ObjROI.bmp");
            //return m_objBlobs.ref_intNumSelectedObject;

            m_objEBlobs.BuildObjects_Filter_GetElement(objROI, !m_blnWhiteOnBlack, true, 0, m_intThresholdValue,
                Convert.ToInt32(Math.Floor(m_fFilterMinArea)), objROI.ref_ROIWidth * objROI.ref_ROIHeight, blnRemoveBorder, 0x0F);

            return m_objEBlobs.ref_intNumSelectedObject;
        }

        private void BlobObjectCopyTo(BlobObject objSource, ref BlobObject objDestination)
        {
            objDestination.intNoID = objSource.intNoID;
            objDestination.blnSelected = objSource.blnSelected;

            objDestination.intObjNo = objSource.intObjNo;
            objDestination.intArea = objSource.intArea;
            objDestination.fStartX = objSource.fStartX;
            objDestination.fStartY = objSource.fStartY;
            objDestination.fEndX = objSource.fEndX;
            objDestination.fEndY = objSource.fEndY;
            objDestination.fLimitCenterX = objSource.fLimitCenterX;
            objDestination.fLimitCenterY = objSource.fLimitCenterY;
            objDestination.fWidth = objSource.fWidth;
            objDestination.fHeight = objSource.fHeight;
            
            objDestination.fMinArea = objSource.fMinArea;
            objDestination.fMinOffSet = objSource.fMinOffSet;

            objDestination.intMatchSampleIndex = objSource.intMatchSampleIndex;
            objDestination.fDistanceToSampleObjects = objSource.fDistanceToSampleObjects;
        }

        public bool CheckArea()
        {
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                if (m_arrTemplateObjects[i].intMatchSampleIndex < 0)
                    continue;

                if (m_arrSampleObjects[m_arrTemplateObjects[i].intMatchSampleIndex].intArea > m_arrTemplateObjects[i].fMinArea)
                {
                    m_intUnitPresentFailMask = i + 1;
                    m_strErrorMessage += "*Unit " + (i + 1).ToString() + " still present!";
                    return false;
                }
            }

            m_intUnitPresentFailMask = 0;
            return true;
        }

        public bool CheckExtraObjects()
        {
            for (int i = 0; i < m_arrSampleObjects.Count; i++)
            {
                if (m_arrSampleObjects[i].intObjNo < 0)
                {
                    m_strErrorMessage += "*Extra objects Exist!";
                    m_blnFailExtraObjects = true;
                    return false;
                }
            }

            return true;
        }

        public bool CheckOffSet()
        {
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                if (m_arrTemplateObjects[i].fDistanceToSampleObjects > m_arrTemplateObjects[i].fMinOffSet)
                {
                    m_intUnitOffSetFailMask = i + 1;
                    m_strErrorMessage += "*Unit " + (i + 1).ToString() + " off set fail!";
                    return false;
                }
            }

            m_intUnitOffSetFailMask = 0;
            return true;
        }

        public void CollectSampleOjects()
        {
            m_arrSampleObjects.Clear();

            int intNumSelectedObject = m_objEBlobs.ref_intNumSelectedObject;

            //m_objBlobs.SetFirstListBlobs();
            float fCenterX = 0, fCenterY = 0, fWidth = 0, fHeight = 0;
            int intArea = 0;
            float fShortestDistance = float.MaxValue;
            int intSelectedIndex = -1;
            for (int i = 0; i < intNumSelectedObject; i++)
            {
                //m_objBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                //m_objBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                //m_objBlobs.GetSelectedListBlobsWidth(ref fWidth);
                //m_objBlobs.GetSelectedListBlobsHeight(ref fHeight);
                //m_objBlobs.GetSelectedListBlobsArea(ref intArea);

                fCenterX = m_objEBlobs.ref_arrLimitCenterX[i];
                fCenterY = m_objEBlobs.ref_arrLimitCenterY[i];
                fWidth = m_objEBlobs.ref_arrWidth[i];
                fHeight = m_objEBlobs.ref_arrHeight[i];
                intArea = m_objEBlobs.ref_arrArea[i];

                float fStartX = fCenterX - fWidth / 2;
                float fStartY = fCenterY - fHeight / 2;
                float fEndX = fCenterX + fWidth / 2;
                float fEndY = fCenterY + fHeight / 2;

                 m_arrTemporaryObjects.Add(new BlobObject());
                m_arrTemporaryObjects[i].blnSelected = true;
                m_arrTemporaryObjects[i].fLimitCenterX = fCenterX;
                m_arrTemporaryObjects[i].fLimitCenterY = fCenterY;
                m_arrTemporaryObjects[i].fWidth = fWidth;
                m_arrTemporaryObjects[i].fHeight = fHeight;
                m_arrTemporaryObjects[i].fStartX = fStartX;
                m_arrTemporaryObjects[i].fStartY = fStartY;
                m_arrTemporaryObjects[i].fEndX = fEndX;
                m_arrTemporaryObjects[i].fEndY = fEndY;

                float fDistance = (float)Math.Pow(Math.Pow(fCenterX, 2) + Math.Pow(fCenterY, 2), 0.5);

                if (fDistance < fShortestDistance)
                {
                    fShortestDistance = fDistance;
                    intSelectedIndex = i;
                }

                //m_objBlobs.SetListBlobsToNext();
            }

        }

        public void CopyTemplateUnitROIToTemporaryUnitROI(ROI objSearchROI)
        {
            m_arrTemporaryObjects.Clear();
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                m_arrTemporaryObjects.Add(new BlobObject());
                m_arrTemporaryObjects[i].blnSelected = m_arrTemplateObjects[i].blnSelected;
                m_arrTemporaryObjects[i].fLimitCenterX = m_arrTemplateObjects[i].fLimitCenterX;
                m_arrTemporaryObjects[i].fLimitCenterY = m_arrTemplateObjects[i].fLimitCenterY;
                m_arrTemporaryObjects[i].fWidth = m_arrTemplateObjects[i].fWidth;
                m_arrTemporaryObjects[i].fHeight = m_arrTemplateObjects[i].fHeight;
                m_arrTemporaryObjects[i].fStartX = m_arrTemplateObjects[i].fStartX;
                m_arrTemporaryObjects[i].fStartY = m_arrTemplateObjects[i].fStartY;
                m_arrTemporaryObjects[i].fEndX = m_arrTemplateObjects[i].fEndX;
                m_arrTemporaryObjects[i].fEndY = m_arrTemplateObjects[i].fEndY;
                m_arrTemporaryObjects[i].fMinArea = m_arrTemplateObjects[i].fMinArea;
                m_arrTemporaryObjects[i].fMinOffSet = m_arrTemplateObjects[i].fMinOffSet;

                m_arrTemporaryObjects[i].objUnitROI.AttachImage(objSearchROI);
                m_arrTemplateObjects[i].objUnitROI.CopyToNew(ref m_arrTemporaryObjects[i].objUnitROI);
            }
        }

        public void CopyTemporaryUnitROIToTemporaryUnitROI(ROI objSearchROI)
        {
            m_arrTemplateObjects.Clear();
            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                m_arrTemplateObjects.Add(new BlobObject());
                m_arrTemplateObjects[i].blnSelected = m_arrTemporaryObjects[i].blnSelected;
                m_arrTemplateObjects[i].fLimitCenterX = m_arrTemporaryObjects[i].fLimitCenterX;
                m_arrTemplateObjects[i].fLimitCenterY = m_arrTemporaryObjects[i].fLimitCenterY;
                m_arrTemplateObjects[i].fWidth = m_arrTemporaryObjects[i].fWidth;
                m_arrTemplateObjects[i].fHeight = m_arrTemporaryObjects[i].fHeight;
                m_arrTemplateObjects[i].fStartX = m_arrTemporaryObjects[i].fStartX;
                m_arrTemplateObjects[i].fStartY = m_arrTemporaryObjects[i].fStartY;
                m_arrTemplateObjects[i].fEndX = m_arrTemporaryObjects[i].fEndX;
                m_arrTemplateObjects[i].fEndY = m_arrTemporaryObjects[i].fEndY;
                m_arrTemplateObjects[i].fMinArea = m_arrTemporaryObjects[i].fMinArea;
                m_arrTemplateObjects[i].fMinOffSet = m_arrTemporaryObjects[i].fMinOffSet;

                m_arrTemplateObjects[i].objUnitROI.AttachImage(objSearchROI);
                m_arrTemporaryObjects[i].objUnitROI.CopyToNew(ref m_arrTemplateObjects[i].objUnitROI);
            }
        }

        public void CopySelectedTemporaryObjectsToTemplateObjects()
        {
            // Backup TemplateObjects previous settng before clear
            List<float> arrMinArea = new List<float>();
            List<float> arrMinOffSet = new List<float>();
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                arrMinArea.Add(m_arrTemplateObjects[i].fMinArea);
                arrMinOffSet.Add(m_arrTemplateObjects[i].fMinOffSet);
            }

            m_arrTemplateObjects.Clear();

            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                if (m_arrTemporaryObjects[i].blnSelected)
                {
                    int intTemplateIndex = m_arrTemplateObjects.Count;
                    m_arrTemplateObjects.Add(new BlobObject());
                    m_arrTemplateObjects[intTemplateIndex].blnSelected = m_arrTemporaryObjects[i].blnSelected;
                    m_arrTemplateObjects[intTemplateIndex].fLimitCenterX = m_arrTemporaryObjects[i].fLimitCenterX;
                    m_arrTemplateObjects[intTemplateIndex].fLimitCenterY = m_arrTemporaryObjects[i].fLimitCenterY;
                    m_arrTemplateObjects[intTemplateIndex].fWidth = m_arrTemporaryObjects[i].fWidth;
                    m_arrTemplateObjects[intTemplateIndex].fHeight = m_arrTemporaryObjects[i].fHeight;
                    m_arrTemplateObjects[intTemplateIndex].fStartX = m_arrTemporaryObjects[i].fStartX;
                    m_arrTemplateObjects[intTemplateIndex].fStartY = m_arrTemporaryObjects[i].fStartY;
                    m_arrTemplateObjects[intTemplateIndex].fEndX = m_arrTemporaryObjects[i].fEndX;
                    m_arrTemplateObjects[intTemplateIndex].fEndY = m_arrTemporaryObjects[i].fEndY;

                    if (intTemplateIndex < arrMinArea.Count)
                    {
                        m_arrTemplateObjects[intTemplateIndex].fMinArea = arrMinArea[i];
                        m_arrTemplateObjects[intTemplateIndex].fMinOffSet = arrMinOffSet[i];
                    }
                    else
                    {
                        // if new template objects count bigger then previous template objects count, then use last previous template object setting to new template objects
                        m_arrTemplateObjects[intTemplateIndex].fMinArea = arrMinArea[arrMinArea.Count - 1];
                        m_arrTemplateObjects[intTemplateIndex].fMinOffSet = arrMinOffSet[arrMinArea.Count - 1];
                    }
                }
            }
        }

        public void DefineFirstSampleObject()
        {
            /*
             * First sample object = object nearest to top left and == to template first object
             */

            float fShortestDistance = float.MaxValue;
            int intSelectedIndex = -1;
            for (int i = 0; i < m_arrSampleObjects.Count; i++)
            {
                float fDistance = (float)Math.Pow(Math.Pow(m_arrSampleObjects[i].fLimitCenterX, 2) + Math.Pow(m_arrSampleObjects[i].fLimitCenterY, 2), 0.5);

                if (fDistance < fShortestDistance)
                {
                    fShortestDistance = fDistance;
                    intSelectedIndex = i;
                }
            }

            if (intSelectedIndex >= 0)
            {
                m_arrSampleObjects[intSelectedIndex].intObjNo = 0;

                m_fFirstObjectOffSetX = m_arrSampleObjects[intSelectedIndex].fLimitCenterX - m_arrTemplateObjects[0].fLimitCenterX;
                m_fFirstObjectOffSetY = m_arrSampleObjects[intSelectedIndex].fLimitCenterY - m_arrTemplateObjects[0].fLimitCenterY;
            }
        }

        public bool DoInspection_BlobObjects(ROI objSampleROI)
        {
            // Build sample objects
            BuildObjects(objSampleROI, true);

            // Collect sample objects
            FillBlobsToSampleObjects();

            // Define first object
            DefineFirstSampleObject();

            // Match sample objects and template objects
            MatchSampleObjectsWithTemplateObjects();

            bool blnResult = true;
            // Check area
            if (!CheckArea())
                blnResult = false;

            // Check offset
            if (blnResult)
            {
                if (!CheckOffSet())
                    blnResult = false;
            }

            // Check Extra objects/Foreign Material/Contamination
            if (blnResult)
            {
                if (!CheckExtraObjects())
                    blnResult = false;
            }

            m_bFinalResult = true;

            return blnResult;
        }

        public bool DoInspection_UnitROI2(ROI objSampleROI)
        {
            // Define min area to build blobs
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                m_arrTemplateObjects[i].objUnitROI.AttachImage(objSampleROI);

                if (i == 0)
                {
                    m_fFilterMinArea = m_arrTemplateObjects[i].fMinArea / 2;
                    continue;
                }

                if (m_fFilterMinArea > (m_arrTemplateObjects[i].fMinArea / 2))
                    m_fFilterMinArea = m_arrTemplateObjects[i].fMinArea / 2;
            }

            // Build sample objects
            BuildObjectsUnitROI(objSampleROI, false);

            // Collect sample objects
            FillBlobsToSampleObjects();

            // Match sample objects and template objects
            MatchSampleObjectsWithTemplateUnitROI();

            bool blnResult = true;
            // Check area
            if (!CheckArea())
                blnResult = false;

            m_bFinalResult = true;

            return blnResult;
        }

        public bool DoInspection_UnitROI(ROI objSampleROI)
        {
            bool blnResult = true;

            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                m_arrTemplateObjects[i].objUnitROI.AttachImage(objSampleROI);

                if (BuildObjectsUnitROI(m_arrTemplateObjects[i].objUnitROI, false) == 0)
                {
                    m_arrSampleArea.Add(0);
                    // Empty Unit
                    continue;
                }
                else
                {
                    int intArea = 0;

                    //m_objBlobs.SetFirstListBlobs();
                    //m_objBlobs.GetSelectedListBlobsArea(ref intArea);

                    intArea = m_objEBlobs.ref_arrArea[i];

                    m_arrSampleArea.Add(intArea);

                    // Check area
                    if (intArea > m_arrTemplateObjects[i].fMinArea)
                    {
                        m_intUnitPresentFailMask = i + 1;
                        m_strErrorMessage += "*Unit " + (i + 1).ToString() + " still present!";
                        blnResult = false;
                        break;
                    }
                }
            }

            //// Check Extra objects/Foreign Material/Contamination
            //if (blnResult)
            //{
            //    if (!CheckExtraObjects())
            //        blnResult = false;
            //}

            m_bFinalResult = true;

            return blnResult;
        }

        public void DrawBlobsObjects(Graphics g, float fScaleX, float fScaleY)
        {
            m_objEBlobs.DrawSelectedBlobs(g, fScaleX, fScaleY);
        }

        public void DrawInspectionResult(Graphics g, float fScaleX, float fScaleY, int intROITotalX, int intROITotalY)
        {
            if (!m_bFinalResult)
                return;

            Font font = new Font("Verdana", 12, FontStyle.Bold);
            SolidBrush brushLime = new SolidBrush(Color.Lime);
            SolidBrush brushRed = new SolidBrush(Color.Red);

            for (int i = 0; i < m_arrSampleObjects.Count; i++)
            {
                if (m_arrSampleObjects[i].intObjNo >= 0)
                {
                    if (((m_intUnitPresentFailMask - 1) == m_arrSampleObjects[i].intObjNo) ||
                        ((m_intUnitOffSetFailMask - 1) == m_arrSampleObjects[i].intObjNo))
                    {
                        g.DrawString((m_arrSampleObjects[i].intObjNo + 1).ToString(), font, brushRed, (intROITotalX + m_arrSampleObjects[i].fLimitCenterX - m_arrSampleObjects[i].fWidth) * fScaleX,
                          (intROITotalY + m_arrSampleObjects[i].fLimitCenterY - m_arrSampleObjects[i].fHeight - 15) * fScaleY);

                    }
                    else
                    {
                        g.DrawString((m_arrSampleObjects[i].intObjNo + 1).ToString(), font, brushLime, (intROITotalX + m_arrSampleObjects[i].fLimitCenterX - m_arrSampleObjects[i].fWidth) * fScaleX,
                        (intROITotalY + m_arrSampleObjects[i].fLimitCenterY - m_arrSampleObjects[i].fHeight - 15) * fScaleY);

                    }
                }
            }
        }

        public void DrawExtraObjects(Graphics g, float fScaleX, float fScaleY, int intROITotalX, int intROITotalY)
        {
            if (!m_blnFailExtraObjects)
                return;

            Pen pen = new Pen(Color.Red, 2);

            for (int i = 0; i < m_arrSampleObjects.Count; i++)
            {
                if (m_arrSampleObjects[i].intObjNo < 0)
                {
                    g.DrawRectangle(pen, (intROITotalX + m_arrSampleObjects[i].fLimitCenterX - m_arrSampleObjects[i].fWidth) * fScaleX,
                                     (intROITotalY + m_arrSampleObjects[i].fLimitCenterY - m_arrSampleObjects[i].fHeight) * fScaleY,
                                     m_arrSampleObjects[i].fWidth * 2 * fScaleX, m_arrSampleObjects[i].fHeight * 2 * fScaleY);

                }
            }
        }

        public void DrawTemporaryBlobsObjects(Graphics g, float fScaleX, float fScaleY)
        {
            int intNumSelectedObjects = m_arrTemporaryObjects.Count;

            for (int i = 0; i < intNumSelectedObjects; i++)
            {
                if (m_arrTemporaryObjects[i].blnSelected)
                    m_objEBlobs.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Lime, i);
                else
                    m_objEBlobs.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red, i);
            }
        }

        public void DrawTemporaryUnitROI(Graphics g, float fScaleX, float fScaleY, ROI objSearchROI)
        {
            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                m_arrTemporaryObjects[i].objUnitROI.DrawROI(g, fScaleX, fScaleY, 
                    m_arrTemporaryObjects[i].objUnitROI.GetROIHandle(), 
                    (i+1).ToString(),
                    2, 
                    Color.Blue);
            }

            objUpLine.DrawLine(g, objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY, 640, 480, Color.Yellow, 2);
            objLeftLine.DrawLine(g, objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY, 640, 480, Color.Yellow, 2);
            objRightLine.DrawLine(g, objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY, 640, 480, Color.Yellow, 2);
            objBottomLine.DrawLine(g, objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY, 640, 480, Color.Yellow, 2);
        }

        public void DrawInspectionResultUnitROI(Graphics g, float fScaleX, float fScaleY)
        {
            if (!m_bFinalResult)
                return;

            try
            {
                for (int i = 0; i < m_arrTemplateObjects.Count; i++)
                {

                    m_arrTemplateObjects[i].objUnitROI.DrawROI(g, fScaleX, fScaleY,
                    m_arrTemplateObjects[i].objUnitROI.GetROIHandle(),
                    (i + 1).ToString(),
                    2,
                    Color.Lime);

                }

                int intNumSelectedObjects = m_objEBlobs.ref_intNumSelectedObject;

                for (int i = 0; i < intNumSelectedObjects; i++)
                {
                    if (m_arrSampleObjects[i].intObjNo >= 0)
                    {
                        if (m_arrSampleObjects[i].intArea > m_arrTemplateObjects[m_arrSampleObjects[i].intObjNo].fMinArea)
                        {
                            m_objEBlobs.DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red, i);
                            m_arrTemplateObjects[m_arrSampleObjects[i].intObjNo].objUnitROI.DrawROI(g, fScaleX, fScaleY,
                        m_arrTemplateObjects[m_arrSampleObjects[i].intObjNo].objUnitROI.GetROIHandle(),
                        (m_arrSampleObjects[i].intObjNo + 1).ToString(),
                        2,
                        Color.Red);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void DrawTemplateBlobsObjects(Graphics g, float fScaleX, float fScaleY, int intROITotalX, int intROITotalY)
        {
            Font font = new Font("Verdana", 12, FontStyle.Bold);
            Pen pen = new Pen(Color.Blue);
            SolidBrush brush = new SolidBrush(Color.Blue);
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                g.DrawRectangle(pen, (intROITotalX + m_arrTemplateObjects[i].fLimitCenterX - m_arrTemplateObjects[i].fWidth) * fScaleX,
                                                     (intROITotalY + m_arrTemplateObjects[i].fLimitCenterY - m_arrTemplateObjects[i].fHeight) * fScaleY,
                                                     m_arrTemplateObjects[i].fWidth * 2 * fScaleX, m_arrTemplateObjects[i].fHeight * 2 * fScaleY);

                g.DrawString((i + 1).ToString(), font, brush, (intROITotalX + m_arrTemplateObjects[i].fLimitCenterX - m_arrTemplateObjects[i].fWidth) * fScaleX,
                                                     (intROITotalY + m_arrTemplateObjects[i].fLimitCenterY - m_arrTemplateObjects[i].fHeight - 15) * fScaleY);
            }
        }

        public void FillBlobsToTemporaryObjects()
        {
            int intNumSelectedObjects = m_objEBlobs.ref_intNumSelectedObject;

            m_arrTemporaryObjects.Clear();

            //m_objBlobs.SetFirstListBlobs();
            float fCenterX = 0, fCenterY = 0, fWidth = 0, fHeight = 0;
            int intArea = 0;
            for (int i = 0; i < intNumSelectedObjects; i++)
            {
                //m_objBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                //m_objBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                //m_objBlobs.GetSelectedListBlobsWidth(ref fWidth);
                //m_objBlobs.GetSelectedListBlobsHeight(ref fHeight);
                //m_objBlobs.GetSelectedListBlobsArea(ref intArea);

                fCenterX = m_objEBlobs.ref_arrLimitCenterX[i];
                fCenterY = m_objEBlobs.ref_arrLimitCenterY[i];
                fWidth = m_objEBlobs.ref_arrWidth[i];
                fHeight = m_objEBlobs.ref_arrHeight[i];
                intArea = m_objEBlobs.ref_arrArea[i];

                float fStartX = fCenterX - fWidth / 2;
                float fStartY = fCenterY - fHeight / 2;
                float fEndX = fCenterX + fWidth / 2;
                float fEndY = fCenterY + fHeight / 2;

                m_arrTemporaryObjects.Add(new BlobObject());
                m_arrTemporaryObjects[i].blnSelected = true;
                m_arrTemporaryObjects[i].fLimitCenterX = fCenterX;
                m_arrTemporaryObjects[i].fLimitCenterY = fCenterY;
                m_arrTemporaryObjects[i].fWidth = fWidth;
                m_arrTemporaryObjects[i].fHeight = fHeight;
                m_arrTemporaryObjects[i].fStartX = fStartX;
                m_arrTemporaryObjects[i].fStartY = fStartY;
                m_arrTemporaryObjects[i].fEndX = fEndX;
                m_arrTemporaryObjects[i].fEndY = fEndY;

                //m_objBlobs.SetListBlobsToNext();
            }
        }

        public void FillBlobsToSampleObjects()
        {
            int intNumSelectedObjects = m_objEBlobs.ref_intNumSelectedObject;

            m_arrSampleObjects.Clear();

            //m_objBlobs.SetFirstListBlobs();
            float fCenterX = 0, fCenterY = 0, fWidth = 0, fHeight = 0;
            int intArea = 0;
            for (int i = 0; i < intNumSelectedObjects; i++)
            {
                //m_objBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                //m_objBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                //m_objBlobs.GetSelectedListBlobsWidth(ref fWidth);
                //m_objBlobs.GetSelectedListBlobsHeight(ref fHeight);
                //m_objBlobs.GetSelectedListBlobsArea(ref intArea);

                fCenterX = m_objEBlobs.ref_arrLimitCenterX[i];
                fCenterY = m_objEBlobs.ref_arrLimitCenterY[i];
                fWidth = m_objEBlobs.ref_arrWidth[i];
                fHeight = m_objEBlobs.ref_arrHeight[i];
                intArea = m_objEBlobs.ref_arrArea[i];

                float fStartX = fCenterX - fWidth / 2;
                float fStartY = fCenterY - fHeight / 2;
                float fEndX = fCenterX + fWidth / 2;
                float fEndY = fCenterY + fHeight / 2;

                m_arrSampleObjects.Add(new BlobObject());
                m_arrSampleObjects[i].blnSelected = true;
                m_arrSampleObjects[i].fLimitCenterX = fCenterX;
                m_arrSampleObjects[i].fLimitCenterY = fCenterY;
                m_arrSampleObjects[i].fWidth = fWidth;
                m_arrSampleObjects[i].fHeight = fHeight;
                m_arrSampleObjects[i].fStartX = fStartX;
                m_arrSampleObjects[i].fStartY = fStartY;
                m_arrSampleObjects[i].fEndX = fEndX;
                m_arrSampleObjects[i].fEndY = fEndY;
                m_arrSampleObjects[i].intArea = intArea;

                //m_objBlobs.SetListBlobsToNext();
            }
        }

        public float GetTemplateObjectMinArea(int intTemplateObjectIndex)
        {
            return m_arrTemplateObjects[intTemplateObjectIndex].fMinArea;
        }

        public float GetTemplateObjectMinOffSet(int intTemplateObjectIndex)
        {
            return m_arrTemplateObjects[intTemplateObjectIndex].fMinOffSet;
        }

        public int GetSampleObjectArea(int intTemplateMatchIndex)
        {
            if (intTemplateMatchIndex >= m_arrTemplateObjects.Count)
                return 0;

            if (m_arrTemplateObjects[intTemplateMatchIndex].intMatchSampleIndex < 0)
                return 0;

            return m_arrSampleObjects[m_arrTemplateObjects[intTemplateMatchIndex].intMatchSampleIndex].intArea;
        }

        public int GetSampleUnitROIArea(int intUnitROIIndex)
        {
            if (intUnitROIIndex < m_arrSampleArea.Count)
                return m_arrSampleArea[intUnitROIIndex];
            else
                return 0;
        }

        public float GetSampleObjectOffSet(int intTemplateMatchIndex)
        {
            return m_arrTemplateObjects[intTemplateMatchIndex].fDistanceToSampleObjects;
        }

        public bool IsUnitPresent(int intTemplateMatchIndex)
        {
            if (intTemplateMatchIndex >= m_arrTemplateObjects.Count)
                return false;

            if (m_arrTemplateObjects[intTemplateMatchIndex].intMatchSampleIndex < 0)
                return false;

            if (m_arrSampleObjects[m_arrTemplateObjects[intTemplateMatchIndex].intMatchSampleIndex].intArea >= m_arrTemplateObjects[intTemplateMatchIndex].fMinArea)
                return true;    // Unit Present if sample area >= than template min area
            else
                return false;   // Empty Unit if sample area smaller than template min area
        }

        public int IsUnitEmpty(int intUnitROIIndex)
        {
            if (intUnitROIIndex < m_arrSampleArea.Count)
            {
                if (m_arrSampleArea[intUnitROIIndex] > m_arrTemplateObjects[intUnitROIIndex].fMinArea)
                    return 0;
                else
                    return 1;
            }
            else
                return -1;  // No result
        }

        public bool IsUnitWithinOffSetLimit(int intTemplateMatchIndex)
        {
            if (m_arrTemplateObjects[intTemplateMatchIndex].intMatchSampleIndex < 0)
                return false;

            if (m_arrTemplateObjects[intTemplateMatchIndex].fDistanceToSampleObjects <= m_arrTemplateObjects[intTemplateMatchIndex].fMinOffSet)
                return true;    
            else
                return false;   
        }

        public int GetTotalSelectedTemporaryObjects()
        {
            int intCount = 0;

            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                if (m_arrTemporaryObjects[i].blnSelected)
                    intCount++;
            }

            return intCount;
        }

        public void LoadUnitPresent(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_blnWhiteOnBlack = objFile.GetValueAsBoolean("WhiteOnBlack", false, 1);
            m_intThresholdValue = objFile.GetValueAsInt("ThresholdValue", 127, 1);
            m_fFilterMinArea = objFile.GetValueAsFloat("FilterMinArea", 10, 1);
            m_fFilterMaxArea = objFile.GetValueAsFloat("FilterMaxArea", 250000, 1);
            m_fHalfPitch = objFile.GetValueAsFloat("HalfPitch", 10, 1);
            m_intUnitROICountX = objFile.GetValueAsInt("UnitROICountX", 0, 1);
            m_intUnitROICountY = objFile.GetValueAsInt("UnitROICountY", 0, 1);

            int intTotalBlobObjects = objFile.GetValueAsInt("TotalTemplateBlobObject", 0, 1);
            m_arrTemplateObjects.Clear();
            for (int i = 0; i < intTotalBlobObjects; i++)
            {
                objFile.GetSecondSection("BlobObject" + i);

                m_arrTemplateObjects.Add(new BlobObject());
                m_arrTemplateObjects[i].blnSelected = true;
                m_arrTemplateObjects[i].fLimitCenterX = objFile.GetValueAsFloat("LimitCenterX", 0, 2);
                m_arrTemplateObjects[i].fLimitCenterY = objFile.GetValueAsFloat("LimitCenterY", 0, 2);
                m_arrTemplateObjects[i].fWidth = objFile.GetValueAsFloat("Width", 0, 2);
                m_arrTemplateObjects[i].fHeight = objFile.GetValueAsFloat("Height", 0, 2);
                m_arrTemplateObjects[i].intArea = objFile.GetValueAsInt("Area", 0, 2);
                m_arrTemplateObjects[i].fStartX = objFile.GetValueAsFloat("StartX", 0, 2);
                m_arrTemplateObjects[i].fStartX = objFile.GetValueAsFloat("StartY", 0, 2);
                m_arrTemplateObjects[i].fEndX = objFile.GetValueAsFloat("EndX", 0, 2);
                m_arrTemplateObjects[i].fEndY = objFile.GetValueAsFloat("EndY", 0, 2);
                m_arrTemplateObjects[i].fMinArea = objFile.GetValueAsFloat("MinArea", 0, 2);
                m_arrTemplateObjects[i].fMinOffSet = objFile.GetValueAsFloat("MinOffSet", 0, 2);

                m_arrTemplateObjects[i].objUnitROI.LoadROISetting(objFile.GetValueAsInt("ROIStartX", 0, 2), 
                    objFile.GetValueAsInt("ROIStartY", 0, 2),
                    objFile.GetValueAsInt("ROIWidth", 10, 2),
                    objFile.GetValueAsInt("ROIHeight", 10, 2));


            }
        }

        private void MatchSampleObjectsWithTemplateObjects()
        {
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                float fShortestDistance = float.MaxValue;
                int intSelectedIndex = -1;

                for (int j = 0; j < m_arrSampleObjects.Count; j++)
                {
                    float fDistanceX = Math.Abs(m_arrSampleObjects[j].fLimitCenterX - m_arrTemplateObjects[i].fLimitCenterX - m_fFirstObjectOffSetX);
                    float fDistanceY = Math.Abs(m_arrSampleObjects[j].fLimitCenterY - m_arrTemplateObjects[i].fLimitCenterY - m_fFirstObjectOffSetY);
                    float fDistance = (float)Math.Pow(Math.Pow(fDistanceX, 2) + Math.Pow(fDistanceY, 2), 0.5);

                    if ((fDistance < m_fHalfPitch) && (fDistance < fShortestDistance))
                    {
                        fShortestDistance = fDistance;
                        intSelectedIndex = j;
                    }
                }

                if (intSelectedIndex >= 0)
                {
                    m_arrTemplateObjects[i].intMatchSampleIndex = intSelectedIndex;
                    m_arrTemplateObjects[i].fDistanceToSampleObjects = fShortestDistance;
                    m_arrSampleObjects[intSelectedIndex].intObjNo = i;
                }
            }
        }

        private void MatchSampleObjectsWithTemplateUnitROI()
        {
            for (int i = 0; i < m_arrSampleObjects.Count; i++)
            {
                float fShortestDistance = float.MaxValue;
                int intSelectedIndex = -1;

                for (int j = 0; j < m_arrTemplateObjects.Count; j++)
                {
                    // blob size bigger than unit roi size
                    if (m_arrSampleObjects[i].intArea > m_arrTemplateObjects[j].objUnitROI.ref_ROIWidth * m_arrTemplateObjects[j].objUnitROI.ref_ROIHeight)
                    {
                        //check is unit roi center point in blob area
                        if ((m_arrTemplateObjects[j].objUnitROI.ref_ROICenterX > m_arrSampleObjects[i].fStartX) &&
                            (m_arrTemplateObjects[j].objUnitROI.ref_ROICenterX < m_arrSampleObjects[i].fEndX) &&
                            (m_arrTemplateObjects[j].objUnitROI.ref_ROICenterY > m_arrSampleObjects[i].fStartY) &&
                            (m_arrTemplateObjects[j].objUnitROI.ref_ROICenterY < m_arrSampleObjects[i].fEndY))
                        {
                            intSelectedIndex = j;
                            fShortestDistance = 0;
                            break;

                        }
                    }

                    {
                        float fDistanceX = Math.Abs(m_arrSampleObjects[i].fLimitCenterX - m_arrTemplateObjects[j].objUnitROI.ref_ROICenterX);
                        float fDistanceY = Math.Abs(m_arrSampleObjects[i].fLimitCenterY - m_arrTemplateObjects[j].objUnitROI.ref_ROICenterY);
                        float fDistance = (float)Math.Pow(Math.Pow(fDistanceX, 2) + Math.Pow(fDistanceY, 2), 0.5);

                        if (m_arrTemplateObjects[j].intMatchSampleIndex < 0 && (fDistance < fShortestDistance))
                        {
                            fShortestDistance = fDistance;
                            intSelectedIndex = j;
                        }
                    }
                }

                if (intSelectedIndex >= 0)
                {
                    m_arrTemplateObjects[intSelectedIndex].intMatchSampleIndex = i;
                    m_arrTemplateObjects[intSelectedIndex].fDistanceToSampleObjects = fShortestDistance;
                    m_arrSampleObjects[i].intObjNo = intSelectedIndex;
                }
            }
        }      

        public void ResetInspectionResult()
        {
            m_blnFailExtraObjects = false;
            if (m_intDefineUnitMethod == 0)
            {
                m_intUnitPresentFailMask = 0;
                m_intUnitOffSetFailMask = 0;
            }
            else
            {
                m_intUnitPresentFailMask = -1;
                m_intUnitOffSetFailMask = -1;
            }
            m_strErrorMessage = "";

            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                m_arrTemplateObjects[i].intMatchSampleIndex = -1;
                m_arrTemplateObjects[i].fDistanceToSampleObjects = 0;
            }

            m_arrSampleObjects.Clear();
            m_arrSampleArea.Clear();
        }

        public void SetTemplateObjectMinArea(int intTemplateObjectIndex, float fMinArea)
        {
            m_arrTemplateObjects[intTemplateObjectIndex].fMinArea = fMinArea;
        }

        public void SetTemplateObjectMinOffSet(int intTemplateObjectIndex, float fMinOffSet)
        {
            m_arrTemplateObjects[intTemplateObjectIndex].fMinOffSet = fMinOffSet;
        }

        public void SelectTemporaryObjects(System.Drawing.Point p1, System.Drawing.Point p2, int intROITotalX, int intROITotalY)
        {
            p1 = new System.Drawing.Point(p1.X - intROITotalX, p1.Y - intROITotalY);
            p2 = new System.Drawing.Point(p2.X - intROITotalX, p2.Y - intROITotalY);

            List<int> arrSelectedIndex = new List<int>();
            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                bool blnFound = false;
                for (int x = p1.X; x < p2.X; x++)
                {
                    for (int y = p1.Y; y < p2.Y; y++)
                    {
                        if ((x >= m_arrTemporaryObjects[i].fStartX) &&
                               (x <= m_arrTemporaryObjects[i].fEndX) &&
                               (y >= m_arrTemporaryObjects[i].fStartY) &&
                               (y <= m_arrTemporaryObjects[i].fEndY))
                        {
                            blnFound = true;
                            break;
                        }
                    }

                    if (blnFound)
                        break;
                }

                if (blnFound)
                    arrSelectedIndex.Add(i);
            }

            for (int i = 0; i < arrSelectedIndex.Count; i++)
            {
                bool blnOverlap = false;
                for (int j = 0; j < arrSelectedIndex.Count; j++)
                {
                    if (i == j)
                        continue;

                    BlobObject objBlob1 = m_arrTemporaryObjects[i];
                    BlobObject objBlob2 = m_arrTemporaryObjects[j];

                    // check is blob overlap with other blobs (using 4 corner points matching methodology)?
                    if ((((objBlob1.fStartX > objBlob2.fStartX) &&
                        (objBlob1.fStartX < objBlob2.fEndX) &&
                        (objBlob1.fStartY > objBlob2.fStartY) &&
                        (objBlob1.fStartY < objBlob2.fEndY)) ||
                        ((objBlob1.fStartX > objBlob2.fStartX) &&
                        (objBlob1.fStartX < objBlob2.fEndX) &&
                        (objBlob1.fEndY > objBlob2.fStartY) &&
                        (objBlob1.fEndY < objBlob2.fEndY)) ||
                        ((objBlob1.fEndX > objBlob2.fStartX) &&
                        (objBlob1.fEndX < objBlob2.fEndX) &&
                        (objBlob1.fStartY > objBlob2.fStartY) &&
                        (objBlob1.fStartY < objBlob2.fEndY)) ||
                        ((objBlob1.fEndX > objBlob2.fStartX) &&
                        (objBlob1.fEndX < objBlob2.fEndX) &&
                        (objBlob1.fEndY > objBlob2.fStartY) &&
                        (objBlob1.fEndY < objBlob2.fEndY))) ||

                        (((objBlob2.fStartX > objBlob1.fStartX) &&
                        (objBlob2.fStartX < objBlob1.fEndX) &&
                        (objBlob2.fStartY > objBlob1.fStartY) &&
                        (objBlob2.fStartY < objBlob1.fEndY)) ||
                        ((objBlob2.fStartX > objBlob1.fStartX) &&
                        (objBlob2.fStartX < objBlob1.fEndX) &&
                        (objBlob2.fEndY > objBlob1.fStartY) &&
                        (objBlob2.fEndY < objBlob1.fEndY)) ||
                        ((objBlob2.fEndX > objBlob1.fStartX) &&
                        (objBlob2.fEndX < objBlob1.fEndX) &&
                        (objBlob2.fStartY > objBlob1.fStartY) &&
                        (objBlob2.fStartY < objBlob1.fEndY)) ||
                        ((objBlob2.fEndX > objBlob1.fStartX) &&
                        (objBlob2.fEndX < objBlob1.fEndX) &&
                        (objBlob2.fEndY > objBlob1.fStartY) &&
                        (objBlob2.fEndY < objBlob1.fEndY))))
                    {
                        // check is blob1 size smaller than blob2
                        if (((objBlob1.fEndX - objBlob1.fStartX) * (objBlob1.fEndY - objBlob1.fStartY)) >
                            ((objBlob2.fEndX - objBlob2.fStartX) * (objBlob2.fEndY - objBlob2.fStartY)))
                        {
                            blnOverlap = true;  // no select
                            break;
                        }
                    }
                }

                if (!blnOverlap)
                {
                    m_arrTemporaryObjects[arrSelectedIndex[i]].blnSelected = !m_arrTemporaryObjects[arrSelectedIndex[i]].blnSelected;
                }
            }
        }

        public void SetTemporaryBlobsToUnselected()
        {
            int intNumSelectedObjects = m_arrTemporaryObjects.Count;

            for (int i = 0; i < intNumSelectedObjects; i++)
            {
                m_arrTemporaryObjects[i].blnSelected = false;
            }
        }

        public void SetTemporaryUnitROI(int intUnitROICountX, int intUnitROICountY, ROI objSearchROI, ImageDrawing objImage)
        {
            if (intUnitROICountX == 0 || intUnitROICountY == 0)
                return;

            if ((intUnitROICountX * intUnitROICountY) == m_arrTemplateObjects.Count)
            {
                for (int i = 0; i < m_arrTemplateObjects.Count; i++)
                {
                    m_arrTemplateObjects[i].objUnitROI.AttachImage(objSearchROI);
                }
                // copy template object to temporary object
                CopyTemplateUnitROIToTemporaryUnitROI(objSearchROI);
            }
            else
            {
                int intUnitROIWidth = objSearchROI.ref_ROIWidth / intUnitROICountX;
                int intUnitROIHeight  = objSearchROI.ref_ROIHeight / intUnitROICountY;
                
                m_arrTemporaryObjects.Clear();
                int i = 0;
                for (int y = 0; y < intUnitROICountY; y++)
                {
                    for (int x = 0; x < intUnitROICountX; x++)
                    {
                        m_arrTemporaryObjects.Add(new BlobObject());
                        m_arrTemporaryObjects[i].objUnitROI.LoadROISetting(x * intUnitROIWidth, y* intUnitROIHeight, intUnitROIWidth, intUnitROIHeight);
                        i++;
                    }
                }
            }

            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                m_arrTemporaryObjects[i].objUnitROI.AttachImage(objSearchROI);
            }

            m_intUnitROICountX = intUnitROICountX;
            m_intUnitROICountY = intUnitROICountY;
        }

        public void DefineTemplateObjectsHalfPitch()
        {
            // Get smallest pitch x and y
            float fSmallestPitch = float.MaxValue;
            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                for (int j = 0; j < m_arrTemplateObjects.Count; j++)
                {
                    if (i == j)
                        continue;

                    // Get pitch between 2 objects
                    float fPitch = (float)Math.Pow(Math.Pow(m_arrTemplateObjects[i].fLimitCenterX - m_arrTemplateObjects[j].fLimitCenterX, 2) +
                        Math.Pow(m_arrTemplateObjects[i].fLimitCenterY - m_arrTemplateObjects[j].fLimitCenterY, 2), 0.5);
                    // compare an get smallest pitch
                    if (fSmallestPitch > fPitch)
                        fSmallestPitch = fPitch;
                }
            }

            // Get half pitch
            m_fHalfPitch = fSmallestPitch / 2;

        }

        public void SortObjectNumber()
        {    
            List<BlobObject> arrLocalObjects = new List<BlobObject>();

            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                int intInsertIndex = arrLocalObjects.Count;
                bool blnFoundRow = false;


                for (int j = 0; j < arrLocalObjects .Count; j++)
                {
                    if ((m_arrTemplateObjects[i].fLimitCenterY > (arrLocalObjects[j].fLimitCenterY - m_fHalfPitch)) &&
                        (m_arrTemplateObjects[i].fLimitCenterY < (arrLocalObjects[j].fLimitCenterY + m_fHalfPitch)))
                    {
                        if (!blnFoundRow)
                            blnFoundRow = true;

                        if (m_arrTemplateObjects[i].fLimitCenterX < arrLocalObjects[j].fLimitCenterX)
                        {
                            intInsertIndex = j;
                            break;
                        }


                    }
                    else if (blnFoundRow)
                    {
                        intInsertIndex = j;
                        break;
                    }
                    else
                    {
                        if (m_arrTemplateObjects[i].fLimitCenterY < arrLocalObjects[j].fLimitCenterY)
                        {
                            intInsertIndex = j;
                            break;
                        }
                    }
                }

                BlobObject objBlobObject = new BlobObject();
                BlobObjectCopyTo(m_arrTemplateObjects[i], ref objBlobObject);
                 arrLocalObjects.Insert(intInsertIndex, objBlobObject);
            }

            // Transfer back localObject sorted list to templateObject array
            m_arrTemplateObjects.Clear();
            for (int i = 0; i < arrLocalObjects.Count; i++)
            {
                m_arrTemplateObjects.Add(arrLocalObjects[i]);
            }

            arrLocalObjects.Clear();
        }

        public void SaveUnitPresent(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Rectangle gauge template measurement result
            objFile.WriteElement1Value("WhiteOnBlack", m_blnWhiteOnBlack);
            objFile.WriteElement1Value("ThresholdValue", m_intThresholdValue);
            objFile.WriteElement1Value("FilterMinArea", m_fFilterMinArea);
            objFile.WriteElement1Value("FilterMaxArea", m_fFilterMaxArea);
            objFile.WriteElement1Value("TotalTemplateBlobObject", m_arrTemplateObjects.Count);
            objFile.WriteElement1Value("HalfPitch", m_fHalfPitch);
            objFile.WriteElement1Value("UnitROICountX", m_intUnitROICountX);
            objFile.WriteElement1Value("UnitROICountY", m_intUnitROICountY);

            for (int i = 0; i < m_arrTemplateObjects.Count; i++)
            {
                objFile.WriteElement1Value("BlobObject" + i, "");
                objFile.WriteElement2Value("LimitCenterX", m_arrTemplateObjects[i].fLimitCenterX);
                objFile.WriteElement2Value("LimitCenterY", m_arrTemplateObjects[i].fLimitCenterY);
                objFile.WriteElement2Value("Width", m_arrTemplateObjects[i].fWidth);
                objFile.WriteElement2Value("Height", m_arrTemplateObjects[i].fHeight);
                objFile.WriteElement2Value("Area", m_arrTemplateObjects[i].intArea);
                objFile.WriteElement2Value("StartX", m_arrTemplateObjects[i].fStartX);
                objFile.WriteElement2Value("StartY", m_arrTemplateObjects[i].fStartY);
                objFile.WriteElement2Value("EndX", m_arrTemplateObjects[i].fEndX);
                objFile.WriteElement2Value("EndY", m_arrTemplateObjects[i].fEndY);
                objFile.WriteElement2Value("MinArea", m_arrTemplateObjects[i].fMinArea);
                objFile.WriteElement2Value("MinOffSet", m_arrTemplateObjects[i].fMinOffSet);
                objFile.WriteElement2Value("ROIStartX", m_arrTemplateObjects[i].objUnitROI.ref_ROIPositionX);
                objFile.WriteElement2Value("ROIStartY", m_arrTemplateObjects[i].objUnitROI.ref_ROIPositionY);
                objFile.WriteElement2Value("ROIWidth", m_arrTemplateObjects[i].objUnitROI.ref_ROIWidth);
                objFile.WriteElement2Value("ROIHeight", m_arrTemplateObjects[i].objUnitROI.ref_ROIHeight);
            }

            objFile.WriteEndElement();
        }

        public void SaveUnitPresent_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            // Rectangle gauge template measurement result
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WhiteOnBlack", m_blnWhiteOnBlack);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_ThresholdValue", m_intThresholdValue);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_FilterMinArea", m_fFilterMinArea);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_FilterMaxArea", m_fFilterMaxArea);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_TotalTemplateBlobObject", m_arrTemplateObjects.Count);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_HalfPitch", m_fHalfPitch);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_UnitROICountX", m_intUnitROICountX);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_UnitROICountY", m_intUnitROICountY);

            //for (int i = 0; i < 10; i++)
            //{
            //    if (m_arrTemplateObjects.Count > i)
            //    {
            //        //objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i, "");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_LimitCenterX", m_arrTemplateObjects[i].fLimitCenterX);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_LimitCenterY", m_arrTemplateObjects[i].fLimitCenterY);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_Width", m_arrTemplateObjects[i].fWidth);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_Height", m_arrTemplateObjects[i].fHeight);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_Area", m_arrTemplateObjects[i].intArea);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_StartX", m_arrTemplateObjects[i].fStartX);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_StartY", m_arrTemplateObjects[i].fStartY);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_EndX", m_arrTemplateObjects[i].fEndX);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_EndY", m_arrTemplateObjects[i].fEndY);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_MinArea", m_arrTemplateObjects[i].fMinArea);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_MinOffSet", m_arrTemplateObjects[i].fMinOffSet);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIStartX", m_arrTemplateObjects[i].objUnitROI.ref_ROIPositionX);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIStartY", m_arrTemplateObjects[i].objUnitROI.ref_ROIPositionY);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIWidth", m_arrTemplateObjects[i].objUnitROI.ref_ROIWidth);
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIHeight", m_arrTemplateObjects[i].objUnitROI.ref_ROIHeight);
            //    }
            //    else
            //    {
            //        //objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i, "");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_LimitCenterX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_LimitCenterY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_Width", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_Height", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_Area", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_StartX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_StartY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_EndX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_EndY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_MinArea", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_MinOffSet", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIStartX", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIStartY", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIWidth", "NA");
            //        objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BlobObject" + i + "_ROIHeight", "NA");
            //    }
            //}

            objFile.WriteEndElement();
        }

        public bool VerifyTemporaryUnitROI(int intPositionX, int intPositionY)
        {
            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                if (m_arrTemporaryObjects[i].objUnitROI.VerifyROIArea(intPositionX, intPositionY))
                {
                    return true;
                }
            }

            return false;
        }

        public bool DragTemporaryUnitROI(int nNewPositionX, int nNewPositionY)
        {
            bool blnFound = false;
            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                m_arrTemporaryObjects[i].objUnitROI.VerifyROIHandleShape(nNewPositionX, nNewPositionY);
                if (m_arrTemporaryObjects[i].objUnitROI.GetROIHandle())
                {
                    m_arrTemporaryObjects[i].objUnitROI.DragROI(nNewPositionX, nNewPositionY);

                    blnFound = true;
                }
            }

            if (blnFound)
            {
                if (m_blnFollowFirstROISize)
                {
                    if (m_arrTemporaryObjects[0].objUnitROI.GetROIHandle())
                    {
                        int intWidth = m_arrTemporaryObjects[0].objUnitROI.ref_ROIWidth;
                        int intHeight = m_arrTemporaryObjects[0].objUnitROI.ref_ROIHeight;

                        for (int i = 1; i < m_arrTemporaryObjects.Count; i++)
                        {
                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIWidth = intWidth;
                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIHeight = intHeight;
                        }

                    }
                }

                if (m_blnAdjustBasedOnCornerROI)
                {
                    int intIndexTL = 0;
                    int intIndexTR = m_intUnitROICountX - 1;
                    int intIndexBL = m_intUnitROICountX * (m_intUnitROICountY - 1);
                    int intIndexBR = m_intUnitROICountX * m_intUnitROICountY - 1;

                    if (m_arrTemporaryObjects[intIndexTL].objUnitROI.GetROIHandle() ||
                        m_arrTemporaryObjects[intIndexTR].objUnitROI.GetROIHandle() ||
                        m_arrTemporaryObjects[intIndexBL].objUnitROI.GetROIHandle() ||
                        m_arrTemporaryObjects[intIndexBR].objUnitROI.GetROIHandle())
                    {
                        objUpLine.CalculateStraightLine(
                            new PointF(m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterX,
                            m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterY),
                            new PointF(m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterX,
                            m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterY));

                        objLeftLine.CalculateStraightLine(
                           new PointF(m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterX,
                           m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterY),
                           new PointF(m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterX,
                           m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterY));

                        objRightLine.CalculateStraightLine(
                           new PointF(m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterX,
                           m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterY),
                           new PointF(m_arrTemporaryObjects[intIndexBR].objUnitROI.ref_ROICenterX,
                           m_arrTemporaryObjects[intIndexBR].objUnitROI.ref_ROICenterY));

                        objBottomLine.CalculateStraightLine(
                           new PointF(m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterX,
                           m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterY),
                           new PointF(m_arrTemporaryObjects[intIndexBR].objUnitROI.ref_ROICenterX,
                           m_arrTemporaryObjects[intIndexBR].objUnitROI.ref_ROICenterY));

                        int intUpWidth = (m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterX -
                                            m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterX) / (m_intUnitROICountX - 1);

                        int intDownWidth = (m_arrTemporaryObjects[intIndexBR].objUnitROI.ref_ROICenterX -
                                                                    m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterX) / (m_intUnitROICountX - 1);

                        int intLeftHeight = (m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterY -
                                            m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterY) / (m_intUnitROICountY - 1);

                        int intRightHeight = (m_arrTemporaryObjects[intIndexBR].objUnitROI.ref_ROICenterY -
                                            m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterY) / (m_intUnitROICountY - 1);

                        // Reposition for upper line of unit ROIs
                        for (int i = 1; i < m_intUnitROICountX - 1; i++)
                        {
                            int intX = m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterX + intUpWidth * i;
                            int intY = (int)Math.Round(objUpLine.GetPointY(intX), 0, MidpointRounding.AwayFromZero);

                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIPositionX = Math.Max(0, intX - m_arrTemporaryObjects[i].objUnitROI.ref_ROIWidth / 2);
                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIPositionY = Math.Max(0, intY - m_arrTemporaryObjects[i].objUnitROI.ref_ROIHeight / 2);
                        }

                        // 

                        // Reposition for left line of unit ROIs
                        for (int i = 1; i < m_intUnitROICountY - 1; i++)
                        {
                            int intY = m_arrTemporaryObjects[intIndexTL].objUnitROI.ref_ROICenterY + intLeftHeight * i;
                            int intX = (int)Math.Round(objLeftLine.GetPointX(intY), 0, MidpointRounding.AwayFromZero);

                            m_arrTemporaryObjects[i * m_intUnitROICountX].objUnitROI.ref_ROIPositionX = Math.Max(0, intX - m_arrTemporaryObjects[i].objUnitROI.ref_ROIWidth / 2);
                            m_arrTemporaryObjects[i * m_intUnitROICountX].objUnitROI.ref_ROIPositionY = Math.Max(0, intY - m_arrTemporaryObjects[i].objUnitROI.ref_ROIHeight / 2);
                        }

                        // Reposition for right line of unit ROIs
                        int intIndex = 1;
                        for (int i = intIndexTR + m_intUnitROICountX; i < intIndexBR; i = i + m_intUnitROICountX)
                        {
                            int intY = m_arrTemporaryObjects[intIndexTR].objUnitROI.ref_ROICenterY + intRightHeight * intIndex;
                            int intX = (int)Math.Round(objRightLine.GetPointX(intY), 0, MidpointRounding.AwayFromZero);

                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIPositionX = Math.Max(0, intX - m_arrTemporaryObjects[i].objUnitROI.ref_ROIWidth / 2);
                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIPositionY = Math.Max(0, intY - m_arrTemporaryObjects[i].objUnitROI.ref_ROIHeight / 2);
                            intIndex++;
                        }

                        // Reposition for bottom line of unit ROIs
                        intIndex = 1;
                        for (int i = intIndexBL + 1; i < intIndexBR; i++)
                        {
                            int intX = m_arrTemporaryObjects[intIndexBL].objUnitROI.ref_ROICenterX + intDownWidth * intIndex;
                            int intY = (int)Math.Round(objBottomLine.GetPointY(intX), 0, MidpointRounding.AwayFromZero);

                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIPositionX = Math.Max(0, intX - m_arrTemporaryObjects[i].objUnitROI.ref_ROIWidth / 2);
                            m_arrTemporaryObjects[i].objUnitROI.ref_ROIPositionY = Math.Max(0, intY - m_arrTemporaryObjects[i].objUnitROI.ref_ROIHeight / 2);
                            intIndex++;
                        }

                        // Reposition all center line of unit ROIs
                        int intObjIndex = m_intUnitROICountX + 1;
                        for (int y = 1; y < m_intUnitROICountY - 1; y++)
                        {
                            intIndex = 1;
                            intObjIndex = m_intUnitROICountX * y + 1;
                            Line objCurrentLine = new Line();
                            objCurrentLine.CalculateStraightLine(
                                                new PointF(m_arrTemporaryObjects[m_intUnitROICountX * y].objUnitROI.ref_ROICenterX,
                                               m_arrTemporaryObjects[m_intUnitROICountX * y].objUnitROI.ref_ROICenterY),
                                               new PointF(m_arrTemporaryObjects[m_intUnitROICountX * y + m_intUnitROICountX - 1].objUnitROI.ref_ROICenterX,
                                               m_arrTemporaryObjects[m_intUnitROICountX * y + m_intUnitROICountX - 1].objUnitROI.ref_ROICenterY));

                            int intWidth = (m_arrTemporaryObjects[m_intUnitROICountX * y + m_intUnitROICountX - 1].objUnitROI.ref_ROICenterX -
                                            m_arrTemporaryObjects[m_intUnitROICountX * y].objUnitROI.ref_ROICenterX) / (m_intUnitROICountX - 1);
                            for (int x = 1; x < m_intUnitROICountX - 1; x++)
                            {
                                int intX = m_arrTemporaryObjects[m_intUnitROICountX * y].objUnitROI.ref_ROICenterX + intWidth * intIndex;
                                int intY = (int)Math.Round(objCurrentLine.GetPointY(intX), 0, MidpointRounding.AwayFromZero);

                                m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIPositionX = Math.Max(0, intX - m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIWidth / 2);
                                m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIPositionY = Math.Max(0, intY - m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIHeight / 2);
                                intIndex++;
                                intObjIndex++;
                            }
                        }
                    }
                    else
                    {
                        bool blnBorderSelected = false; // Left Right border only
                        int intSelectedIndex = -1;
                        for (int i = intIndexTL + m_intUnitROICountX; i < intIndexBR; i = i + m_intUnitROICountX)
                        {
                            if (m_arrTemporaryObjects[i].objUnitROI.GetROIHandle())
                            {
                                intSelectedIndex = i;
                                blnBorderSelected = true;
                                break;
                            }

                            if (m_arrTemporaryObjects[i + m_intUnitROICountX - 1].objUnitROI.GetROIHandle())
                            {
                                intSelectedIndex = i;
                                blnBorderSelected = true;
                                break;
                            }
                        }

                        if (blnBorderSelected)
                        {
                            // Reposition all center line of unit ROIs
                            int intObjIndex = intSelectedIndex + 1;

                            Line objCurrentLine = new Line();
                            objCurrentLine.CalculateStraightLine(
                                                new PointF(m_arrTemporaryObjects[intSelectedIndex].objUnitROI.ref_ROICenterX,
                                               m_arrTemporaryObjects[intSelectedIndex].objUnitROI.ref_ROICenterY),
                                               new PointF(m_arrTemporaryObjects[intSelectedIndex + m_intUnitROICountX - 1].objUnitROI.ref_ROICenterX,
                                               m_arrTemporaryObjects[intSelectedIndex + m_intUnitROICountX - 1].objUnitROI.ref_ROICenterY));

                            int intWidth = (m_arrTemporaryObjects[intSelectedIndex + m_intUnitROICountX - 1].objUnitROI.ref_ROICenterX -
                                            m_arrTemporaryObjects[intSelectedIndex].objUnitROI.ref_ROICenterX) / (m_intUnitROICountX - 1);
                            for (int x = 1; x < m_intUnitROICountX - 1; x++)
                            {
                                int intX = m_arrTemporaryObjects[intSelectedIndex].objUnitROI.ref_ROICenterX + intWidth * x;
                                int intY = (int)Math.Round(objCurrentLine.GetPointY(intX), 0, MidpointRounding.AwayFromZero);

                                m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIPositionX = Math.Max(0, intX - m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIWidth / 2);
                                m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIPositionY = Math.Max(0, intY - m_arrTemporaryObjects[intObjIndex].objUnitROI.ref_ROIHeight / 2);
                                intObjIndex++;
                            }

                        }

                    }
                }
            }
            return blnFound;
        }

        public void ClearDragHandle()
        {
            for (int i = 0; i < m_arrTemporaryObjects.Count; i++)
            {
                m_arrTemporaryObjects[i].objUnitROI.ClearDragHandle();
            }
        }
    }
}
