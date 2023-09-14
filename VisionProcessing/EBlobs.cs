using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Threading;

namespace VisionProcessing
{
    public class EBlobs
    {
        #region Member Variables

        private Object m_Lock = new object();

        private EC24 m_C24LowColorThreshold;
        private EC24 m_C24HighColorThreshold;

        private int m_intNumSelectedObject = 0; // Record latest num of selected blobs after last build object or filter
        private int m_intROIStartX = 0;         // Record ori X of ROI used for last build object
        private int m_intROIStartY = 0;         // Record ori X of ROI used for last build object
        private int m_intMaxAreaLimit = 999999999;  // Record max area limit used during last build blobs
        private int m_intMinAreaLimit = 0;          // Record min area limit used during last build blobs
        private int m_intAbsoluteThreshold = 125;   // Record absolute threshold used during last build blobs (0-255)
        private int m_intAbsoluteLowThreshold = 125;   // Record absolute low double threshold used during last build blobs (0-255)
        private int m_intAbsoluteHighThreshold = 125;   // Record absolute high double threshold used during last build blobs (0-255)
        private int m_intConnexity = 4;             // Record connexity used during last build blobs ( 4 or 8)
        private int m_intCriteria = 0xFF;           // Record Criteria used during last build blobs 
        private int m_intClassSelection = 0x01;     // Record Class Selection used during last build blobs  0x01=Black, 0x02=White, 0x04=Neutral
        private Color[] m_colorList = { Color.Aqua, Color.Lime, Color.DeepPink, Color.Aquamarine, Color.OrangeRed, Color.Gold, Color.Fuchsia, Color.MediumSpringGreen, Color.Blue, Color.Violet };
        private ERGBColor m_objColor = new ERGBColor(0, 0, 0);
        private EImageEncoder encoder = new EImageEncoder();
        private EObjectSelection selection = new EObjectSelection();
        private ECodedImage2 codedImage = new ECodedImage2();
#if (Debug_2_12 || Release_2_12)
        private Euresys.Open_eVision_2_12.Segmenters.EGrayscaleSingleThresholdSegmenter segmenter = null;
        private Euresys.Open_eVision_2_12.Segmenters.EGrayscaleDoubleThresholdSegmenter segmenters = null;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        private Euresys.Open_eVision_1_2.Segmenters.EGrayscaleSingleThresholdSegmenter segmenter = null;
        private Euresys.Open_eVision_1_2.Segmenters.EGrayscaleDoubleThresholdSegmenter segmenters = null;
#endif

        private List<float> m_arrGravityCenterX = new List<float>();
        private List<float> m_arrGravityCenterY = new List<float>();
        private List<float> m_arrLimitCenterX = new List<float>();
        private List<float> m_arrLimitCenterY = new List<float>();
        private List<float> m_arrWidth = new List<float>();
        private List<float> m_arrHeight = new List<float>();
        private List<int> m_arrArea = new List<int>();
        private List<int> m_arrContourX = new List<int>();
        private List<int> m_arrContourY = new List<int>();
        private List<float> m_arrRectLimitCenterX = new List<float>();
        private List<float> m_arrRectLimitCenterY = new List<float>();
        private List<float> m_arrRectWidth = new List<float>();
        private List<float> m_arrRectHeight = new List<float>();
        private List<float> m_arrRectAngle = new List<float>();
        private List<float> m_arrEccentricity = new List<float>();
        private List<float> m_arrAverageGray = new List<float>();
        private int m_intTotalArea = 0;
        private float m_fTotalLimitWidth = 0;
        private float m_fTotalLimitHeight = 0;
        private float m_fTotalLimitCenterX = 0;
        private float m_fTotalLimitCenterY = 0;
        #endregion

        #region Properties
        public EC24 ref_C24LowColorThreshold { get { return m_C24LowColorThreshold; } set { m_C24LowColorThreshold = value; } }
        public EC24 ref_C24HighColorThreshold { get { return m_C24HighColorThreshold; } set { m_C24HighColorThreshold = value; } }
        public int ref_intNumSelectedObject { get { return m_intNumSelectedObject; } }
        public int ref_intAbsoluteThreshold { get { return m_intAbsoluteThreshold; } set { m_intAbsoluteThreshold = value; } }
        public int ref_intAbsoluteLowThreshold { get { return m_intAbsoluteLowThreshold; } set { m_intAbsoluteLowThreshold = value; } }
        public int ref_intAbsoluteHighThreshold { get { return m_intAbsoluteHighThreshold; } set { m_intAbsoluteHighThreshold = value; } }
        public int ref_intMinAreaLimit { get { return m_intMinAreaLimit; } set { m_intMinAreaLimit = value; } }
        public int ref_intMaxAreaLimit { get { return m_intMaxAreaLimit; } set { m_intMaxAreaLimit = value; } }
        public int ref_intConnexity { get { return m_intConnexity; } set { m_intConnexity = value; } }
        public int ref_intCriteria { get { return m_intCriteria; } set { m_intCriteria = value; } }
        public int ref_intClassSelection { get { return m_intClassSelection; } set { m_intClassSelection = value; } }
        public List<float> ref_arrGravityCenterX { get { return m_arrGravityCenterX; } }
        public List<float> ref_arrGravityCenterY { get { return m_arrGravityCenterY; } }
        public List<float> ref_arrLimitCenterX { get { return m_arrLimitCenterX; } }
        public List<float> ref_arrLimitCenterY { get { return m_arrLimitCenterY; } }
        public List<float> ref_arrWidth { get { return m_arrWidth; } }
        public List<float> ref_arrHeight { get { return m_arrHeight; } }
        
        public List<float> ref_arrRectLimitCenterX { get { return m_arrRectLimitCenterX; } }
        public List<float> ref_arrRectLimitCenterY { get { return m_arrRectLimitCenterY; } }
        public List<float> ref_arrRectWidth { get { return m_arrRectWidth; } }
        public List<float> ref_arrRectHeight { get { return m_arrRectHeight; } }
        public List<float> ref_arrRectAngle { get { return m_arrRectAngle; } }
        public List<int> ref_arrArea { get { return m_arrArea; } }
        public List<int> ref_arrContourX { get { return m_arrContourX; } }
        public List<int> ref_arrContourY { get { return m_arrContourY; } }
        public List<float> ref_arrEccentricity { get { return m_arrEccentricity; } }
        public List<float> ref_arrAverageGray { get { return m_arrAverageGray; } }
        public int ref_intTotalArea { get { return m_intTotalArea; } }
        public float ref_fTotalLimitWidth{ get { return m_fTotalLimitWidth; } }
        public float ref_fTotalLimitHeight { get { return m_fTotalLimitHeight; } }
        public float ref_fTotalLimitCenterX { get { return m_fTotalLimitCenterX; } }
        public float ref_fTotalLimitCenterY { get { return m_fTotalLimitCenterY; } }


        public ECodedImage2 ref_Blob { get { return codedImage; } }
        #endregion

        public EBlobs()
        {

        }

