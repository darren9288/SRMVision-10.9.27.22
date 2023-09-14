using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Collections;

namespace VisionProcessing
{
    public class Position
    {
        #region Member Variables

        private bool m_bFinalResult = false;
        private bool m_bEmptyUnit = false;
        private int m_intPositionImageIndex = 0;
        private int m_intEmptyImageIndex = 0;
        private int m_intMinBorderScore = 100;
        private int m_intMinEmptyScore = 50;
        private int m_intDieAngleLimit = 0;
        private float m_fPositionXTolerance = 0;
        private float m_fPositionYTolerance = 0;
        private float m_fSampleDieWidth = 0;   // in um
        private float m_fSampleDieHeight = 0; // in um
        private float m_fTemplateDieWidth = 0;
        private float m_fTemplateDieHeight = 0;
        private float m_fGainValue = 1f;
        private float m_fGainValue2 = 1f; // Use for second image positioning test only
        private float m_fCompensateX = 0;
        private float m_fCompensateY = 0;
        private int m_intMethod;
        private int m_intPRSMode = 0;
        private int m_intFlipThreshold = 125;
        private int m_intFlipAreaLimit = 0;
        private int m_intImageIndex = 0;
        private int m_intEmptyThreshold = 128;
        private int m_intEmptyWhiteBlackArea = 0;
        private Line objVirtualLineWidth = new Line();
        private Line objVirtualLineHeight = new Line();
        private Line[] arrLines2 = new Line[8];

        private string m_strErrorMessage = "";
        private int m_intFailResultMask = 0;    // 0x01=Fail Unit Score, 0x02=Fail Gauge Measure Edge, 0x04=Fail Empty Unit Score, 0x08=Double Unit, 0x10=Fail Positioining Location and Angle, 0x20=Unit flipped

        private EMatcher m_objMatcher = new EMatcher();
        private EMatcher m_objEmptyMatcher = new EMatcher();
        private TrackLog m_objTrackLog = new TrackLog();

        // Orient
        private bool m_blnOrientON = false;
        private int m_intHighestScoreDirection = 0;
        private float m_fOrientMinScore = 50;
        private float m_fOrientPosX = 0;
        private float m_fOrientPosY = 0;
        private float m_fOrientPosTolerance = 5;    // Default tolerance 5 pixels
        private int m_intDirections = 4;        // 2=2 sides (for rectange unit), 4:4 sides (for square unit)
        private EMatcher m_objOrientMatcher;

        // result
        private float m_fObjectOffsetX = 0, m_fObjectOffsetY = 0;
        private float m_fObjectAngle = 0;
        private PointF[] m_pCorner = new PointF[4]; // 0 = top left corner, 1 = top right corner, 2 = bottom right corner, 3 = bottom left corner
        private PointF m_pObjectCenter = new PointF(0, 0);
        private PointF[] m_arrObjectCorner = new PointF[4];
        private Line[] m_objCrossLines = new Line[2];
        private float m_fObjectWidth = 0, m_fObjectHeight = 0;
        private float m_fObjectScore = 0;
        private EBlobs m_objEBlobs = new EBlobs();
        private Crosshair m_objCrosshair = new Crosshair(false);
        ImageDrawing[] m_objRotatedImage = new ImageDrawing[4];

        // PH
        private ArrayList m_arrDefectList = new ArrayList();
        private int m_intLargestBlob;
        private bool m_blnPHResult= false;
        private int m_intPHMinArea = 20;
        private int m_intPHBlackArea = 50;
        private int m_intPHBlobBlackArea = 50;
        private int m_intPHThreshold = 0;
        #endregion

        #region Properties
      
        public bool ref_bEmptyUnit { get { return m_bEmptyUnit; } set { m_bEmptyUnit = value; } }
        public int ref_intEmptyThreshold { get { return m_intEmptyThreshold; } set { m_intEmptyThreshold = value; } }
        public int ref_intEmptyWhiteArea { get { return m_intEmptyWhiteBlackArea; } set { m_intEmptyWhiteBlackArea = value; } }
        public int ref_intFlipThreshold { get { return m_intFlipThreshold; } set { m_intFlipThreshold = value; } }
        public int ref_intFlipAreaLimit { get { return m_intFlipAreaLimit; } set { m_intFlipAreaLimit = value; } }
        public int ref_intPositionImageIndex { get { return m_intPositionImageIndex; } set { m_intPositionImageIndex = value; } }
        public int ref_intEmptyImageIndex { get { return m_intEmptyImageIndex; } set { m_intEmptyImageIndex = value; } }
        public int ref_intMinBorderScore { get { return m_intMinBorderScore; } set { m_intMinBorderScore = value; } }
        public int ref_intMinEmptyScore { get { return m_intMinEmptyScore; } set { m_intMinEmptyScore = value; } }
        public int ref_intDieAngleLimit
        {
            get { return m_intDieAngleLimit; }
            set
            {
                m_intDieAngleLimit = value;
                m_objMatcher.MinAngle = -m_intDieAngleLimit;
                m_objMatcher.MaxAngle = m_intDieAngleLimit;
            }
        }
        public float ref_fPositionXTolerance { get { return m_fPositionXTolerance; } set { m_fPositionXTolerance = value; } }
        public float ref_fPositionYTolerance { get { return m_fPositionYTolerance; } set { m_fPositionYTolerance = value; } }

        public float ref_fSampleDieWidth { get { return m_fSampleDieWidth; } set { m_fSampleDieWidth = value; } }
        public float ref_fSampleDieHeight { get { return m_fSampleDieHeight; } set { m_fSampleDieHeight = value; } }
        public int ref_intMethod { get { return m_intMethod; } set { m_intMethod = value; } }
        public int ref_intPRSMode { get { return m_intPRSMode; } set { m_intPRSMode = value; } }
        public float ref_fGainValue { get { return m_fGainValue; } set { m_fGainValue = value; } }
        public float ref_fGainValue2 { get { return m_fGainValue2; } set { m_fGainValue2 = value; } }

        public float ref_fCompensateX { get { return m_fCompensateX; } set { m_fCompensateX = value; } }
        public float ref_fCompensateY { get { return m_fCompensateY; } set { m_fCompensateY = value; } }

        // Orient
        public int ref_intHighestScoreDirection { get { return m_intHighestScoreDirection; } set { m_intHighestScoreDirection = value; } }
        public float ref_fOrientMinScore { get { return m_fOrientMinScore; } set { m_fOrientMinScore = value; } }
        public float ref_fOrientPosX { get { return m_fOrientPosX; } set { m_fOrientPosX = value; } }
        public float ref_fOrientPosY { get { return m_fOrientPosY; } set { m_fOrientPosY = value; } }
        public float ref_fOrientPosTolerance { get { return m_fOrientPosTolerance; } set { m_fOrientPosTolerance = value; } }
        public int ref_intDirections { get { return m_intDirections; } set { m_intDirections = value; } }

        // result
        public PointF[] ref_arrObjectCorner { get { return m_arrObjectCorner; } set { m_arrObjectCorner = value; } }
        public PointF ref_fObjectCenter { get { return m_pObjectCenter; } }
        public float ref_fObjectCenterX { get { return m_pObjectCenter.X; } set { m_pObjectCenter.X = value; } }
        public float ref_fObjectCenterY { get { return m_pObjectCenter.Y; } set { m_pObjectCenter.Y = value; } }
        public float ref_fObjectWidth { get { return m_fObjectWidth; } set { m_fObjectWidth = value; } }
        public float ref_fObjectHeight { get { return m_fObjectHeight; } set { m_fObjectHeight = value; } }
        public float ref_fObjectOffsetX { get { return m_fObjectOffsetX; } set { m_fObjectOffsetX = value; } }
        public float ref_fObjectOffsetY { get { return m_fObjectOffsetY; } set { m_fObjectOffsetY = value; } }
        public float ref_fObjectAngle { get { return m_fObjectAngle; } set { m_fObjectAngle = value; } }
        public float ref_fObjectScore
        {
            get
            {
                if (m_objMatcher.NumPositions > 0)
                    return m_objMatcher.GetPosition(0).Score * 100;
                else
                    return 0;
            }
        }
        public float ref_fEmptyObjectScore
        {
            get
            {
                if (m_objEmptyMatcher.NumPositions > 0)
                    return Math.Max(0,m_objEmptyMatcher.GetPosition(0).Score) * 100;
                else
                    return 0;
            }
        }
        public bool ref_bFinalResult { get { return m_bFinalResult; } set { m_bFinalResult = value; } }

        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public int ref_intFailResultMask { get { return m_intFailResultMask; } }
        public Crosshair ref_objCrosshair { get { return m_objCrosshair; } set { m_objCrosshair = value; } }
        public int ref_intPHThreshold { get { return m_intPHThreshold; } set { m_intPHThreshold = value; } }
        public int ref_intPHBlackArea { get { return m_intPHBlackArea; } set { m_intPHBlackArea = value; } }
        public int ref_intPHBlobBlackArea { get { return m_intPHBlobBlackArea; } set { m_intPHBlobBlackArea = value; } }
        public int ref_intPHMinArea { get { return m_intPHMinArea; } set { m_intPHMinArea = value; } }
        public bool ref_blnDrawPHResult { get { return m_blnPHResult; } set { m_blnPHResult = value; } }
        public int ref_intLargestBlob { get { return m_intLargestBlob; } set { m_intLargestBlob = value; } }

        struct Defect
        {
            public string ref_strName;
            public float ref_fDimX;
            public float ref_fDimY;
            public float ref_fArea;
            public float ref_fCenterX;
            public float ref_fCenterY;
            public float ref_fAngle;
            public int ref_intBlobNo;
            public int ref_intFailMask; // 0x01=Fail WitdhInMM, 0x02=Fail HeightInMM, 0x04= Fail AreaInMM2
            public int ref_intFailedImage;
        }

        #endregion



