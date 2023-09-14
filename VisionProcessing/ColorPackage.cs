using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;

namespace VisionProcessing
{
    public class ColorPackage
    {
        #region constant variables

        private const int m_intFeature = 7;
        private const int m_intConnexity = 8;

        #endregion

        #region Member Variables

        private int m_intStartPixelFromEdge = 1;
        private bool m_bCheckChipAtBorder = false;

        private int m_intFailMask = 0; // 1 = flip; 2= scratch; 3= burr; 4= oxidation; 5 = exposed copper; 6=chip; 7=Black Dot Scratch
        private int[] m_intCopperColor = new int[3];
        private int[] m_intCopperColorTolerance = new int[3];
        private int[] m_intOxidationColor = new int[3];
        private int[] m_intOxidationColorTolerance = new int[3];
        private int[] m_intFlipColor = new int[3];
        private int[] m_intFlipColorTolerance = new int[3];
        private string m_strErrorMessage = "";

        private List<string> m_arrCopThresName = new List<string>();
        private List<string> m_arrCopSystemColor = new List<string>();
        private List<int[]> m_arrCopperColor = new List<int[]>();
        private List<int[]> m_arrCopperColorTolerance = new List<int[]>();
        private List<int> m_arrCopMinArea = new List<int>();
        private int m_intSelectedCopThresIndex = 0;

        private int m_intTempAutoTHValue = 0;
        private int m_intThresholdTolerance = 0;

        // Extra Save blobs settings
        private int m_intExposedCopperMinArea = 0;
        private int m_intScratchMinArea = 0;
        private int m_intScratchThreshold = 0;
        
        
        private Blobs m_objCopperBlobs = new Blobs();
        private Blobs m_objOxidationBlobs = new Blobs();
        private Blobs m_objWOBBlobs = new Blobs();
        private Blobs m_objBurrWOBBlobs = new Blobs();
        private Blobs m_objCopperWOBBlobs = new Blobs();
        private Blobs m_objFlipWOBBlobs = new Blobs();
        private Blobs m_objChipBOWBlobs = new Blobs();
        private Blobs m_objBorderChipBOWBlobs = new Blobs();
        private Blobs m_objLatentScratchBlobs = new Blobs();

        private List<ROI> m_arrChipROI = new List<ROI>();

        private EC24 m_intFlipLowThreshold = new EC24();
        private EC24 m_intFlipHighThreshold = new EC24();
        private EColorLookup m_objColorLookup = new EColorLookup();
        private TrackLog m_objLog = new TrackLog();
        private ImageDrawing m_objExposeCopperImage = new ImageDrawing();
        #endregion

        #region Properties