        // =================================================Basic Function=================================================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objROI"></param>
        /// <param name="blnBlackOnWhite"></param>
        /// <param name="blnConnexity4"></param>
        /// <param name="blnRemoveBorder"></param>
        /// <param name="intCriteria">0x01=Area, 0x02=Gravity Center Point, 0x04=Bounding Center Point, 0x08=Bounding Size, 0x10Contour</param>
        /// <returns></returns>
        public int BuildObjects_Filter_GetElement(ROI objROI, bool blnBlackOnWhite, bool blnConnexity4,
            bool blnRemoveBorder, int intCriteria)
        {
            try
            {
                CleanAllBlobs();    // Clear all selection first before build blobs

                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;

                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenter = encoder.GrayscaleSingleThresholdSegmenter;

                // Set classification
                if (segmenter.BlackLayerEncoded != blnBlackOnWhite)
                    segmenter.BlackLayerEncoded = blnBlackOnWhite;
                if (segmenter.WhiteLayerEncoded != !blnBlackOnWhite)
                    segmenter.WhiteLayerEncoded = !blnBlackOnWhite;

#if (Debug_2_12 || Release_2_12)
                // Set threshold
                if (m_intAbsoluteThreshold == -4)
                {
                    EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                    segmenter.AbsoluteThreshold = objBW8.Value;
                }
                else
                {
                    if (segmenter.AbsoluteThreshold != m_intAbsoluteThreshold)
                        segmenter.AbsoluteThreshold = (uint)m_intAbsoluteThreshold;
                }
                
                encoder.Encode(objROI.ref_ROI, codedImage);

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)m_intMinAreaLimit, (uint)m_intMaxAreaLimit, EDoubleThresholdMode.Outside);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Set threshold
                if (m_intAbsoluteThreshold == -4)
                {
                    EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                    segmenter.AbsoluteThreshold = objBW8.Value;
                }
                else
                {
                    if (segmenter.AbsoluteThreshold != m_intAbsoluteThreshold)
                        segmenter.AbsoluteThreshold = m_intAbsoluteThreshold;
                }

                encoder.Encode(objROI.ref_ROI, codedImage);

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, m_intMinAreaLimit, m_intMaxAreaLimit, EDoubleThresholdMode.Outside);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#endif