        public Position(bool blnOnOrient)
        {
            for (int i = 0; i < 2; i++)
                m_objCrossLines[i] = new Line();

            for (int i = 0; i < 4; i++)
            {
                m_pCorner[i] = new PointF(0, 0);
                m_objRotatedImage[i] = new ImageDrawing();
            }

            if (blnOnOrient)
            {
                m_blnOrientON = true;
                m_objOrientMatcher = new EMatcher();
            }
        }
        public Position()
        {
            m_objEBlobs.ref_intAbsoluteThreshold = m_intPHThreshold;
            m_objEBlobs.ref_intConnexity = 4;
            m_objEBlobs.ref_intCriteria = 0x1F;   // area and object center
            m_objEBlobs.ref_intMinAreaLimit = m_intPHMinArea;
            m_objEBlobs.ref_intMinAreaLimit = 15000;
        }
        // Inspection
        public bool DoInspection_BottomPosition(ImageDrawing objImage, List<ROI> arrROIs, List<LGauge> arrLineGauges, float fCalibX, float fCalibY, 
            bool blnUseBlobsToFindPreLocation, bool blnIncludeLimitInspection, bool blnCheckDiffOrient, bool blnCheckDoubleUnit, bool blnCheckEmptyUnit, bool blnCheckUnitFlip)
        {
            m_bFinalResult = false;
            m_pObjectCenter = new PointF(0, 0);
            m_arrObjectCorner = new PointF[4];
            m_fObjectOffsetX = 0;
            m_fObjectOffsetY = 0;
            m_fObjectAngle = 0;
            m_strErrorMessage = "";
            m_intFailResultMask = 0;

            if (blnCheckEmptyUnit)
            {
                if (m_objEmptyMatcher.PatternLearnt)
                {
                    m_objEmptyMatcher.Match(arrROIs[0].ref_ROI);
                    if (m_objEmptyMatcher.NumPositions > 0)
                    {
                        if ((m_objEmptyMatcher.GetPosition(0).Score * 100) > m_intMinEmptyScore)
                        {
                            m_bEmptyUnit = true;
                            m_strErrorMessage = "Empty Unit. Set Score=" + m_intMinEmptyScore.ToString() + ", Result=" + m_objEmptyMatcher.GetPosition(0).Score * 100;
                            m_intFailResultMask |= 0x04;
                            return false;
                        }
                    }
                }
            }
            bool blnPositionFound;
            m_bEmptyUnit = false;

            if (m_intMethod == 1)
            {
                if (m_intPRSMode == 0)
                {
                    if (blnCheckDiffOrient)
                        blnPositionFound = InspectionWith4DirectionPRS(arrROIs[0], fCalibX, fCalibY, blnCheckDiffOrient, blnCheckDoubleUnit);
                    else
                        blnPositionFound = InspectionWith1DirectionPRS(arrROIs[0], fCalibX, fCalibY, blnCheckDoubleUnit);
                }
                else
                    blnPositionFound = InspectionWithPRS_LightSurface(arrROIs[0], fCalibX, fCalibY, blnCheckDiffOrient, blnCheckDoubleUnit);
            }
            else
            {
                blnPositionFound = InspectionWithLineGauges(objImage, arrLineGauges, fCalibX, fCalibY);
            }

            // Check flip
            if (blnPositionFound)
            {
                if (blnCheckUnitFlip)
                {
                    ImageDrawing m_objRotatedImage = new ImageDrawing();
                    ROI objlargeROI = new ROI();
                    objlargeROI.AttachImage(objImage);
                    objlargeROI.LoadAngleROISetting(m_fObjectAngle, m_pObjectCenter.X, m_pObjectCenter.Y,
                        m_fObjectWidth, m_fObjectHeight, arrROIs[0].ref_ROITotalX, arrROIs[0].ref_ROITotalY);
                    ROI.Rotate0Degree(objImage, objlargeROI, m_fObjectAngle, ref m_objRotatedImage);

                    ROI objUnitROI = new ROI();
                    objUnitROI.AttachImage(m_objRotatedImage);
                    objUnitROI.LoadROISetting((int)(m_pObjectCenter.X - m_fObjectWidth / 2), (int)(m_pObjectCenter.Y - m_fObjectHeight / 2),
                        (int)m_fObjectWidth, (int)m_fObjectHeight);
                    if (IsUnitFlip(objUnitROI))
                    {
                        m_strErrorMessage = "Unit is flipped.";
                        m_intFailResultMask |= 0x20;
                        return false;
                    }
                }
            }

            if (blnPositionFound)
            {
                if (blnIncludeLimitInspection)
                {
                    if ((m_fObjectOffsetX < -m_fPositionXTolerance) || (m_fObjectOffsetX > m_fPositionXTolerance))
                    {
                        m_strErrorMessage = "Out of Position X Tolerance. Set Min=" + -m_fPositionXTolerance + " Max=" + m_fPositionXTolerance + " Result=" + m_fObjectOffsetX;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    if ((m_fObjectOffsetY < -m_fPositionYTolerance) || (m_fObjectOffsetY > m_fPositionYTolerance))
                    {
                        m_strErrorMessage = "Out of Position Y Tolerance. Set Min=" + -m_fPositionYTolerance + " Max=" + m_fPositionYTolerance + " Result=" + m_fObjectOffsetY;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    if ((m_fObjectAngle < -m_intDieAngleLimit) || (m_fObjectAngle > m_intDieAngleLimit))
                    {
                        m_strErrorMessage = "Out of Angle limit. Set Min=" + -m_intDieAngleLimit + " Max=" + m_intDieAngleLimit + " Result=" + m_fObjectAngle;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    return true;
                }
                else
                    return true;
            }
            else
                return false;
        }

        // Inspection
        public bool DoInspection_BottomPositionOrient(ImageDrawing objImage, List<ROI> arrROIs, List<LGauge> arrLineGauges, float fCalibX, float fCalibY,
            bool blnUseBlobsToFindPreLocation, bool blnIncludeLimitInspection, bool blnCheckDiffOrient, bool blnCheckDoubleUnit, bool blnCheckEmptyUnit, 
            bool blnCheckUnitFlip, bool blnTestUnitOrientation)
        {
            m_bFinalResult = false;
            m_pObjectCenter = new PointF(0, 0);
            m_arrObjectCorner = new PointF[4];
            m_fObjectOffsetX = 0;
            m_fObjectOffsetY = 0;
            m_fObjectAngle = 0;
            m_strErrorMessage = "";
            m_intFailResultMask = 0;

            if (blnCheckEmptyUnit)
            {
                if (m_objEmptyMatcher.PatternLearnt)
                {
                    m_objEmptyMatcher.Match(arrROIs[0].ref_ROI);
                    if (m_objEmptyMatcher.NumPositions > 0)
                    {
                        if ((m_objEmptyMatcher.GetPosition(0).Score * 100) > m_intMinEmptyScore)
                        {
                            m_bEmptyUnit = true;
                            m_strErrorMessage = "Empty Unit. Set Score=" + m_intMinEmptyScore.ToString() + ", Result=" + m_objEmptyMatcher.GetPosition(0).Score * 100;
                            m_intFailResultMask |= 0x04;
                            return false;
                        }
                    }
                }
            }
            bool blnPositionFound;
            m_bEmptyUnit = false;

            if (m_intMethod == 1)
            {
                blnPositionFound = InspectionWith4DirectionPRSAndOrient(arrROIs[0], fCalibX, fCalibY, blnCheckDiffOrient, blnCheckDoubleUnit);             
            }
            else
            {
                blnPositionFound = InspectionWithLineGauges(objImage, arrLineGauges, fCalibX, fCalibY);
                if (blnTestUnitOrientation)
                {

                }
            }

            // Check flip
            if (blnPositionFound)
            {
                if (blnCheckUnitFlip)
                {
                    ImageDrawing m_objRotatedImage = new ImageDrawing();
                    ROI objlargeROI = new ROI();
                    objlargeROI.AttachImage(objImage);
                    objlargeROI.LoadAngleROISetting(m_fObjectAngle, m_pObjectCenter.X, m_pObjectCenter.Y,
                        m_fObjectWidth, m_fObjectHeight, arrROIs[0].ref_ROITotalX, arrROIs[0].ref_ROITotalY);
                    ROI.Rotate0Degree(objImage, objlargeROI, m_fObjectAngle, ref m_objRotatedImage);

                    ROI objUnitROI = new ROI();
                    objUnitROI.AttachImage(m_objRotatedImage);
                    objUnitROI.LoadROISetting((int)(m_pObjectCenter.X - m_fObjectWidth / 2), (int)(m_pObjectCenter.Y - m_fObjectHeight / 2),
                        (int)m_fObjectWidth, (int)m_fObjectHeight);
                    if (IsUnitFlip(objUnitROI))
                    {
                        m_strErrorMessage = "Unit is flipped.";
                        m_intFailResultMask |= 0x20;
                        return false;
                    }
                }
            }

            if (blnPositionFound)
            {
                if (blnIncludeLimitInspection)
                {
                    if ((m_fObjectOffsetX < -m_fPositionXTolerance) || (m_fObjectOffsetX > m_fPositionXTolerance))
                    {
                        m_strErrorMessage = "Out of Position X Tolerance. Set Min=" + -m_fPositionXTolerance + " Max=" + m_fPositionXTolerance + " Result=" + m_fObjectOffsetX;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    if ((m_fObjectOffsetY < -m_fPositionYTolerance) || (m_fObjectOffsetY > m_fPositionYTolerance))
                    {
                        m_strErrorMessage = "Out of Position Y Tolerance. Set Min=" + -m_fPositionYTolerance + " Max=" + m_fPositionYTolerance + " Result=" + m_fObjectOffsetY;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    if ((m_fObjectAngle < -m_intDieAngleLimit) || (m_fObjectAngle > m_intDieAngleLimit))
                    {
                        m_strErrorMessage = "Out of Angle limit. Set Min=" + -m_intDieAngleLimit + " Max=" + m_intDieAngleLimit + " Result=" + m_fObjectAngle;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    return true;
                }
                else
                    return true;
            }
            else
                return false;

            if (blnPositionFound)
            {
                if (blnTestUnitOrientation)
                {
                    if (m_intMethod == 1) // Find unit with PRS
                    {
                        // Define Orient Search ROI in Unit ROI
                        ROI objOrientSearchROI = new ROI();
                        objOrientSearchROI.AttachImage(arrROIs[0]);



                        if (blnCheckDiffOrient)
                        {
                            StartOrientInspection();
                        }
                        else
                        {
                            StartOrientInspection();
                        }
                    }
                    else // Find unit with Gauge
                    {
                        // Define Unit ROI

                        StartOrientInspection();
                    }
                }
            }
        }

        private bool StartOrientInspection()
        {
            return true;
        }

        public bool DoInspection_InPocket(ImageDrawing objImage, List<ROI> arrROIs, List<LGauge> arrLineGauges, float fCalibX, float fCalibY,
            bool blnUseBlobsToFindPreLocation, bool blnIncludeLimitInspection, bool blnWantAccuratePosition, bool blnCheckDoubleUnit, bool blnCheckEmptyUnit, bool blnCheckUnitFlip)
        {
            m_bFinalResult = false;
            m_pObjectCenter = new PointF(0, 0);
            m_arrObjectCorner = new PointF[4];
            m_fObjectOffsetX = 0;
            m_fObjectOffsetY = 0;
            m_fObjectAngle = 0;
            m_strErrorMessage = "";
            m_intFailResultMask = 0;

            arrROIs[0].AttachImage(objImage);

            bool blnPositionFound;
            m_bEmptyUnit = false;

            if (m_intMethod == 1)
            {
                if (m_intPRSMode == 0)
                {
                    blnPositionFound = InspectionPRS_IPM(arrROIs[0], fCalibX, fCalibY, blnWantAccuratePosition);
                }
                else
                    blnPositionFound = InspectionWithPRS_LightSurface(arrROIs[0], fCalibX, fCalibY, false, blnCheckDoubleUnit);
            }
            else
            {
                blnPositionFound = InspectionWithLineGauges_InPocket(objImage, arrLineGauges, fCalibX, fCalibY);
            }

            // Check flip
            if (blnPositionFound)
            {
                if (blnCheckUnitFlip)
                {
                    ImageDrawing m_objRotatedImage = new ImageDrawing();
                    ROI objlargeROI = new ROI();
                    objlargeROI.AttachImage(objImage);
                    objlargeROI.LoadAngleROISetting(m_fObjectAngle, m_pObjectCenter.X, m_pObjectCenter.Y,
                        m_fObjectWidth, m_fObjectHeight, arrROIs[0].ref_ROITotalX, arrROIs[0].ref_ROITotalY);
                    ROI.Rotate0Degree(objImage, objlargeROI, m_fObjectAngle, ref m_objRotatedImage);

                    ROI objUnitROI = new ROI();
                    objUnitROI.AttachImage(m_objRotatedImage);
                    objUnitROI.LoadROISetting((int)(m_pObjectCenter.X - m_fObjectWidth / 2), (int)(m_pObjectCenter.Y - m_fObjectHeight / 2),
                        (int)m_fObjectWidth, (int)m_fObjectHeight);
                    if (IsUnitFlip(objUnitROI))
                    {
                        m_strErrorMessage = "Unit is flipped.";
                        m_intFailResultMask |= 0x20;
                        return false;
                    }
                }
            }

            if (blnPositionFound)
            {
                if (blnIncludeLimitInspection)
                {
                    if ((m_fObjectOffsetX < -m_fPositionXTolerance) || (m_fObjectOffsetX > m_fPositionXTolerance))
                    {
                        m_strErrorMessage = "Out of Position X Tolerance. Set Min=" + -m_fPositionXTolerance + " Max=" + m_fPositionXTolerance + " Result=" + m_fObjectOffsetX;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    if ((m_fObjectOffsetY < -m_fPositionYTolerance) || (m_fObjectOffsetY > m_fPositionYTolerance))
                    {
                        m_strErrorMessage = "Out of Position Y Tolerance. Set Min=" + -m_fPositionYTolerance + " Max=" + m_fPositionYTolerance + " Result=" + m_fObjectOffsetY;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    if ((m_fObjectAngle < -m_intDieAngleLimit) || (m_fObjectAngle > m_intDieAngleLimit))
                    {
                        m_strErrorMessage = "Out of Angle limit. Set Min=" + -m_intDieAngleLimit + " Max=" + m_intDieAngleLimit + " Result=" + m_fObjectAngle;
                        m_intFailResultMask |= 0x10;
                        return false;
                    }

                    return true;
                }
                else
                    return true;
            }
            else
                return false;
        }

        public bool IsEmptyUnit_InPocket(ROI objROI)
        {
            /* To save Empty PRS inspection time, make sure the matcher setting is as following:
             * Angle = 0;
             * Final Reduction = 2;
             */

            try
            {

                if (m_objEmptyMatcher.PatternLearnt)
                {
                    if (m_objEmptyMatcher.MinAngle != 0)
                        m_objEmptyMatcher.MinAngle = 0;
                    if (m_objEmptyMatcher.MaxAngle != 0)
                        m_objEmptyMatcher.MaxAngle = 0;
                    if (m_objEmptyMatcher.FinalReduction != 2)
                        m_objEmptyMatcher.FinalReduction = 2;

                    m_objEmptyMatcher.Match(objROI.ref_ROI);
                    //objROI.ref_ROI.Save("D:\\ROI.bmp");
                    //m_objEmptyMatcher.Save("D:\\attern.mch");
                    if (m_objEmptyMatcher.NumPositions > 0)
                    {
                        if ((m_objEmptyMatcher.GetPosition(0).Score * 100) > m_intMinEmptyScore)
                        {
                            m_bEmptyUnit = true;
                            m_strErrorMessage = "InPocket Empty Pass. Set = " + m_intMinEmptyScore.ToString() + ", Result = " + Math.Max(0, m_objEmptyMatcher.GetPosition(0).Score) * 100;
                            //   SRMMessageBox.Show("Empty Unit. Set Score=" + m_intMinEmptyScore.ToString() + ", Result=" + m_objEmptyMatcher.GetPosition(0).Score * 100);
                            // m_intFailResultMask |= 0x04;
                            return true;
                        }
                        else
                        {
                            m_bEmptyUnit = false;
                            //  m_strErrorMessage = "Empty Unit Pass";
                            m_strErrorMessage = "InPocket No Empty Fail. Set = " + m_intMinEmptyScore.ToString() + ", Result = " + Math.Max(0, m_objEmptyMatcher.GetPosition(0).Score) * 100;
                            //   SRMMessageBox.Show("Empty Unit. Set Score=" + m_intMinEmptyScore.ToString() + ", Result=" + m_objEmptyMatcher.GetPosition(0).Score * 100);
                            m_intFailResultMask |= 0x04;
                            return false;
                        }
                    }
                }
                else
                {
                    m_strErrorMessage = "No Empty Pattern Learned!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("OriX=" + objROI.ref_ROI.OrgX +
                                     ", OriY=" + objROI.ref_ROI.OrgY +
                                     ", Width=" + objROI.ref_ROI.Width +
                                     ", Height=" + objROI.ref_ROI.Height);

                objROI.SaveImage("D:\\objROI.bmp");
                SRMMessageBox.Show("Position -> IsEmptyUnit_InPocket() -> Exception: " + ex.ToString());
                return false;
            }

            return false;
        }
        public void BuildObjects(ROI objROI)
        {
            int intSelectedObject = m_objEBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_intPHThreshold, m_intPHMinArea, 999999, true, 0x1F);
            if (intSelectedObject > 0)
            {


                float Max = 0;
               
                for (int j = 0; j < m_objEBlobs.ref_arrArea.Count; j++)
                {
                    float Area = m_objEBlobs.ref_arrArea[j];
                    if (Area > Max)
                    {
                        Max = Area;
                        m_intLargestBlob = j;

                    }

                }
                m_intPHBlobBlackArea = (int)Max;
            }

        }
        public void DrawPHObjects(Graphics g, float X, float Y , int i)
        {
            m_objEBlobs.DrawSelectedBlob(g, X, Y, Color.Cyan, i);
        }


        public bool CheckPH(ROI objROI)
        {
            m_arrDefectList.Clear();
            int intSelectedObject = m_objEBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_intPHThreshold,
                m_intPHMinArea, objROI.ref_ROIWidth * objROI.ref_ROIHeight, true, 0x1F);

            if (intSelectedObject > 0)
            {
                int intObjectArea = 0;
                float fObjectHeight = 0, fObjectWidth = 0, fObjectCenterX = 0, fObjectCenterY = 0;
                bool blnOverAllFail = false;
                float Max = 0;
                int intStartIndex = 0, intEndIndex = 0;
                for (int j = 0; j < m_objEBlobs.ref_arrArea.Count; j++)
                {
                    float Area = m_objEBlobs.ref_arrArea[j];
                    if (Area > Max)
                    {
                        Max = Area;
                        intStartIndex = j;
                        intEndIndex = intStartIndex + 1;
                    }

                }
                m_intPHBlobBlackArea = (int)Max;
                for (int i = intStartIndex; i < intEndIndex; i++)
                {
                    intObjectArea = m_objEBlobs.ref_arrArea[i];
                    fObjectHeight = m_objEBlobs.ref_arrHeight[i];
                    fObjectWidth = m_objEBlobs.ref_arrWidth[i];
                    fObjectCenterX = m_objEBlobs.ref_arrLimitCenterX[i];
                    fObjectCenterY = m_objEBlobs.ref_arrLimitCenterY[i];

                

                    bool blnFail = false;
                    int intFailMask = 0;
                    string strDefectName = "PH";


                    if (intObjectArea <= m_intPHBlackArea)
                    {
                        m_strErrorMessage = "* PH Area Fail. Set = " + (m_intPHBlackArea).ToString() + " pixels,   Result = " + (intObjectArea).ToString() + " pixels";
                        blnFail = true;
                        intFailMask |= 0x04;
                        strDefectName = "PH";
                    }


                    if (blnFail)
                    {
                        Defect objDefect = new Defect();
                        objDefect.ref_fDimX = fObjectWidth;
                        objDefect.ref_fDimY = fObjectHeight;
                        objDefect.ref_fArea = intObjectArea;
                        objDefect.ref_fCenterX = objROI.ref_ROITotalX + fObjectCenterX;
                        objDefect.ref_fCenterY = objROI.ref_ROITotalY + fObjectCenterY;
                        objDefect.ref_intBlobNo = i;
                        objDefect.ref_intFailMask = intFailMask;
                        objDefect.ref_strName = strDefectName;
                        objDefect.ref_intFailedImage = 1;
                        m_arrDefectList.Add(objDefect);

                        if (!blnOverAllFail)
                            blnOverAllFail = true;
                    }
                    else
                    {
                        Defect objDefect = new Defect();
                        objDefect.ref_fDimX = fObjectWidth;
                        objDefect.ref_fDimY = fObjectHeight;
                        objDefect.ref_fArea = intObjectArea;
                        objDefect.ref_fCenterX = objROI.ref_ROITotalX + fObjectCenterX;
                        objDefect.ref_fCenterY = objROI.ref_ROITotalY + fObjectCenterY;
                        objDefect.ref_intBlobNo = i;
                        objDefect.ref_intFailMask = 0;
                        objDefect.ref_strName = strDefectName;
                        objDefect.ref_intFailedImage = 1;
                        m_arrDefectList.Add(objDefect);
                    }
                }
               

               

                if (blnOverAllFail)
                {
                 //   m_objEBlobs.Dispose();
                    return false;
                }
                else
                {
                //    m_objEBlobs.Dispose();
                    return true;
                }
            }
            //  m_objEBlobs.Dispose();
            m_strErrorMessage = "* PH Area Fail. Set = " + (m_intPHBlackArea).ToString() + " pixels,   Result = 0 pixels";  // 2019 09 17 - CCENG : User dun know what is blob. "* PH Area Fail. No Blob Built.";
            return false;
        }
        public void GetDefectInfo(ref float fDimX, ref float fDimY, ref float fArea, ref int FailMask)
        {
            if (m_arrDefectList.Count == 0)
            {
                fArea = -1;
                return;
            }
            Defect objDefect = ((Defect)m_arrDefectList[0]);
            fDimX = objDefect.ref_fDimX;
            fDimY = objDefect.ref_fDimY;
            fArea = objDefect.ref_fArea;
            FailMask = objDefect.ref_intFailMask;
        }

        public void DrawPHDefectObjects(Graphics g, int intObjectIndex, float fDrawingScaleX, float fDrawingScaleY)
        {
            if (intObjectIndex < m_arrDefectList.Count)
            {
                Defect objDefect = (Defect)m_arrDefectList[intObjectIndex];
                
                    if (objDefect.ref_intFailMask > 0)
                    {
                        g.DrawRectangle(new Pen(Color.Red), (objDefect.ref_fCenterX - (objDefect.ref_fDimX / 2)) * fDrawingScaleX,
                                                            (objDefect.ref_fCenterY - (objDefect.ref_fDimY / 2)) * fDrawingScaleY,
                                                            objDefect.ref_fDimX * fDrawingScaleX, objDefect.ref_fDimY * fDrawingScaleY);
                    }
                    else
                    {
                        g.DrawRectangle(new Pen(Color.Lime), (objDefect.ref_fCenterX - (objDefect.ref_fDimX / 2)) * fDrawingScaleX,
                                                            (objDefect.ref_fCenterY - (objDefect.ref_fDimY / 2)) * fDrawingScaleY,
                                                            objDefect.ref_fDimX * fDrawingScaleX, objDefect.ref_fDimY * fDrawingScaleY);
                    }
                
            }
        }

        public bool IsUnitEmptyAfterThreshold_InPocket(ROI objROI)
        {
            // Get pixel count for high and low threshold pixel count


            int intBlackArea = ROI.GetPixelArea(objROI, m_intEmptyThreshold, 0);

            // For In Pocket, the pocket hole is black color and the pocket surface is white color.
            // If black color higher than Min Black color setting, then consider as empty pocket.
            if (intBlackArea > m_intEmptyWhiteBlackArea)
            {
                //Check is the blob touched the ROI corner, if yes mean some foreign object blocked the pocket
                int intSelectedObject = m_objEBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_intEmptyThreshold,
                    50, 999999, false, 0x0D);

                if (intSelectedObject > 0)
                {
                    float fObjectHeight = 0, fObjectWidth = 0, fObjectCenterX = 0, fObjectCenterY = 0;
                    fObjectHeight = m_objEBlobs.ref_arrHeight[0];
                    fObjectWidth = m_objEBlobs.ref_arrWidth[0];
                    fObjectCenterX = m_objEBlobs.ref_arrLimitCenterX[0];
                    fObjectCenterY = m_objEBlobs.ref_arrLimitCenterY[0];

                    float fObjArea = 0;
                    fObjArea = m_objEBlobs.ref_arrArea[0];

                    //if (((fObjectCenterX - fObjectWidth / 2) > 0 && (fObjectCenterY - fObjectHeight / 2) > 0) &&
                    //  ((fObjectCenterX + fObjectWidth / 2) < objROI.ref_ROIWidth && (fObjectCenterY - fObjectHeight / 2) > 0) &&
                    //   ((fObjectCenterX - fObjectWidth / 2) > 0 && (fObjectCenterY + fObjectHeight / 2) < objROI.ref_ROIHeight) &&
                    //    ((fObjectCenterX + fObjectWidth / 2) < objROI.ref_ROIWidth && (fObjectCenterY + fObjectHeight / 2) < objROI.ref_ROIHeight))
                    //{
                    //    return true;
                    //}
                    //else
                    //{
                    //    m_strErrorMessage = "NoEmpty. Wrong Threshold Setting or Foreign Object Present.";
                    //    return false;
                    //}

                    if (fObjectWidth >= objROI.ref_ROIWidth && fObjectHeight >= objROI.ref_ROIHeight && fObjArea / (objROI.ref_ROIWidth * objROI.ref_ROIHeight) >= 0.95f)
                    {
                        m_strErrorMessage = "NoEmpty. Wrong Threshold Setting or Foreign Object Present.";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            m_strErrorMessage = "NoEmpty. Set Black Area=" + m_intEmptyWhiteBlackArea.ToString() + ", Result=" + intBlackArea.ToString();
            return false;
        }

        public bool IsEmptyUnit(ROI objROI)
        {
            HiPerfTimer t1 = new HiPerfTimer();
            t1.Start();
            if (m_objEmptyMatcher.PatternLearnt)
            {
                m_objEmptyMatcher.Match(objROI.ref_ROI);
                if (m_objEmptyMatcher.NumPositions > 0)
                {
                    t1.Stop();
                    float ff = t1.Duration;
                    //m_objEmptyMatcher.Save("D:\\TS\\EmptyMatcher.mch");
                    //objROI.ref_ROI.Save("D:\\TS\\EmptyImagem.bmp");
                    if ((m_objEmptyMatcher.GetPosition(0).Score * 100) > m_intMinEmptyScore)
                    {
                        m_bEmptyUnit = true;
                        m_strErrorMessage = "Empty Unit. Set Score=" + m_intMinEmptyScore.ToString() + ", Result=" + m_objEmptyMatcher.GetPosition(0).Score * 100;
                        m_intFailResultMask |= 0x04;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsUnitFlip(ROI objROI)
        {
            // Get pixel count for high and low threshold pixel count
            int intLowThresholdPixelCount = 0, intBtwThresholdPixelCount = 0, intHighThresholdPixelCount = 0;
            EBW8 bwFlipThreshold = new EBW8((byte)m_intFlipThreshold);
            EasyImage.PixelCount(objROI.ref_ROI, bwFlipThreshold, bwFlipThreshold,
                                 out intLowThresholdPixelCount, out intBtwThresholdPixelCount, out intHighThresholdPixelCount);

            // SPM Unit: Unit is considered flipped if white pixel area lower than limit
            if (intHighThresholdPixelCount < m_intFlipAreaLimit)
            {
                return true;
            }

            return false;
        }

        public bool DoInspection_PadPosition(ImageDrawing objImage, List<ROI> arrROIs, List<LGauge> arrLineGauges, 
                                             float fCalibX, float fCalibY, 
                                             bool blnUseBlobsToFindPreLocation, bool blnIncludeLimitInspection, 
                                             bool blnCheckDiffOrient, bool blnCheckDoubleUnit)
        {
            try
            {   
                // Reset positioning result
                ResetPositionData();

                float fUnitWidth = m_fSampleDieWidth * fCalibX * 1000;
                float fUnitHeight = m_fSampleDieHeight * fCalibY * 1000;
                float fMinBorderScore = 50;

                m_bFinalResult = GetPositionByEdgeFlexiCorner(objImage, arrLineGauges, 
                                                              fUnitWidth, fUnitHeight, fMinBorderScore,
                                                              ref m_pObjectCenter, ref m_arrObjectCorner, 
                                                              ref m_fObjectWidth, ref m_fObjectHeight, 
                                                              ref m_fObjectAngle, ref m_strErrorMessage);

                if (m_bFinalResult)
                {
                    // Calculate object offset value
                    m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                    m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;

                    if (blnIncludeLimitInspection)
                    {
                        if ((m_fObjectOffsetX < -m_fPositionXTolerance) || (m_fObjectOffsetX > m_fPositionXTolerance))
                        {
                            m_strErrorMessage = "Out of Position X Tolerance. Set Min=" + -m_fPositionXTolerance + " Max=" + m_fPositionXTolerance + " Result=" + m_fObjectOffsetX;
                            m_intFailResultMask |= 0x10;
                            return false;
                        }

                        if ((m_fObjectOffsetY < -m_fPositionYTolerance) || (m_fObjectOffsetY > m_fPositionYTolerance))
                        {
                            m_strErrorMessage = "Out of Position Y Tolerance. Set Min=" + -m_fPositionYTolerance + " Max=" + m_fPositionYTolerance + " Result=" + m_fObjectOffsetY;
                            m_intFailResultMask |= 0x10;
                            return false;
                        }

                        if ((m_fObjectAngle < -m_intDieAngleLimit) || (m_fObjectAngle > m_intDieAngleLimit))
                        {
                            m_strErrorMessage = "Out of Angle limit. Set Min=" + -m_intDieAngleLimit + " Max=" + m_intDieAngleLimit + " Result=" + m_fObjectAngle;
                            m_intFailResultMask |= 0x10;
                            return false;
                        }
                        return true;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("DoInspection_PadPosition ex:" + ex.ToString());
                return false;
            }
        }

        public bool InspectionWithLineGauges(ImageDrawing objImage, List<LGauge> arrLineGauges, float fCalibX, float fCalibY)
        {
            try
            {
                ResetPositionData();
                float fUnitWidth = m_fSampleDieWidth * fCalibX * 1000;
                float fUnitHeight = m_fSampleDieHeight * fCalibY * 1000;
                float fMinBorderScore = 50;

                if (arrLineGauges.Count > 4)
                {
                    m_bFinalResult = GetPositionByEdgeFlexiCorner(objImage, arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore,
                                  ref m_pObjectCenter, ref m_arrObjectCorner, ref m_fObjectWidth, ref m_fObjectHeight, ref m_fObjectAngle, ref m_strErrorMessage);
                }
                else
                {
                    m_bFinalResult = GetPositionByEdge(objImage, arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore,
                                  ref m_pObjectCenter, ref m_arrObjectCorner, ref m_fObjectWidth, ref m_fObjectHeight, ref m_fObjectAngle, ref m_strErrorMessage);
                }

                if (m_bFinalResult)
                {
                    // Calibration value error
                    m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                    m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                    return true;
                }
                else
                {
                    m_intFailResultMask |= 0x02;
                    return false;
                }
            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("InspectionWithLineGauges ex: " + ex.ToString());
                return false;
            }
        }

        public bool InspectionWithLineGauges_InPocket(ImageDrawing objImage, List<LGauge> arrLineGauges, float fCalibX, float fCalibY)
        {
            try
            {
                ResetPositionData();
                float fUnitWidth = m_fSampleDieWidth * fCalibX * 1000;
                float fUnitHeight = m_fSampleDieHeight * fCalibY * 1000;
                float fMinBorderScore = 50;

                if (arrLineGauges.Count > 4)
                {
                    m_bFinalResult = GetPositionByEdgeFlexiCorner(objImage, arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore,
                                  ref m_pObjectCenter, ref m_arrObjectCorner, ref m_fObjectWidth, ref m_fObjectHeight, ref m_fObjectAngle, ref m_strErrorMessage);
                }
                else
                {
                    m_bFinalResult = GetPositionByEdge_InPocket(objImage, arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore,
                                  ref m_pObjectCenter, ref m_arrObjectCorner, ref m_fObjectWidth, ref m_fObjectHeight, ref m_fObjectAngle, ref m_strErrorMessage);
                }

                if (m_bFinalResult)
                {
                    // Calibration value error
                    m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                    m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                    return true;
                }
                else
                {
                    m_intFailResultMask |= 0x02;
                    return false;
                }
            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("InspectionWithLineGauges ex: " + ex.ToString());
                return false;
            }
        }

        public bool InspectionWith1DirectionPRS(ROI objSearchROI, float fCalibX, float fCalibY, bool blnCheckDoubleUnit)
        {
            try
            {
                ResetPositionData();

                m_objMatcher.Interpolate = true;
                m_objMatcher.FinalReduction = 0;
                if (m_intDieAngleLimit == 0)
                {
                    if (m_objMatcher.MinAngle != 0) m_objMatcher.MinAngle = 0;
                    if (m_objMatcher.MaxAngle != 0) m_objMatcher.MaxAngle = 0;
                }
                else
                {
                    if (m_objMatcher.MinAngle != -(m_intDieAngleLimit + 3)) m_objMatcher.MinAngle = -(m_intDieAngleLimit + 3);  // Add fix tolerance angle +-3
                    if (m_objMatcher.MaxAngle != m_intDieAngleLimit + 3) m_objMatcher.MaxAngle = m_intDieAngleLimit + 3;     // Add fix tolerance angle +-3
                }
                if (m_objMatcher.MaxPositions != 2) m_objMatcher.MaxPositions = 2;  // Set 2 for double unit test and get accurate positioining
                //if (blnCheckDoubleUnit)
                //{
                //    if (m_objMatcher.MaxPositions != 2) m_objMatcher.MaxPositions = 2;
                //}
                //else
                //{
                //    if (m_objMatcher.MaxPositions != 1) m_objMatcher.MaxPositions = 1;  
                //}

                m_objMatcher.Match(objSearchROI.ref_ROI);

                if (m_objMatcher.NumPositions > 0)
                {
                    m_fObjectScore = m_objMatcher.GetPosition(0).Score * 100;
                    if (m_fObjectScore >= m_intMinBorderScore)
                    {
                        if (blnCheckDoubleUnit)
                        {
                            float fScore2;
                            if (m_objMatcher.NumPositions > 1)
                            {
                                fScore2 = m_objMatcher.GetPosition(1).Score * 100;
                                if (fScore2 >= (m_fObjectScore - 10))
                                {
                                    m_strErrorMessage = "Positioning : Possible double unit found.";
                                    m_intFailResultMask |= 0x08;
                                    return false;
                                }
                            }
                        }

                        m_pObjectCenter = new PointF(objSearchROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX, objSearchROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY);
                        m_fObjectWidth = m_objMatcher.PatternWidth;
                        m_fObjectHeight = m_objMatcher.PatternHeight;
                        // Calibration value error
                        m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                        m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                        m_fObjectAngle = m_objMatcher.GetPosition(0).Angle;
                        m_bFinalResult = true;
                    }
                    else
                    {
                        m_strErrorMessage = "Positioning : Unit Fail. Unit Score lower than min setting";
                        m_intFailResultMask |= 0x01;
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage = "Positioning : Unable to find any unit. Pattern Match get 0 object.";
                    m_intFailResultMask |= 0x01;
                    return false;
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }

            return true;
        }

        public bool InspectionPRS_IPM(ROI objSearchROI, float fCalibX, float fCalibY, bool blnWantAccuratePosition)
        {
            try
            {
                ResetPositionData();

                if (blnWantAccuratePosition)
                {
                    if (!m_objMatcher.Interpolate) m_objMatcher.Interpolate = true;
                    if (m_objMatcher.FinalReduction != 0)   m_objMatcher.FinalReduction = 0;// Set 0 to get accurate position.
                }
                else
                {
                    if (m_objMatcher.Interpolate) m_objMatcher.Interpolate = false;
                    if (m_objMatcher.FinalReduction != 2) m_objMatcher.FinalReduction = 2;  // Since accurate position is not important, set 2 will reduce the PRS time.
                }
                    
                if (m_objMatcher.MinAngle != 0) m_objMatcher.MinAngle = 0;
                if (m_objMatcher.MaxAngle != 0) m_objMatcher.MaxAngle = 0;               
                if (m_objMatcher.MaxPositions != 1) m_objMatcher.MaxPositions = 1;

                m_objMatcher.Match(objSearchROI.ref_ROI);

                if (m_objMatcher.NumPositions > 0)
                {
                    m_fObjectScore = m_objMatcher.GetPosition(0).Score * 100;
                    if (m_fObjectScore >= m_intMinBorderScore)
                    {
                        m_pObjectCenter = new PointF(objSearchROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX, objSearchROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY);
                        m_fObjectWidth = m_objMatcher.PatternWidth;
                        m_fObjectHeight = m_objMatcher.PatternHeight;
                        // Calibration value error
                        m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                        m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                        m_fObjectAngle = m_objMatcher.GetPosition(0).Angle;
                        m_bFinalResult = true;
                    }
                    else
                    {
                        m_strErrorMessage = "Positioning : Unit Fail. Unit Score lower than min setting";
                        m_intFailResultMask |= 0x01;
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage = "Positioning : Unable to find any unit. Pattern Match get 0 object.";
                    m_intFailResultMask |= 0x01;
                    return false;
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }

            return true;
        }

        public bool InspectionWith4DirectionPRS(ROI objSearchROI, float fCalibX, float fCalibY, bool blnCheckDiffOrient, bool blnCheckDoubleUnit)
        {
            try
            {
                ResetPositionData();

                m_objMatcher.Interpolate = false;
                m_objMatcher.MinAngle = 0;
                m_objMatcher.MaxAngle = 0;
                m_objMatcher.FinalReduction = 1;
                if (m_objMatcher.MaxPositions != 1)
                    m_objMatcher.MaxPositions = 1;
                //Find the best orientation
                ROI objRotatedSearchROI = new ROI();
                float fHighestScore = 0;
                float fHighestScore2 = 0;
                m_intHighestScoreDirection = -1;
                for (int i = 0; i < 4; i += 1)
                {
                    if (!blnCheckDiffOrient)
                    {
                        if (i > 0)
                            break;
                    }

                    if (i == 0)
                        m_objMatcher.Match(objSearchROI.ref_ROI);
                    else
                    {
                        //m_objRotatedImage[i] = new ImageDrawing();
                        m_objRotatedImage[i].SetImageSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
                        objSearchROI.CopyToImage(ref m_objRotatedImage[i]);
                        objRotatedSearchROI.AttachImage(m_objRotatedImage[i]);
                        objRotatedSearchROI.LoadROISetting(0, 0, m_objRotatedImage[i].ref_intImageWidth, m_objRotatedImage[i].ref_intImageHeight);
                        ROI.Rotate0Degree(objRotatedSearchROI, 90 * i, ref m_objRotatedImage[i]);
                        m_objMatcher.Match(objRotatedSearchROI.ref_ROI);
                    }

                    if (m_objMatcher.NumPositions > 0)
                    {
                        if (fHighestScore < m_objMatcher.GetPosition(0).Score)
                        {
                            fHighestScore = m_objMatcher.GetPosition(0).Score;
                            m_intHighestScoreDirection = i;
                        }

                        if (m_objMatcher.NumPositions > 1)
                        {
                            if (fHighestScore2 < m_objMatcher.GetPosition(1).Score)
                            {
                                fHighestScore2 = m_objMatcher.GetPosition(1).Score;
                            }
                        }
                    }
                }

                m_objMatcher.Interpolate = true;

                int intAngleTolerance;
                if (m_intDieAngleLimit == 0)
                    intAngleTolerance = 0;
                else
                    intAngleTolerance = 3;
                if (m_intHighestScoreDirection == 3)
                {
                    m_objMatcher.MinAngle = 270 - (m_intDieAngleLimit + intAngleTolerance);
                    m_objMatcher.MaxAngle = 270 + m_intDieAngleLimit + intAngleTolerance;
                }
                else if (m_intHighestScoreDirection == 2)
                {
                    m_objMatcher.MinAngle = 180 - (m_intDieAngleLimit + intAngleTolerance);
                    m_objMatcher.MaxAngle = 180 + m_intDieAngleLimit + intAngleTolerance;
                }
                else if (m_intHighestScoreDirection == 1)
                {
                    m_objMatcher.MinAngle = 90 - (m_intDieAngleLimit + intAngleTolerance);
                    m_objMatcher.MaxAngle = 90 + m_intDieAngleLimit + intAngleTolerance;  
                }
                else
                {
                    m_objMatcher.MinAngle = -(m_intDieAngleLimit + intAngleTolerance);
                    m_objMatcher.MaxAngle = m_intDieAngleLimit + intAngleTolerance;
                }

                if (blnCheckDoubleUnit)
                {
                    if (m_objMatcher.MaxPositions != 2)
                        m_objMatcher.MaxPositions = 2;
                }
                else
                {
                    if (m_objMatcher.MaxPositions != 1)
                        m_objMatcher.MaxPositions = 1;
                }
                m_objMatcher.FinalReduction = 0;
                m_objMatcher.Match(objSearchROI.ref_ROI);

                if (m_objMatcher.NumPositions > 0)
                {
                    m_fObjectScore = m_objMatcher.GetPosition(0).Score * 100;
                    if (m_fObjectScore >= m_intMinBorderScore)
                    {
                        if (blnCheckDoubleUnit)
                        {
                            float fScore2;
                            if (m_objMatcher.NumPositions > 1)
                            {
                                fScore2 = m_objMatcher.GetPosition(1).Score * 100;
                                if (fScore2 >= (m_fObjectScore - 10))
                                {
                                    m_strErrorMessage = "Positioning : Possible double unit found.";
                                    m_intFailResultMask |= 0x08;
                                    return false;
                                }
                            }
                        }

                        m_pObjectCenter = new PointF(objSearchROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX, objSearchROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY);
                        m_fObjectWidth = m_objMatcher.PatternWidth;
                        m_fObjectHeight = m_objMatcher.PatternHeight;
                        // Calibration value error
                        m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                        m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                        m_fObjectAngle = m_objMatcher.GetPosition(0).Angle - 90 * m_intHighestScoreDirection;
                        m_bFinalResult = true;
                    }
                    else
                    {
                        m_strErrorMessage = "Positioning : Unit Fail. Unit Score lower than min setting";
                        m_intFailResultMask |= 0x01;
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage = "Positioning : Unable to find any unit. Pattern Match get 0 object.";
                    m_intFailResultMask |= 0x01;
                    return false;
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }

            return true;
        }

        public bool InspectionWith4DirectionPRSAndOrient(ROI objSearchROI, float fCalibX, float fCalibY, bool blnCheckDiffOrient, bool blnCheckDoubleUnit)
        {
            try
            {
                ResetPositionData();

                m_objMatcher.Interpolate = false;
                m_objMatcher.MinAngle = 0;
                m_objMatcher.MaxAngle = 0;
                m_objMatcher.FinalReduction = 1;
                if (m_objMatcher.MaxPositions != 1)
                    m_objMatcher.MaxPositions = 1;
                //Find the best orientation
                ROI objRotatedSearchROI = new ROI();
                float fHighestScore = 0;
                m_intHighestScoreDirection = -1;
                for (int i = 0; i < 4; i += 1)
                {
                    if (m_intDirections == 2)   // for Unit with Rectange size.
                    {
                        if (i == 1 || i == 3)
                            continue;
                    }

                    if (i == 0)
                        m_objMatcher.Match(objSearchROI.ref_ROI);
                    else
                    {
                        m_objRotatedImage[i].SetImageSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
                        objSearchROI.CopyToImage(ref m_objRotatedImage[i]);
                        objRotatedSearchROI.AttachImage(m_objRotatedImage[i]);
                        objRotatedSearchROI.LoadROISetting(0, 0, m_objRotatedImage[i].ref_intImageWidth, m_objRotatedImage[i].ref_intImageHeight);
                        ROI.Rotate0Degree(objRotatedSearchROI, 90 * i, ref m_objRotatedImage[i]);
                        m_objMatcher.Match(objRotatedSearchROI.ref_ROI);
                    }

                    if (m_objMatcher.NumPositions > 0)
                    {
                        // check is unit score pass
                        if ((m_objMatcher.GetPosition(0).Score * 100) >= m_intMinBorderScore)
                        {
                            // Define Orient Search ROI in Unit ROI with PRS size tolerance or minimum 15 pixels tolerance
                            ROI objUnitROI = new ROI();
                            if (i == 0)
                                objUnitROI.AttachImage(objSearchROI);
                            else
                                objUnitROI.AttachImage(objRotatedSearchROI);
                            int intStartX = (int)Math.Round(m_objMatcher.GetPosition(0).CenterX - (float)m_objMatcher.PatternWidth / 2, 0, MidpointRounding.AwayFromZero);
                            int intStartY = (int)Math.Round(m_objMatcher.GetPosition(0).CenterY - (float)m_objMatcher.PatternHeight / 2, 0, MidpointRounding.AwayFromZero);
                            objUnitROI.LoadROISetting(intStartX, intStartY, m_objMatcher.PatternWidth, m_objMatcher.PatternHeight);

                            ROI objPreciseOrientROI = new ROI();
                            objPreciseOrientROI.AttachImage(objUnitROI);
                            intStartX = (int)m_fOrientPosX - Math.Max(15, m_objOrientMatcher.PatternWidth);
                            intStartY = (int)m_fOrientPosY - Math.Max(15, m_objOrientMatcher.PatternHeight);
                            int intWidth = Math.Max(30, m_objOrientMatcher.PatternWidth * 2);
                            int intHeight = Math.Max(30, m_objOrientMatcher.PatternHeight * 2);
                            objPreciseOrientROI.LoadROISetting(intStartX, intStartY, intWidth, intHeight);

                            m_objOrientMatcher.Match(objUnitROI.ref_ROI);

                            if (m_objOrientMatcher.NumPositions > 0)
                            {
                                if ((m_objOrientMatcher.GetPosition(0).Score) >= m_fOrientMinScore)
                                {
                                    if (fHighestScore < m_objOrientMatcher.GetPosition(0).Score)
                                    {
                                        // Get Orient PRS highest score
                                        fHighestScore = m_objOrientMatcher.GetPosition(0).Score;
                                        m_intHighestScoreDirection = i;

                                        // Get Unit Position Information
                                        m_pObjectCenter = new PointF(objSearchROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX, objSearchROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY);
                                        m_fObjectWidth = m_objMatcher.PatternWidth;
                                        m_fObjectHeight = m_objMatcher.PatternHeight;

                                        // Turn point to original dimension
                                        m_pObjectCenter = Math2.Turn90Point(new PointF(objSearchROI.ref_ROICenterX, objSearchROI.ref_ROICenterY),
                                                          m_pObjectCenter, 90 * i);
                                        if (i == 1 || i == 3)
                                        {
                                            m_fObjectWidth = m_objMatcher.PatternHeight;
                                            m_fObjectHeight = m_objMatcher.PatternWidth;
                                        }

                                        // Calibration value error
                                        m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                                        m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                                        m_fObjectAngle = m_objMatcher.GetPosition(0).Angle;
                                        m_bFinalResult = true;

                                    }
                                }
                            }
                        }

                                               
                    }
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }

            if (m_intHighestScoreDirection < 0)
            {
                m_strErrorMessage = "Orientation Fail. Orient Score lower than min setting";
                m_intFailResultMask |= 0x01;
                return false;
            }
            else
                return true;
        }


        public bool InspectionWithPRS_LightSurface(ROI objSearchROI, float fCalibX, float fCalibY, bool blnCheckDiffOrient, bool blnCheckDoubleUnit)
        {
            try
            {
                ResetPositionData();

                m_objMatcher.Interpolate = true;
                m_objMatcher.MinAngle = -(m_intDieAngleLimit + 3);
                m_objMatcher.MaxAngle = m_intDieAngleLimit + 3;
                m_objMatcher.FinalReduction = 0;
                if (blnCheckDoubleUnit)
                {
                    if (m_objMatcher.MaxPositions != 2)
                        m_objMatcher.MaxPositions = 2;
                }
                else
                {
                    if (m_objMatcher.MaxPositions != 1)
                        m_objMatcher.MaxPositions = 1;
                }
                //Find the best orientation
                bool blnUnitFound = false;
                ROI objRotatedSearchROI = new ROI();
                for (int i = 0; i < 2; i += 1)
                {
                    if (i == 0)
                        m_objMatcher.Match(objSearchROI.ref_ROI);
                    else
                    {
                        //m_objRotatedImage[i] = new ImageDrawing();
                        m_objRotatedImage[i].SetImageSize(objSearchROI.ref_ROI.TopParent.Width, objSearchROI.ref_ROI.TopParent.Height);
                        EasyImage.Copy(objSearchROI.ref_ROI.TopParent, m_objRotatedImage[i].ref_objMainImage);
                        objRotatedSearchROI.AttachImage(m_objRotatedImage[i]);
                        objRotatedSearchROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY, objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
                        ROI.Rotate0Degree(objRotatedSearchROI, 90 * i, ref m_objRotatedImage[i]);
                        m_objMatcher.Match(objRotatedSearchROI.ref_ROI);
                    }

                    if (m_objMatcher.NumPositions > 0)
                    {
                        blnUnitFound = true;

                        float fScore = m_objMatcher.GetPosition(0).Score * 100;
                        if (fScore >= m_fObjectScore)
                        {
                            m_fObjectScore = fScore;

                            if (i == 0)
                            m_pObjectCenter = new PointF(objSearchROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX, objSearchROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY);
                            else
                            {
                                m_pObjectCenter = new PointF(objSearchROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX, objSearchROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY);
                                PointF pPrefererencePoint = new PointF(objSearchROI.ref_ROITotalX + (float)objSearchROI.ref_ROIWidth / 2,
                                                                       objSearchROI.ref_ROITotalY + (float)objSearchROI.ref_ROIHeight / 2);
                                m_pObjectCenter = Math2.Turn90Point(pPrefererencePoint, m_pObjectCenter, 90);
                            }
                            m_fObjectWidth = m_objMatcher.PatternWidth;
                            m_fObjectHeight = m_objMatcher.PatternHeight;
                            // Calibration value error
                            m_fObjectOffsetX = (m_pObjectCenter.X - m_objCrosshair.ref_intCrosshairX + m_fCompensateX) / fCalibX;
                            m_fObjectOffsetY = (m_pObjectCenter.Y - m_objCrosshair.ref_intCrosshairY + m_fCompensateY) / fCalibY;
                            m_fObjectAngle = m_objMatcher.GetPosition(0).Angle;
                            m_bFinalResult = true;   
                        }
                    }
                }

                if (blnUnitFound)
                {
                    m_bFinalResult = true;

                    if (m_fObjectScore < m_intMinBorderScore)
                    {
                        m_strErrorMessage = "Unit Fail. Unit Score lower than min setting";
                        m_intFailResultMask |= 0x01;
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage = "Unable to find any unit. Pattern Match get 0 object.";
                    m_intFailResultMask |= 0x01;
                    return false;
                }
 
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check whether template is learn before. If not, it is not allowed to do matching
        /// </summary>
        public bool IsPatternLearnt()
        {
            return m_objMatcher.PatternLearnt;
        }

        public bool LearnPatternWithPolygon(ROI objROI, Polygon objPolygon)
        {
            return LearnPatternWithPolygon(objROI, null, objPolygon);
        }

        public bool LearnPatternWithPolygon(ROI objROI, ROI objOrientROI, Polygon objPolygon)
        {
            try
            {
                if (objPolygon.GetPolygonCount() > 0)
                {
                    ImageDrawing objDontCareImage = new ImageDrawing();

                    EasyImage.Copy(objROI.ref_ROI.TopParent, objDontCareImage.ref_objMainImage);

                    ROI objDontCareROI = new ROI();
                    objDontCareROI.AttachImage(objDontCareImage);
                    objDontCareROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

                    // Off set +1 to train roi pixel gray value
                    ROI.ModifyImageGain(objDontCareROI, objDontCareImage);

                    // Fill polygon on image by changing pixel
                    objPolygon.FillPolygonOnImage(ref objDontCareImage);
#if (Debug_2_12 || Release_2_12)
                    m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                    m_objMatcher.DontCareThreshold = 1;
                    m_objMatcher.LearnPattern(objDontCareROI.ref_ROI);

                    if (m_blnOrientON && objOrientROI != null)
                    {
                        m_objOrientMatcher.DontCareThreshold = 1;
                        ROI objLocalOrientROI = new ROI();  // Create new ROI to prevent original OrientROI attach to other parent.
                        objLocalOrientROI.AttachImage(objDontCareROI);
                        objLocalOrientROI.LoadROISetting(objOrientROI.ref_ROIPositionX, objOrientROI.ref_ROIPositionY,
                                                         objOrientROI.ref_ROIWidth, objOrientROI.ref_ROIHeight);
#if (Debug_2_12 || Release_2_12)
                        m_objOrientMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_objOrientMatcher.LearnPattern(objLocalOrientROI.ref_ROI);
                    }
                }
                else
                {
                    LearnPattern(objROI, objOrientROI);
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern With Polygon Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern With Polygon Error: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LearnPattern(ROI objROI)
        {
            return LearnPattern(objROI, null);
        }

        public bool LearnPattern(ROI objROI, ROI objOrientROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objMatcher.LearnPattern(objROI.ref_ROI);

                if (m_blnOrientON && objOrientROI != null)
                {
#if (Debug_2_12 || Release_2_12)
                    m_objOrientMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                    m_objOrientMatcher.LearnPattern(objOrientROI.ref_ROI);
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LearnOrientPattern(ROI objROI, Polygon objPolygon)
        {
            try
            {
                if (objPolygon.GetPolygonCount() > 0)
                {
                    ImageDrawing objDontCareImage = new ImageDrawing();

                    EasyImage.Copy(objROI.ref_ROI.TopParent, objDontCareImage.ref_objMainImage);

                    ROI objDontCareROI = new ROI();
                    objDontCareROI.AttachImage(objDontCareImage);
                    objDontCareROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

                    // Off set +1 to train roi pixel gray value
                    ROI.ModifyImageGain(objDontCareROI, objDontCareImage);

                    // Fill polygon on image by changing pixel
                    objPolygon.FillPolygonOnImage(ref objDontCareImage);
#if (Debug_2_12 || Release_2_12)
                    m_objOrientMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                    m_objOrientMatcher.DontCareThreshold = 1;
                    m_objOrientMatcher.LearnPattern(objDontCareROI.ref_ROI);
                }
                else
                {
#if (Debug_2_12 || Release_2_12)
                    m_objOrientMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                    m_objOrientMatcher.LearnPattern(objROI.ref_ROI);
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern With Polygon Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern With Polygon Error: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool IsEmptyPatternLearnt()
        {
            return m_objEmptyMatcher.PatternLearnt;
        }

        public bool LearnEmptyPattern(ROI objROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objEmptyMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objEmptyMatcher.LearnPattern(objROI.ref_ROI);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Empty Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Empty Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LoadPattern(string strPosFilePath)
        {
            return LoadPattern(strPosFilePath, "");
        }

        public bool LoadPattern(string strPosFilePath, string strOrientFilePath)
        {
            try
            {
                if (File.Exists(strPosFilePath))
                    m_objMatcher.Load(strPosFilePath);

                if (m_blnOrientON && m_objOrientMatcher != null)
                    if (File.Exists(strOrientFilePath))
                        m_objOrientMatcher.Load(strOrientFilePath);

            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Load Pattern Error: " + ex.ToString() + " FilePath:" + strPosFilePath;
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString() + " FilePath:" + strPosFilePath);
                return false;
            }
            return true;
        }

        public bool SavePattern(string strPosFilePath)
        {
            return SavePattern(strPosFilePath, "");
        }

        public bool SavePattern(string strPosFilePath, string strOrientFilePath)
        {
            try
            {
                string strDirectoryName = System.IO.Path.GetDirectoryName(strPosFilePath);
                DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                if (!directory.Exists)
                    CreateUnexistDirectory(directory);

                m_objMatcher.Save(strPosFilePath);

                if (m_blnOrientON && strOrientFilePath != "")
                {
                    strDirectoryName = System.IO.Path.GetDirectoryName(strOrientFilePath);
                    directory = new DirectoryInfo(strDirectoryName);
                    if (!directory.Exists)
                        CreateUnexistDirectory(directory);

                    m_objOrientMatcher.Save(strOrientFilePath);
                }
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LoadEmptyPattern(string strFilePath)
        {
            try
            {
                m_objEmptyMatcher.Load(strFilePath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Load Empty Pattern Error: " + ex.ToString() + " FilePath:" + strFilePath;
                m_objTrackLog.WriteLine("Positioning Load Empty Pattern: " + ex.ToString() + " FilePath:" + strFilePath);
                return false;
            }
            return true;
        }

        public bool SaveEmptyPattern(string strFilePath)
        {
            try
            {
                string strDirectoryName = System.IO.Path.GetDirectoryName(strFilePath);
                DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                if (!directory.Exists)
                    CreateUnexistDirectory(directory);

                m_objEmptyMatcher.Save(strFilePath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Save Empty Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Save Empty Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        // Drawing
        public void DrawObjectCrossLinePosition(Graphics g)
        {
            if (m_intMethod != 0)
                return;

            if (!m_bFinalResult)
                return;

            PointF pxy = new PointF();
            PointF pxY = new PointF();
            PointF pXy = new PointF();
            PointF pXY = new PointF();
            Math2.GetCornerPoints(m_pObjectCenter.X, m_pObjectCenter.Y, m_fObjectAngle, m_fObjectWidth, m_fObjectHeight, ref pxy, ref pXy, ref pxY, ref pXY);
            g.DrawLine(new Pen(Color.Lime), pxy, pxY);
            g.DrawLine(new Pen(Color.Lime), pxY, pXY);
            g.DrawLine(new Pen(Color.Lime), pXY, pXy);
            g.DrawLine(new Pen(Color.Lime), pXy, pxy);
            g.DrawLine(new Pen(Color.Lime), m_pObjectCenter.X - 5 + m_fCompensateX, m_pObjectCenter.Y + m_fCompensateY, m_pObjectCenter.X + 5 + m_fCompensateX, m_pObjectCenter.Y + m_fCompensateY);
            g.DrawLine(new Pen(Color.Lime), m_pObjectCenter.X + m_fCompensateX, m_pObjectCenter.Y - 5 + m_fCompensateY, m_pObjectCenter.X + m_fCompensateX, m_pObjectCenter.Y + 5 + m_fCompensateY);

            //if (m_objCrossLines[0] != null)
            //    m_objCrossLines[0].DrawLineByPoints(g, Convert.ToInt32(m_pCorner[1].X), Convert.ToInt32(m_pCorner[3].X), Color.Lime, 1);
            //if (m_objCrossLines[1] != null)
            //    m_objCrossLines[1].DrawLineByPoints(g, Convert.ToInt32(m_pCorner[0].X), Convert.ToInt32(m_pCorner[2].X), Color.Lime, 1);

            //Pen pen = new Pen(Color.Lime, 1);
            //g.DrawLine(pen, m_pCorner[0], m_pCorner[1]);
            //g.DrawLine(pen, m_pCorner[1], m_pCorner[2]);
            //g.DrawLine(pen, m_pCorner[2], m_pCorner[3]);
            //g.DrawLine(pen, m_pCorner[3], m_pCorner[0]);


            //---------------------------------------------------------------------------


            //if (!m_bFinalResult)
            //    return;



            //Pen pen = new Pen(Color.Lime, 1);
            //g.DrawLine(pen, m_pObjectCenter.X, m_pObjectCenter.Y - 5, m_pObjectCenter.X, m_pObjectCenter.Y + 5);
            //g.DrawLine(pen, m_pObjectCenter.X - 5, m_pObjectCenter.Y, m_pObjectCenter.X + 5, m_pObjectCenter.Y);

            //g.DrawRectangle(pen, m_pObjectCenter.X - m_fObjectWidth / 2, m_pObjectCenter.Y - m_fObjectHeight / 2,
            //                    m_fObjectWidth, m_fObjectHeight);

        }

        public void DrawSmallCrossLinePosition(Graphics g)
        {
            if (!m_bFinalResult)
                return;

            if (m_pObjectCenter.X > 5 && m_pObjectCenter.X < 635 && m_pObjectCenter.Y > 5 && m_pObjectCenter.Y < 475)
            {
                Pen pen = new Pen(Color.Lime, 1);
                g.DrawLine(pen, m_pObjectCenter.X, m_pObjectCenter.Y - 5, m_pObjectCenter.X, m_pObjectCenter.Y + 5);
                g.DrawLine(pen, m_pObjectCenter.X - 5, m_pObjectCenter.Y, m_pObjectCenter.X + 5, m_pObjectCenter.Y);

                g.DrawRectangle(pen, m_pObjectCenter.X - m_fObjectWidth / 2, m_pObjectCenter.Y - m_fObjectHeight / 2,
                                    m_fObjectWidth, m_fObjectHeight);
            }
        }

        public void DrawPRSObject(Graphics g)
        {
            if (m_intMethod != 1)
                return;

            if (!m_bFinalResult)
                return;

            if (m_objMatcher.NumPositions > 0)
            {
                //m_objMatcher.DrawPosition(g, new Pen(Color.Red), 0);

                PointF pxy = new PointF();
                PointF pxY = new PointF();
                PointF pXy = new PointF();
                PointF pXY = new PointF();
                Math2.GetCornerPoints(m_pObjectCenter.X, m_pObjectCenter.Y, m_fObjectAngle, m_fObjectWidth, m_fObjectHeight, ref pxy, ref pXy, ref pxY, ref pXY);
                PointF pObjectCenter = new PointF();
                pObjectCenter.X = m_pObjectCenter.X;
                pObjectCenter.Y = m_pObjectCenter.Y;
                if (!m_bFinalResult)
                    return;
                g.DrawLine(new Pen(Color.Lime), pxy, pxY);
                g.DrawLine(new Pen(Color.Lime), pxY, pXY);
                g.DrawLine(new Pen(Color.Lime), pXY, pXy);
                g.DrawLine(new Pen(Color.Lime), pXy, pxy);
                g.DrawLine(new Pen(Color.Lime), pObjectCenter.X - 5 + m_fCompensateX, pObjectCenter.Y + m_fCompensateY, pObjectCenter.X + 5 + m_fCompensateX, pObjectCenter.Y + m_fCompensateY);
                g.DrawLine(new Pen(Color.Lime), pObjectCenter.X + m_fCompensateX, pObjectCenter.Y - 5 + m_fCompensateY, pObjectCenter.X + m_fCompensateX, pObjectCenter.Y + 5 + m_fCompensateY);
            }
        }


        public void DrawGaugeMeasurePosition(Graphics g, List<LGauge> arrLineGauges)
        {
            for (int v = 0; v < arrLineGauges.Count; v++)
            {
                arrLineGauges[v].DrawGauge(g);
            }

            if (m_pObjectCenter.X > 5 && m_pObjectCenter.X < 635 && m_pObjectCenter.Y > 5 && m_pObjectCenter.Y < 475)
            {
                g.DrawLine(new Pen(Color.Red, 2), m_pObjectCenter.X - 5, m_pObjectCenter.Y, m_pObjectCenter.X + 5, m_pObjectCenter.Y);
                g.DrawLine(new Pen(Color.Red, 2), m_pObjectCenter.X, m_pObjectCenter.Y - 5, m_pObjectCenter.X, m_pObjectCenter.Y + 5);
            }
            for (int i = 0; i < m_arrObjectCorner.Length; i++)
            {
                if (m_arrObjectCorner[i].X > 5 && m_arrObjectCorner[i].X < 635 && m_arrObjectCorner[i].Y > 5 && m_arrObjectCorner[i].Y < 475)
                {
                    g.DrawLine(new Pen(Color.Red, 2), m_arrObjectCorner[i].X - 5, m_arrObjectCorner[i].Y, m_arrObjectCorner[i].X + 5, m_arrObjectCorner[i].Y);
                    g.DrawLine(new Pen(Color.Red, 2), m_arrObjectCorner[i].X, m_arrObjectCorner[i].Y - 5, m_arrObjectCorner[i].X, m_arrObjectCorner[i].Y + 5);
                }
            }
        }

        public void LoadPosition(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            // Load Position Setting
            m_intPositionImageIndex = objFile.GetValueAsInt("PositionImageIndex", 0);
            m_intEmptyImageIndex = objFile.GetValueAsInt("EmptyImageIndex", 0);
            m_intMinBorderScore = objFile.GetValueAsInt("MinBorderScore", 0);
            m_intMinEmptyScore = objFile.GetValueAsInt("MinEmptyScore", 50);
            m_intDieAngleLimit = objFile.GetValueAsInt("AngleLimit", 0);
            m_fPositionXTolerance = objFile.GetValueAsFloat("PositionXTolerance", 0);   // in um
            m_fPositionYTolerance = objFile.GetValueAsFloat("PositionYTolerance", 0);   // in um
            m_fSampleDieWidth = objFile.GetValueAsFloat("DieWidth", 0);   // in um
            m_fSampleDieHeight = objFile.GetValueAsFloat("DieHeight", 0);   // in um
            m_intMethod = objFile.GetValueAsInt("Method", 0);
            m_intPRSMode = objFile.GetValueAsInt("PRSMode", 0);
            m_fGainValue = objFile.GetValueAsFloat("GainValue", 1f);
            m_fGainValue2 = objFile.GetValueAsFloat("GainValue2", 1f);
            m_fCompensateX = objFile.GetValueAsFloat("CompensateX", 1f);
            m_fCompensateY = objFile.GetValueAsFloat("CompensateY", 1f);
            m_intFlipThreshold = objFile.GetValueAsInt("FlipThreshold", 125);
            m_intFlipAreaLimit = objFile.GetValueAsInt("FlipAreaLimit", 1000);
            //Empty
            m_intEmptyThreshold = objFile.GetValueAsInt("EmptyThreshold", 128);
            m_intEmptyWhiteBlackArea = objFile.GetValueAsInt("EmptyWhiteArea", 0);
            //PH
            m_intPHThreshold = objFile.GetValueAsInt("PHThreshold", 128);
            m_intPHBlackArea = objFile.GetValueAsInt("PHBlackArea", 50);
            m_intPHMinArea = objFile.GetValueAsInt("PHMinArea", 20);

            m_objCrosshair.LoadCrosshair(strPath, "PositionCrosshair");

            if (m_blnOrientON)
            {
                LoadOrient(strPath, "Orient");
            }
        }
        public void LoadPositionToleranceFromFile(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("PositionSettings");
            
            //Empty
            m_intEmptyWhiteBlackArea = objFile.GetValueAsInt("EmptyWhiteArea", 0);
            //PH
            m_intPHBlackArea = objFile.GetValueAsInt("PHBlackArea", 50);
        }
        private void LoadOrient(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            // Load Position Setting
            m_fOrientMinScore = objFile.GetValueAsFloat("MinScore", 0.5f);
            m_fOrientPosX = objFile.GetValueAsFloat("PositionX", 0f);
            m_fOrientPosY = objFile.GetValueAsFloat("PositionY", 0f);
            m_fOrientPosTolerance = objFile.GetValueAsFloat("PosTolerance", 5f);

        }

        public void SavePosition(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Rectangle gauge template measurement result
            objFile.WriteElement1Value("PositionImageIndex", m_intPositionImageIndex);
            objFile.WriteElement1Value("EmptyImageIndex", m_intEmptyImageIndex);
            objFile.WriteElement1Value("MinBorderScore", m_intMinBorderScore);
            objFile.WriteElement1Value("MinEmptyScore", m_intMinEmptyScore);
            objFile.WriteElement1Value("AngleLimit", m_intDieAngleLimit);
            objFile.WriteElement1Value("PositionXTolerance", m_fPositionXTolerance);
            objFile.WriteElement1Value("PositionYTolerance", m_fPositionYTolerance);
            objFile.WriteElement1Value("DieWidth", m_fSampleDieWidth);
            objFile.WriteElement1Value("DieHeight", m_fSampleDieHeight);
            objFile.WriteElement1Value("Method", m_intMethod);
            objFile.WriteElement1Value("PRSMode", m_intPRSMode);
            objFile.WriteElement1Value("GainValue", m_fGainValue);
            objFile.WriteElement1Value("GainValue2", m_fGainValue2);
            objFile.WriteElement1Value("CompensateX", m_fCompensateX);
            objFile.WriteElement1Value("CompensateY", m_fCompensateY);
            objFile.WriteElement1Value("FlipThreshold", m_intFlipThreshold);
            objFile.WriteElement1Value("FlipAreaLimit", m_intFlipAreaLimit);
            //Empty
            objFile.WriteElement1Value("EmptyThreshold", m_intEmptyThreshold);
            objFile.WriteElement1Value("EmptyWhiteArea", m_intEmptyWhiteBlackArea);
            //PH
            objFile.WriteElement1Value("PHThreshold", m_intPHThreshold);
            objFile.WriteElement1Value("PHBlackArea", m_intPHBlackArea);
            objFile.WriteElement1Value("PHMinArea", m_intPHMinArea);

            objFile.WriteEndElement();

            m_objCrosshair.SaveCrosshair(strPath, false, "PositionCrosshair", false);

            if (m_blnOrientON)
            {
                SaveOrient(strPath, false, "Orient", false);
            }
        }

        public void SavePosition_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            //XmlParser objFile = new XmlParser(strPath, blnNewFile);

            //objFile.WriteSectionElement(strSectionName, blnNewSection);

            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            // Rectangle gauge template measurement result
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PositionImageIndex", m_intPositionImageIndex);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_EmptyImageIndex", m_intEmptyImageIndex);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MinBorderScore", m_intMinBorderScore);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MinEmptyScore", m_intMinEmptyScore);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_AngleLimit", m_intDieAngleLimit);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PositionXTolerance", m_fPositionXTolerance);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PositionYTolerance", m_fPositionYTolerance);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_DieWidth", m_fSampleDieWidth);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_DieHeight", m_fSampleDieHeight);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_Method", m_intMethod);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PRSMode", m_intPRSMode);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GainValue", m_fGainValue);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GainValue2", m_fGainValue2);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_CompensateX", m_fCompensateX);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_CompensateY", m_fCompensateY);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_FlipThreshold", m_intFlipThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_FlipAreaLimit", m_intFlipAreaLimit);
            //Empty
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_EmptyThreshold", m_intEmptyThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_EmptyWhiteArea", m_intEmptyWhiteBlackArea);
            //PH
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PHThreshold", m_intPHThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PHBlackArea", m_intPHBlackArea);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PHMinArea", m_intPHMinArea);

            objFile.WriteEndElement();

            //m_objCrosshair.SaveCrosshair(strPath, false, "PositionCrosshair", false);

            if (m_blnOrientON)
            {
                SaveOrient_SECSGEM(strPath, "PositionOrient", strVisionName);
            }
        }
        public void SavePositionToleranceToFile(string strPath, bool blnNewFile, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement("PositionSettings", blnNewSection);
         
            //Empty
            objFile.WriteElement1Value("EmptyWhiteArea", m_intEmptyWhiteBlackArea);
            //PH
            objFile.WriteElement1Value("PHBlackArea", m_intPHBlackArea);

            objFile.WriteEndElement();
        }
        public void LoadEmptyThreshold(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_intEmptyThreshold = objFile.GetValueAsInt("EmptyThreshold", 128);
            m_intEmptyWhiteBlackArea = objFile.GetValueAsInt("EmptyWhiteArea", 0);

        }
     
        public void LoadPHSetting(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_intPHThreshold = objFile.GetValueAsInt("PHThreshold", 128);
            m_intPHBlackArea = objFile.GetValueAsInt("PHBlackArea", 50);
            m_intPHMinArea = objFile.GetValueAsInt("PHMinArea", 20);

        }
       
        private void SaveOrient(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            objFile.WriteElement1Value("MinScore", m_fOrientMinScore);
            objFile.WriteElement1Value("PositionX", m_fOrientPosX);
            objFile.WriteElement1Value("PositionY", m_fOrientPosY);
            objFile.WriteElement1Value("PosTolerance", m_fOrientPosTolerance);
            objFile.WriteElement1Value("Direction", m_intDirections);

            objFile.WriteEndElement();
        }

        private void SaveOrient_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            //XmlParser objFile = new XmlParser(strPath, blnNewFile);

            //objFile.WriteSectionElement(strSectionName, blnNewSection);

            XmlParser objFile = new XmlParser(strPath);
            objFile.WriteSectionElement("SECSGEMData", false);

            objFile.WriteElement1Value(strVisionName + "_" + strSectionName + "_MinScore", m_fOrientMinScore);
            objFile.WriteElement1Value(strVisionName + "_" + strSectionName + "_PositionX", m_fOrientPosX);
            objFile.WriteElement1Value(strVisionName + "_" + strSectionName + "_PositionY", m_fOrientPosY);
            objFile.WriteElement1Value(strVisionName + "_" + strSectionName + "_PosTolerance", m_fOrientPosTolerance);
            objFile.WriteElement1Value(strVisionName + "_" + strSectionName + "_Direction", m_intDirections);

            objFile.WriteEndElement();
        }

        public void SaveAdvanceOrient(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            objFile.WriteElement1Value("Direction", m_intDirections);

            objFile.WriteEndElement();
        }

        public void ResetPositionData()
        {
            m_bFinalResult = false;
            m_bEmptyUnit = false;

            m_fObjectOffsetX = 0;
            m_fObjectOffsetY = 0;
            m_fObjectAngle = 0;
            m_pCorner = new PointF[4]; // 0 = top left corner, 1 = top right corner, 2 = bottom right corner, 3 = bottom left corner
            m_objCrossLines = new Line[2];

            m_pObjectCenter = new PointF(0, 0);
            m_arrObjectCorner = new PointF[4];
            m_fObjectWidth = 0;
            m_fObjectHeight = 0;
            m_fObjectAngle = 0;
            m_strErrorMessage = "";
            m_intFailResultMask = 0;
            m_fObjectScore = 0;
        }



        private bool GetPositionByEdgeFlexiCorner(ImageDrawing objImage, List<LGauge> arrLineGauges,
                                                                        float fUnitWidth, float fUnitHeight, float fMinBorderScore,
                                                                        ref PointF pMeasureCenterPoint,
                                                                        ref PointF[] arrMeasureCornerPoint,
                                                                        ref float fMeasureWidth, ref float fMeasureHeight,
                                                                        ref float fMeasureAngle, ref string strErrorMessage)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image iwth Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                arrLineGauges[i].Measure(objImage);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                arrLineGauges[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle(arrLineGauges, fMinBorderScore, 2f, ref fTotalAngle, ref nCount, ref blnResult);

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                strErrorMessage = "Not able to measurement edge.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                strErrorMessage = "Not able to measurement edge.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                strErrorMessage = "Not able to measurement edge.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges
            fMeasureAngle = fTotalAngle / nCount;   

            // Build a virtual edge line if the edge is fail
            if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
            {
                Line objLine = new Line();
                arrLineGauges[2].ref_ObjectLine.CopyTo(ref objLine);
                arrLineGauges[0].ref_ObjectLine = objLine;
                arrLineGauges[0].ref_ObjectLine.ShiftYLine(-fUnitHeight);
            }
            if (!blnResult[1])
            {
                Line objLine = new Line();
                arrLineGauges[3].ref_ObjectLine.CopyTo(ref objLine);
                arrLineGauges[1].ref_ObjectLine = objLine;
                arrLineGauges[1].ref_ObjectLine.ShiftXLine(-fUnitWidth);
            }
            if (!blnResult[2])
            {
                Line objLine = new Line();
                arrLineGauges[0].ref_ObjectLine.CopyTo(ref objLine);
                arrLineGauges[2].ref_ObjectLine = objLine;
                arrLineGauges[2].ref_ObjectLine.ShiftYLine(fUnitHeight);
            }
            if (!blnResult[3])
            {
                Line objLine = new Line();
                arrLineGauges[1].ref_ObjectLine.CopyTo(ref objLine);
                arrLineGauges[3].ref_ObjectLine = objLine;
                arrLineGauges[3].ref_ObjectLine.ShiftXLine(fUnitWidth);
            }

            // Get corner point from Edge Line Gauges
            arrMeasureCornerPoint = new PointF[4];
            arrMeasureCornerPoint[0] = Line.GetCrossPoint(arrLineGauges[0].ref_ObjectLine, arrLineGauges[3].ref_ObjectLine);
            arrMeasureCornerPoint[1] = Line.GetCrossPoint(arrLineGauges[0].ref_ObjectLine, arrLineGauges[1].ref_ObjectLine);
            arrMeasureCornerPoint[2] = Line.GetCrossPoint(arrLineGauges[2].ref_ObjectLine, arrLineGauges[1].ref_ObjectLine);
            arrMeasureCornerPoint[3] = Line.GetCrossPoint(arrLineGauges[2].ref_ObjectLine, arrLineGauges[3].ref_ObjectLine);

            // Place Corner Line Gauges to the corner point location
            PlaceCornerLineGaugeLocation(ref arrLineGauges, ref arrMeasureCornerPoint, fUnitWidth, fUnitHeight);

            // Corner line gauges measure unit
            for (int i = 4; i < arrLineGauges.Count; i++)
            {
                arrLineGauges[i].Measure(objImage);
            }

            // Get actual unit position based on all Edge and Corner Line Gauges.
            if (!GetPositionByCorner(arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore, ref pMeasureCenterPoint,
                                        ref arrMeasureCornerPoint, ref fMeasureWidth, ref fMeasureHeight, ref fMeasureAngle, ref strErrorMessage))
                return false;

            return true;
        }

        private bool GetPositionByEdge(ImageDrawing objImage, List<LGauge> arrLineGauges, float fUnitWidth, float fUnitHeight, float fMinBorderScore,
                                                ref PointF pMeasureCenterPoint, ref PointF[] arrMeasureCornerPoint, ref float fMeasureWidth, ref float fMeasureHeight,
                                                ref float fMeasureAngle, ref string strErrorMessage)
        {
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            
            for (int i = 0; i < 4; i++)
            {
                arrLineGauges[i].Measure(objImage);
            }

            FilterEdgeAngle(arrLineGauges, fMinBorderScore, 2f, ref fTotalAngle, ref nCount, ref blnResult);

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                strErrorMessage = "Measure edge fail.";
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                strErrorMessage = "Measure edge fail.";
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                strErrorMessage = "Measure edge fail.";
                return false;
            }

            fMeasureAngle = fTotalAngle / nCount;

            if (!blnResult[0]) // top border fail. Duplicate using bottom border using sample's height
            {
                Line objLine = new Line();
                arrLineGauges[2].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[0].ref_ObjectLine = objLine;
                arrLineGauges[0].GetObjectLine().ShiftYLine(-fUnitHeight);
            }
            if (!blnResult[1])
            {
                Line objLine = new Line();
                arrLineGauges[3].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[1].ref_ObjectLine = objLine;
                arrLineGauges[1].GetObjectLine().ShiftXLine(-fUnitWidth);
            }
            if (!blnResult[2])
            {
                Line objLine = new Line();
                arrLineGauges[0].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[2].ref_ObjectLine = objLine;
                arrLineGauges[2].GetObjectLine().ShiftYLine(fUnitHeight);
            }
            if (!blnResult[3])
            {
                Line objLine = new Line();
                arrLineGauges[1].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[3].ref_ObjectLine = objLine;
                arrLineGauges[3].GetObjectLine().ShiftXLine(fUnitWidth);
            }

            arrMeasureCornerPoint = new PointF[4];
            arrMeasureCornerPoint[0] = Line.GetCrossPoint(arrLineGauges[0].GetObjectLine(), arrLineGauges[3].GetObjectLine());
            arrMeasureCornerPoint[1] = Line.GetCrossPoint(arrLineGauges[0].GetObjectLine(), arrLineGauges[1].GetObjectLine());
            arrMeasureCornerPoint[2] = Line.GetCrossPoint(arrLineGauges[2].GetObjectLine(), arrLineGauges[1].GetObjectLine());
            arrMeasureCornerPoint[3] = Line.GetCrossPoint(arrLineGauges[2].GetObjectLine(), arrLineGauges[3].GetObjectLine());

            Line[] arrCrossLines = new Line[2];
            arrCrossLines[0] = new Line();
            arrCrossLines[1] = new Line();
            arrCrossLines[0].CalculateStraightLine(arrMeasureCornerPoint[1], arrMeasureCornerPoint[3]);
            arrCrossLines[1].CalculateStraightLine(arrMeasureCornerPoint[0], arrMeasureCornerPoint[2]);

            pMeasureCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
            fMeasureWidth = ((arrMeasureCornerPoint[1].X + arrMeasureCornerPoint[2].X) / 2) - ((arrMeasureCornerPoint[0].X + arrMeasureCornerPoint[3].X) / 2);
            fMeasureHeight = ((arrMeasureCornerPoint[2].Y + arrMeasureCornerPoint[3].Y) / 2) - ((arrMeasureCornerPoint[0].Y + arrMeasureCornerPoint[1].Y) / 2);

            return true;
        }

        private bool GetPositionByEdge_InPocket(ImageDrawing objImage, List<LGauge> arrLineGauges, float fUnitWidth, float fUnitHeight, float fMinBorderScore,
                                        ref PointF pMeasureCenterPoint, ref PointF[] arrMeasureCornerPoint, ref float fMeasureWidth, ref float fMeasureHeight,
                                        ref float fMeasureAngle, ref string strErrorMessage)
        {
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                arrLineGauges[i].Measure(objImage);
            }

            FilterEdgeAngle(arrLineGauges, fMinBorderScore, 10, ref fTotalAngle, ref nCount, ref blnResult);

            if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            {
                strErrorMessage = "Only " + nCount.ToString() + " gauge has valid result";
                return false;
            }

            fMeasureAngle = fTotalAngle / nCount;

            if (!blnResult[0]) // top border fail. Duplicate using bottom border using sample's height
            {
                Line objLine = new Line();
                arrLineGauges[2].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[0].ref_ObjectLine = objLine;
                arrLineGauges[0].GetObjectLine().ShiftYLine(-fUnitHeight);
            }
            if (!blnResult[1])
            {
                Line objLine = new Line();
                arrLineGauges[3].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[1].ref_ObjectLine = objLine;
                arrLineGauges[1].GetObjectLine().ShiftXLine(-fUnitWidth);
            }
            if (!blnResult[2])
            {
                Line objLine = new Line();
                arrLineGauges[0].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[2].ref_ObjectLine = objLine;
                arrLineGauges[2].GetObjectLine().ShiftYLine(fUnitHeight);
            }
            if (!blnResult[3])
            {
                Line objLine = new Line();
                arrLineGauges[1].GetObjectLine().CopyTo(ref objLine);
                arrLineGauges[3].ref_ObjectLine = objLine;
                arrLineGauges[3].GetObjectLine().ShiftXLine(fUnitWidth);
            }

            arrMeasureCornerPoint = new PointF[4];
            arrMeasureCornerPoint[0] = Line.GetCrossPoint(arrLineGauges[0].GetObjectLine(), arrLineGauges[3].GetObjectLine());
            arrMeasureCornerPoint[1] = Line.GetCrossPoint(arrLineGauges[0].GetObjectLine(), arrLineGauges[1].GetObjectLine());
            arrMeasureCornerPoint[2] = Line.GetCrossPoint(arrLineGauges[2].GetObjectLine(), arrLineGauges[1].GetObjectLine());
            arrMeasureCornerPoint[3] = Line.GetCrossPoint(arrLineGauges[2].GetObjectLine(), arrLineGauges[3].GetObjectLine());

            Line[] arrCrossLines = new Line[2];
            arrCrossLines[0] = new Line();
            arrCrossLines[1] = new Line();
            arrCrossLines[0].CalculateStraightLine(arrMeasureCornerPoint[1], arrMeasureCornerPoint[3]);
            arrCrossLines[1].CalculateStraightLine(arrMeasureCornerPoint[0], arrMeasureCornerPoint[2]);

            pMeasureCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
            fMeasureWidth = ((arrMeasureCornerPoint[1].X + arrMeasureCornerPoint[2].X) / 2) - ((arrMeasureCornerPoint[0].X + arrMeasureCornerPoint[3].X) / 2);
            fMeasureHeight = ((arrMeasureCornerPoint[2].Y + arrMeasureCornerPoint[3].Y) / 2) - ((arrMeasureCornerPoint[0].Y + arrMeasureCornerPoint[1].Y) / 2);

            return true;
        }

        private bool GetPositionByCorner(List<LGauge> arrLineGauges, float fUnitWidth, float fUnitHeight, float fMinBorderScore,
                                        ref PointF pMeasureCenterPoint, ref PointF[] arrCorner, ref float fMeasureWidth, ref float fMeasureHeight,
                                        ref float fMeasureAngle, ref string strErrorMessage)
        {
            //  4		0	      6
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10



            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[arrLineGauges.Count];
            if (!DefineCornerPoints(arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore, ref fTotalAngle, ref nCount, ref blnResult, ref arrCorner))
            {
                return false;
            }

            // Get rectangle edge lines
            PointF[] arrEdgeCenterPoint = new PointF[4];
            Line[] arrEdgeCenterLines = new Line[4];
            Line[] arrEdgeLines = new Line[4];
            fTotalAngle = 0;
            int intPassCount = 0;
            if ((arrCorner[0].X != 0) && (arrCorner[1].X != 0))
            {
                arrEdgeLines[0] = new Line();
                arrEdgeLines[0].CalculateStraightLine(arrCorner[0], arrCorner[1]);
                if (arrEdgeLines[0].ref_dAngle > 0)
                    fTotalAngle += (float)arrEdgeLines[0].ref_dAngle - 90;
                else
                    fTotalAngle += (float)arrEdgeLines[0].ref_dAngle + 90;
                intPassCount++;
            }
            if ((arrCorner[1].X != 0) && (arrCorner[2].X != 0))
            {
                arrEdgeLines[1] = new Line();
                arrEdgeLines[1].CalculateStraightLine(arrCorner[1], arrCorner[2]);
                fTotalAngle += (float)arrEdgeLines[1].ref_dAngle;
                intPassCount++;
            }
            if ((arrCorner[2].X != 0) && (arrCorner[3].X != 0))
            {
                arrEdgeLines[2] = new Line();
                arrEdgeLines[2].CalculateStraightLine(arrCorner[2], arrCorner[3]);
                if (arrEdgeLines[2].ref_dAngle > 0)
                    fTotalAngle += (float)arrEdgeLines[2].ref_dAngle - 90;
                else
                    fTotalAngle += (float)arrEdgeLines[2].ref_dAngle + 90;
                intPassCount++;
            }
            if ((arrCorner[3].X != 0) && (arrCorner[0].X != 0))
            {
                arrEdgeLines[3] = new Line();
                arrEdgeLines[3].CalculateStraightLine(arrCorner[3], arrCorner[0]);
                fTotalAngle += (float)arrEdgeLines[3].ref_dAngle;
                intPassCount++;
            }
            
            // Calculate rectangle angle
            fMeasureAngle = fTotalAngle / intPassCount;

            // Get rectangle corner close lines
            Line[] arrCrossLines = new Line[2];
            arrCrossLines[0] = new Line();
            arrCrossLines[1] = new Line();

            // Calculate center point
            // -------- ver line ---------------------------------------
            if (arrEdgeLines[0] != null && arrEdgeLines[2] != null)
            {
                arrCrossLines[0].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[1].X) / 2, (arrCorner[0].Y + arrCorner[1].Y) / 2),
                                                       new PointF((arrCorner[2].X + arrCorner[3].X) / 2, (arrCorner[2].Y + arrCorner[3].Y) / 2));

                fMeasureWidth = (Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]) + Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3])) / 2;
            }
            else if (arrEdgeLines[0] != null)
            {
                arrCrossLines[0].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[1].X) / 2, (arrCorner[0].Y + arrCorner[1].Y) / 2),
                                                        arrEdgeLines[0].ref_dAngle + 90);

                fMeasureWidth = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
            }
            else if (arrEdgeLines[2] != null)
            {
                arrCrossLines[0].CalculateStraightLine(new PointF((arrCorner[2].X + arrCorner[3].X) / 2, (arrCorner[2].Y + arrCorner[3].Y) / 2),
                                                        arrEdgeLines[2].ref_dAngle + 90);

                fMeasureWidth = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
            }

            // ---------- Hor line ------------
            if (arrEdgeLines[1] != null && arrEdgeLines[3] != null)
            {
                arrCrossLines[1].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[3].X) / 2, (arrCorner[0].Y + arrCorner[3].Y) / 2),
                                                       new PointF((arrCorner[1].X + arrCorner[2].X) / 2, (arrCorner[1].Y + arrCorner[2].Y) / 2));

                fMeasureHeight = (Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]) + Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2])) / 2;
            }
            else if (arrEdgeLines[1] != null)
            {
                arrCrossLines[1].CalculateStraightLine(new PointF((arrCorner[1].X + arrCorner[2].X) / 2, (arrCorner[1].Y + arrCorner[2].Y) / 2),
                                                        arrEdgeLines[1].ref_dAngle + 90);

                fMeasureHeight = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
            }
            else if (arrEdgeLines[3] != null)
            {
                arrCrossLines[1].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[3].X) / 2, (arrCorner[0].Y + arrCorner[3].Y) / 2),
                                                        arrEdgeLines[3].ref_dAngle + 90);

                fMeasureHeight = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]);
            }

            pMeasureCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);

            return true;
        }

        private bool PlaceCornerLineGaugeLocation(ref List<LGauge> arrLineGauges, ref PointF[] arrMeasureCornerPoint, float fUnitWidth, float fUnitHeight)
        {
            //  4		0	      6
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10


            if (arrLineGauges.Count < 12)
            {
                SRMMessageBox.Show("PlaceCornerLineGaugeLocation() - line gauge count is not 12.");
                return false;
            }
            
            float fGuageTolerance = Math.Min(fUnitWidth / 3, fUnitHeight / 3);
            float fGuageLength = fUnitWidth / 3 + fGuageTolerance;
            arrLineGauges[4].SetGaugePlacement(arrMeasureCornerPoint[0].X + fGuageLength / 2 - fGuageTolerance,
                                                               arrMeasureCornerPoint[0].Y,
                                                               fGuageTolerance,
                                                               fGuageLength,
                                                               arrLineGauges[0].ref_ObjectAngle);
            arrLineGauges[6].SetGaugePlacement(arrMeasureCornerPoint[1].X - fGuageLength / 2 + fGuageTolerance,
                                                                           arrMeasureCornerPoint[1].Y,
                                                                           fGuageTolerance,
                                                                           fGuageLength,
                                                                           arrLineGauges[0].ref_ObjectAngle);
            arrLineGauges[8].SetGaugePlacement(arrMeasureCornerPoint[3].X + fGuageLength / 2 - fGuageTolerance,
                                                                                       arrMeasureCornerPoint[3].Y,
                                                                                       fGuageTolerance,
                                                                                       fGuageLength,
                                                                                       arrLineGauges[2].ref_ObjectAngle + 180);
            arrLineGauges[10].SetGaugePlacement(arrMeasureCornerPoint[2].X - fGuageLength / 2 + fGuageTolerance,
                                                                                       arrMeasureCornerPoint[2].Y,
                                                                                       fGuageTolerance,
                                                                                       fGuageLength,
                                                                                       arrLineGauges[2].ref_ObjectAngle + 180);

            fGuageLength = fUnitHeight / 3 + fGuageTolerance;
            arrLineGauges[5].SetGaugePlacement(arrMeasureCornerPoint[0].X,
                                                               arrMeasureCornerPoint[0].Y + fGuageLength / 2 - fGuageTolerance,
                                                               fGuageTolerance,
                                                               fGuageLength,
                                                               arrLineGauges[0].ref_ObjectAngle + 270);
            arrLineGauges[9].SetGaugePlacement(arrMeasureCornerPoint[3].X,
                                                   arrMeasureCornerPoint[3].Y - fGuageLength / 2 + fGuageTolerance,
                                                   fGuageTolerance,
                                                   fGuageLength,
                                                   arrLineGauges[3].ref_ObjectAngle + 270);
            arrLineGauges[7].SetGaugePlacement(arrMeasureCornerPoint[1].X,
                                                   arrMeasureCornerPoint[1].Y + fGuageLength / 2 - fGuageTolerance,
                                                   fGuageTolerance,
                                                   fGuageLength,
                                                   arrLineGauges[1].ref_ObjectAngle + 90);
            arrLineGauges[11].SetGaugePlacement(arrMeasureCornerPoint[2].X,
                                                   arrMeasureCornerPoint[2].Y - fGuageLength / 2 + fGuageTolerance,
                                                   fGuageTolerance,
                                                   fGuageLength,
                                                   arrLineGauges[2].ref_ObjectAngle + 90);

            return true;
        }

        private bool DefineCornerPoints(List<LGauge> arrLineGauges, float fUnitWidth, float fUnitHeight, float fMinBorderScore, ref float fTotalAngle, ref int nCount, ref bool[] blnResult, ref PointF[] arrCorner)
        {
            //  4		0	      6
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10

            blnResult = new bool[arrLineGauges.Count];

            // Filter angle with score
            int intFailCount = 0;
            float[] fUnsortAngle = new float[arrLineGauges.Count];
            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                if ((i < 4) && arrLineGauges[i].ref_ObjectScore < fMinBorderScore)
                {
                    blnResult[i] = false;
                    intFailCount++;
                }
                else if ((i >= 4) && arrLineGauges[i].ref_ObjectScore < (fMinBorderScore / 3))
                {
                    blnResult[i] = false;
                    intFailCount++;
                }
                else
                {
                    blnResult[i] = true;
                    fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                    arrLineGauges[i].GetObjectLine();
                }
            }



            // Get mode 
            float fMinData = float.MaxValue;
            float fMaxData = float.MinValue;
            Math2.MinMax(fUnsortAngle, blnResult, ref fMinData, ref fMaxData);
            float fRange = fMaxData - fMinData;
            float fWidth = 1; // Start with 1.
            float fStartBar;
            float fEndBar;
            int intHighestRatioBar = 0;
            float fHighestRatio = 0;
            float fHigestRatioWidth = 0;
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 401");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                int intTotalBar = (int)Math.Ceiling(fRange / fWidth);
                int[] intMatchCount = new int[intTotalBar];
                int intHighestCount = 0;
                int intHigestBar = 0;

                for (int c = 0; c < intTotalBar; c++)
                {
                    fStartBar = fMinData + fWidth * c;
                    fEndBar = fMinData + fWidth * (c + 1);
                    for (int i = 0; i < fUnsortAngle.Length; i++)
                    {
                        if (!blnResult[i])
                            continue;

                        // within bar
                        if ((fUnsortAngle[i] >= fStartBar) && (fUnsortAngle[i] < fEndBar))
                        {
                            intMatchCount[c]++;
                        }
                    }

                    // Keep highest count bar
                    if (intHighestCount < intMatchCount[c])
                    {
                        intHighestCount = intMatchCount[c];
                        intHigestBar = c;
                    }
                }

                // ------- Get HIGHTEST RATIO -----------------------------
                float fRatio = (float)intHighestCount / fWidth;
                if (fHighestRatio < fRatio)
                {
                    fHighestRatio = fRatio;
                    intHighestRatioBar = intHigestBar;
                    fHigestRatioWidth = fWidth;
                }
                // --------------------------------------------------------

                float fPercentage = (float)intHighestCount / (fUnsortAngle.Length - intFailCount);
                if (fPercentage >= 0.75)
                    break;

                fWidth += 0.5f;


            } while (true);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            float fLowestValue;
            float fHighestValue;
            fStartBar = fLowestValue = fMinData + fHigestRatioWidth * intHighestRatioBar;
            fEndBar = fHighestValue = fMinData + fHigestRatioWidth * (intHighestRatioBar + 1);
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                if ((fUnsortAngle[i] >= fStartBar) && (fUnsortAngle[i] < fEndBar))
                {
                    if (fLowestValue > fUnsortAngle[i])
                        fLowestValue = fUnsortAngle[i];

                    if (fHighestValue < fUnsortAngle[i])
                        fHighestValue = fUnsortAngle[i];
                }
            }