        public bool ref_bCheckChipAtBorder { get { return m_bCheckChipAtBorder; } set { m_bCheckChipAtBorder = value; } }
        public int ref_intStartPixelFromEdge { set { m_intStartPixelFromEdge = value; } get { return m_intStartPixelFromEdge; } }
        public int ref_intTempAutoTHValue { get { return m_intTempAutoTHValue; } set { m_intTempAutoTHValue = value; } }
        public int ref_intThresholdTolerance { get { return ref_intThresholdTolerance; } set { ref_intThresholdTolerance = value; } }
        public int ref_intFailMask { get { return m_intFailMask; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }

        public int ref_intBurrMinArea { get { return m_objBurrWOBBlobs.ref_intMinArea; } set { m_objBurrWOBBlobs.ref_intMinArea = value; } }
        public int ref_intChipMinArea { get { return m_objChipBOWBlobs.ref_intMinArea; } set { m_objChipBOWBlobs.ref_intMinArea = value; } }
        public int ref_intBorderChipMinArea { get { return m_objBorderChipBOWBlobs.ref_intMinArea; } set { m_objBorderChipBOWBlobs.ref_intMinArea = value; } }
        public int ref_intCopperMinArea
        {
            get
            {
                return m_intExposedCopperMinArea;
                //return m_objCopperBlobs.ref_intMinArea; 
            }
            set
            {
                m_intExposedCopperMinArea = value;
                m_objCopperBlobs.ref_intMinArea = value;
                m_objCopperWOBBlobs.ref_intMinArea = value;
            }
        }
        public int ref_intOxidationMinArea { get { return m_objOxidationBlobs.ref_intMinArea; } set { m_objOxidationBlobs.ref_intMinArea = value; } }
        public int ref_intWOBMinArea
        {
            get { return m_intScratchMinArea; }
            set
            {
                m_intScratchMinArea = value;
                m_objWOBBlobs.ref_intMinArea = value;
            }
        }
        public int ref_intLatentScratchMinArea { get { return m_objLatentScratchBlobs.ref_intMinArea; } set { m_objLatentScratchBlobs.ref_intMinArea = value; } }

        public int ref_intBurrThreshold { get { return m_objBurrWOBBlobs.ref_intThreshold; } set { m_objBurrWOBBlobs.ref_intThreshold = value; } }
        public int ref_intWOBThreshold { get { return m_intScratchThreshold; } set { m_intScratchThreshold= value; } }
        //public int ref_intWOBThreshold { get { return m_objWOBBlobs.ref_intThreshold; } set { m_objWOBBlobs.ref_intThreshold = value; } }
        public int ref_intLatentScratchThreshold { get { return m_objLatentScratchBlobs.ref_intThreshold; } set { m_objLatentScratchBlobs.ref_intThreshold = value; } }
        public int ref_intChipThreshold
        {
            get { return m_objChipBOWBlobs.ref_intThreshold; }
            set
            {
                m_objChipBOWBlobs.ref_intThreshold = value; 
                m_objBorderChipBOWBlobs.ref_intThreshold = value;
            }
        }
        public int[] ref_intCopperColor { get { return m_intCopperColor; } set { m_intCopperColor = value; } }
        public int[] ref_intOxidationColor { get { return m_intOxidationColor; } set { m_intOxidationColor = value; } }
        public int[] ref_intFlipColor { get { return m_intFlipColor; } set { m_intFlipColor = value; } }

        public int[] ref_intCopperColorTolerance { get { return m_intCopperColorTolerance; } set { m_intCopperColorTolerance = value; } }
        public int[] ref_intOxidationColorTolerance { get { return m_intOxidationColorTolerance; } set { m_intOxidationColorTolerance = value; } }
        public int[] ref_intFlipColorTolerance { get { return m_intFlipColorTolerance; } set { m_intFlipColorTolerance = value; } }


        public List<string> ref_arrCopThresName { get { return m_arrCopThresName; } }
        public List<string> ref_arrCopSystemColor { get { return m_arrCopSystemColor; } }
        public List<int[]> ref_arrCopperColor { get { return m_arrCopperColor; } }
        public List<int[]> ref_arrCopperColorTolerance { get { return m_arrCopperColorTolerance; } }
        public List<int> ref_arrCopMinArea { get { return m_arrCopMinArea; } }


        #endregion



        public ColorPackage()
        {
            m_objColorLookup.ConvertFromRgb(EColorSystem.Lsh);

            m_objCopperBlobs.SetConnexity(m_intConnexity);
            m_objCopperBlobs.SetClassSelection(2);
            m_objCopperBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objCopperBlobs.SetObjectAreaRange(20, 9999999);
                       
            m_objOxidationBlobs.SetConnexity(m_intConnexity);
            m_objOxidationBlobs.SetClassSelection(2);
            m_objOxidationBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objOxidationBlobs.SetObjectAreaRange(20, 9999999);

            m_objWOBBlobs.SetConnexity(m_intConnexity);
            m_objWOBBlobs.SetClassSelection(2);
            m_objWOBBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objWOBBlobs.SetObjectAreaRange(20, 9999999);

            m_objCopperWOBBlobs.SetConnexity(m_intConnexity);
            m_objCopperWOBBlobs.ref_intThreshold = 240;
            m_objCopperWOBBlobs.SetClassSelection(2);
            m_objCopperWOBBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objCopperWOBBlobs.SetObjectAreaRange(20, 9999999);

            m_objFlipWOBBlobs.SetConnexity(m_intConnexity);
            m_objFlipWOBBlobs.ref_intThreshold = 200;
            m_objFlipWOBBlobs.SetClassSelection(2);
            m_objFlipWOBBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objFlipWOBBlobs.SetObjectAreaRange(200, 9999999);
            
            m_objBurrWOBBlobs.SetConnexity(m_intConnexity);
            m_objBurrWOBBlobs.SetClassSelection(2);
            m_objBurrWOBBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objBurrWOBBlobs.SetObjectAreaRange(20, 9999999);

            m_objChipBOWBlobs.SetConnexity(m_intConnexity);
            m_objChipBOWBlobs.SetClassSelection(1);
            m_objChipBOWBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objChipBOWBlobs.SetObjectAreaRange(20, 9999999);

            m_objBorderChipBOWBlobs.SetConnexity(m_intConnexity);
            m_objBorderChipBOWBlobs.SetClassSelection(1);
            m_objBorderChipBOWBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objBorderChipBOWBlobs.SetObjectAreaRange(20, 9999999);

            m_objLatentScratchBlobs.SetConnexity(m_intConnexity);   
            m_objLatentScratchBlobs.SetClassSelection(1);            // Black on white
            m_objLatentScratchBlobs.ref_intFeature = m_intFeature;   // area and object center
            m_objLatentScratchBlobs.SetObjectAreaRange(20, 9999999);            

            for (int i = 0; i < 8; i++)
            {
                m_arrChipROI.Add(new ROI());
                m_arrChipROI[i].LoadROISetting(0, 0, 10, 10);
            }
        }


        public void ResetColorThresholdData()
        {
            m_arrCopThresName.Clear();
            m_arrCopSystemColor.Clear();
            m_arrCopperColor.Clear();
            m_arrCopperColorTolerance.Clear();
            m_arrCopMinArea.Clear();
        }

        public bool AddColorThresholdData(string strCopThresName, string strCopSystemColor,
                                                        int intColorThreshold1, int intColorThreshold2, int intColorThreshold3,
                                                        int intColorThresholdTolerance1, int intColorThresholdTolerance2, int intColorThresholdTolerance3,
                                                        int intCopMinArea)
        {
            try
            {
                m_arrCopThresName.Add(strCopThresName);
                m_arrCopSystemColor.Add(strCopSystemColor);

                int intIndex = m_arrCopperColor.Count;
                m_arrCopperColor.Add(new int[3]);
                m_arrCopperColor[intIndex][0] = intColorThreshold1;
                m_arrCopperColor[intIndex][1] = intColorThreshold2;
                m_arrCopperColor[intIndex][2] = intColorThreshold3;

                m_arrCopperColorTolerance.Add(new int[3]);
                m_arrCopperColorTolerance[intIndex][0] = intColorThresholdTolerance1;
                m_arrCopperColorTolerance[intIndex][1] = intColorThresholdTolerance2;
                m_arrCopperColorTolerance[intIndex][2] = intColorThresholdTolerance3;

                m_arrCopMinArea.Add(intCopMinArea);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrColorImages"></param>
        /// <param name="arrImages"></param>
        /// <param name="objUnitROI"></param>
        /// <param name="objTrainROI"></param>
        /// <param name="objMonoUnitROI"></param>
        /// <param name="objMonoTrainROI"></param>
        /// <returns></returns>
        public bool DoInspection(List<CImageDrawing> arrColorImages, List<ImageDrawing> arrImages, CROI objColorUnitROI, CROI objColorTrainROI, ROI objMonoUnitROI, ROI objMonoTrainROI, ROI objUnitROIForImage0)
        {
            m_strErrorMessage = "";
            m_intFailMask = 0;
            int intValue = 0;

            m_objCopperBlobs.ResetBlobs();
            m_objWOBBlobs.ResetBlobs();
            m_objChipBOWBlobs.ResetBlobs();
            m_objBorderChipBOWBlobs.ResetBlobs();
            m_objBurrWOBBlobs.ResetBlobs();

     
#region search exposed copper
            objColorTrainROI.AttachImage(arrColorImages[0]);
            ROI objNewROI = new ROI();
            objNewROI.LoadROISetting(objColorTrainROI.ref_ROITotalX, objColorTrainROI.ref_ROITotalY, objColorTrainROI.ref_ROIWidth, objColorTrainROI.ref_ROIHeight);
            if ((m_objExposeCopperImage.ref_intImageWidth != arrColorImages[0].ref_intImageWidth) || (m_objExposeCopperImage.ref_intImageHeight != arrColorImages[0].ref_intImageHeight))
                m_objExposeCopperImage.ref_objMainImage.SetSize(arrColorImages[0].ref_intImageWidth, arrColorImages[0].ref_intImageHeight);
            objNewROI.AttachImage(m_objExposeCopperImage);

            if (BuildExposedCopper(objColorTrainROI, objNewROI) > 0)
            {
                string strErrorMessage = "";
                int intTotalArea = 0;
                m_objCopperWOBBlobs.SetFirstListBlobs();
                bool blnFailExposedCopper = false;
                for (int i = 0; i < m_objCopperWOBBlobs.ref_intNumSelectedObject; i++)
                {

                    m_objCopperWOBBlobs.GetSelectedListBlobsArea(ref intValue); // get epoxy object area              
                    intTotalArea += intValue;
                    if (intValue >= m_intExposedCopperMinArea)
                    {
                        strErrorMessage += "*Copper " + (i + 1) + " area: set = " + m_objCopperWOBBlobs.ref_intMinArea + " Score = " + intValue;
                        if (!blnFailExposedCopper)
                            blnFailExposedCopper = true;
                    }
                    m_objCopperWOBBlobs.SetListBlobsToNext();
                }

                // Considered unit flipped if copper area cover 40% of package ROI
                if (((float)intTotalArea / (objColorTrainROI.ref_ROIWidth * objColorTrainROI.ref_ROIHeight)) > 0.3)
                {
                    m_strErrorMessage += "*Sample is flipped";
                    m_intFailMask = 1;
                    return false;
                }
                else if (blnFailExposedCopper)
                {
                    m_strErrorMessage += "*Exposed copper / Oxidation have been found" + strErrorMessage;
                    m_intFailMask = 5;
                    return false;
                }
            }
#endregion
                      
            // search whether sample is scratched
            objMonoTrainROI.AttachImage(arrImages[1]);
            if (BuildWOBObjects(objMonoTrainROI) > 0)
            {
                m_strErrorMessage += "*Scratches have been found on sample";
                m_intFailMask = 2;

                m_objWOBBlobs.SetFirstListBlobs();
                for (int i = 0; i < m_objWOBBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objWOBBlobs.GetSelectedListBlobsArea(ref intValue); // get epoxy object area              
                    m_strErrorMessage += "*Scratch " + (i + 1) + " area: set = " + m_intScratchMinArea + " Score = " + intValue;
                    m_objWOBBlobs.SetListBlobsToNext();
                }              
                return false;
            }

             //search whether sample is scratched
            objMonoTrainROI.AttachImage(arrImages[0]);
            if (!CheckLatentScratch(objMonoTrainROI))
                return false;

            int intWidth = Convert.ToInt32(Math.Sqrt(m_objChipBOWBlobs.ref_intMinArea)) + 1;
            // Top Left
            m_arrChipROI[0].AttachImage(objUnitROIForImage0);
            m_arrChipROI[0].ref_ROIPositionX = 0;
            m_arrChipROI[0].ref_ROIPositionY = 0;
            m_arrChipROI[0].ref_ROIWidth = m_arrChipROI[0].ref_ROIHeight = intWidth;

            int intROIRightX = objUnitROIForImage0.ref_ROIWidth - m_arrChipROI[1].ref_ROIWidth;
            // Top Right
            m_arrChipROI[1].AttachImage(objUnitROIForImage0);
            m_arrChipROI[1].ref_ROIPositionX = intROIRightX;
            m_arrChipROI[1].ref_ROIPositionY = 0;
            m_arrChipROI[1].ref_ROIWidth = m_arrChipROI[1].ref_ROIHeight = intWidth;

            int intROIBottomY = objUnitROIForImage0.ref_ROIHeight - m_arrChipROI[2].ref_ROIWidth;
            // Bottom Left
            m_arrChipROI[2].AttachImage(objUnitROIForImage0);
            m_arrChipROI[2].ref_ROIPositionX = 0;
            m_arrChipROI[2].ref_ROIPositionY = intROIBottomY;
            m_arrChipROI[2].ref_ROIWidth = m_arrChipROI[2].ref_ROIHeight = intWidth;

            // Bottom Right
            m_arrChipROI[3].AttachImage(objUnitROIForImage0);
            m_arrChipROI[3].ref_ROIPositionX = intROIRightX;
            m_arrChipROI[3].ref_ROIPositionY = intROIBottomY;
            m_arrChipROI[3].ref_ROIWidth = m_arrChipROI[3].ref_ROIHeight = intWidth;
           
            if (BuildChipBOWObject() > 0)
            {
                m_strErrorMessage += "*Chip have been found on sample's edge";
                m_intFailMask = 6;

                m_objChipBOWBlobs.SetFirstListBlobs();
                for (int i = 0; i < m_objChipBOWBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objChipBOWBlobs.GetSelectedListBlobsArea(ref intValue); // get chip object area              
                    m_strErrorMessage += "*Chip " + (i + 1) + " area: set = " + m_objChipBOWBlobs.ref_intMinArea + " Score = " + intValue;
                    m_objChipBOWBlobs.SetListBlobsToNext();
                }
                return false;
            }

            ROI objCenterROI = new ROI();
            objCenterROI.LoadROISetting((objMonoUnitROI.ref_ROIWidth / 2) - (objMonoTrainROI.ref_ROIWidth / 2),
                (objMonoUnitROI.ref_ROIHeight / 2) - (objMonoTrainROI.ref_ROIHeight / 2),
                objMonoTrainROI.ref_ROIWidth, objMonoTrainROI.ref_ROIHeight);

            if (m_bCheckChipAtBorder)
            {               
                objMonoUnitROI.AttachImage(arrImages[0]);
                int intHorizontalWidth = ((objMonoUnitROI.ref_ROIWidth - objCenterROI.ref_ROIWidth) / 2) - 2;
                int intHorizontalHeight = objMonoUnitROI.ref_ROIHeight - (intWidth * 2);
                int intVerticalWidth = objMonoUnitROI.ref_ROIWidth - (intWidth * 2);
                int intVerticalHeight = ((objMonoUnitROI.ref_ROIHeight - objCenterROI.ref_ROIHeight) / 2) - 2;

                // Border Left
                m_arrChipROI[4].AttachImage(objMonoUnitROI);
                m_arrChipROI[4].ref_ROIPositionX = 2;
                m_arrChipROI[4].ref_ROIPositionY = intWidth;
                m_arrChipROI[4].ref_ROIWidth = intHorizontalWidth;
                m_arrChipROI[4].ref_ROIHeight = intHorizontalHeight;
                // Border Right
                m_arrChipROI[5].AttachImage(objMonoUnitROI);
                m_arrChipROI[5].ref_ROIPositionX = objMonoUnitROI.ref_ROIWidth - intHorizontalWidth - 2;
                m_arrChipROI[5].ref_ROIPositionY = intWidth;
                m_arrChipROI[5].ref_ROIWidth = intHorizontalWidth;
                m_arrChipROI[5].ref_ROIHeight = intHorizontalHeight;
                // Border Top
                m_arrChipROI[6].AttachImage(objMonoUnitROI);
                m_arrChipROI[6].ref_ROIPositionX = intWidth;
                m_arrChipROI[6].ref_ROIPositionY = 2;
                m_arrChipROI[6].ref_ROIWidth = intVerticalWidth;
                m_arrChipROI[6].ref_ROIHeight = intVerticalHeight;
                // Border Bottom
                m_arrChipROI[7].AttachImage(objMonoUnitROI);
                m_arrChipROI[7].ref_ROIPositionX = intWidth;
                m_arrChipROI[7].ref_ROIPositionY = objMonoUnitROI.ref_ROIHeight - intVerticalHeight - 2;
                m_arrChipROI[7].ref_ROIWidth = intVerticalWidth;
                m_arrChipROI[7].ref_ROIHeight = intVerticalHeight;

                if (BuildBorderChipBOWObject() > 0)
                {
                    m_strErrorMessage += "*Chip have been found on sample's border";
                    m_intFailMask = 6;

                    m_objBorderChipBOWBlobs.SetFirstListBlobs();
                    for (int x = 0; x < m_objBorderChipBOWBlobs.ref_intNumSelectedObject; x++)
                    {
                        m_objBorderChipBOWBlobs.GetSelectedListBlobsArea(ref intValue); // get chip object area              
                        m_strErrorMessage += "*Chip " + (x + 1) + " area: set = " + m_objBorderChipBOWBlobs.ref_intMinArea + " Score = " + intValue;
                        m_objBorderChipBOWBlobs.SetListBlobsToNext();
                    }
                    return false;
                }
            }
            
            objMonoUnitROI.AttachImage(arrImages[1]);
            objCenterROI.AttachImage(objMonoUnitROI);    // same as train roi
            Shape.FillRectangle(objCenterROI, 0);
            if (BuildBurrWOBObject(objMonoUnitROI) > 0)
            {
                m_strErrorMessage += "*Burr have been found on sample's border";
                m_intFailMask = 3;

                m_objBurrWOBBlobs.SetFirstListBlobs();
                for (int i = 0; i < m_objBurrWOBBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objBurrWOBBlobs.GetSelectedListBlobsArea(ref intValue); // get epoxy object area              
                    m_strErrorMessage += "*Burr " + (i + 1) + " area: set = " + m_objBurrWOBBlobs.ref_intMinArea + " Score = " + intValue;
                    m_objBurrWOBBlobs.SetListBlobsToNext();
                }
                return false;
            }


          
            //// search whether sample is oxidized
            //if (BuildOxidazation(objUnitROI) > 0)
            //{
            //    m_strErrorMessage += "Oxidation damaged have been found on sample";
            //    return false;
            //}



            return true;
        }
        /// <summary>
        /// search exposed copper
        /// </summary>
        /// <param name="objROI">inspection area</param>
        /// <returns>no of copper detected after filtering</returns>
        public int BuildBurrWOBObject(ROI objROI)
        {
            return m_objBurrWOBBlobs.BuildObjects(objROI, false);
        }

        public int BuildChipBOWObject()
        {
            for (int i = 0; i < 4; i++)
            {
               int intObject = m_objChipBOWBlobs.BuildObjects(m_arrChipROI[i], false);
               if (intObject > 0)
                   return intObject;
            }

            return 0;
        }

        public int BuildBorderChipBOWObject() 
        {
            for (int i = 4; i < 8; i++)
            {
                int intObject = m_objBorderChipBOWBlobs.BuildObjects(m_arrChipROI[i], false);
                if (intObject > 0)
                    return intObject;
            }      

            return 0;
        }
        /// <summary>
        /// search exposed copper
        /// </summary>
        /// <param name="objROI">inspection area</param>
        /// <returns>no of copper detected after filtering</returns>
        public int BuildExposedCopper(CROI objROI, ROI objNewROI)
        {
            EasyImage.Threshold(objROI.ref_CROI, objNewROI.ref_ROI, m_objCopperBlobs.ref_intLowColorThreshold, m_objCopperBlobs.ref_intHighColorThreshold, m_objColorLookup);
            m_objCopperWOBBlobs.ref_intMinArea = 0;


            return m_objCopperWOBBlobs.BuildObjects(objNewROI, false);
        }  
        /// <summary>
        /// search oxidation of sample
        /// </summary>
        /// <param name="objROI">inspection area</param>
        /// <returns>no of oxidation objects detected after filtering</returns>
        public int BuildOxidazation(CROI objROI)
        {
            return m_objOxidationBlobs.BuildObjects(objROI, false);
        }
        /// <summary>
        /// search White object on black surface
        /// </summary>
        /// <param name="objROI">inspection area</param>
        /// <returns>no of white objects detected after filtering</returns>
        public int BuildWOBObjects(ROI objROI)
        {
            int intTestMethod = 2;

            if (intTestMethod == 0)
            {
                if (m_objWOBBlobs.ref_intThreshold != m_intScratchThreshold)
                    m_objWOBBlobs.ref_intThreshold = m_intScratchThreshold;

                float fMinBorderPixel = (float)Math.Pow(m_objWOBBlobs.ref_intMinArea, 0.5) * 4;
                float fMinAreaTolerance = fMinBorderPixel + (float)(m_objWOBBlobs.ref_intMinArea - fMinBorderPixel) / 2;


                if (m_objWOBBlobs.ref_intMinArea != (m_intScratchMinArea - fMinAreaTolerance))
                {
                    m_objWOBBlobs.ref_intMinArea = (int)(m_intScratchMinArea - fMinAreaTolerance); // risk: negative value
                }

                if (m_objWOBBlobs.BuildObjects(objROI, false) > 0)
                {
                    int intArea = 0;
                    m_objWOBBlobs.SetFirstListBlobs();
                    m_objWOBBlobs.GetSelectedListBlobsArea(ref intArea);


                    if (Math.Abs(intArea - m_intScratchMinArea) < fMinAreaTolerance)
                    {
                        int intAutoThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);
                        int intDiffAutoThreshold = intAutoThresholdValue - m_intTempAutoTHValue;
                        m_objWOBBlobs.ref_intThreshold = m_intScratchThreshold + intDiffAutoThreshold;
                        m_objWOBBlobs.ref_intMinArea = m_intScratchMinArea;

                        m_objWOBBlobs.BuildObjects(objROI, false);
                    }

                }

                return m_objWOBBlobs.ref_intNumSelectedObject;
            }
            else if (intTestMethod == 1)
            {

                m_objWOBBlobs.ref_intMinArea = m_intScratchMinArea;
                //-------------------------------------------------------------------------------------------------------------
                if (m_objWOBBlobs.ref_intThreshold != m_intScratchThreshold)
                    m_objWOBBlobs.ref_intThreshold = m_intScratchThreshold;

                if (m_objWOBBlobs.BuildObjects(objROI, false) > 0)
                {
                    int intArea = 0;
                    m_objWOBBlobs.SetFirstListBlobs();
                    m_objWOBBlobs.GetSelectedListBlobsArea(ref intArea);

                    float fMinBorderPixel = (float)Math.Pow(m_objWOBBlobs.ref_intMinArea, 0.5) * 4;
                    float fMinAreaTolerance = fMinBorderPixel + (float)(m_objWOBBlobs.ref_intMinArea - fMinBorderPixel) / 2;
                    if ((intArea - m_objWOBBlobs.ref_intMinArea) < fMinAreaTolerance)
                    {
                        int intAutoThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);
                        int intDiffAutoThreshold = intAutoThresholdValue - m_intTempAutoTHValue;
                        m_objWOBBlobs.ref_intThreshold = m_intScratchThreshold + intDiffAutoThreshold;

                        m_objWOBBlobs.BuildObjects(objROI, false);
                    }

                }

                return m_objWOBBlobs.ref_intNumSelectedObject;
                //-------------------------------------------------------------------------------------------------------------
            }
            else
            {
                if (m_objWOBBlobs.ref_intThreshold != m_intScratchThreshold)
                    m_objWOBBlobs.ref_intThreshold = m_intScratchThreshold;

                if (m_objWOBBlobs.ref_intMinArea != m_intScratchMinArea)
                    m_objWOBBlobs.ref_intMinArea = m_intScratchMinArea;
                return m_objWOBBlobs.BuildObjects(objROI, false);
            }
        }

        public int BuildLatentScratchObject(ROI objROI)
        {
            return m_objLatentScratchBlobs.BuildObjects(objROI, false);
        }



        public void ChangeChipArea(int intArea)
        {
            int intWidth = Convert.ToInt32(Math.Sqrt(intArea));
            m_arrChipROI[0].ref_ROIWidth = m_arrChipROI[0].ref_ROIHeight = intWidth;

            int intX = m_arrChipROI[1].ref_ROIWidth - intWidth;
            int intY =m_arrChipROI[2].ref_ROIHeight - intWidth ;
            m_arrChipROI[1].ref_ROIPositionX += intX;
            m_arrChipROI[1].ref_ROIWidth = m_arrChipROI[1].ref_ROIHeight = intWidth;

            m_arrChipROI[2].ref_ROIPositionY += intY;
            m_arrChipROI[2].ref_ROIWidth = m_arrChipROI[2].ref_ROIHeight = intWidth;

            m_arrChipROI[3].ref_ROIPositionX += intX;
            m_arrChipROI[3].ref_ROIPositionY += intY;
            m_arrChipROI[3].ref_ROIWidth = m_arrChipROI[3].ref_ROIHeight = intWidth;
        }
        public bool CheckLatentScratch(ROI objROI)
        {
            if (BuildLatentScratchObject(objROI) > 0)
            {
                m_strErrorMessage += "*Black Dot Scratches have been found on sample";
                m_intFailMask = 7;

                int intValue = 0;
                m_objLatentScratchBlobs.SetFirstListBlobs();
                for (int i = 0; i < m_objLatentScratchBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objLatentScratchBlobs.GetSelectedListBlobsArea(ref intValue); // get epoxy object area              
                    m_strErrorMessage += "*Black Dot Scratch " + (i + 1) + " area: set = " + m_objLatentScratchBlobs.ref_intMinArea + " Score = " + intValue;
                    m_objLatentScratchBlobs.SetListBlobsToNext();
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// draw burr object in different color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawBurrObjects(Graphics g)
        {
            if (m_objBurrWOBBlobs.ref_intNumSelectedObject == 0)
                return;
            m_objBurrWOBBlobs.DrawSelectedBlobs(g, 1f, 1f);
        }
        /// <summary>
        /// draw chip object in different color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawChipInspectionArea(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            for (int i = 0; i < 8; i++)
            {
                m_arrChipROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, false);
            }
        }
        /// <summary>
        /// draw chip object in different color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawChipObjects(Graphics g)
        {
            if (m_objChipBOWBlobs.ref_intNumSelectedObject != 0)
            {
                m_objChipBOWBlobs.DrawSelectedBlobs(g, 1f, 1f);
            }

            if (m_objBorderChipBOWBlobs.ref_intNumSelectedObject != 0)
            {
                m_objBorderChipBOWBlobs.DrawSelectedBlobs(g, 1f, 1f);
            }
        }

        public void DrawBorderChipObjects(Graphics g)
        {
            if (m_objBorderChipBOWBlobs.ref_intNumSelectedObject == 0)
                return;
            m_objBorderChipBOWBlobs.DrawSelectedBlobs(g, 1f, 1f);
        }
        /// <summary>
        /// draw copper object in different color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawCopperObjects(Graphics g)
        {
            if (m_objCopperWOBBlobs.ref_intNumSelectedObject == 0)
                return;
            m_objCopperWOBBlobs.DrawSelectedBlobs(m_intExposedCopperMinArea, g, 1f, 1f);
            //m_objCopperWOBBlobs.DrawSelectedBlobs(g);
        }
        /// <summary>
        /// draw oxidation object in color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawOxidationObjects(Graphics g)
        {
            if (m_objOxidationBlobs.ref_intNumSelectedObject == 0)
                return;
            m_objOxidationBlobs.DrawSelectedBlobs(g, 1f, 1f);
        }
        /// <summary>
        /// Draw white objects in color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawWOBObjects(Graphics g)
        {
            if (m_objWOBBlobs.ref_intNumSelectedObject == 0)
                return;

            m_objWOBBlobs.DrawSelectedBlobs(m_intScratchMinArea, g, 1f, 1f);
            //m_objWOBBlobs.DrawSelectedBlobs(g);
        }
        /// <summary>
        /// Draw black object in color
        /// </summary>
        /// <param name="g">destination image to draw</param>
        public void DrawLatentScratchObjects(Graphics g)
        {
            if (m_objLatentScratchBlobs.ref_intNumSelectedObject == 0)
                return;
            m_objLatentScratchBlobs.DrawSelectedBlobs(g, 1f, 1f);
        }


        public void SetChipArea(int intArea)
        {
            int intWidth = Convert.ToInt32(Math.Sqrt(intArea));
            for (int i = 0; i < 4; i++)
            {
                m_arrChipROI[i].ref_ROIWidth = m_arrChipROI[i].ref_ROIHeight = intWidth;
            }
        }

        public void SetChipPosition(int intUnitTopLeftX, int intUnitTopLeftY, int intUnitWidth, int intUnitHeight, ImageDrawing objImage)
        {
            // Top Left
            m_arrChipROI[0].AttachImage(objImage);
            m_arrChipROI[0].ref_ROIPositionX = intUnitTopLeftX;
            m_arrChipROI[0].ref_ROIPositionY = intUnitTopLeftY;
      
            int intROIRightX = intUnitTopLeftX + intUnitWidth - m_arrChipROI[1].ref_ROIWidth;
            // Top Right
            m_arrChipROI[1].AttachImage(objImage);
            m_arrChipROI[1].ref_ROIPositionX = intROIRightX;
            m_arrChipROI[1].ref_ROIPositionY = intUnitTopLeftY;

            int intROIBottomY = intUnitTopLeftY + intUnitHeight - m_arrChipROI[2].ref_ROIWidth;
            // Bottom Left
            m_arrChipROI[2].AttachImage(objImage);
            m_arrChipROI[2].ref_ROIPositionX = intUnitTopLeftX;
            m_arrChipROI[2].ref_ROIPositionY = intROIBottomY;
            // Bottom Right
            m_arrChipROI[3].AttachImage(objImage);
            m_arrChipROI[3].ref_ROIPositionX = intROIRightX;
            m_arrChipROI[3].ref_ROIPositionY = intROIBottomY;
        }
        /// <summary>
        /// Use value in RGB format to set blob threshold
        /// </summary>
        /// <param name="intCopperColor">RGB value</param>
        /// <param name="intCopperColorTolerance">RGB value tolerance</param>
        public void SetCopperBlobThreshold(int[] intCopperColor, int[] intCopperColorTolerance)
        {
            m_intCopperColor = intCopperColor;
            m_intCopperColorTolerance = intCopperColorTolerance;

            m_objCopperBlobs.ref_intLowColorThreshold = ColorProcessing.CalculateMinColor(intCopperColor, intCopperColorTolerance).ref_Color24;
            m_objCopperBlobs.ref_intHighColorThreshold = ColorProcessing.CalculateMaxColor(intCopperColor, intCopperColorTolerance).ref_Color24;     
        }

        public void SetFlipBlobThreshold(int[] intFlipColor, int[] intFlipColorTolerance)
        {
            m_intFlipColor = intFlipColor;
            m_intFlipColorTolerance = intFlipColorTolerance;

            m_intFlipLowThreshold = ColorProcessing.CalculateMinColor(intFlipColor, intFlipColorTolerance).ref_Color24;
            m_intFlipHighThreshold = ColorProcessing.CalculateMaxColor(intFlipColor, intFlipColorTolerance).ref_Color24;
        }
        /// <summary>
        /// Use value in RGB format to set blob threshold
        /// </summary>
        /// <param name="intR">red color</param>
        /// <param name="intG">green color</param>
        /// <param name="intB">blue color</param>
        /// <param name="intRTolerance">red color tolerance</param>
        /// <param name="intGTolerance">green color tolerance</param>
        /// <param name="intBTolerance">blue color tolerance</param>
        public void SetCopperBlobThreshold(int intR, int intG, int intB, int intRTolerance, int intGTolerance, int intBTolerance)
        {
            m_intCopperColor[0] = intR;
            m_intCopperColor[1] = intG;
            m_intCopperColor[2] = intB;

            m_intCopperColorTolerance[0] = intRTolerance;
            m_intCopperColorTolerance[1] = intGTolerance;
            m_intCopperColorTolerance[2] = intBTolerance;

            m_objCopperBlobs.ref_intLowColorThreshold = ColorProcessing.CalculateMinColor(m_intCopperColor, m_intCopperColorTolerance).ref_Color24;
            m_objCopperBlobs.ref_intHighColorThreshold = ColorProcessing.CalculateMaxColor(m_intCopperColor, m_intCopperColorTolerance).ref_Color24;
        }
        /// <summary>
        /// Use value in RGB format to set blob threshold
        /// </summary>
        /// <param name="intR">red color</param>
        /// <param name="intG">green color</param>
        /// <param name="intB">blue color</param>
        /// <param name="intRTolerance">red color tolerance</param>
        /// <param name="intGTolerance">green color tolerance</param>
        /// <param name="intBTolerance">blue color tolerance</param>
        public void SetFlipBlobThreshold(int intR, int intG, int intB, int intRTolerance, int intGTolerance, int intBTolerance)
        {
            m_intFlipColor[0] = intR;
            m_intFlipColor[1] = intG;
            m_intFlipColor[2] = intB;

            m_intFlipColorTolerance[0] = intRTolerance;
            m_intFlipColorTolerance[1] = intGTolerance;
            m_intFlipColorTolerance[2] = intBTolerance;

            m_intFlipLowThreshold = ColorProcessing.CalculateMinColor(m_intFlipColor, m_intFlipColorTolerance).ref_Color24;
            m_intFlipHighThreshold = ColorProcessing.CalculateMaxColor(m_intFlipColor, m_intFlipColorTolerance).ref_Color24;
        }
        /// <summary>
        /// Use value in RGB format to set blob threshold
        /// </summary>
        /// <param name="intOxidationColor">RGB value</param>
        /// <param name="intOxidationColorTolerance">RGB value tolerance</param>
        public void SetOxidationBlobThreshold(int[] intOxidationColor, int[] intOxidationColorTolerance)
        {
            m_intOxidationColor = intOxidationColor;
            m_intOxidationColorTolerance = intOxidationColorTolerance;

            m_objOxidationBlobs.ref_intLowColorThreshold = ColorProcessing.CalculateMinColor(intOxidationColor, intOxidationColorTolerance).ref_Color24;
            m_objOxidationBlobs.ref_intHighColorThreshold = ColorProcessing.CalculateMaxColor(intOxidationColor, intOxidationColorTolerance).ref_Color24;
        }
        /// <summary>
        /// Use value in RGB format to set blob threshold
        /// </summary>
        /// <param name="intR">red color</param>
        /// <param name="intG">green color</param>
        /// <param name="intB">blue color</param>
        /// <param name="intRTolerance">red color tolerance</param>
        /// <param name="intGTolerance">green color tolerance</param>
        /// <param name="intBTolerance">blue color tolerance</param>
        public void SetOxidationBlobThreshold(int intR, int intG, int intB, int intRTolerance, int intGTolerance, int intBTolerance)
        {
            m_intOxidationColor[0] = intR;
            m_intOxidationColor[1] = intG;
            m_intOxidationColor[2] = intB;

            m_intOxidationColorTolerance[0] = intRTolerance;
            m_intOxidationColorTolerance[1] = intGTolerance;
            m_intOxidationColorTolerance[2] = intBTolerance;

            m_objOxidationBlobs.ref_intLowColorThreshold = ColorProcessing.CalculateMinColor(m_intOxidationColor, m_intOxidationColorTolerance).ref_Color24;
            m_objOxidationBlobs.ref_intHighColorThreshold = ColorProcessing.CalculateMaxColor(m_intOxidationColor, m_intOxidationColorTolerance).ref_Color24;
        }

        public void LoadColorPackage(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            m_intStartPixelFromEdge = objFile.GetValueAsInt("PixelFromEdge", 0);
            m_intTempAutoTHValue = objFile.GetValueAsInt("TemplateAutoTHValue", 0);
            m_intThresholdTolerance = objFile.GetValueAsInt("ThresholdTolerance", 10);
            m_bCheckChipAtBorder = objFile.GetValueAsBoolean("CheckChipAtBorder", false);

            m_intScratchMinArea = m_objWOBBlobs.ref_intMinArea = objFile.GetValueAsInt("WOBMinArea", 0);
            m_intExposedCopperMinArea = objFile.GetValueAsInt("CopperMinArea", 0);      // For exposedcopper min area, set it in independent variable rather than blob inself because blobs need to build object with min area 0.
            m_objCopperBlobs.ref_intMinArea = 0;
            m_objOxidationBlobs.ref_intMinArea = objFile.GetValueAsInt("OxidationMinArea", 0);

            m_objBurrWOBBlobs.ref_intMinArea = objFile.GetValueAsInt("BurrMinArea", 20);
            m_objLatentScratchBlobs.ref_intMinArea = objFile.GetValueAsInt("LatentScratchMinArea", 0);
            m_objBorderChipBOWBlobs.ref_intMinArea = objFile.GetValueAsInt("BorderChipMinArea", 20);
            int intValue = objFile.GetValueAsInt("ChipMinArea", 20);
            m_objChipBOWBlobs.ref_intMinArea  = intValue;
            SetChipArea(intValue);

            //m_objWOBBlobs.ref_intThreshold = objFile.GetValueAsInt("WOBThreshold", 2);
            m_intScratchThreshold = objFile.GetValueAsInt("WOBThreshold", 2);
            m_objChipBOWBlobs.ref_intThreshold = objFile.GetValueAsInt("ChipViewThreshold", 150);
            m_objBorderChipBOWBlobs.ref_intThreshold = m_objChipBOWBlobs.ref_intThreshold;
            m_objBurrWOBBlobs.ref_intThreshold = objFile.GetValueAsInt("BurrThreshold", 2);
            m_objLatentScratchBlobs.ref_intThreshold = objFile.GetValueAsInt("LatentScratchThreshold", 2);
            SetCopperBlobThreshold(objFile.GetValueAsInt("CopperThreshold1", 0),
                                   objFile.GetValueAsInt("CopperThreshold2", 0),
                                   objFile.GetValueAsInt("CopperThreshold3", 0),
                                   objFile.GetValueAsInt("CopperTolerance1", 0),
                                   objFile.GetValueAsInt("CopperTolerance2", 0),
                                   objFile.GetValueAsInt("CopperTolerance3", 0));
            SetOxidationBlobThreshold(objFile.GetValueAsInt("OxidationThreshold1", 0),
                                      objFile.GetValueAsInt("OxidationThreshold2", 0),
                                      objFile.GetValueAsInt("OxidationThreshold3", 0),
                                      objFile.GetValueAsInt("OxidationTolerance1", 0),
                                      objFile.GetValueAsInt("OxidationTolerance2", 0),
                                      objFile.GetValueAsInt("OxidationTolerance3", 0));

        }

        public void SaveColorPackage(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);
            objFile.WriteSectionElement(strSectionName, blnNewSection);

            objFile.WriteElement1Value("PixelFromEdge", m_intStartPixelFromEdge);
            objFile.WriteElement1Value("TemplateAutoTHValue", m_intTempAutoTHValue);
            objFile.WriteElement1Value("ThresholdTolerance", m_intThresholdTolerance);
            objFile.WriteElement1Value("CheckChipAtBorder", m_bCheckChipAtBorder);

            objFile.WriteElement1Value("WOBMinArea", m_intScratchMinArea);
            objFile.WriteElement1Value("CopperMinArea", m_intExposedCopperMinArea);
            objFile.WriteElement1Value("OxidationMinArea", m_objOxidationBlobs.ref_intMinArea);
            objFile.WriteElement1Value("BorderChipMinArea", m_objBorderChipBOWBlobs.ref_intMinArea);
            objFile.WriteElement1Value("BurrMinArea", m_objBurrWOBBlobs.ref_intMinArea);
            objFile.WriteElement1Value("ChipMinArea", m_objChipBOWBlobs.ref_intMinArea);
            objFile.WriteElement1Value("LatentScratchMinArea", m_objLatentScratchBlobs.ref_intMinArea);

            objFile.WriteElement1Value("BurrThreshold", m_objBurrWOBBlobs.ref_intThreshold);
            //objFile.WriteElement1Value("WOBThreshold", m_objWOBBlobs.ref_intThreshold);
            objFile.WriteElement1Value("WOBThreshold", m_intScratchThreshold);
            objFile.WriteElement1Value("ChipViewThreshold", m_objChipBOWBlobs.ref_intThreshold);
            objFile.WriteElement1Value("LatentScratchThreshold", m_objLatentScratchBlobs.ref_intThreshold);
            objFile.WriteElement1Value("CopperThreshold1", m_intCopperColor[0]);
            objFile.WriteElement1Value("CopperThreshold2", m_intCopperColor[1]);
            objFile.WriteElement1Value("CopperThreshold3", m_intCopperColor[2]);
            objFile.WriteElement1Value("CopperTolerance1", m_intCopperColorTolerance[0]);
            objFile.WriteElement1Value("CopperTolerance2", m_intCopperColorTolerance[1]);
            objFile.WriteElement1Value("CopperTolerance3", m_intCopperColorTolerance[2]);

            objFile.WriteElement1Value("OxidationThreshold1", m_intOxidationColor[0]);
            objFile.WriteElement1Value("OxidationThreshold2", m_intOxidationColor[1]);
            objFile.WriteElement1Value("OxidationThreshold3", m_intOxidationColor[2]);
            objFile.WriteElement1Value("OxidationTolerance1", m_intOxidationColorTolerance[0]);
            objFile.WriteElement1Value("OxidationTolerance2", m_intOxidationColorTolerance[1]);
            objFile.WriteElement1Value("OxidationTolerance3", m_intOxidationColorTolerance[2]);

            objFile.WriteEndElement();
        }      
    }
}