                selection.Sort(EFeature.Area, ESortDirection.Descending);

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                LoadElemet(intCriteria);

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                return m_intNumSelectedObject;
            }
            catch (Exception ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + m_intAbsoluteThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->m_intMinAreaLimit =" + m_intMinAreaLimit);
                STTrackLog.WriteLine("EBlobs->BuildObjects->m_intMaxAreaLimit =" + m_intMaxAreaLimit);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objROI"></param>
        /// <param name="blnBlackOnWhite"></param>
        /// <param name="blnConnexity4"></param>
        /// <param name="intSegmenterMode">// 0=Absolute, 1=Relative, 2=MinResidue, 3=MaxEntropy, 4=Iso-Data</param>
        /// <param name="intThreshold"></param>
        /// <param name="intAreaLow"></param>
        /// <param name="intAreaHigh"></param>
        /// <param name="blnRemoveBorder"></param>
        /// <param name="intCriteria">0x01=Area, 0x02=Gravity Center Point, 0x04=Bounding Center Point, 0x08=Bounding Size, 0x10Contour </param>
        /// <returns></returns>
        public int BuildObjects_Filter_GetElement(ROI objROI, bool blnBlackOnWhite, bool blnConnexity4, int intSegmenterMode, int intThreshold,
            int intAreaLow, int intAreaHigh, bool blnRemoveBorder, int intCriteria)
        {
            try
            {
                HiPerfTimer m_T1 = new HiPerfTimer();
                m_T1.Start();
                string m_strTrack = "";
                float m_fTimingPrev = 0;
                float m_fTiming = 0;

                CleanAllBlobs();    // Clear all selection first before build blobs
                m_intNumSelectedObject = 0;
                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;
                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenter = encoder.GrayscaleSingleThresholdSegmenter;

                // Set classification
                if (segmenter.BlackLayerEncoded != blnBlackOnWhite)
                    segmenter.BlackLayerEncoded = blnBlackOnWhite;
                if (segmenter.WhiteLayerEncoded != !blnBlackOnWhite)
                    segmenter.WhiteLayerEncoded = !blnBlackOnWhite;

                //2020-09-02 ZJYEOH : If put -4 as threshold, the blob built will be different from blob built using auto threshold value
                if (intThreshold == -4)
                    intSegmenterMode = 2;
                else
                    intSegmenterMode = 0;

                // Set threshold
                if (segmenter.Mode != (EGrayscaleSingleThreshold)intSegmenterMode)
                    segmenter.Mode = (EGrayscaleSingleThreshold)intSegmenterMode;  // 0=Absolute, 1=Relative, 2=MinResidue, 3=MaxEntropy, 4=Iso-Data

#if (Debug_2_12 || Release_2_12)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = (uint)intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.Width, (uint)objROI.ref_ROI.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.Width, objROI.ref_ROI.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#endif

                selection.Sort(EFeature.Area, ESortDirection.Descending);
                selection.AttachedImage = objROI.ref_ROI;

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 3=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                LoadElemet(intCriteria);


                if (m_intAbsoluteThreshold != intThreshold)
                    m_intAbsoluteThreshold = intThreshold;

                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 4=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                return m_intNumSelectedObject;
            }
            catch (EException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
            catch (ApplicationException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }

        }
        public int BuildObjects_Filter_GetElement_SortByX(ROI objROI, bool blnBlackOnWhite, bool blnConnexity4, int intSegmenterMode, int intThreshold,
            int intAreaLow, int intAreaHigh, bool blnRemoveBorder, int intCriteria, bool blnIsTop)
        {
            try
            {
                HiPerfTimer m_T1 = new HiPerfTimer();
                m_T1.Start();
                string m_strTrack = "";
                float m_fTimingPrev = 0;
                float m_fTiming = 0;

                CleanAllBlobs();    // Clear all selection first before build blobs
                m_intNumSelectedObject = 0;
                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;
                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenter = encoder.GrayscaleSingleThresholdSegmenter;

                // Set classification
                if (segmenter.BlackLayerEncoded != blnBlackOnWhite)
                    segmenter.BlackLayerEncoded = blnBlackOnWhite;
                if (segmenter.WhiteLayerEncoded != !blnBlackOnWhite)
                    segmenter.WhiteLayerEncoded = !blnBlackOnWhite;

                //2020-09-02 ZJYEOH : If put -4 as threshold, the blob built will be different from blob built using auto threshold value
                if (intThreshold == -4)
                    intSegmenterMode = 2;
                else
                    intSegmenterMode = 0;

                // Set threshold
                if (segmenter.Mode != (EGrayscaleSingleThreshold)intSegmenterMode)
                    segmenter.Mode = (EGrayscaleSingleThreshold)intSegmenterMode;  // 0=Absolute, 1=Relative, 2=MinResidue, 3=MaxEntropy, 4=Iso-Data

#if (Debug_2_12 || Release_2_12)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = (uint)intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnIsTop)
                    selection.RemoveUsingIntegerFeature(EFeature.BottomLimit, objROI.ref_ROIHeight - 1, ESingleThresholdMode.Less);
                else
                    selection.RemoveUsingIntegerFeature(EFeature.TopLimit, 0, ESingleThresholdMode.Greater);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.Width, (uint)objROI.ref_ROI.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnIsTop)
                    selection.RemoveUsingIntegerFeature(EFeature.BottomLimit, objROI.ref_ROIHeight - 1, ESingleThresholdMode.Less);
                else
                    selection.RemoveUsingIntegerFeature(EFeature.TopLimit, 0, ESingleThresholdMode.Greater);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.Width, objROI.ref_ROI.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#endif

                selection.Sort(EFeature.BoundingBoxCenterX, ESortDirection.Ascending);
                selection.AttachedImage = objROI.ref_ROI;

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 3=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                LoadElemet(intCriteria);
                
                if (m_intAbsoluteThreshold != intThreshold)
                    m_intAbsoluteThreshold = intThreshold;

                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 4=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                return m_intNumSelectedObject;
            }
            catch (EException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
            catch (ApplicationException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }

        }
        public int BuildObjects_Filter_GetElement_SortByY(ROI objROI, bool blnBlackOnWhite, bool blnConnexity4, int intSegmenterMode, int intThreshold,
            int intAreaLow, int intAreaHigh, bool blnRemoveBorder, int intCriteria, bool blnIsLeft)
        {
            try
            {
                HiPerfTimer m_T1 = new HiPerfTimer();
                m_T1.Start();
                string m_strTrack = "";
                float m_fTimingPrev = 0;
                float m_fTiming = 0;

                CleanAllBlobs();    // Clear all selection first before build blobs
                m_intNumSelectedObject = 0;
                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;
                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenter = encoder.GrayscaleSingleThresholdSegmenter;

                // Set classification
                if (segmenter.BlackLayerEncoded != blnBlackOnWhite)
                    segmenter.BlackLayerEncoded = blnBlackOnWhite;
                if (segmenter.WhiteLayerEncoded != !blnBlackOnWhite)
                    segmenter.WhiteLayerEncoded = !blnBlackOnWhite;

                //2020-09-02 ZJYEOH : If put -4 as threshold, the blob built will be different from blob built using auto threshold value
                if (intThreshold == -4)
                    intSegmenterMode = 2;
                else
                    intSegmenterMode = 0;

                // Set threshold
                if (segmenter.Mode != (EGrayscaleSingleThreshold)intSegmenterMode)
                    segmenter.Mode = (EGrayscaleSingleThreshold)intSegmenterMode;  // 0=Absolute, 1=Relative, 2=MinResidue, 3=MaxEntropy, 4=Iso-Data

#if (Debug_2_12 || Release_2_12)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = (uint)intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnIsLeft)
                    selection.RemoveUsingIntegerFeature(EFeature.RightLimit, objROI.ref_ROIWidth - 1, ESingleThresholdMode.Less);
                else
                    selection.RemoveUsingIntegerFeature(EFeature.LeftLimit, 0, ESingleThresholdMode.Greater);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.Width, (uint)objROI.ref_ROI.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnIsLeft)
                    selection.RemoveUsingIntegerFeature(EFeature.RightLimit, objROI.ref_ROIWidth - 1, ESingleThresholdMode.Less);
                else
                    selection.RemoveUsingIntegerFeature(EFeature.LeftLimit, 0, ESingleThresholdMode.Greater);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.Width, objROI.ref_ROI.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#endif

                selection.Sort(EFeature.BoundingBoxCenterY, ESortDirection.Ascending);
                selection.AttachedImage = objROI.ref_ROI;

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 3=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                LoadElemet(intCriteria);


                if (m_intAbsoluteThreshold != intThreshold)
                    m_intAbsoluteThreshold = intThreshold;

                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 4=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                return m_intNumSelectedObject;
            }
            catch (EException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
            catch (ApplicationException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }

        }

        public int BuildObjects_Filter_GetElement_BlobLimit(ROI objROI, bool blnBlackOnWhite, bool blnConnexity4, int intSegmenterMode, int intThreshold,
            int intAreaLow, int intAreaHigh, int intCriteria)
        {
            try
            {
                HiPerfTimer m_T1 = new HiPerfTimer();
                m_T1.Start();
                string m_strTrack = "";
                float m_fTimingPrev = 0;
                float m_fTiming = 0;

                CleanAllBlobs();    // Clear all selection first before build blobs
                m_intNumSelectedObject = 0;
                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;
                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenter = encoder.GrayscaleSingleThresholdSegmenter;

                // Set classification
                if (segmenter.BlackLayerEncoded != blnBlackOnWhite)
                    segmenter.BlackLayerEncoded = blnBlackOnWhite;
                if (segmenter.WhiteLayerEncoded != !blnBlackOnWhite)
                    segmenter.WhiteLayerEncoded = !blnBlackOnWhite;

                //2020-09-02 ZJYEOH : If put -4 as threshold, the blob built will be different from blob built using auto threshold value
                if (intThreshold == -4)
                    intSegmenterMode = 2;
                else
                    intSegmenterMode = 0;

                // Set threshold
                if (segmenter.Mode != (EGrayscaleSingleThreshold)intSegmenterMode)
                    segmenter.Mode = (EGrayscaleSingleThreshold)intSegmenterMode;  // 0=Absolute, 1=Relative, 2=MinResidue, 3=MaxEntropy, 4=Iso-Data

#if (Debug_2_12 || Release_2_12)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = (uint)intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                selection.RemoveObjectsUsingRectangle(codedImage, 1, 1, (uint)Math.Max(2, objROI.ref_ROI.Width - 2), (uint)Math.Max(2, objROI.ref_ROI.Height - 2), ERectangleMode.EntirelyInside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);
                
                selection.RemoveObjectsUsingRectangle(codedImage, 1, 1,  Math.Max(2, objROI.ref_ROI.Width - 2), Math.Max(2, objROI.ref_ROI.Height - 2), ERectangleMode.EntirelyInside);
                
#endif

                selection.Sort(EFeature.BoundingBoxCenterX, ESortDirection.Ascending);
                selection.AttachedImage = objROI.ref_ROI;

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 3=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                LoadElemet(intCriteria);

                if (m_intAbsoluteThreshold != intThreshold)
                    m_intAbsoluteThreshold = intThreshold;

                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 4=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                return m_intNumSelectedObject;
            }
            catch (EException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
            catch (ApplicationException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }

        }

        public int BuildLeadObjects_Filter_GetElement(ROI objROI, bool blnBlackOnWhite, bool blnConnexity4, int intSegmenterMode, int intThreshold,
            int intAreaLow, int intAreaHigh, bool blnRemoveBorder, int intCriteria)
        {
            try
            {
                HiPerfTimer m_T1 = new HiPerfTimer();
                m_T1.Start();
                string m_strTrack = "";
                float m_fTimingPrev = 0;
                float m_fTiming = 0;

                CleanAllBlobs();    // Clear all selection first before build blobs
                m_intNumSelectedObject = 0;
                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;
                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenter = encoder.GrayscaleSingleThresholdSegmenter;

                // Set classification
                if (segmenter.BlackLayerEncoded != blnBlackOnWhite)
                    segmenter.BlackLayerEncoded = blnBlackOnWhite;
                if (segmenter.WhiteLayerEncoded != !blnBlackOnWhite)
                    segmenter.WhiteLayerEncoded = !blnBlackOnWhite;

                // Set threshold
                if (segmenter.Mode != (EGrayscaleSingleThreshold)intSegmenterMode)
                    segmenter.Mode = (EGrayscaleSingleThreshold)intSegmenterMode;  // 0=Absolute, 1=Relative, 2=MinResidue, 3=MaxEntropy, 4=Iso-Data

#if (Debug_2_12 || Release_2_12)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = (uint)intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 1, 1, (uint)objROI.ref_ROI.Width-2, (uint)objROI.ref_ROI.Height-2, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (segmenter.Mode == EGrayscaleSingleThreshold.Absolute)
                {
                    if (intThreshold == -4)
                    {
                        EBW8 objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
                        segmenter.AbsoluteThreshold = objBW8.Value;
                    }
                    else
                    {
                        if (segmenter.AbsoluteThreshold != intThreshold)
                            segmenter.AbsoluteThreshold = intThreshold;
                    }
                }

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 1=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                encoder.Encode(objROI.ref_ROI, codedImage);

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 2=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);

                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 1, 1, objROI.ref_ROI.Width - 2, objROI.ref_ROI.Height - 2, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#endif

                selection.Sort(EFeature.Area, ESortDirection.Descending);
                selection.AttachedImage = objROI.ref_ROI;

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 3=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                LoadElemet(intCriteria);


                if (m_intAbsoluteThreshold != intThreshold)
                    m_intAbsoluteThreshold = intThreshold;

                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                m_fTiming = m_T1.Timing;
                m_strTrack += ", 4=" + (m_fTiming - m_fTimingPrev).ToString();
                m_fTimingPrev = m_fTiming;

                return m_intNumSelectedObject;
            }
            catch (EException ex)
            {
                if (objROI.ref_ROI.TopParent != null)
                {
                    objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                }
                else
                {
                    STTrackLog.WriteLine("objROI's topParent is NULL");
                }
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
            catch (ApplicationException ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intThreshold =" + intThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }

        }
        public int BuildObjects_Filter_GetElement_DoubleThreshold(ROI objROI, bool blnBlackLayer, bool blnNewtralLayer, bool blnWhiteLayer, bool blnConnexity4,
            int intLowThreshold, int intHighThreshold, int intAreaLow, int intAreaHigh, bool blnRemoveBorder, int intCriteria)
        {
            try
            {
                if (!blnBlackLayer && blnNewtralLayer && !blnWhiteLayer)
                {
                    if (intHighThreshold != 255)
                    {
                        intHighThreshold++;
                    }
                    else
                    {
                        blnWhiteLayer = true;
                    }
                }

                m_intNumSelectedObject = 0;

                CleanAllBlobs();    // Clear all selection first before build blobs

                encoder = new EImageEncoder();

                encoder.SegmentationMethod = ESegmentationMethod.GrayscaleDoubleThreshold;

                if (blnConnexity4)
                    encoder.EncodingConnexity = EEncodingConnexity.Four;
                else
                    encoder.EncodingConnexity = EEncodingConnexity.Eight;

                segmenters = encoder.GrayscaleDoubleThresholdSegmenter;

                // Set classification
                if (segmenters.BlackLayerEncoded != blnBlackLayer)
                    segmenters.BlackLayerEncoded = blnBlackLayer;
                if (segmenters.NeutralLayerEncoded != blnNewtralLayer)
                    segmenters.NeutralLayerEncoded = false;
                if (segmenters.WhiteLayerEncoded != blnWhiteLayer)
                    segmenters.WhiteLayerEncoded = blnWhiteLayer;


                //segmenters.BlackLayerEncoded = true;

                //segmenters.NeutralLayerEncoded = true;

                // segmenters.WhiteLayerEncoded = true;
                // Set threshold

#if (Debug_2_12 || Release_2_12)
                if (segmenters.LowThreshold != intLowThreshold)
                    segmenters.LowThreshold = (uint)intLowThreshold;

                if (segmenters.HighThreshold != intHighThreshold)
                    segmenters.HighThreshold = (uint)intHighThreshold;

                encoder.Encode(objROI.ref_ROI, codedImage);

                int intTotalObjCount = 0;
                if (blnBlackLayer)
                    intTotalObjCount += (int)codedImage.GetObjCount(0);
                if (blnNewtralLayer)
                    intTotalObjCount += (int)codedImage.GetObjCount(1);
                if (blnWhiteLayer)
                    intTotalObjCount += (int)codedImage.GetObjCount(2);

                if (intTotalObjCount == 0)
                    return 0;


                selection.AddObjects(codedImage);
                
                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);
                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;


                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)objROI.ref_ROI.TopParent.Width, (uint)objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (segmenters.LowThreshold != intLowThreshold)
                    segmenters.LowThreshold = intLowThreshold;

                if (segmenters.HighThreshold != intHighThreshold)
                    segmenters.HighThreshold = intHighThreshold;

                encoder.Encode(objROI.ref_ROI, codedImage);

                int intTotalObjCount = 0;
                if (blnBlackLayer)
                    intTotalObjCount += (int)codedImage.GetObjCount(0);
                if (blnNewtralLayer)
                    intTotalObjCount += (int)codedImage.GetObjCount(1);
                if (blnWhiteLayer)
                    intTotalObjCount += (int)codedImage.GetObjCount(2);

                if (intTotalObjCount == 0)
                    return 0;


                selection.AddObjects(codedImage);

                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);
                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;


                if (blnRemoveBorder)
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.OutsideOrOnBorder);
                else
                    selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height, ERectangleMode.EntirelyOutside);

#endif

                selection.Sort(EFeature.Area, ESortDirection.Descending);

                m_intNumSelectedObject = (int)selection.ElementCount;
                m_intROIStartX = objROI.ref_ROI.TotalOrgX;
                m_intROIStartY = objROI.ref_ROI.TotalOrgY;

                LoadElemet(intCriteria);

                if (m_intAbsoluteLowThreshold != intLowThreshold)
                    m_intAbsoluteLowThreshold = intLowThreshold;

                if (m_intAbsoluteHighThreshold != intHighThreshold)
                {
                    if (intHighThreshold > 255)
                        m_intAbsoluteHighThreshold = 255;
                    else
                        m_intAbsoluteHighThreshold = intHighThreshold;
                }
                    

                if (m_intMinAreaLimit == intAreaLow)
                    m_intMinAreaLimit = intAreaLow;

                if (m_intMaxAreaLimit == intAreaHigh)
                    m_intMaxAreaLimit = intAreaHigh;

                if (blnConnexity4)
                    m_intConnexity = 4;
                else
                    m_intConnexity = 8;

                return m_intNumSelectedObject;
            }
            catch (Exception ex)
            {
                objROI.ref_ROI.Save("D:\\TS\\BuildBlobFailROI.bmp");
                STTrackLog.WriteLine("EBlobs->BuildObjects->intLowThreshold =" + intLowThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intHighThreshold =" + intHighThreshold);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaLow =" + intAreaLow);
                STTrackLog.WriteLine("EBlobs->BuildObjects->intAreaHigh =" + intAreaHigh);
                STTrackLog.WriteLine("EBlobs->BuildObjects->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->BuildObjects->" + ex.ToString());
                return 0;
            }
        }

        public void DrawSelectedBlobs(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            try
            {
                int colorIndex = 0;

                for (int i = 0; i < m_intNumSelectedObject; i++)
                {
                    if (m_objColor.Red != m_colorList[colorIndex].R)
                        m_objColor.Red = m_colorList[colorIndex].R;
                    if (m_objColor.Green != m_colorList[colorIndex].G)
                        m_objColor.Green = m_colorList[colorIndex].G;
                    if (m_objColor.Blue != m_colorList[colorIndex].B)
                        m_objColor.Blue = m_colorList[colorIndex].B;
#if (Debug_2_12 || Release_2_12)
                    codedImage.Draw(g, m_objColor, selection, (uint)i, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    codedImage.Draw(g, m_objColor, selection, i, fDrawingScaleX, fDrawingScaleY);
#endif


                    if (++colorIndex >= 10)
                        colorIndex = 0;
                }
            }
            catch
            {
            }
        }

        public void DrawSelectedBlobs(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor)
        {
            try
            {
                for (int i = 0; i < m_intNumSelectedObject; i++)
                {
                    if (m_objColor.Red != objColor.R)
                        m_objColor.Red = objColor.R;
                    if (m_objColor.Green != objColor.G)
                        m_objColor.Green = objColor.G;
                    if (m_objColor.Blue != objColor.B)
                        m_objColor.Blue = objColor.B;
#if (Debug_2_12 || Release_2_12)
                    codedImage.Draw(g, m_objColor, selection, (uint)i, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    codedImage.Draw(g, m_objColor, selection, i, fDrawingScaleX, fDrawingScaleY);
#endif

                }
            }
            catch
            {
            }
        }
        public void DrawSelectedBlobs(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor, float fPanX, float fPanY)
        {
            try
            {
                for (int i = 0; i < m_intNumSelectedObject; i++)
                {
                    if (m_objColor.Red != objColor.R)
                        m_objColor.Red = objColor.R;
                    if (m_objColor.Green != objColor.G)
                        m_objColor.Green = objColor.G;
                    if (m_objColor.Blue != objColor.B)
                        m_objColor.Blue = objColor.B;
#if (Debug_2_12 || Release_2_12)
                    codedImage.Draw(g, m_objColor, selection, (uint)i, fDrawingScaleX, fDrawingScaleY, fPanX, fPanY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    codedImage.Draw(g, m_objColor, selection, i, fDrawingScaleX, fDrawingScaleY, fPanX, fPanY);
#endif

                }
            }
            catch
            {
            }
        }
        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor)
        {
            try
            {
                if (m_objColor.Red != objColor.R)
                    m_objColor.Red = objColor.R;
                if (m_objColor.Green != objColor.G)
                    m_objColor.Green = objColor.G;
                if (m_objColor.Blue != objColor.B)
                    m_objColor.Blue = objColor.B;

                for (int i = 0; i < m_intNumSelectedObject; i++)
                {
#if (Debug_2_12 || Release_2_12)
                    if (selection != null)
                        codedImage.Draw(g, m_objColor, selection, (uint)i, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    if (selection != null)
                        codedImage.Draw(g, m_objColor, selection, i, fDrawingScaleX, fDrawingScaleY);
#endif

                }
            }
            catch
            {
            }
        }

        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor, int intObjectNum)
        {
            if (intObjectNum >= m_intNumSelectedObject)
                return;

            if (m_objColor.Red != objColor.R)
                m_objColor.Red = objColor.R;
            if (m_objColor.Green != objColor.G)
                m_objColor.Green = objColor.G;
            if (m_objColor.Blue != objColor.B)
                m_objColor.Blue = objColor.B;

#if (Debug_2_12 || Release_2_12)
            if (selection != null)
                codedImage.Draw(g, m_objColor, selection, (uint)intObjectNum, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (selection != null)
                codedImage.Draw(g, m_objColor, selection, intObjectNum, fDrawingScaleX, fDrawingScaleY);
#endif

        }

        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor, int intObjectNum, int[] arrSelectedBlob)
        {
            if (intObjectNum >= m_intNumSelectedObject)
                return;

            if (m_objColor.Red != objColor.R)
                m_objColor.Red = objColor.R;
            if (m_objColor.Green != objColor.G)
                m_objColor.Green = objColor.G;
            if (m_objColor.Blue != objColor.B)
                m_objColor.Blue = objColor.B;

            if (selection != null && arrSelectedBlob != null)
            {
                for (int i = 0; i < arrSelectedBlob.Length; i++)
                {
#if (Debug_2_12 || Release_2_12)
                    codedImage.Draw(g, m_objColor, selection, (uint)arrSelectedBlob[i], fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    codedImage.Draw(g, m_objColor, selection, arrSelectedBlob[i], fDrawingScaleX, fDrawingScaleY);
#endif

                }
            }
        }

        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intColorIndex, int intObjectNum)
        {
            if (intObjectNum >= m_intNumSelectedObject)
                return;

            int intCount = (int)codedImage.GetObjCount();
            intColorIndex %= 10;

            if (m_objColor.Red != m_colorList[intColorIndex].R)
                m_objColor.Red = m_colorList[intColorIndex].R;
            if (m_objColor.Green != m_colorList[intColorIndex].G)
                m_objColor.Green = m_colorList[intColorIndex].G;
            if (m_objColor.Blue != m_colorList[intColorIndex].B)
                m_objColor.Blue = m_colorList[intColorIndex].B;
#if (Debug_2_12 || Release_2_12)
            if (selection != null)
                codedImage.Draw(g, m_objColor, selection, (uint)intObjectNum, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (selection != null)
                codedImage.Draw(g, m_objColor, selection, intObjectNum, fDrawingScaleX, fDrawingScaleY);
#endif

        }

        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intColorIndex, int intObjectNum, int[] arrSelectedBlob)
        {
            if (intObjectNum >= m_intNumSelectedObject)
                return;

            int intCount = (int)codedImage.GetObjCount();
            intColorIndex %= 10;

            if (m_objColor.Red != m_colorList[intColorIndex].R)
                m_objColor.Red = m_colorList[intColorIndex].R;
            if (m_objColor.Green != m_colorList[intColorIndex].G)
                m_objColor.Green = m_colorList[intColorIndex].G;
            if (m_objColor.Blue != m_colorList[intColorIndex].B)
                m_objColor.Blue = m_colorList[intColorIndex].B;

            if (selection != null && arrSelectedBlob != null)
            {
                for (int i = 0; i < arrSelectedBlob.Length; i++)
                {
#if (Debug_2_12 || Release_2_12)
                    codedImage.Draw(g, m_objColor, selection, (uint)arrSelectedBlob[i], fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    codedImage.Draw(g, m_objColor, selection, arrSelectedBlob[i], fDrawingScaleX, fDrawingScaleY);
#endif

                }
            }
        }

        public void DrawSelectedBlobHole(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor, int intObjectNum, int intHoleIndex)
        {
            if (intObjectNum >= m_intNumSelectedObject)
                return;

            if (m_objColor.Red != objColor.R)
                m_objColor.Red = objColor.R;
            if (m_objColor.Green != objColor.G)
                m_objColor.Green = objColor.G;
            if (m_objColor.Blue != objColor.B)
                m_objColor.Blue = objColor.B;

#if (Debug_2_12 || Release_2_12)
            if (selection != null)
                codedImage.DrawHole(g, m_objColor, (uint)intObjectNum, (uint)intHoleIndex, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (selection != null)
                codedImage.DrawHole(g, m_objColor, intObjectNum, intHoleIndex, fDrawingScaleX, fDrawingScaleY);
#endif

        }
        public void DrawAllSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intColorIndex)
        {
            intColorIndex %= 10;

            if (m_objColor.Red != m_colorList[intColorIndex].R)
                m_objColor.Red = m_colorList[intColorIndex].R;
            if (m_objColor.Green != m_colorList[intColorIndex].G)
                m_objColor.Green = m_colorList[intColorIndex].G;
            if (m_objColor.Blue != m_colorList[intColorIndex].B)
                m_objColor.Blue = m_colorList[intColorIndex].B;
#if (Debug_2_12 || Release_2_12)
            if (selection != null)
                codedImage.Draw(g, m_objColor, selection, fDrawingScaleX, fDrawingScaleY);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (selection != null)
                codedImage.Draw(g, m_objColor, selection, fDrawingScaleX, fDrawingScaleY);
#endif

        }
        public void CleanAllBlobs()
        {
            if (segmenter != null)
            {
                segmenter.Dispose();
                segmenter = null;
            }

            if (segmenters != null)
            {
                segmenters.Dispose();
                segmenters = null;
            }

            if (encoder != null)
            {
                encoder.Dispose();
                encoder = null;
            }

            if (codedImage != null)
            {
                codedImage.ClearFeatureCache();
            }

            if (selection != null)
            {
                selection.Clear();
                selection.ClearFeatureCache();
            }

            if (encoder != null)
            {
                encoder.Dispose();
                encoder = null;
            }
        }

        public void Dispose()
        {
            if (selection != null)
                selection.Dispose();

            if (encoder != null)
                encoder.Dispose();

            if (codedImage != null)
                codedImage.Dispose();

        }

        public bool DisposeElement(int intIndex)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                selection.GetElement((uint)intIndex).Dispose();
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                selection.GetElement(intIndex).Dispose();
#endif

                return true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("EBlobs -> DisposeElement -> Ex =" + ex.ToString());
                return false;
            }
        }

        public bool CheckHole(int intBlobIndex, int intHoleMinSize, ref int intArea, ref int intHoleIndex)
        {
            try
            {
                if (codedImage.GetObjCount() > intBlobIndex)
                {
#if (Debug_2_12 || Release_2_12)
                    EObject blob = codedImage.GetObj((uint)intBlobIndex);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EObject blob = codedImage.GetObj(intBlobIndex);
#endif

                    int intCount = (int)blob.HoleCount;

                    if (intCount == 0)
                    {
                        blob.Dispose();
                        return false;
                    }

                    for (intHoleIndex = 0; intHoleIndex < intCount; intHoleIndex++)
                    {
#if (Debug_2_12 || Release_2_12)
                        EHole eHole = blob.GetHole((uint)intHoleIndex);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EHole eHole = blob.GetHole(intHoleIndex);
#endif

                        if (eHole.Area > intHoleMinSize)
                        {
                            intArea = (int)eHole.Area;
                            eHole.Dispose();
                            blob.Dispose();
                            return true;
                        }
                        eHole.Dispose();
                    }

                    blob.Dispose();
                    return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("EBlobs->GetHoleCount->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->GetHoleCount->" + ex.ToString());
                return false;
            }
        }

        public void GetBlobsHole(int intBlobIndex, ref float HoleCenterX, ref float HoleCenterY, ref float HoleArea)
        {
            try
            {
                if (codedImage.GetObjCount() > intBlobIndex)
                {
#if (Debug_2_12 || Release_2_12)
                    EObject blob = codedImage.GetObj((uint)intBlobIndex);

                    int intCount = (int)blob.HoleCount;

                    for (uint holeIndex = 0; holeIndex < intCount; holeIndex++)
                    {
                        HoleCenterX = blob.GetHole(holeIndex).BoundingBoxCenterX;
                        HoleCenterY = blob.GetHole(holeIndex).BoundingBoxCenterY;
                        HoleArea += blob.GetHole(holeIndex).Area;
                        blob.GetHole(holeIndex).Dispose();
                    }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EObject blob = codedImage.GetObj(intBlobIndex);

                    int intCount = (int)blob.HoleCount;

                    for (int holeIndex = 0; holeIndex < intCount; holeIndex++)
                    {
                        HoleCenterX = blob.GetHole(holeIndex).BoundingBoxCenterX;
                        HoleCenterY = blob.GetHole(holeIndex).BoundingBoxCenterY;
                        HoleArea += blob.GetHole(holeIndex).Area;
                        blob.GetHole(holeIndex).Dispose();
                    }
#endif

                    //return true;
                }
                // else
                // return false;
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("EBlobs->GetHoleCount->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->GetHoleCount->" + ex.ToString());
            //    return false;
            }
        }
        public bool GetHoleCount(int intBlobIndex, int intHoleMinSize, ref List<PointF> arrHoleCenterPoint, ref List<SizeF> arrHoleSize)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                if (codedImage.GetObjCount() > intBlobIndex)
                {
                    EObject blob = codedImage.GetObj((uint)intBlobIndex);

                    int intCount = (int)blob.HoleCount;

                    for (uint holeIndex = 0; holeIndex < intCount; holeIndex++)
                    {
                        if (blob.GetHole(holeIndex).Area > intHoleMinSize)
                        {
                            arrHoleCenterPoint.Add(new PointF(blob.GetHole(holeIndex).BoundingBoxCenterX, blob.GetHole(holeIndex).BoundingBoxCenterY));
                            arrHoleSize.Add(new SizeF(blob.GetHole(holeIndex).BoundingBoxWidth, blob.GetHole(holeIndex).BoundingBoxHeight));
                        }
                        blob.GetHole(holeIndex).Dispose();
                    }

                    return true;
                }
                else
                    return false;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (codedImage.GetObjCount() > intBlobIndex)
                {
                    EObject blob = codedImage.GetObj(intBlobIndex);

                    int intCount = (int)blob.HoleCount;

                    for (int holeIndex = 0; holeIndex < intCount; holeIndex++)
                    {
                        if (blob.GetHole(holeIndex).Area > intHoleMinSize)
                        {
                            arrHoleCenterPoint.Add(new PointF(blob.GetHole(holeIndex).BoundingBoxCenterX, blob.GetHole(holeIndex).BoundingBoxCenterY));
                            arrHoleSize.Add(new SizeF(blob.GetHole(holeIndex).BoundingBoxWidth, blob.GetHole(holeIndex).BoundingBoxHeight));
                        }
                        blob.GetHole(holeIndex).Dispose();
                    }

                    return true;
                }
                else
                    return false;
#endif

            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("EBlobs->GetHoleCount->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->GetHoleCount->" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intCriteria">0x01=Area, 0x02=Gravity Center Point, 0x04=Bounding Center Point, 0x08=Bounding Size, 0x10Contour </param>
        /// <returns></returns>
        public bool LoadElemet(int intCriteria)
        {
            m_arrContourX.Clear();
            m_arrContourY.Clear();
            m_arrGravityCenterX.Clear();
            m_arrGravityCenterY.Clear();
            m_arrLimitCenterX.Clear();
            m_arrLimitCenterY.Clear();
            m_arrWidth.Clear();
            m_arrHeight.Clear();
            m_arrRectLimitCenterX.Clear();
            m_arrRectLimitCenterY.Clear();
            m_arrRectWidth.Clear();
            m_arrRectHeight.Clear();
            m_arrArea.Clear();
            m_arrRectAngle.Clear();
	        m_arrEccentricity.Clear();
            m_arrAverageGray.Clear();

            bool blnWantArea = ((intCriteria & 0x01) > 0);
            bool blnWantGravityCenterPoint = ((intCriteria & 0x02) > 0);
            bool blnWantBoundingCenterPoint = ((intCriteria & 0x04) > 0);
            bool blnWantBoundingSize = ((intCriteria & 0x08) > 0);
            bool blnWantContour = ((intCriteria & 0x10) > 0);
            bool blnWantMinRectCenterPoint = ((intCriteria & 0x20) > 0);
            bool blnWantMinRectSize = ((intCriteria & 0x40) > 0);
            bool blnWantMinRectAngle = ((intCriteria & 0x80) > 0);
            bool blnWantTotalArea = ((intCriteria & 0x101) > 0);    // need to make sure WantArea is ON true. Then can direct get the area from array, not from blob itself which is consume time. 
            bool blnWantTotalBoundingSizeAndCenter = ((intCriteria & 0x20C) > 0); // need to make sure WantBounding size and center point is ON true. Then can direct get the bounding info from array, not from blob itself which is consume time. 
            bool blnWantEccentricity = ((intCriteria & 0x400) > 0);
            bool blnWantAverageGray = ((intCriteria & 0x800) > 0);
            float fBoundingStartX = -1;
            float fBoundingStartY = -1;
            float fBoundingEndX = -1;
            float fBoundingEndY = -1;

            for (int i = 0; i < m_intNumSelectedObject; i++)
            {
#if (Debug_2_12 || Release_2_12)
                ECodedElement element = selection.GetElement((uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                ECodedElement element = selection.GetElement(i);

#endif

                if (blnWantArea)
                {
                    m_arrArea.Add((int)element.Area);

                    if (blnWantTotalArea)
                    {
                        m_intTotalArea += m_arrArea[i];
                    }
                }

                if (blnWantGravityCenterPoint)
                {
                    m_arrGravityCenterX.Add(element.GravityCenterX);
                    m_arrGravityCenterY.Add(element.GravityCenterY);
                }

                if (blnWantBoundingCenterPoint)
                {
                    m_arrLimitCenterX.Add(element.BoundingBoxCenterX);
                    m_arrLimitCenterY.Add(element.BoundingBoxCenterY);
                }

                if (blnWantBoundingSize)
                {
                    m_arrWidth.Add(element.BoundingBoxWidth + 1);
                    m_arrHeight.Add(element.BoundingBoxHeight + 1);
                }

                if (blnWantBoundingSize && blnWantBoundingCenterPoint && blnWantTotalBoundingSizeAndCenter)
                {
                    float fStartX = m_arrLimitCenterX[i] - m_arrWidth[i] / 2;
                    float fStartY = m_arrLimitCenterY[i] - m_arrHeight[i] / 2;
                    float fEndX = m_arrLimitCenterX[i] + m_arrWidth[i] / 2;
                    float fEndY = m_arrLimitCenterY[i] + m_arrHeight[i] / 2;

                    if (fBoundingStartX == -1 || fStartX < fBoundingStartX)
                    {
                        fBoundingStartX = fStartX;
                    }

                    if (fBoundingStartY == -1 || fStartY < fBoundingStartY)
                    {
                        fBoundingStartY = fStartY;
                    }

                    if (fBoundingEndX == -1 || fEndX > fBoundingEndX)
                    {
                        fBoundingEndX = fEndX;
                    }

                    if (fBoundingEndY == -1 || fEndY > fBoundingEndY)
                    {
                        fBoundingEndY = fEndY;
                    }
                }

                if (blnWantContour)
                {
                    m_arrContourX.Add(element.ContourX);
                    m_arrContourY.Add(element.ContourY);
                }

                if (blnWantMinRectCenterPoint)
                {
                    m_arrRectLimitCenterX.Add(element.MinimumEnclosingRectangleCenterX);
                    m_arrRectLimitCenterY.Add(element.MinimumEnclosingRectangleCenterY);
                }

                if (blnWantMinRectSize)
                {
                    m_arrRectWidth.Add(element.MinimumEnclosingRectangleWidth + 1);
                    m_arrRectHeight.Add(element.MinimumEnclosingRectangleHeight + 1);
                }

                if (blnWantMinRectAngle)
                {
                    m_arrRectAngle.Add(element.MinimumEnclosingRectangleAngle);
                }

                if (blnWantEccentricity)
                {
                    m_arrEccentricity.Add(element.Eccentricity);
                }

                if (blnWantAverageGray)
                    m_arrAverageGray.Add(element.ComputePixelGrayAverage(selection.AttachedImage));

                //EPathVector objPV = new EPathVector();
                //element.ComputeConvexHull(objPV);
                //for (int j = 0; j < objPV.NumElements; j++)
                //{
                //    //Point a = new Point(objPV.GetElement(j).X, objPV.GetElement(j).Y);
                //    STTrackLog.WriteLine(objPV.GetElement(j).X.ToString() + "," + objPV.GetElement(j).Y.ToString());
                //}

                element.Dispose();
                element = null;
            }

            if (blnWantBoundingSize && blnWantBoundingCenterPoint && blnWantTotalBoundingSizeAndCenter)
            {
                m_fTotalLimitWidth = fBoundingEndX - fBoundingStartX;
                m_fTotalLimitHeight = fBoundingEndY - fBoundingStartY;
                m_fTotalLimitCenterX = (fBoundingEndX + fBoundingStartX) / 2;
                m_fTotalLimitCenterY = (fBoundingEndY + fBoundingStartY) / 2;
            }

            return true;
        }

        public int FilterObject(int intAreaLow, int intAreaHigh)
        {
            try
            {
                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection = new EObjectSelection();
                selection.AddObjects(codedImage);

#if (Debug_2_12 || Release_2_12)
                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);
                selection.Sort(EFeature.Area, ESortDirection.Descending);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);
                selection.Sort(EFeature.Area, ESortDirection.Descending);
#endif


                m_intNumSelectedObject = (int)selection.ElementCount;

                return m_intNumSelectedObject;
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("EBlobs->FilterObject->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->FilterObject->" + ex.ToString());

                return 0;
            }
        }

        public int FilterObject(int intAreaLow, int intAreaHigh, int intRectWidth, int intRectHeight)
        {
            try
            {
                if (codedImage.GetObjCount() == 0)
                    return 0;

                selection = new EObjectSelection();
                selection.AddObjects(codedImage);

#if (Debug_2_12 || Release_2_12)
                 // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, (uint)intAreaLow, (uint)intAreaHigh, EDoubleThresholdMode.Outside);

                selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, (uint)intRectWidth, (uint)intRectHeight, ERectangleMode.OutsideOrOnBorder);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Remove the Small blobs 
                selection.RemoveUsingUnsignedIntegerFeature(EFeature.Area, intAreaLow, intAreaHigh, EDoubleThresholdMode.Outside);

                selection.RemoveObjectsUsingRectangle(codedImage, 0, 0, intRectWidth, intRectHeight, ERectangleMode.OutsideOrOnBorder);

#endif

                selection.Sort(EFeature.Area, ESortDirection.Descending);
                return (int)selection.ElementCount;
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("EBlobs->FilterObject->" + ex.ToString());
                SRMMessageBox.Show("EBlobs->FilterObject->" + ex.ToString());

                return 0;
            }
        }

        public void SetObjectAreaRange(int intMinArea, int intMaxArea)
        {
            if (m_intMinAreaLimit != intMinArea)
                m_intMinAreaLimit = intMinArea;
            if (m_intMaxAreaLimit != intMaxArea)
                m_intMaxAreaLimit = intMaxArea;
        }

        public int GetTopBlob()
        {
            int intIndex = 0;
            for (int i = 0; i < m_intNumSelectedObject; i++)
            {
                if (intIndex != i)
                    if ((m_arrLimitCenterY[intIndex] - m_arrHeight[intIndex] / 2) > (m_arrLimitCenterY[i] - m_arrHeight[i] / 2))
                        intIndex = i;
            }
            return intIndex;
        }
        public int GetBottomBlob()
        {
            int intIndex = 0;
            for (int i = 0; i < m_intNumSelectedObject; i++)
            {
                if (intIndex != i)
                    if ((m_arrLimitCenterY[intIndex] + m_arrHeight[intIndex] / 2) < (m_arrLimitCenterY[i] + m_arrHeight[i] / 2))
                        intIndex = i;
            }
            return intIndex;
        }
        public int GetLeftBlob()
        {
            int intIndex = 0;
            for (int i = 0; i < m_intNumSelectedObject; i++)
            {
                if (intIndex != i)
                    if ((m_arrLimitCenterX[intIndex] - m_arrWidth[intIndex] / 2) > (m_arrLimitCenterX[i] - m_arrWidth[i] / 2))
                        intIndex = i;
            }
            return intIndex;
        }
        public int GetRightBlob()
        {
            int intIndex = 0;
            for (int i = 0; i < m_intNumSelectedObject; i++)
            {
                if (intIndex != i)
                    if ((m_arrLimitCenterX[intIndex] + m_arrWidth[intIndex] / 2) < (m_arrLimitCenterX[i] + m_arrWidth[i] / 2))
                        intIndex = i;
            }
            return intIndex;
        }

        public void SaveLearnIndividualBlobsImage(ROI objROI, List<float> arrBuildStartX, List<float> arrBuildStartY, List<float> arrBuildEndX, List<float> arrBuildEndY, string strpath, List<int> arrLengthMode, List<int> arrObjectNo,
                                                  ImageDrawing ImgWhiteImage, ImageDrawing ImgBlackImage)
        {
            try
            {

                lock (m_Lock)
                {
                    ImageDrawing objImage = new ImageDrawing(true, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                    ImgBlackImage.CopyTo(objImage);
                    ImageDrawing objImageWhite = new ImageDrawing(true, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                    ImgWhiteImage.CopyTo(objImageWhite);
                    List<int> arrSkipNo = new List<int>();
                    for (int i = 0; i < m_intNumSelectedObject; i++)
                    {
                        float fWidth = m_arrWidth[i], fHeight = m_arrHeight[i], fCenterX = m_arrLimitCenterX[i], fCenterY = m_arrLimitCenterY[i];
                        float fStartX = 0, fStartY = 0, fEndX = 0, fEndY = 0;
                        //fCenterX += objROI.ref_ROITotalX;
                        //fCenterY += objROI.ref_ROITotalY;
                        //if (intLengthMode == 1)
                        //{
                        //    fStartX = fCenterX - fHeight / 2;
                        //    fStartY = fCenterY - fWidth / 2;
                        //    fEndX = fCenterX + fHeight / 2;
                        //    fEndY = fCenterY + fWidth / 2;
                        //}
                        //else
                        {
                            fStartX = fCenterX - fWidth / 2;
                            fStartY = fCenterY - fHeight / 2;
                            fEndX = fCenterX + fWidth / 2;
                            fEndY = fCenterY + fHeight / 2;
                        }
                        for (int j = 0; j < arrBuildStartX.Count; j++)//2021-11-20 ZJYEOH : Should use arrBuildStartX.Count instead of m_intNumSelectedObject
                        {
                            if (arrSkipNo.Contains(j))
                                continue;

                            if (fStartX == (arrBuildStartX[j]) &&
                            fEndX == (arrBuildEndX[j]) &&
                            fStartY == (arrBuildStartY[j]) &&
                            fEndY == (arrBuildEndY[j])
                            )
                            {
                                arrSkipNo.Add(j);

                                if (m_objColor.Red != Color.White.R)
                                    m_objColor.Red = Color.White.R;
                                if (m_objColor.Green != Color.White.G)
                                    m_objColor.Green = Color.White.G;
                                if (m_objColor.Blue != Color.White.B)
                                    m_objColor.Blue = Color.White.B;

                                IntPtr ptr = Easy.OpenImageGraphicContext(objImage.ref_objMainImage);

#if (Debug_2_12 || Release_2_12)
                                codedImage.Draw(ptr, m_objColor, selection, (uint)arrObjectNo[j], 1.0f, 1.0f);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                   codedImage.Draw(ptr, m_objColor, selection, arrObjectNo[j], 1.0f, 1.0f);
#endif

                                Easy.CloseImageGraphicContext(objImage.ref_objMainImage, ptr);
                                //objImage.SaveImage(strpath);
                                ROI objPadROI = new ROI();
                                objPadROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                                objPadROI.AttachImage(objImage);
                                objPadROI.SaveImage(strpath + "_" + j + ".bmp");
                                objPadROI.Dispose();
                                ImageDrawing.SubtractImage(objImageWhite, objImage);
                                ImgBlackImage.CopyTo(objImage);
                                break;
                            }
                        }
                    }
                    ROI objPadROIWhite = new ROI();
                    objPadROIWhite.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                    objPadROIWhite.AttachImage(objImageWhite);
                    objPadROIWhite.SaveImage(strpath + ".bmp");
                    objPadROIWhite.Dispose();
                    objImage.Dispose();
                    objImageWhite.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void DrawBlobOnImage(ROI objROI, ImageDrawing ImgWhiteImage, ImageDrawing ImgBlackImage)
        {
            try
            {

                lock (m_Lock)
                {
                    //ImageDrawing objImage = new ImageDrawing(true, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                    //ImgBlackImage.CopyTo(objImage);
                    //ImageDrawing objImageWhite = new ImageDrawing(true, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                    //ImgWhiteImage.CopyTo(objImageWhite);
                    //List<int> arrSkipNo = new List<int>();
                    for (int i = 0; i < m_intNumSelectedObject; i++)
                    {
                        if (m_objColor.Red != Color.White.R)
                            m_objColor.Red = Color.White.R;
                        if (m_objColor.Green != Color.White.G)
                            m_objColor.Green = Color.White.G;
                        if (m_objColor.Blue != Color.White.B)
                            m_objColor.Blue = Color.White.B;

                        IntPtr ptr = Easy.OpenImageGraphicContext(objROI.ref_ROI.TopParent);
#if (Debug_2_12 || Release_2_12)
                        codedImage.Draw(ptr, m_objColor, selection, (uint)i, 1.0f, 1.0f);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                   codedImage.Draw(ptr, m_objColor, selection, i, 1.0f, 1.0f);
#endif
                        Easy.CloseImageGraphicContext(objROI.ref_ROI.TopParent, ptr);
                        //objROI.SaveImage("D:\\objROI.bmp");
                        //ROI objPadROI = new ROI();
                        //objPadROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                        //objPadROI.AttachImage(objImage);
                        //objPadROI.Dispose();
                        //ImageDrawing.SubtractImage(objImageWhite, objImage);
                        //ImgBlackImage.CopyTo(objImage);
                    }
                    //ROI objPadROIWhite = new ROI();
                    //objPadROIWhite.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                    //objPadROIWhite.AttachImage(objImageWhite);
                    //objPadROIWhite.Dispose();
                    //objImage.Dispose();
                    //objImageWhite.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void RemoveBlobElement(int intIndex)
        {
            if (selection == null || intIndex >= selection.ElementCount)
                return;
            for (int i = m_intNumSelectedObject - 1; i >= 0; i--)
            {
                if (intIndex == i)
                {
#if (Debug_2_12 || Release_2_12)
                    ECodedElement element = selection.GetElement((uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    ECodedElement element = selection.GetElement(i);

#endif

                    selection.RemoveUsingUnsignedIntegerFeature(EFeature.ElementIndex, element.ElementIndex, ESingleThresholdMode.Equal);
                    selection.Sort(EFeature.Area, ESortDirection.Descending);
                    m_intNumSelectedObject--;
                    element.Dispose();
                    element = null;
                    break;
                }
            }
        }
    }
}