            float fModeAngle = (fLowestValue + fHighestValue) / 2;
            float fSelectedAngle = fTotalAngle = fModeAngle;

            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                if (!blnResult[i])
                    continue;

                arrLineGauges[i].GetObjectLine();
            }


            #region Determine which line to use
            arrLines2 = new Line[8];
            bool[] arrFailLines2 = new bool[8];
            if (blnResult[4] && blnResult[0])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[4].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[0].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[0] = arrLineGauges[4].ref_ObjectLine;
                else
                    arrLines2[0] = arrLineGauges[0].ref_ObjectLine;
            }
            else if (blnResult[0])
                arrLines2[0] = arrLineGauges[0].ref_ObjectLine;
            else if (blnResult[4])
                arrLines2[0] = arrLineGauges[4].ref_ObjectLine;
            else
                arrFailLines2[0] = true;

            if (blnResult[5] && blnResult[3])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[5].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[3].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[1] = arrLineGauges[5].ref_ObjectLine;
                else
                    arrLines2[1] = arrLineGauges[3].ref_ObjectLine;
            }
            else if (blnResult[3])
                arrLines2[1] = arrLineGauges[3].ref_ObjectLine;
            else if (blnResult[5])
                arrLines2[1] = arrLineGauges[5].ref_ObjectLine;
            else
                arrFailLines2[1] = true;

            if (blnResult[6] && blnResult[0])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[6].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[0].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[2] = arrLineGauges[6].ref_ObjectLine;
                else
                    arrLines2[2] = arrLineGauges[0].ref_ObjectLine;
            }
            else if (blnResult[0])
                arrLines2[2] = arrLineGauges[0].ref_ObjectLine;
            else if (blnResult[6])
                arrLines2[2] = arrLineGauges[6].ref_ObjectLine;
            else
                arrFailLines2[2] = true;


            if (blnResult[7] && blnResult[1])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[7].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[1].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[3] = arrLineGauges[7].ref_ObjectLine;
                else
                    arrLines2[3] = arrLineGauges[1].ref_ObjectLine;
            }
            else if (blnResult[1])
                arrLines2[3] = arrLineGauges[1].ref_ObjectLine;
            else if (blnResult[7])
                arrLines2[3] = arrLineGauges[7].ref_ObjectLine;
            else
                arrFailLines2[3] = true;

            if (blnResult[8] && blnResult[2])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[8].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[2].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[4] = arrLineGauges[8].ref_ObjectLine;
                else
                    arrLines2[4] = arrLineGauges[2].ref_ObjectLine;
            }
            else if (blnResult[2])
                arrLines2[4] = arrLineGauges[2].ref_ObjectLine;
            else if (blnResult[8])
                arrLines2[4] = arrLineGauges[8].ref_ObjectLine;
            else
                arrFailLines2[4] = true;

            if (blnResult[9] && blnResult[3])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[9].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[3].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[5] = arrLineGauges[9].ref_ObjectLine;
                else
                    arrLines2[5] = arrLineGauges[3].ref_ObjectLine;
            }
            else if (blnResult[3])
                arrLines2[5] = arrLineGauges[3].ref_ObjectLine;
            else if (blnResult[9])
                arrLines2[5] = arrLineGauges[9].ref_ObjectLine;
            else
                arrFailLines2[5] = true;

            if (blnResult[10] && blnResult[2])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[10].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[2].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[6] = arrLineGauges[10].ref_ObjectLine;
                else
                    arrLines2[6] = arrLineGauges[2].ref_ObjectLine;
            }
            else if (blnResult[2])
                arrLines2[6] = arrLineGauges[2].ref_ObjectLine;
            else if (blnResult[10])
                arrLines2[6] = arrLineGauges[10].ref_ObjectLine;
            else
                arrFailLines2[6] = true;

            if (blnResult[11] && blnResult[1])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[11].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[1].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    arrLines2[7] = arrLineGauges[11].ref_ObjectLine;
                else
                    arrLines2[7] = arrLineGauges[1].ref_ObjectLine;
            }
            else if (blnResult[1])
                arrLines2[7] = arrLineGauges[1].ref_ObjectLine;
            else if (blnResult[11])
                arrLines2[7] = arrLineGauges[11].ref_ObjectLine;
            else
                arrFailLines2[7] = true;

            #endregion

            // Get corner points
            float[] arrCornerGaugeAngles = new float[4];
            if (arrFailLines2[0] || arrFailLines2[1])
            {
                arrCorner[0] = new PointF(0, 0);
                arrCornerGaugeAngles[0] = 0f;
            }
            else
            {
                arrCorner[0] = Line.GetCrossPoint(arrLines2[0], arrLines2[1]);
                arrCornerGaugeAngles[0] = Math2.GetAngle(arrLines2[0], arrLines2[1]);
            }

            if (arrFailLines2[2] || arrFailLines2[3])
            {
                arrCorner[1] = new PointF(0, 0);
                arrCornerGaugeAngles[1] = 0f;
            }
            else
            {
                arrCorner[1] = Line.GetCrossPoint(arrLines2[2], arrLines2[3]);
                arrCornerGaugeAngles[1] = Math2.GetAngle(arrLines2[2], arrLines2[3]);
            }


            if (arrFailLines2[6] || arrFailLines2[7])
            {
                arrCorner[2] = new PointF(0, 0);
                arrCornerGaugeAngles[2] = 0f;
            }
            else
            {
                arrCorner[2] = Line.GetCrossPoint(arrLines2[6], arrLines2[7]);
                arrCornerGaugeAngles[2] = Math2.GetAngle(arrLines2[6], arrLines2[7]);
            }


            if (arrFailLines2[4] || arrFailLines2[5])
            {
                arrCorner[3] = new PointF(0, 0);
                arrCornerGaugeAngles[3] = 0f;
            }
            else
            {
                arrCorner[3] = Line.GetCrossPoint(arrLines2[4], arrLines2[5]);
                arrCornerGaugeAngles[3] = Math2.GetAngle(arrLines2[4], arrLines2[5]);
            }

            List<int> arrFailIndex = new List<int>();
            float fLimit = 5;
            // ------------------- checking loop timeout ---------------------------------------------------
            timeout.Start();

            do
            {
                arrFailIndex.Clear();
                for (int i = 0; i < 4; i++)
                {
                    if (Math.Abs(arrCornerGaugeAngles[i] - 90) > fLimit)
                    {
                        arrFailIndex.Add(i);
                    }
                }

                if (arrFailIndex.Count > 0)
                    break;

                fLimit -= 0.5f;


                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 400");
                    break;
                }
                // ---------------------------------------------------------------------------------------------



            } while (fLimit > 2);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            if (arrFailIndex.Count == 1)
            {
                List<int> arrFailIndex2 = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    if (Math.Abs(arrCornerGaugeAngles[i] - 90) > 2)
                    {
                        arrFailIndex2.Add(i);
                    }
                }

                bool blnDone = false;
                if (arrFailIndex2.Count == 2)
                {
                    switch (arrFailIndex[0])
                    {
                        case 0:
                        case 2:
                            if (arrFailIndex2[0] == 0 && arrFailIndex2[1] == 2)
                            {
                                arrCorner[0] = Line.GetCrossPoint(arrLines2[2], arrLines2[5]);
                                arrCorner[2] = Line.GetCrossPoint(arrLines2[3], arrLines2[3]);
                                arrCornerGaugeAngles[0] = Math2.GetAngle(arrLines2[2], arrLines2[5]);
                                arrCornerGaugeAngles[2] = Math2.GetAngle(arrLines2[3], objVirtualLineHeight);
                                blnDone = true;
                            }
                            break;
                        case 1:
                        case 3:
                            if (arrFailIndex2[0] == 1 && arrFailIndex2[1] == 3)
                            {
                                arrCorner[1] = Line.GetCrossPoint(arrLines2[0], arrLines2[7]);
                                arrCorner[3] = Line.GetCrossPoint(arrLines2[1], arrLines2[6]);
                                arrCornerGaugeAngles[1] = Math2.GetAngle(arrLines2[0], arrLines2[7]);
                                arrCornerGaugeAngles[3] = Math2.GetAngle(arrLines2[1], arrLines2[6]);
                                blnDone = true;
                            }
                            break;
                    }
                }

                if (!blnDone)
                {

                    objVirtualLineWidth = new Line();
                    objVirtualLineHeight = new Line();
                    // calculate the fail point using other 3 correct points
                    float fShiftHeight, fShiftWidth;
                    switch (arrFailIndex[0])
                    {
                        case 0:
                            // x line
                            if (Math.Abs(arrLines2[4].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[6].ref_dAngle - fSelectedAngle))
                                arrLines2[4].CopyTo(ref objVirtualLineWidth);
                            else
                                arrLines2[6].CopyTo(ref objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
                            objVirtualLineWidth.ShiftYLine(-fShiftHeight);

                            // y line
                            if (Math.Abs(arrLines2[3].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[7].ref_dAngle - fSelectedAngle))
                                arrLines2[3].CopyTo(ref objVirtualLineHeight);
                            else
                                arrLines2[7].CopyTo(ref objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
                            objVirtualLineHeight.ShiftXLine2(-fShiftWidth);
                            break;
                        case 1:
                            // x line
                            if (Math.Abs(arrLines2[4].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[6].ref_dAngle - fSelectedAngle))
                                arrLines2[4].CopyTo(ref objVirtualLineWidth);
                            else
                                arrLines2[6].CopyTo(ref objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]);
                            objVirtualLineWidth.ShiftYLine(-fShiftHeight);

                            // y line
                            if (Math.Abs(arrLines2[1].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[5].ref_dAngle - fSelectedAngle))
                                arrLines2[1].CopyTo(ref objVirtualLineHeight);
                            else
                                arrLines2[5].CopyTo(ref objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
                            objVirtualLineHeight.ShiftXLine2(fShiftWidth);
                            break;
                        case 2:
                            // x line
                            if (Math.Abs(arrLines2[0].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[2].ref_dAngle - fSelectedAngle))
                                arrLines2[0].CopyTo(ref objVirtualLineWidth);
                            else
                                arrLines2[2].CopyTo(ref objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]);
                            objVirtualLineWidth.ShiftYLine(fShiftHeight);

                            // y line
                            if (Math.Abs(arrLines2[1].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[5].ref_dAngle - fSelectedAngle))
                                arrLines2[1].CopyTo(ref objVirtualLineHeight);
                            else
                                arrLines2[5].CopyTo(ref objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
                            objVirtualLineHeight.ShiftXLine2(fShiftWidth);
                            break;
                        case 3:
                            // x line
                            if (Math.Abs(arrLines2[0].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[2].ref_dAngle - fSelectedAngle))
                                arrLines2[0].CopyTo(ref objVirtualLineWidth);
                            else
                                arrLines2[2].CopyTo(ref objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
                            objVirtualLineWidth.ShiftYLine(fShiftHeight);

                            // y line
                            if (Math.Abs(arrLines2[3].ref_dAngle - fSelectedAngle) < Math.Abs(arrLines2[7].ref_dAngle - fSelectedAngle))
                                arrLines2[3].CopyTo(ref objVirtualLineHeight);
                            else
                                arrLines2[7].CopyTo(ref objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
                            objVirtualLineHeight.ShiftXLine2(-fShiftWidth);
                            break;
                    }

                    arrCorner[arrFailIndex[0]] = Line.GetCrossPoint(objVirtualLineWidth, objVirtualLineHeight);
                    arrCornerGaugeAngles[arrFailIndex[0]] = Math2.GetAngle(objVirtualLineWidth, objVirtualLineHeight);
                }
            }
            else if (arrFailIndex.Count == 2)
            {
                if ((arrFailIndex[0] == 0 && arrFailIndex[1] == 2) ||
                    (arrFailIndex[0] == 1 && arrFailIndex[1] == 3))
                {
                    List<int> arrFailIndex2 = new List<int>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (Math.Abs(arrCornerGaugeAngles[i] - 90) > 2)
                        {
                            arrFailIndex2.Add(i);
                        }
                    }

                    if (arrFailIndex2.Count == 2)
                    {
                        switch (arrFailIndex[0])
                        {
                            case 0:
                                arrCorner[0] = Line.GetCrossPoint(arrLines2[2], arrLines2[5]);
                                arrCorner[2] = Line.GetCrossPoint(arrLines2[3], arrLines2[3]);
                                arrCornerGaugeAngles[0] = Math2.GetAngle(arrLines2[2], arrLines2[5]);
                                arrCornerGaugeAngles[2] = Math2.GetAngle(arrLines2[3], objVirtualLineHeight);
                                break;
                            case 1:
                                arrCorner[1] = Line.GetCrossPoint(arrLines2[0], arrLines2[7]);
                                arrCorner[3] = Line.GetCrossPoint(arrLines2[1], arrLines2[6]);
                                arrCornerGaugeAngles[1] = Math2.GetAngle(arrLines2[0], arrLines2[7]);
                                arrCornerGaugeAngles[3] = Math2.GetAngle(arrLines2[1], arrLines2[6]);
                                break;
                        }
                    }
                    else
                    {
                        m_strErrorMessage = "Corner Points Fail. There are 2 or more invalid corner points";
                        m_intFailResultMask |= 0x02;
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage = "Corner Points Fail. There are 2 or more invalid corner points";
                    m_intFailResultMask |= 0x02;
                    return false;
                }
            }
            else if (arrFailIndex.Count > 2)
            {
                m_strErrorMessage = "Corner Points Fail. There are 2 or more invalid corner points";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Verify are corners points correct
            //Verify4CornerPoint(fUnitWidth, fUnitHeight, ref arrCorner);
            // create line
            float[] arrCornerAngles = new float[4];
            float[] arrEdgeDistances = new float[4];

            arrEdgeDistances[0] = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
            arrEdgeDistances[1] = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
            arrEdgeDistances[2] = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
            arrEdgeDistances[3] = Math2.GetDistanceBtw2Points(arrCorner[3], arrCorner[0]);

            float fCloseDistance1 = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[2]);
            float fCloseDistance2 = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[3]);

            arrCornerAngles[0] = Math2.GetAngle(arrEdgeDistances[0], arrEdgeDistances[3], fCloseDistance2);
            arrCornerAngles[1] = Math2.GetAngle(arrEdgeDistances[0], arrEdgeDistances[1], fCloseDistance1);
            arrCornerAngles[2] = Math2.GetAngle(arrEdgeDistances[1], arrEdgeDistances[2], fCloseDistance2);
            arrCornerAngles[3] = Math2.GetAngle(arrEdgeDistances[2], arrEdgeDistances[3], fCloseDistance1);

            int intFailCount2 = 0;
            for (int i = 0; i < 4; i++)
            {
                if (Math.Abs(arrCornerAngles[i] - 90) > 3)
                {
                    arrCorner[i] = new PointF(0, 0);
                    intFailCount2++;
                }
            }

            if (intFailCount2 >= 2)
            {
                m_strErrorMessage = "Fail to measure clear corner edge.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            return true;
        }

        private float GetFormatedAngle(float fSourceAngle)
        {
            if (fSourceAngle > -45 && fSourceAngle <= 45)
                return fSourceAngle;
            else if (fSourceAngle > 45 && fSourceAngle <= 135)
                return fSourceAngle - 90;
            else if (fSourceAngle > 135 && fSourceAngle <= 225)
                return fSourceAngle - 180;
            else if (fSourceAngle > 225 && fSourceAngle <= 315)
                return fSourceAngle - 270;
            else if (fSourceAngle > 315)
                return fSourceAngle - 360;
            else if (fSourceAngle > -135 && fSourceAngle <= -45)
                return fSourceAngle + 90;
            else if (fSourceAngle > -225 && fSourceAngle <= -135)
                return fSourceAngle + 180;
            else if (fSourceAngle > -315 && fSourceAngle <= -225)
                return fSourceAngle + 270;
            else if (fSourceAngle <= -315)
                return fSourceAngle + 360;
            else 
                return fSourceAngle;
        }
        /// <summary>
        /// If the specific directory does not exist, create a new one
        /// </summary>
        private void CreateUnexistDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
            {
                CreateUnexistDirectory(directory.Parent);
            }

            Directory.CreateDirectory(directory.FullName);

        }

        private void FilterEdgeAngle(List<LGauge> arrLineGauges, float fMinBorderScore, float fAngleModeTolerance, ref float fTotalAngle, ref int nCount, ref bool[] blnResult)
        {
            int intUseGaugeCount = 4;
            blnResult = new bool[intUseGaugeCount];

            // Filter angle with score
            float[] fUnsortAngle = new float[intUseGaugeCount];
            for (int i = 0; i < intUseGaugeCount; i++)
            {
                if ((i < 4) && arrLineGauges[i].ref_ObjectScore < fMinBorderScore)
                {
                    blnResult[i] = false;
                }
                else
                {
                    blnResult[i] = true;
                    fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                    arrLineGauges[i].GetObjectLine();
                }
            }

            // Sort angle from increase
            List<float> fSortAngle = new List<float>();
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                int intIndex = fSortAngle.Count;
                for (int j = 0; j < fSortAngle.Count; j++)
                {
                    if (fSortAngle[j] > fUnsortAngle[i])
                    {
                        intIndex = j;
                        break;
                    }
                }
                fSortAngle.Insert(intIndex, fUnsortAngle[i]);
            }

            // Find the smallest gap between the angle value
            float fSmallestGap = float.MaxValue;
            float fCenterAngle = 0;
            for (int i = 1; i < fSortAngle.Count; i++)
            {
                if ((fSortAngle[i] - fSortAngle[i - 1]) < fSmallestGap)
                {
                    fSmallestGap = fSortAngle[i] - fSortAngle[i - 1];
                    fCenterAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                }
            }

            // Filter angle with fAngleModeTolerance from smallest angle
            fTotalAngle = 0;
            nCount = 0;
            for (int i = 0; i < fSortAngle.Count; i++)
            {
                if ((fSortAngle[i] >= (fCenterAngle - fAngleModeTolerance)) && (fSortAngle[i] <= (fCenterAngle + fAngleModeTolerance)))
                {
                    fTotalAngle += fSortAngle[i];
                    nCount++;
                }
                else
                {
                    fSortAngle[i] = -999;
                }

            }

            // Set the blnResult to false if the angle has been filter out.
            for (int j = 0; j < blnResult.Length; j++)
            {
                if (blnResult[j])
                {
                    bool blnFound = false;
                    for (int i = 0; i < fSortAngle.Count; i++)
                    {
                        if (fUnsortAngle[j] == fSortAngle[i])
                        {
                            blnFound = true;
                            break;
                        }
                    }
                    if (!blnFound)
                        blnResult[j] = false;
                }
            }
        }

        public static void DrawPosition(Graphics g, float fCenterPointX, float fCenterPointY, float fAngle, float fWidth, float fHeight)
        {
            PointF pxy = new PointF();
            PointF pxY = new PointF();
            PointF pXy = new PointF();
            PointF pXY = new PointF();
            Math2.GetCornerPoints(fCenterPointX, fCenterPointY, fAngle, fWidth, fHeight, ref pxy, ref pXy, ref pxY, ref pXY);

            g.DrawLine(new Pen(Color.Lime), pxy, pxY);
            g.DrawLine(new Pen(Color.Lime), pxY, pXY);
            g.DrawLine(new Pen(Color.Lime), pXY, pXy);
            g.DrawLine(new Pen(Color.Lime), pXy, pxy);
            g.DrawLine(new Pen(Color.Lime), fCenterPointX - 5, fCenterPointY, fCenterPointX + 5, fCenterPointY);
            g.DrawLine(new Pen(Color.Lime), fCenterPointX, fCenterPointY - 5, fCenterPointX, fCenterPointY + 5);
        }
    }
}
