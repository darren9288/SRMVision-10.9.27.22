using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;


namespace VisionProcessing
{
    public class Orient
    {
        #region Member Variables      

        private bool m_blnWantCheckOrientation = true;
        private bool m_blnWantCheckOrientAngleTolerance = false;
        private bool m_blnWantCheckOrientXTolerance = false;
        private bool m_blnWantCheckOrientYTolerance = false;

        private bool m_blnWantSubROI = false;
        private int m_intCorrectAngleMethod = 0; // 0=Guage, 1=PRS
        private float m_fTemplateCenterX;
        private float m_fTemplateCenterY;
        private float m_fObjectOriCenterX;
        private float m_fObjectOriCenterY;
        private float m_fObjectCenterX = -1;
        private float m_fObjectCenterY = -1;
        private float m_fSubObjectCenterX = -1;
        private float m_fSubObjectCenterY = -1;
        private float m_fDegAngleResult = 0;
        private float m_fOrientMinScore = 0.7f;
        private float m_fAngleTolerance = 0;
        private float m_fXTolerance = 0;
        private float m_fYTolerance = 0;
        private float m_fHighestScore = -1;
        private List<float> m_arrMatchScoreList = new List<float>();
        private List<float> m_arrMatchPositionXList = new List<float>();
        private List<float> m_arrMatchPositionYList = new List<float>();
        private int m_intAngleResult = 4;
        private int m_intDirections = 4;       // 2 = Rectangle units and 4 = Square unit
        private int m_intCheckMarkAngleMinMaxTolerance = 10;   // Default Orient Matcher Min Max is +-10
        private bool m_blnTemplateOrientationIsLeft = false;
        private bool m_blnTemplateOrientationIsTop = false;
        private bool m_blnWantUsePositionCheckOrientation = false;
        private float m_fCheckPositionOrientationWhenBelowDifferentScore = 0.1f;
        private string m_strErrorMessage = "";
        private ImageDrawing m_objRotateImage = new ImageDrawing();
        private EMatcher m_objUnitPRMatcher;
        private EMatcher m_objMatcher = new EMatcher();
        private EMatcher m_objSubMatcher = new EMatcher();
        private TrackLog m_objTrackLog = new TrackLog();
        private List<EMatcher> m_arrMatcher = new List<EMatcher>();

        private int m_intEmptyThreshold = 0;
        private int m_intEmptyMaxArea = 0;
        private int m_intMatcherOffSetCenterX = 0;    // Template EMatcher center X off set to Unit center x
        private int m_intMatcherOffSetCenterY = 0;    // Template EMatcher center Y off set to Unit center y
        private int m_intUnitSurfaceOffsetX = 0;      // Offset ROI X between Unit PR ROI and Unit Surface ROI
        private int m_intUnitSurfaceOffsetY = 0;      // Offset ROI Y between Unit PR ROI and Unit Surface ROI
        private int m_intRotatedAngle = 0;

        // Calibration
        private float m_fMMPerPixelX = 1;
        private float m_fMMPerPixelY = 1;
        private float m_fMMToPixelXValue = 1;
        private float m_fMMToPixelYValue = 1;
        private float m_fMMtoPixelAreaValue = 1;

        private float m_fMMToUnitValue = 1;
        private float m_fMMToUnitValueArea = 1;
        #endregion

        #region Properties
        public bool ref_blnWantCheckOrientation { get { return m_blnWantCheckOrientation; } set { m_blnWantCheckOrientation = value; } }
        public bool ref_blnWantCheckOrientAngleTolerance { get { return m_blnWantCheckOrientAngleTolerance; } set { m_blnWantCheckOrientAngleTolerance = value; } }
        public bool ref_blnWantCheckOrientXTolerance { get { return m_blnWantCheckOrientXTolerance; } set { m_blnWantCheckOrientXTolerance = value; } }
        public bool ref_blnWantCheckOrientYTolerance { get { return m_blnWantCheckOrientYTolerance; } set { m_blnWantCheckOrientYTolerance = value; } }
        public bool ref_blnUnitPRMatcherExist { get { return m_objUnitPRMatcher != null; } }
        public int ref_intRotatedAngle { get { return m_intRotatedAngle; } set { m_intRotatedAngle = value; } }
        public bool ref_blnWantSubROI { get { return m_blnWantSubROI; } set { m_blnWantSubROI = value; } }
        public int ref_intCorrectAngleMethod { get { return m_intCorrectAngleMethod; } set { m_intCorrectAngleMethod = value; } }
        public int ref_intAngleResult { get { return m_intAngleResult; } set { m_intAngleResult = value; } }
        public int ref_intDirections { get { return m_intDirections; } set { m_intDirections = value; } }
        public int ref_intCheckMarkAngleMinMaxTolerance { get { return m_intCheckMarkAngleMinMaxTolerance; } set { m_intCheckMarkAngleMinMaxTolerance = value; } }

        public bool ref_blnTemplateOrientationIsLeft { get { return m_blnTemplateOrientationIsLeft; } set { m_blnTemplateOrientationIsLeft = value; } }
        public bool ref_blnTemplateOrientationIsTop { get { return m_blnTemplateOrientationIsTop; } set { m_blnTemplateOrientationIsTop = value; } }
        public bool ref_blnWantUsePositionCheckOrientation { get { return m_blnWantUsePositionCheckOrientation; } set { m_blnWantUsePositionCheckOrientation = value; } }
        public float ref_fCheckPositionOrientationWhenBelowDifferentScore { get { return m_fCheckPositionOrientationWhenBelowDifferentScore; } set { m_fCheckPositionOrientationWhenBelowDifferentScore = value; } }
        public int ref_intDontCareThreshold { get { return (int)m_objMatcher.DontCareThreshold; } }
        public int ref_intUnitPatternWidth { get { return m_objMatcher.PatternWidth; } }
        public int ref_intUnitPatternHeight { get { return m_objMatcher.PatternHeight; } }
        public int ref_intSubPatternWidth { get { return m_objSubMatcher.PatternWidth; } }
        public int ref_intSubPatternHeight { get { return m_objSubMatcher.PatternHeight; } }
        public float ref_fMinScore { get { return m_fOrientMinScore; } set { m_fOrientMinScore = value; } }
        public float ref_fAngleTolerance { get { return m_fAngleTolerance; } set { m_fAngleTolerance = value; } }
        public float ref_fXTolerance { get { return m_fXTolerance; } set { m_fXTolerance = value; } }
        public float ref_fYTolerance { get { return m_fYTolerance; } set { m_fYTolerance = value; } }
        public float ref_fXTolerance_Pixel { get { return m_fXTolerance * m_fMMToPixelXValue; } }
        public float ref_fYTolerance_Pixel { get { return m_fYTolerance * m_fMMToPixelYValue; } }
        public float ref_fObjectX { get { return m_fObjectCenterX; } }
        public float ref_fObjectY { get { return m_fObjectCenterY; } }
        public float ref_fSubObjectX { get { return m_fSubObjectCenterX; } }
        public float ref_fSubObjectY { get { return m_fSubObjectCenterY; } }
        public float ref_fObjectOriX { get { return m_fObjectOriCenterX; } }
        public float ref_fObjectOriY { get { return m_fObjectOriCenterY; } }
        public float ref_fTemplateX { get { return m_fTemplateCenterX; } set { m_fTemplateCenterX = value; } }
        public float ref_fTemplateY { get { return m_fTemplateCenterY; } set { m_fTemplateCenterY = value; } }
        public float ref_fDegAngleResult { get { return m_fDegAngleResult; } }
        public List<float> ref_arrMatchScoreList { get { return m_arrMatchScoreList; } }
        public List<float> ref_arrMatchPositionXList { get { return m_arrMatchPositionXList; } }
        public List<float> ref_arrMatchPositionYList { get { return m_arrMatchPositionYList; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public ImageDrawing ref_objRotatedImage { get { return m_objRotateImage; } }

        public int ref_intEmptyThreshold { get { return m_intEmptyThreshold; } set { m_intEmptyThreshold = value; } }
        public int ref_intEmptyMaxArea { get { return m_intEmptyMaxArea; } set { m_intEmptyMaxArea = value; } }
        public int ref_intMatcherOffSetCenterX { get { return m_intMatcherOffSetCenterX; } set { m_intMatcherOffSetCenterX = value; } }
        public int ref_intMatcherOffSetCenterY { get { return m_intMatcherOffSetCenterY; } set { m_intMatcherOffSetCenterY = value; } }
        public int ref_intUnitSurfaceOffsetX { get { return m_intUnitSurfaceOffsetX; } set { m_intUnitSurfaceOffsetX = value; } }
        public int ref_intUnitSurfaceOffsetY { get { return m_intUnitSurfaceOffsetY; } set { m_intUnitSurfaceOffsetY = value; } }
        #endregion

        public Orient()
        {
            m_objRotateImage.ref_objMainImage.SetSize(640, 480);
        }
        public Orient(int Width, int Height)
        {
            m_objRotateImage.ref_objMainImage.SetSize(Width, Height);
        }

        /// <summary>
        /// Check whether Orient template is learn before. If not, it is not allowed to do matching
        /// </summary>
        public bool IsPatternLearnt()
        {
            return m_objMatcher.PatternLearnt;
        }
        /// <summary>
        /// Record this selected area of image as template
        /// </summary>
        /// <param name="trainROI">the area of dot, logo or character(s)</param>
        public bool LearnPattern(ROI objROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objMatcher.LearnPattern(objROI.ref_ROI);
                m_fTemplateCenterX = objROI.ref_ROICenterX;
                m_fTemplateCenterY = objROI.ref_ROICenterY;
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Orient Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Orient Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }
        public bool LearnPattern(ROI objROI, ROI objSearchROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objMatcher.LearnPattern(objROI.ref_ROI);
                m_fTemplateCenterX = objSearchROI.ref_ROITotalX + objROI.ref_ROICenterX;
                m_fTemplateCenterY = objSearchROI.ref_ROITotalY + objROI.ref_ROICenterY;
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Orient Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Orient Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }
        public bool LearnPattern4Direction(ROI objROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objMatcher.LearnPattern(objROI.ref_ROI);

                EImageBW8 objImage = new EImageBW8();
                objImage.SetSize(objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                EasyImage.Copy(objROI.ref_ROI.TopParent, objImage);

                m_arrMatcher.Clear();

                for (int i = 0; i < 4; i++)
                {
                    m_arrMatcher.Add(new EMatcher());
                    if (i == 0)
                    {
#if (Debug_2_12 || Release_2_12)
                        m_arrMatcher[i].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_arrMatcher[i].LearnPattern(objROI.ref_ROI);
                    }
                    else
                    {
                        EROIBW8 objSourceROI = new EROIBW8();
                        objSourceROI.Detach();
                        objSourceROI.Attach(objROI.ref_ROI.TopParent);
                        float fSquareSize = (float)Math.Max(objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                        float fROICenterX = objROI.ref_ROITotalX + (float)objROI.ref_ROIWidth / 2;
                        float fROICenterY = objROI.ref_ROITotalY + (float)objROI.ref_ROIHeight / 2;
                        objSourceROI.SetPlacement((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)fSquareSize, (int)fSquareSize);

                        EROIBW8 objDestROI = new EROIBW8();
                        objDestROI.Detach();
                        objDestROI.Attach(objImage);
                        objDestROI.SetPlacement(objSourceROI.OrgX, objSourceROI.OrgY, objSourceROI.Width, objSourceROI.Height);

                        int intRotateAngle;
                        if (i == 1)
                            intRotateAngle = -90;
                        else if (i == 2)
                            intRotateAngle = 180;
                        else
                            intRotateAngle = 90;
                        EasyImage.ScaleRotate(objSourceROI, objSourceROI.Width / 2f, objSourceROI.Height / 2f, objDestROI.Width / 2f, objDestROI.Height / 2f, 1, 1, intRotateAngle, objDestROI, 8);

                        EROIBW8 objPatternROI = new EROIBW8();
                        objPatternROI.Detach();
                        objPatternROI.Attach(objImage);
                        if (i == 2)
                        {
                            objPatternROI.SetPlacement(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                        }
                        else
                        {
                            // Width and Height are switched each other   
                            objPatternROI.SetPlacement((int)Math.Round(fROICenterX - (float)objROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(fROICenterY - (float)objROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       objROI.ref_ROIHeight, objROI.ref_ROIWidth);
                        }

#if (Debug_2_12 || Release_2_12)
                        m_arrMatcher[i].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_arrMatcher[i].LearnPattern(objPatternROI);
                        m_arrMatcher[i].FinalReduction = 2;

                    }

                }

                objImage.Dispose();
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Orient Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Orient Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LearnPattern4Direction_SRM(ROI objROI, float fCenterX, float fCenterY)
        {
            try
            {
                m_fTemplateCenterX = fCenterX;
                m_fTemplateCenterY = fCenterY;

                STTrackLog.WriteLine("Save=" + "*******************_i1o-1");
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objMatcher.LearnPattern(objROI.ref_ROI);

                STTrackLog.WriteLine("Save=" + "*******************_i1o-2");

                ImageDrawing objImage = new ImageDrawing(true);

                STTrackLog.WriteLine("Save=" + "*******************_i1o-3");

                objImage.SetImageSize(objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                EasyImage.Copy(objROI.ref_ROI.TopParent, objImage.ref_objMainImage);

                STTrackLog.WriteLine("Save=" + "*******************_i1o-4");
                //m_arrMatcher.Clear();

                STTrackLog.WriteLine("Save=" + "*******************_i1o-5");
                for (int i = 0; i < 4; i++)
                {
                    STTrackLog.WriteLine("Save=" + "*******************_i1o-5a-" + i.ToString());

                    if (m_arrMatcher.Count <= i)
                    {
                        m_arrMatcher.Add(new EMatcher());
                    }

                    STTrackLog.WriteLine("Save=" + "*******************_i1o-5b-" + i.ToString());

                    if (i == 0)
                    {
                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5c-" + i.ToString());
#if (Debug_2_12 || Release_2_12)
                        m_arrMatcher[i].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_arrMatcher[i].LearnPattern(objROI.ref_ROI);

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5d-" + i.ToString());
                    }
                    else
                    {
                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5e-" + i.ToString());

                        ROI objSourceROI = new ROI();

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5f-" + i.ToString());

                        objSourceROI.AttachROITopParrent(objROI);

                        float fSquareSize = (float)Math.Max(objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                        float fROICenterX = objROI.ref_ROITotalX + (float)objROI.ref_ROIWidth / 2;
                        float fROICenterY = objROI.ref_ROITotalY + (float)objROI.ref_ROIHeight / 2;

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5f-" + i.ToString() + "[" + fSquareSize.ToString() + "," +
                                                                                                            fROICenterX.ToString() + "," +
                                                                                                            fROICenterY + "," +
                                                                                                            ((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero)).ToString() + "," +
                                                                                                            ((int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero)).ToString());

                        //objSourceROI.SetPlacement((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                        //                                  (int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                        //                                  (int)fSquareSize, (int)fSquareSize);

                        objSourceROI.LoadROISetting((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)fSquareSize, (int)fSquareSize);


                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5g-" + i.ToString());
                        ROI objDestROI = new ROI();
                        objDestROI.AttachImage(objImage);
                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5g-" + i.ToString() + "[" +
                                                                                    objSourceROI.ref_ROIPositionX.ToString() + "," +
                                                                                    objSourceROI.ref_ROIPositionY.ToString() + "," +
                                                                                    objSourceROI.ref_ROIWidth.ToString() + "," +
                                                                                    objSourceROI.ref_ROIHeight.ToString());

                        objDestROI.LoadROISetting(objSourceROI.ref_ROIPositionX, objSourceROI.ref_ROIPositionY, objSourceROI.ref_ROIWidth, objSourceROI.ref_ROIHeight);

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5h-" + i.ToString());
                        int intRotateAngle;
                        if (i == 1)
                            intRotateAngle = -90;
                        else if (i == 2)
                            intRotateAngle = 180;
                        else
                            intRotateAngle = 90;

                        EasyImage.ScaleRotate(objSourceROI.ref_ROI, objSourceROI.ref_ROIWidth / 2f, objSourceROI.ref_ROIHeight / 2f, objDestROI.ref_ROIWidth / 2f, objDestROI.ref_ROIHeight / 2f, 1, 1, intRotateAngle, objDestROI.ref_ROI, 8);

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5i-" + i.ToString());

                        ROI objPatternROI = new ROI();
                        objPatternROI.AttachImage(objImage);

                        if (i == 2)
                        {
                            STTrackLog.WriteLine("Save=" + "*******************_i1o-5j-" + i.ToString() + "[" +
                                                                                    objROI.ref_ROITotalX.ToString() + "," +
                                                                                    objROI.ref_ROITotalY.ToString() + "," +
                                                                                    objROI.ref_ROIWidth.ToString() + "," +
                                                                                    objROI.ref_ROIHeight.ToString());

                            objPatternROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                        }
                        else
                        {
                            STTrackLog.WriteLine("Save=" + "*******************_i1o-5k-" + i.ToString() + "[" +
                                                                                   ((int)Math.Round(fROICenterX - (float)objROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero)).ToString() + "," +
                                                                                   ((int)Math.Round(fROICenterY - (float)objROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero)).ToString() + "," +
                                                                                   objROI.ref_ROIWidth.ToString() + "," +
                                                                                   objROI.ref_ROIHeight.ToString());
                            // Width and Height are switched each other   
                            objPatternROI.LoadROISetting((int)Math.Round(fROICenterX - (float)objROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(fROICenterY - (float)objROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       objROI.ref_ROIHeight, objROI.ref_ROIWidth);
                        }
                        
                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5L-" + i.ToString());
#if (Debug_2_12 || Release_2_12)
                        m_arrMatcher[i].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_arrMatcher[i].LearnPattern(objPatternROI.ref_ROI);

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5M-" + i.ToString());

                        m_arrMatcher[i].FinalReduction = 2;
                        objPatternROI.Dispose();
                        objSourceROI.Dispose();

                        STTrackLog.WriteLine("Save=" + "*******************_i1o-5n-" + i.ToString());
                    }

                }

                STTrackLog.WriteLine("Save=" + "*******************_i1o-6");

                objImage.Dispose();
                STTrackLog.WriteLine("Save=" + "*******************_i1o-6a");
                objImage = null;
                STTrackLog.WriteLine("Save=" + "*******************_i1o-6b");
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Orient Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Orient Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LearnUnitPRPattern(ROI objROI)
        {
            try
            {
                if (m_objUnitPRMatcher == null)
                    m_objUnitPRMatcher = new EMatcher();
#if (Debug_2_12 || Release_2_12)
                m_objUnitPRMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objUnitPRMatcher.LearnPattern(objROI.ref_ROI);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Orient Learn Unit PR Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Orient Learn Unit PR Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public void SavePattern4Direction(string strFolderName, string strFileName)
        {
            try
            {
                for (int i = 0; i < m_arrMatcher.Count; i++)
                {
                    m_arrMatcher[i].Save(strFolderName + strFileName + "_" + (90 * i).ToString() + "Deg.mch");
                }
            }
            catch (Exception ex)
            {
                m_strErrorMessage = "SavePattern4Direction() - Pattern file is not found. Exception:" + ex.ToString();
                m_objTrackLog.WriteLine(m_strErrorMessage);
            }
        }

        public bool LearnSubPattern(ROI objROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objSubMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_objSubMatcher.LearnPattern(objROI.ref_ROI);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Sub Orient Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Sub Orient Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool IsPocketEmpty(ROI objROI)
        {
            // Get pixel count for high and low threshold pixel count
            int intLowThresholdPixelCount = 0, intBtwThresholdPixelCount = 0, intHighThresholdPixelCount = 0;
            EBW8 bwEmptyThreshold = new EBW8((byte)m_intEmptyThreshold);
            EasyImage.PixelCount(objROI.ref_ROI, bwEmptyThreshold, bwEmptyThreshold,
                                 out intLowThresholdPixelCount, out intBtwThresholdPixelCount, out intHighThresholdPixelCount);

            // Die area is considered as empty if die inverted area lower than limit
            if (intHighThresholdPixelCount <= m_intEmptyMaxArea)
            {
                return true;
            }

            return false;
        }

        public float GetMinScore()
        {
            return m_fHighestScore;
        }

        public int DoOrientationInspection(ROI objSearchROI, int intFinalReduction)
        {
            return DoOrientationInspection(objSearchROI, intFinalReduction, false);
        }


        public int DoOrientationInspection(ROI objSearchROI, int intFinalReduction, bool blnWantDegAngleResult)
        {
            /*
             * When blnWantDegAngleResult is true, mean no orient guage to measure unit angle and image is not rotated to 0 deg before Orient Inspection test.
             */

            // Reset inspection result
            m_intAngleResult = 4;
            m_fHighestScore = -1.0f;
            int intHighestScoreAngle = -1;
            m_strErrorMessage = "";

            // Check pattern file exist
            if (m_arrMatcher.Count == 0)
                return m_intAngleResult;

            // Check are all patterns learnt
            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                if (!m_arrMatcher[i].PatternLearnt)
                    return m_intAngleResult;
            }

            // Init data
            int intMatcherIndex = 0;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 707");
                    break;
                }
                // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#endif

                // Start pattern match
                m_arrMatcher[intMatcherIndex].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex].GetPosition(0).Score);

                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                        intHighestScoreAngle = intMatcherIndex;

                        // 2019 08 30 - CCENG: collect highest score matcher center point although highest score lower than m_fOrientMinScore. This center point need to be used for Mark ROI placement.
                        if (intMatcherIndex == 0)
                        {
                            m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                        }
                        m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                        m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;


                        if (fScore >= m_fOrientMinScore)
                        {
                            //if (intMatcherIndex == 0)
                            //{
                            //    m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            //    m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            //}
                            //m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            //m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;

                            m_intAngleResult = intMatcherIndex;
                        }
                    }
                }

                if (m_intDirections == 4)
                    intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                else
                    intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg

            } while (intMatcherIndex < 4);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Get object deg angle result
            if (blnWantDegAngleResult && m_intAngleResult < 4)
            {
                if (m_arrMatcher[m_intAngleResult].FinalReduction != 0)
                    m_arrMatcher[m_intAngleResult].FinalReduction = 0;
                // Make sure basic setting for matcher file
                if (m_arrMatcher[m_intAngleResult].MinAngle != -m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MinAngle = -m_intCheckMarkAngleMinMaxTolerance;
                if (m_arrMatcher[m_intAngleResult].MaxAngle != m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MaxAngle = m_intCheckMarkAngleMinMaxTolerance;

                // Start pattern match
                m_arrMatcher[m_intAngleResult].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[m_intAngleResult].NumPositions > 0)
                {
                    m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX;
                    m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY;
                    m_fDegAngleResult = m_arrMatcher[m_intAngleResult].GetPosition(0).Angle;

                    float fScore = Math.Max(0, m_arrMatcher[intHighestScoreAngle].GetPosition(0).Score);

                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                    }
                }
                else
                    m_fDegAngleResult = 0;
            }
            /* 2019 08 31 - CCENG: 
             * When no gauge, unit is not rotated to 0 deg before Orient maching test. 
             * Due to this reason, the 4 direction orient matching score may be lower than orient min setting because the MinAngle and MaxAngle is set to 0 for faster speed inspection.
             * This is case, we can double check again using MinAngle and MaxAngle 10 to make sure we get better score.
             */
            else if (blnWantDegAngleResult && m_intAngleResult == 4)
            {
                if (intHighestScoreAngle < 0)
                    goto FailALL;

                if (m_arrMatcher[intHighestScoreAngle].FinalReduction != 0)
                    m_arrMatcher[intHighestScoreAngle].FinalReduction = 0;
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intHighestScoreAngle].MinAngle != -10)
                    m_arrMatcher[intHighestScoreAngle].MinAngle = -10;
                if (m_arrMatcher[intHighestScoreAngle].MaxAngle != 10)
                    m_arrMatcher[intHighestScoreAngle].MaxAngle = 10;
                // Start pattern match
                m_arrMatcher[intHighestScoreAngle].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[intHighestScoreAngle].NumPositions > 0)
                {
                    m_fObjectCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX;
                    m_fObjectCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY;
                    m_fDegAngleResult = m_arrMatcher[intHighestScoreAngle].GetPosition(0).Angle;

                    float fScore = Math.Max(0, m_arrMatcher[intHighestScoreAngle].GetPosition(0).Score);

                    m_fHighestScore = fScore;

                    if (intMatcherIndex == 0)
                    {
                        m_fObjectOriCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX;
                        m_fObjectOriCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY;
                    }
                    m_fObjectCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX;
                    m_fObjectCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY;

                    if (fScore >= m_fOrientMinScore)
                    {
                        m_intAngleResult = intHighestScoreAngle;
                    }
                }
                else
                    m_fDegAngleResult = 0;
            }
            else
            {
                if (m_fDegAngleResult != 0)
                    m_fDegAngleResult = 0;
            }

          FailALL:  if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            return m_intAngleResult;
        }

        public int DoOrientationInspection(ROI objSearchROI, int intFinalReduction, bool blnWantDegAngleResult, float fMarkAngleTolerance, ref bool blnMarkAngleFail)
        {
            return DoOrientationInspection(objSearchROI, intFinalReduction, 0, false, blnWantDegAngleResult, fMarkAngleTolerance, ref blnMarkAngleFail, false, false);
        }

        public int DoOrientationInspection(ROI objSearchROI, int intFinalReduction, int intFinalReduction_Precise, bool blnWantOrientationResult4, bool blnWantDegAngleResult, float fMarkAngleTolerance, ref bool blnMarkAngleFail, bool blnForLead, bool blnWhiteUnit)
        {
            /*
             * When blnWantDegAngleResult is true, mean no orient guage to measure unit angle and image is not rotated to 0 deg before Orient Inspection test.
             */

            // Reset inspection result
            m_intAngleResult = 4;
            m_fHighestScore = -1.0f;
            int intHighestScoreAngle = -1;
            m_strErrorMessage = "";
            blnMarkAngleFail = true;
            m_arrMatchScoreList.Clear();
            m_arrMatchPositionXList.Clear();
            m_arrMatchPositionYList.Clear();

            // Check pattern file exist
            if (m_arrMatcher.Count == 0)
                return m_intAngleResult;

            // Check are all patterns learnt
            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                if (!m_arrMatcher[i].PatternLearnt)
                    return m_intAngleResult;
            }

            // Init data
            int intMatcherIndex = 0;
            bool blnWantDebug = false;
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 706");
                    break;
                }
                // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#endif

                if (m_arrMatcher[intMatcherIndex].MaxPositions != 1)
                    m_arrMatcher[intMatcherIndex].MaxPositions = 1;

                // Start pattern match

                if (blnWantDebug)
                {
                    m_arrMatcher[intMatcherIndex].Save("D:\\TS\\Matcher" + intMatcherIndex.ToString() + ".mch");
                    objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI" + intMatcherIndex.ToString() + ".bmp");
                }

                m_arrMatcher[intMatcherIndex].Match(objSearchROI.ref_ROI);

                m_arrMatchScoreList.Add(0);
                m_arrMatchPositionXList.Add(0);
                m_arrMatchPositionYList.Add(0);

                if (m_arrMatcher[intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex].GetPosition(0).Score);

                    m_arrMatchScoreList[intMatcherIndex] = fScore;
                    m_arrMatchPositionXList[intMatcherIndex] = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                    m_arrMatchPositionYList[intMatcherIndex] = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;

                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                        intHighestScoreAngle = intMatcherIndex;

                        // 2019 08 30 - CCENG: collect highest score matcher center point although highest score lower than m_fOrientMinScore. This center point need to be used for Mark ROI placement.
                        if (intMatcherIndex == 0)
                        {
                            m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                        }
                        m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                        m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;


                        // 2020 11 20 - if Mark Orient, in order to display fail orientation, need to return 4 if lower than orient min score 
                        //            - if Mark only, then can return 1 to 4 even though lower than orient min score.
                        if (!blnWantOrientationResult4 || (fScore >= m_fOrientMinScore))
                        {
                            //if (intMatcherIndex == 0)
                            //{
                            //    m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            //    m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            //}
                            //m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            //m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;

                            m_intAngleResult = intMatcherIndex;
                        }
                    }
                }

                if (!blnWantOrientationResult4)
                {
                    intMatcherIndex += 4;           // For no orientation unit like InPocketMark and MarkPackage
                    m_arrMatchScoreList.Add(0);
                    m_arrMatchPositionXList.Add(0);
                    m_arrMatchPositionYList.Add(0);
                    m_arrMatchScoreList.Add(0);
                    m_arrMatchPositionXList.Add(0);
                    m_arrMatchPositionYList.Add(0);
                    m_arrMatchScoreList.Add(0);
                    m_arrMatchPositionXList.Add(0);
                    m_arrMatchPositionYList.Add(0);
                }
                else if (m_intDirections == 4)
                {
                    intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                }
                else
                {
                    intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg
                    m_arrMatchScoreList.Add(0);
                    m_arrMatchPositionXList.Add(0);
                    m_arrMatchPositionYList.Add(0);
                }


            } while (intMatcherIndex < 4);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            Point pROIStart = new Point(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY);
            Point pROIEnd = new Point(objSearchROI.ref_ROIPositionX + objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight);
            Size pROISize = new Size(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            bool blnLoadBackROI = false;
            bool blnUseNarrorROI = false;
            if (blnForLead && m_fHighestScore < 0.7)//2021-07-30 ZJYEOH : Check again with smaller search ROI if score lower than 50
            {
                int intMatcherIndex2 = 0;
                blnLoadBackROI = true;

                int intStartX = Math.Min(pROIStart.X, (int)(pROIStart.X + m_fObjectCenterX) - (m_arrMatcher[0].PatternWidth));
                int intStartY = Math.Min(pROIStart.Y, (int)(pROIStart.Y + m_fObjectCenterY) - (m_arrMatcher[0].PatternHeight));
                int intWidth = (m_arrMatcher[0].PatternWidth * 2);
                int intHeight = (m_arrMatcher[0].PatternHeight * 2);
                if ((intStartX + intWidth) > pROIEnd.X)
                {
                    intWidth -= (intStartX + intWidth) - pROIEnd.X;
                }
                if ((intStartY + intHeight) > pROIEnd.Y)
                {
                    intHeight -= (intStartY + intHeight) - pROIEnd.Y;
                }
                objSearchROI.LoadROISetting(intStartX, intStartY,
                                            intWidth, m_arrMatcher[0].PatternHeight * 2);
                timeout.Start();

                do
                {
                    // ------------------- checking loop timeout ---------------------------------------------------
                    if (timeout.Timing > 10000)
                    {
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 706");
                        break;
                    }
                    // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                    // Make sure basic setting for matcher file
                    if (m_arrMatcher[intMatcherIndex2].FinalReduction != intFinalReduction)
                        m_arrMatcher[intMatcherIndex2].FinalReduction = (uint)intFinalReduction;
                    if (m_arrMatcher[intMatcherIndex2].MinAngle != 0)
                        m_arrMatcher[intMatcherIndex2].MinAngle = 0;
                    if (m_arrMatcher[intMatcherIndex2].MaxAngle != 0)
                        m_arrMatcher[intMatcherIndex2].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex2].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex2].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex2].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex2].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex2].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex2].MaxAngle = 0;

#endif

                    // Start pattern match

                    if (blnWantDebug)
                    {
                        m_arrMatcher[intMatcherIndex2].Save("D:\\TS\\Matcher" + intMatcherIndex2.ToString() + ".mch");
                        objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI" + intMatcherIndex2.ToString() + ".bmp");
                    }

                    m_arrMatcher[intMatcherIndex2].Match(objSearchROI.ref_ROI);

                    if (m_arrMatcher[intMatcherIndex2].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                    {
                        float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex2].GetPosition(0).Score);

                        if (fScore > m_fHighestScore)
                        {
                            blnUseNarrorROI = true;
                            m_arrMatchScoreList[intMatcherIndex2] = fScore;
                            m_arrMatchPositionXList[intMatcherIndex2] = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                            m_arrMatchPositionYList[intMatcherIndex2] = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;

                            m_fHighestScore = fScore;
                            intHighestScoreAngle = intMatcherIndex2;

                            // 2019 08 30 - CCENG: collect highest score matcher center point although highest score lower than m_fOrientMinScore. This center point need to be used for Mark ROI placement.
                            if (intMatcherIndex2 == 0)
                            {
                                m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                                m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;
                            }
                            m_fObjectCenterX = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                            m_fObjectCenterY = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;


                            // 2020 11 20 - if Mark Orient, in order to display fail orientation, need to return 4 if lower than orient min score 
                            //            - if Mark only, then can return 1 to 4 even though lower than orient min score.
                            if (!blnWantOrientationResult4 || (fScore >= m_fOrientMinScore))
                            {
                                //if (intMatcherIndex == 0)
                                //{
                                //    m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                //    m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                //}
                                //m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                //m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;

                                m_intAngleResult = intMatcherIndex2;
                            }
                        }
                    }

                    if (!blnWantOrientationResult4)
                    {
                        intMatcherIndex2 += 4;           // For no orientation unit like InPocketMark and MarkPackage
                    }
                    else if (m_intDirections == 4)
                    {
                        intMatcherIndex2++;     // For square unit 0, 90, 180, -90 deg
                    }
                    else
                    {
                        intMatcherIndex2 += 2; //For rectangle unit 0nly 0 deg and 180 deg
                    }


                } while (intMatcherIndex2 < 4);
                timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
            }
            else if (blnWhiteUnit && m_fHighestScore < 0.7)//2021-07-30 ZJYEOH : Check again with smaller search ROI if score lower than 50
            {
                int intMatcherIndex2 = 0;
                timeout.Start();

                do
                {
                    // ------------------- checking loop timeout ---------------------------------------------------
                    if (timeout.Timing > 10000)
                    {
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 706");
                        break;
                    }
                    // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                    // Make sure basic setting for matcher file
                    if (m_arrMatcher[intMatcherIndex2].FinalReduction != intFinalReduction)
                        m_arrMatcher[intMatcherIndex2].FinalReduction = (uint)intFinalReduction;
                    if (m_arrMatcher[intMatcherIndex2].MinAngle != 0)
                        m_arrMatcher[intMatcherIndex2].MinAngle = 0;
                    if (m_arrMatcher[intMatcherIndex2].MaxAngle != 0)
                        m_arrMatcher[intMatcherIndex2].MaxAngle = 0;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex2].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex2].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex2].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex2].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex2].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex2].MaxAngle = 0;

#endif
                    if (m_arrMatcher[intMatcherIndex2].MaxPositions != 3)
                        m_arrMatcher[intMatcherIndex2].MaxPositions = 3;

                    // Start pattern match

                    if (blnWantDebug)
                    {
                        m_arrMatcher[intMatcherIndex2].Save("D:\\TS\\Matcher" + intMatcherIndex2.ToString() + ".mch");
                        objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI" + intMatcherIndex2.ToString() + ".bmp");
                    }

                    m_arrMatcher[intMatcherIndex2].Match(objSearchROI.ref_ROI);

                    if (m_arrMatcher[intMatcherIndex2].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                    {
                        float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex2].GetPosition(0).Score);

                        if (fScore > m_fHighestScore)
                        {
                            m_arrMatchScoreList[intMatcherIndex2] = fScore;
                            m_arrMatchPositionXList[intMatcherIndex2] = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                            m_arrMatchPositionYList[intMatcherIndex2] = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;

                            m_fHighestScore = fScore;
                            intHighestScoreAngle = intMatcherIndex2;

                            // 2019 08 30 - CCENG: collect highest score matcher center point although highest score lower than m_fOrientMinScore. This center point need to be used for Mark ROI placement.
                            if (intMatcherIndex2 == 0)
                            {
                                m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                                m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;
                            }
                            m_fObjectCenterX = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                            m_fObjectCenterY = m_arrMatcher[intMatcherIndex2].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;


                            // 2020 11 20 - if Mark Orient, in order to display fail orientation, need to return 4 if lower than orient min score 
                            //            - if Mark only, then can return 1 to 4 even though lower than orient min score.
                            if (!blnWantOrientationResult4 || (fScore >= m_fOrientMinScore))
                            {
                                //if (intMatcherIndex == 0)
                                //{
                                //    m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                //    m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                //}
                                //m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                //m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;

                                m_intAngleResult = intMatcherIndex2;
                            }
                        }
                    }

                    if (!blnWantOrientationResult4)
                    {
                        intMatcherIndex2 += 4;           // For no orientation unit like InPocketMark and MarkPackage
                    }
                    else if (m_intDirections == 4)
                    {
                        intMatcherIndex2++;     // For square unit 0, 90, 180, -90 deg
                    }
                    else
                    {
                        intMatcherIndex2 += 2; //For rectangle unit 0nly 0 deg and 180 deg
                    }


                } while (intMatcherIndex2 < 4);
                timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
            }

            // 2021 08 20 - objSearchROI will be set to narrow size to big correct inspection, but sometime narror size may be get better result size. 
            //              So objSearchROI need to reset to original size first before go to precise pattern matching step.
            if (blnLoadBackROI && !blnUseNarrorROI)
            {
                objSearchROI.LoadROISetting(pROIStart.X, pROIStart.Y, pROISize.Width, pROISize.Height);
                blnLoadBackROI = false;
            }

            //m_intAngleResult = 0;
            //intHighestScoreAngle = 0;
            // Get object deg angle result
            if (blnWantDegAngleResult && m_intAngleResult < 4)
            {
#if (Debug_2_12 || Release_2_12)
                if (m_arrMatcher[m_intAngleResult].FinalReduction != intFinalReduction_Precise)
                    m_arrMatcher[m_intAngleResult].FinalReduction = (uint)intFinalReduction_Precise;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_arrMatcher[m_intAngleResult].FinalReduction != intFinalReduction_Precise)
                    m_arrMatcher[m_intAngleResult].FinalReduction = intFinalReduction_Precise;
#endif
                // Make sure basic setting for matcher file
                if (m_arrMatcher[m_intAngleResult].MinAngle != -m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MinAngle = -m_intCheckMarkAngleMinMaxTolerance;
                if (m_arrMatcher[m_intAngleResult].MaxAngle != m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MaxAngle = m_intCheckMarkAngleMinMaxTolerance;

                if (blnWantDebug)
                {
                    m_arrMatcher[m_intAngleResult].Save("D:\\TS\\MatcherF" + intMatcherIndex.ToString() + ".mch");
                    objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROIF" + intMatcherIndex.ToString() + ".bmp");
                }

                // Start pattern match
                m_arrMatcher[m_intAngleResult].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[m_intAngleResult].NumPositions > 0)
                {
                    //m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX;
                    //m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY;
                    //m_fDegAngleResult = m_arrMatcher[m_intAngleResult].GetPosition(0).Angle;

                    float fScore = Math.Max(0, m_arrMatcher[intHighestScoreAngle].GetPosition(0).Score);

                    if (fScore > m_fHighestScore)
                    {
                        // 2021 11 28 - CCENG: Add "objSearchROI.ref_ROIPositionX - pROIStart.X;" to compensate the change of SearchROI due to blnUseNarrorROI feature.
                        //                      If SearchROI no change, then objSearchROI.ref_ROIPositionX same value with pROIStart.X, so no affect orignal formula.
                        //m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX;
                        //m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY;
                        m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                        m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;
                        m_fDegAngleResult = m_arrMatcher[m_intAngleResult].GetPosition(0).Angle;

                        m_fHighestScore = fScore;
                    }
                    else
                    {
                        m_fDegAngleResult = 0;
                    }

                    if (Math.Abs(m_fDegAngleResult) > fMarkAngleTolerance)
                    {
                        blnMarkAngleFail = false;
                    }
                }
                else
                    m_fDegAngleResult = 0;
            }
            /* 2019 08 31 - CCENG: 
             * When no gauge, unit is not rotated to 0 deg before Orient maching test. 
             * Due to this reason, the 4 direction orient matching score may be lower than orient min setting because the MinAngle and MaxAngle is set to 0 for faster speed inspection.
             * This is case, we can double check again using MinAngle and MaxAngle 10 to make sure we get better score.
             */
            else if (blnWantDegAngleResult && m_intAngleResult == 4)
            {
                if (intHighestScoreAngle < 0)
                    goto FailALL;

#if (Debug_2_12 || Release_2_12)
                if (m_arrMatcher[intHighestScoreAngle].FinalReduction != intFinalReduction_Precise)
                    m_arrMatcher[intHighestScoreAngle].FinalReduction = (uint)intFinalReduction_Precise;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_arrMatcher[intHighestScoreAngle].FinalReduction != intFinalReduction_Precise)
                    m_arrMatcher[intHighestScoreAngle].FinalReduction = intFinalReduction_Precise;
#endif
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intHighestScoreAngle].MinAngle != -10)
                    m_arrMatcher[intHighestScoreAngle].MinAngle = -10;
                if (m_arrMatcher[intHighestScoreAngle].MaxAngle != 10)
                    m_arrMatcher[intHighestScoreAngle].MaxAngle = 10;
                // Start pattern match
                m_arrMatcher[intHighestScoreAngle].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[intHighestScoreAngle].NumPositions > 0)
                {
                    // 2021 11 28 - CCENG: Add "objSearchROI.ref_ROIPositionX - pROIStart.X;" to compensate the change of SearchROI due to blnUseNarrorROI feature.
                    //                      If SearchROI no change, then objSearchROI.ref_ROIPositionX same value with pROIStart.X, so no affect orignal formula.
                    //m_fObjectCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX;
                    //m_fObjectCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY;
                    m_fObjectCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                    m_fObjectCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;
                    m_fDegAngleResult = m_arrMatcher[intHighestScoreAngle].GetPosition(0).Angle;

                    float fScore = Math.Max(0, m_arrMatcher[intHighestScoreAngle].GetPosition(0).Score);

                    m_fHighestScore = fScore;

                    if (intMatcherIndex == 0)
                    {
                        // 2021 11 28 - CCENG:  Add "objSearchROI.ref_ROIPositionX - pROIStart.X;" to compensate the change of SearchROI due to blnUseNarrorROI feature.
                        //                      If SearchROI no change, then objSearchROI.ref_ROIPositionX same value with pROIStart.X, so no affect orignal formula.
                        //m_fObjectOriCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX;
                        //m_fObjectOriCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY;
                        m_fObjectOriCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                        m_fObjectOriCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;
                    }

                    // 2021 11 28 - CCENG: Add "objSearchROI.ref_ROIPositionX - pROIStart.X;" to compensate the change of SearchROI due to blnUseNarrorROI feature.
                    //                      If SearchROI no change, then objSearchROI.ref_ROIPositionX same value with pROIStart.X, so no affect orignal formula.
                    //m_fObjectCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX;
                    //m_fObjectCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY;
                    m_fObjectCenterX = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterX + objSearchROI.ref_ROIPositionX - pROIStart.X;
                    m_fObjectCenterY = m_arrMatcher[intHighestScoreAngle].GetPosition(0).CenterY + objSearchROI.ref_ROIPositionY - pROIStart.Y;

                    if (fScore >= m_fOrientMinScore)
                    {
                        m_intAngleResult = intHighestScoreAngle;
                    }
                }
                else
                    m_fDegAngleResult = 0;
            }
            else
            {
                if (m_fDegAngleResult != 0)
                    m_fDegAngleResult = 0;
            }

            FailALL: if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            if (blnLoadBackROI)
            {
                objSearchROI.LoadROISetting(pROIStart.X, pROIStart.Y, pROISize.Width, pROISize.Height);
            }

            return m_intAngleResult;
        }
        
        /// <summary>
        /// Match the template with grabbed image
        /// </summary>
        /// <param name="searchROI">Image area that is used to compared among template and grabbed image</param>
        /// <param name="objGrabbedImage">whole original camera image</param>
        /// <param name="blnRotate">If fail for first time, whether allow to rotate image to compare again
        /// <returns>return angle; 0 = 0 degree, 1 = 270 degree, 2 = 180 degree, 3 = 90 degree, 4 = invalid angle</returns>
        /// 
        public int MatchWithTemplate(ROI objSearchROI, ImageDrawing objGrabbedImage, bool blnRotate)
        {
            return MatchOrient(objSearchROI.ref_ROI, objGrabbedImage.ref_objMainImage, blnRotate);
        }

        /// <summary>
        /// Match the template with grabbed image
        /// </summary>
        /// <param name="searchROI">Image area that is used to compared among template and grabbed image</param>
        /// <param name="objGrabbedImage">whole original camera image</param>
        /// <param name="blnRotate">If fail for first time, whether allow to rotate image to compare again
        /// <returns>return angle; 0 = 0 degree, 1 = 270 degree, 2 = 180 degree, 3 = 90 degree, 4 = invalid angle</returns>
        /// 
        public int MatchWithTemplate(ROI objSearchROI, ImageDrawing objGrabbedImage, bool blnRotate, float fHighestScore, bool blnPreciseAngleCheck, bool blnSubMatcherTest)
        {
            return MatchOrient(objSearchROI.ref_ROI, objGrabbedImage.ref_objMainImage, blnRotate, fHighestScore, blnPreciseAngleCheck, blnSubMatcherTest);
        }

        public float MatchFirstObject(ROI objROI)
        {
            m_objMatcher.Match(objROI.ref_ROI);
            int intNumPosition = (int)m_objMatcher.NumPositions;
            if (intNumPosition > 0)
            {
                m_fObjectCenterX = m_objMatcher.GetPosition(0).CenterX;
                m_fObjectCenterY = m_objMatcher.GetPosition(0).CenterY;
                m_fHighestScore = m_objMatcher.GetPosition(0).Score;
                m_fDegAngleResult = m_objMatcher.GetPosition(0).Angle;
                return m_fDegAngleResult;
            }
            else
            {
                m_fDegAngleResult = 999;
                return 999;
            }
        }

        public void DrawOrientObject(Graphics g)
        {
            if (m_objMatcher.NumPositions > 0)
                m_objMatcher.DrawPositions(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B));
        }

        public float GetAngle(ROI objROI)
        {
            m_objMatcher.Match(objROI.ref_ROI);
            int intNumPosition = (int)m_objMatcher.NumPositions;
            if (intNumPosition > 0)
            {
                return m_objMatcher.GetPosition(0).Angle;
            }
            else
                return 999;
        }

        public void GetCornerAndMidEdges(int intIndex, int intStartSearchROIX, int intStartSearchROIY, ref PointF pxy,
         ref PointF pXy, ref PointF pxY, ref PointF pXY)
        {
#if (Debug_2_12 || Release_2_12)
            double fRadianAngle = (double)m_objMatcher.GetPosition((uint)intIndex).Angle * Math.PI / 180; // Change angle from degree to radius unit

            PointF px = new PointF();
            PointF py = new PointF();
            PointF pX = new PointF();
            PointF pY = new PointF();
            float dSin = (float)Math.Sin(fRadianAngle);
            float dCos = (float)Math.Cos(fRadianAngle);
            float fPatternWidth = m_objMatcher.PatternWidth / 2f;
            float fPatternHeight = m_objMatcher.PatternHeight / 2f;
            float fCenterX = intStartSearchROIX + m_objMatcher.GetPosition((uint)intIndex).CenterX;
            float fCenterY = intStartSearchROIY + m_objMatcher.GetPosition((uint)intIndex).CenterY;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            double fRadianAngle = (double)m_objMatcher.GetPosition(intIndex).Angle * Math.PI / 180; // Change angle from degree to radius unit

            PointF px = new PointF();
            PointF py = new PointF();
            PointF pX = new PointF();
            PointF pY = new PointF();
            float dSin = (float)Math.Sin(fRadianAngle);
            float dCos = (float)Math.Cos(fRadianAngle);
            float fPatternWidth = m_objMatcher.PatternWidth / 2f;
            float fPatternHeight = m_objMatcher.PatternHeight / 2f;
            float fCenterX = intStartSearchROIX + m_objMatcher.GetPosition(intIndex).CenterX;
            float fCenterY = intStartSearchROIY + m_objMatcher.GetPosition(intIndex).CenterY;

#endif

            px.X = fCenterX - (fPatternWidth * dCos);        // Mid edge X at Left Side
            px.Y = fCenterY - (fPatternWidth * dSin);         // Mid edge Y at Left Side

            pX.X = fCenterX + (fPatternWidth * dCos);       // Mid edge X at Right Side
            pX.Y = fCenterY + (fPatternWidth * dSin);        // Mid edge Y at Right Side

            pY.X = fCenterX - (fPatternHeight * dSin);      // Mid edge X at Top Side
            pY.Y = fCenterY + (fPatternHeight * dCos);      // Mid edge Y at Top Side

            py.X = fCenterX + (fPatternHeight * dSin);       // Mid edge X at Bottom Side
            py.Y = fCenterY - (fPatternHeight * dCos);      // Mid edge Y at Bottom Side

            pxy.X = px.X + (fPatternHeight * dSin);           // Corner  at Left Top 
            pxy.Y = px.Y - (fPatternHeight * dCos);

            pXy.X = py.X + (fPatternWidth * dCos);          // Corner at Right Top
            pXy.Y = py.Y + (fPatternWidth * dSin);

            pxY.X = pY.X - (fPatternWidth * dCos);           // Corner at Left Bottom
            pxY.Y = pY.Y - (fPatternWidth * dSin);

            pXY.X = pX.X - (fPatternHeight * dSin);          // Corner at Right Bottom
            pXY.Y = pX.Y + (fPatternHeight * dCos);
        }

        /// <summary>
        /// Load Orient Pattern from selected path
        /// </summary>
        public void LoadPattern(string strFilePath)
        {
            try
            {
                if (File.Exists(strFilePath))
                    m_objMatcher.Load(strFilePath);
            }
            catch
            {
                m_strErrorMessage = "Orient - Pattern file is not found";
                m_objTrackLog.WriteLine(m_strErrorMessage);
            }
        }

        public void LoadUnitPRPattern(string strFilePath)
        {
            try
            {
                if (m_objUnitPRMatcher == null)
                    m_objUnitPRMatcher = new EMatcher();

                if (File.Exists(strFilePath))
                    m_objUnitPRMatcher.Load(strFilePath);
            }
            catch
            {
                m_strErrorMessage = "Orient - Unit PR Pattern file is not found";
                m_objTrackLog.WriteLine(m_strErrorMessage);
            }
        }

        public void LoadPattern4Direction(string strFolderName, string strFileName)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    string strFilePath = strFolderName + strFileName + "_" + (90 * i).ToString() + "Deg.mch";
                    if (File.Exists(strFilePath))
                    {
                        if (i >= m_arrMatcher.Count) //2020-08-17 ZJYEOH : Changed <= to >=
                            m_arrMatcher.Add(new EMatcher());

                        m_arrMatcher[i].Load(strFilePath);
                    }

                }
            }
            catch
            {
                m_strErrorMessage = "Orient - Pattern file is not found";
                m_objTrackLog.WriteLine(m_strErrorMessage);
            }

        }

        public void LoadSubPattern(string strFilePath)
        {
            try
            {
                if (File.Exists(strFilePath))
                    m_objSubMatcher.Load(strFilePath);
            }
            catch
            {
                m_strErrorMessage = "Orient - Pattern file is not found";
                m_objTrackLog.WriteLine(m_strErrorMessage);
            }
        }


        /// <summary>
        /// Reset previous inspection result
        /// </summary>
        public void ResetInspectionData()
        {
            m_fHighestScore = -1;
            m_fObjectCenterX = -1;
            m_fObjectCenterY = -1;
            m_fSubObjectCenterX = -1;
            m_fSubObjectCenterY = -1;
        }
        /// <summary>
        /// Rotate searchROI area to desired angle 
        /// </summary>
        /// <param name="objSearchROI">ROI source to rotate</param>
        /// <param name="objDestROI">Destination ROI after locate image</param>
        /// <param name="intRotateAngle">angle to be rotated</param>
        public void RotateROI(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, ref ImageDrawing objRotatedImage)
        {
            EROIBW8 searchROI = objSearchROI.ref_ROI;
            EROIBW8 destinationROI = new EROIBW8();
            objRotatedImage = new ImageDrawing();
            objSourceImage.CopyTo(ref objRotatedImage);

            if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            else
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            destinationROI.Detach();
            destinationROI.Attach(objRotatedImage.ref_objMainImage);
            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f, searchROI.Height / 2f, destinationROI.Width / 2f, destinationROI.Height / 2f, 1, 1, fRotateAngle, destinationROI, 0);
        }
        /// <summary>
        /// Save all Orient parameter into mch file
        /// </summary>
        /// <param name="strFilePath"></param>
        public void SavePattern(string strFilePath)
        {
            m_objMatcher.Save(strFilePath);
        }

        public void SaveUnitPRPattern(string strFilePath)
        {
            string strDirectory = Path.GetDirectoryName(strFilePath);
            if (!Directory.Exists(strDirectory))
                Directory.CreateDirectory(strDirectory);
            m_objUnitPRMatcher.Save(strFilePath);
        }

        public void SaveSubPattern(string strFilePath)
        {
            m_objSubMatcher.Save(strFilePath);
        }

        public void SetAngleSetting(int intMinAngle, int intMaxAngle)
        {
            m_objMatcher.MinAngle = intMinAngle;
            m_objMatcher.MaxAngle = intMaxAngle;
        }
        public void SetUnitPRAngleSetting(int intMinAngle, int intMaxAngle)
        {
            m_objUnitPRMatcher.MinAngle = intMinAngle;
            m_objUnitPRMatcher.MaxAngle = intMaxAngle;
        }


        public void SetDontCareThreshold(int intTheshold)
        {
#if (Debug_2_12 || Release_2_12)
            m_objMatcher.DontCareThreshold = (uint)intTheshold;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_objMatcher.DontCareThreshold = intTheshold;
#endif

        }

        /// <summary>
        /// Set final reduction to matcher object
        /// </summary>
        /// <param name="intFinalReductionValue">From 0 to 3</param>
        public void SetFinalReduction(int intFinalReductionValue)
        {
            if (intFinalReductionValue > 3)
                return;
#if (Debug_2_12 || Release_2_12)
            m_objMatcher.FinalReduction = (uint)intFinalReductionValue;
            m_objSubMatcher.FinalReduction = (uint)intFinalReductionValue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_objMatcher.FinalReduction = intFinalReductionValue;
            m_objSubMatcher.FinalReduction = intFinalReductionValue;
#endif

        }

        public void SetUnitPRFinalReduction(int intFinalReductionValue)
        {
            if (intFinalReductionValue > 3)
                return;
#if (Debug_2_12 || Release_2_12)
            m_objUnitPRMatcher.FinalReduction = (uint)intFinalReductionValue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_objUnitPRMatcher.FinalReduction = intFinalReductionValue;
#endif

        }

        public void SetMinScore(float fMinScore)
        {
            m_objMatcher.MinScore = fMinScore;
        }

        public void SetScaleXSetting(int intMinScaleX, int intMaxScaleX)
        {
            m_objMatcher.MinScaleX = 1;
            m_objMatcher.MaxScaleX = 1;
        }

        public void SetScaleYSetting(int intMinScaleY, int intMaxScaleY)
        {
            m_objMatcher.MinScaleY = 1;
            m_objMatcher.MaxScaleY = 1;
        }

        /// <summary>
        /// way to compare the pattern and the image
        /// Tells what normalization rule is used to correlate the pattern to the image
        /// </summary>
        /// <param name="intValue">0 = standard, 1 = OffsetNormalized, 2 = GainNormalized, 3 = Normalized</param>
        public void SetCorrelationMode(int intValue)
        {
            switch (intValue)
            {
                case 0:
                    m_objMatcher.CorrelationMode = ECorrelationMode.Standard;
                    break;
                case 1:
                    m_objMatcher.CorrelationMode = ECorrelationMode.OffsetNormalized;
                    break;
                case 2:
                    m_objMatcher.CorrelationMode = ECorrelationMode.GainNormalized;
                    break;
                case 3:
                    m_objMatcher.CorrelationMode = ECorrelationMode.Normalized;
                    break;
            }
        }
        /// <summary>
        /// way to deal with contrast inversions
        /// </summary>
        /// <param name="intValue"> 0 = Normal, 1 = Inverse, 2 = Any</param>
        public void SetContrast(int intValue)
        {
            switch (intValue)
            {
                case 0:
                    m_objMatcher.ContrastMode = EMatchContrastMode.Normal;
                    break;
                case 1:
                    m_objMatcher.ContrastMode = EMatchContrastMode.Inverse;
                    break;
                case 2:
                    m_objMatcher.ContrastMode = EMatchContrastMode.Any;
                    break;
            }
        }
        /// <summary>
        /// select the preprocessing type applied to the image before the decimation: averaging or low-pass filtering. 
        /// LOWPASS mode is indicated if the image presents sharp gray-level transitions.
        /// used at learning stage
        /// </summary>
        /// <param name="intValue"></param>
        public void SetFilteringMode(int intValue)
        {
            switch (intValue)
            {
                case 0:
                    m_objMatcher.FilteringMode = EFilteringMode.Uniform;
                    break;
                case 1:
                    m_objMatcher.FilteringMode = EFilteringMode.LowPass;
                    break;
            }
        }

        public void LoadOrient(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_intRotatedAngle = objFile.GetValueAsInt("RotatedAngle", 0);

            m_intCorrectAngleMethod = objFile.GetValueAsInt("CorrectAngleMethod", 0);
            m_intEmptyThreshold = objFile.GetValueAsInt("EmptyThreshold", 0);
            m_intEmptyMaxArea = objFile.GetValueAsInt("EmptyMaxArea", 0);
            m_intMatcherOffSetCenterX = objFile.GetValueAsInt("MatcherOffSetCenterX", 0);
            m_intMatcherOffSetCenterY = objFile.GetValueAsInt("MatcherOffSetCenterY", 0);
            m_intUnitSurfaceOffsetX = objFile.GetValueAsInt("UnitSurfaceOffsetX", 0);
            m_intUnitSurfaceOffsetY = objFile.GetValueAsInt("UnitSurfaceOffsetY", 0);

            m_fTemplateCenterX = objFile.GetValueAsFloat("TemplateCenterX", 0);
            m_fTemplateCenterY = objFile.GetValueAsFloat("TemplateCenterY", 0);

            m_blnWantCheckOrientation = objFile.GetValueAsBoolean("WantCheckOrientation", true);
            m_blnWantCheckOrientAngleTolerance = objFile.GetValueAsBoolean("WantCheckOrientAngleTolerance", false);
            m_blnWantCheckOrientXTolerance = objFile.GetValueAsBoolean("WantCheckOrientXTolerance", false);
            m_blnWantCheckOrientYTolerance = objFile.GetValueAsBoolean("WantCheckOrientYTolerance", false);

            m_blnTemplateOrientationIsLeft = objFile.GetValueAsBoolean("TemplateOrientationIsLeft", false);
            m_blnTemplateOrientationIsTop = objFile.GetValueAsBoolean("TemplateOrientationIsTop", false);
        }

        public void LoadOrientAdvanceSetting(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_intDirections = objFile.GetValueAsInt("Direction", 4);
            m_blnWantSubROI = objFile.GetValueAsBoolean("WantSubROI", false);
        }

        public void SaveOrient(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            objFile.WriteElement1Value("RotatedAngle", m_intRotatedAngle);
            // Min area
            objFile.WriteElement1Value("CorrectAngleMethod", m_intCorrectAngleMethod);
            objFile.WriteElement1Value("EmptyThreshold", m_intEmptyThreshold);
            objFile.WriteElement1Value("EmptyMaxArea", m_intEmptyMaxArea);
            objFile.WriteElement1Value("MatcherOffSetCenterX", m_intMatcherOffSetCenterX);
            objFile.WriteElement1Value("MatcherOffSetCenterY", m_intMatcherOffSetCenterY);
            objFile.WriteElement1Value("UnitSurfaceOffsetX", m_intUnitSurfaceOffsetX);
            objFile.WriteElement1Value("UnitSurfaceOffsetY", m_intUnitSurfaceOffsetY);

            objFile.WriteElement1Value("TemplateCenterX", m_fTemplateCenterX);
            objFile.WriteElement1Value("TemplateCenterY", m_fTemplateCenterY);

            objFile.WriteElement1Value("WantCheckOrientation", m_blnWantCheckOrientation);
            objFile.WriteElement1Value("WantCheckOrientAngleTolerance", m_blnWantCheckOrientAngleTolerance);
            objFile.WriteElement1Value("WantCheckOrientXTolerance", m_blnWantCheckOrientXTolerance);
            objFile.WriteElement1Value("WantCheckOrientYTolerance", m_blnWantCheckOrientYTolerance);

            objFile.WriteElement1Value("TemplateOrientationIsLeft", m_blnTemplateOrientationIsLeft);
            objFile.WriteElement1Value("TemplateOrientationIsTop", m_blnTemplateOrientationIsTop);

            objFile.WriteEndElement();
        }

        public void SaveOrient_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_RotatedAngle", m_intRotatedAngle);
            // Min area
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_CorrectAngleMethod", m_intCorrectAngleMethod);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_EmptyThreshold", m_intEmptyThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_EmptyMaxArea", m_intEmptyMaxArea);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MatcherOffSetCenterX", m_intMatcherOffSetCenterX);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MatcherOffSetCenterY", m_intMatcherOffSetCenterY);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_UnitSurfaceOffsetX", m_intUnitSurfaceOffsetX);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_UnitSurfaceOffsetY", m_intUnitSurfaceOffsetY);

            objFile.WriteEndElement();
        }

        public void SavOrientAdvanceSetting(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Min area
            objFile.WriteElement1Value("Direction", m_intDirections);
            objFile.WriteElement1Value("WantSubROI", m_blnWantSubROI);

            objFile.WriteEndElement();
        }

        private int MatchOrient(EROIBW8 searchROI, EImageBW8 grabbedImage, bool blnRotate)
        {
            return MatchOrient(searchROI, grabbedImage, blnRotate, -1, true, false);
        }
        public int DoOrientationInspection_WithSubMatcher(ROI objSearchROI, int intFinalReduction, bool blnWantDegAngleResult, bool blnSubMatcher)
        {
            // Reset inspection result
            m_intAngleResult = 4;
            m_fHighestScore = -1.0f;
            int intHighestScoreAngle = -1;
            m_strErrorMessage = "";

            // Check pattern file exist
            if (m_arrMatcher.Count == 0)
                return m_intAngleResult;

            // Check are all patterns learnt
            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                if (!m_arrMatcher[i].PatternLearnt)
                    return m_intAngleResult;
            }
            //EImageBW8 orientImage = new EImageBW8(grabbedImage.Width, grabbedImage.Height);
            //EROIBW8 ROI4ImageRotated = new EROIBW8();
            //EasyImage.Copy(grabbedImage, orientImage);
            //searchROI.Detach();
            //searchROI.Attach(grabbedImage);
            //ROI4ImageRotated.Detach();
            //ROI4ImageRotated.Attach(orientImage);
            //ROI4ImageRotated.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);
            // Init data
            int intMatcherIndex = 0;
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 705");
                    break;
                }
                // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#endif

                // Start pattern match
                m_arrMatcher[intMatcherIndex].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    // Matcher score must >= OrientMinScore, then only can go to submatcher.
                    // 

                    float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex].GetPosition(0).Score);
                    bool blnMatcherScoreHighestNow = false;
                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                        intHighestScoreAngle = intMatcherIndex;

                        if (fScore >= m_fOrientMinScore)
                        {
                            // Collect matcher center point and angle
                            if (intMatcherIndex == 0)
                            {
                                m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            }
                            m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            m_intAngleResult = intMatcherIndex;
                            blnMatcherScoreHighestNow = true;   // 2019 07 12 - Tell sub matcher that the current matcher is the highest score now.
                        }
                    }

                    if (fScore >= m_fOrientMinScore)
                    {
                        // Check sub matcher if matcher score >= orientMinScore
                        if (blnSubMatcher && m_objSubMatcher.PatternLearnt)
                        {
                            EROIBW8 objSubROI = new EROIBW8();
                            objSubROI.Detach();
                            objSubROI.Attach(objSearchROI.ref_ROI);
                            objSubROI.SetPlacement((int)Math.Round(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX - m_arrMatcher[intMatcherIndex].PatternWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY - m_arrMatcher[intMatcherIndex].PatternHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                     m_arrMatcher[intMatcherIndex].PatternWidth, m_arrMatcher[intMatcherIndex].PatternHeight);
                            m_objSubMatcher.FinalReduction = 0;
                            m_objSubMatcher.MaxPositions = 3;   // set 3 position in order to get the best matching result. 

                            // rotate sub matcher angle for different angle. (Matcher not need to set angle because matcher have 4 different direction matcher already but sub matcher have one only.
                            switch (intMatcherIndex)
                            {
                                case 0:
                                    m_objSubMatcher.MinAngle = 0;
                                    m_objSubMatcher.MaxAngle = 0;
                                    break;
                                case 1:
                                    m_objSubMatcher.MinAngle = 90;
                                    m_objSubMatcher.MaxAngle = 90;
                                    break;
                                case 2:
                                    m_objSubMatcher.MinAngle = 180;
                                    m_objSubMatcher.MaxAngle = 180;
                                    break;
                                case 3:
                                    m_objSubMatcher.MinAngle = 270;
                                    m_objSubMatcher.MaxAngle = 270;
                                    break;
                            }
                            m_objSubMatcher.Match(objSubROI);

                            fScore = m_objSubMatcher.GetPosition(0).Score;

                            if (fScore > m_fHighestScore)
                            {
                                m_fHighestScore = fScore;
                                intHighestScoreAngle = intMatcherIndex;

                                // Collect matcher center point, not sub matcher center point.
                                m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX;
                                m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY;
                                m_intAngleResult = intMatcherIndex;
                            }
                            else if (blnMatcherScoreHighestNow)
                            {
                                // When matcher score higher than sub matcher score, correct the submatcher object center point for sub matcher drawing.
                                m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX;
                                m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY;
                            }
                        }
                    }
                }

                if (m_intDirections == 4)
                    intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                else
                    intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg

            } while (intMatcherIndex < 4);

            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            // Get object deg angle result
            if (blnWantDegAngleResult && m_intAngleResult < 4)
            {
                if (m_arrMatcher[m_intAngleResult].FinalReduction != 0)
                    m_arrMatcher[m_intAngleResult].FinalReduction = 0;
                // Make sure basic setting for matcher file
                if (m_arrMatcher[m_intAngleResult].MinAngle != -m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MinAngle = -m_intCheckMarkAngleMinMaxTolerance;
                if (m_arrMatcher[m_intAngleResult].MaxAngle != m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MaxAngle = m_intCheckMarkAngleMinMaxTolerance;
                // Start pattern match
                m_arrMatcher[m_intAngleResult].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[m_intAngleResult].NumPositions > 0)
                {
                    m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX;
                    m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY;
                    m_fDegAngleResult = m_arrMatcher[m_intAngleResult].GetPosition(0).Angle;
                }
                else
                    m_fDegAngleResult = 0;
            }
            else
            {
                if (m_fDegAngleResult != 0)
                    m_fDegAngleResult = 0;
            }

            return m_intAngleResult;
        }

        public int DoOrientationInspection_WithSubMatcher2(ImageDrawing grabbedImage, ROI objSearchROI, int intFinalReduction, bool blnWantDegAngleResult, bool blnSubMatcher)
        {
            // Reset inspection result
            m_intAngleResult = 4;
            m_fHighestScore = -1.0f;
            int intHighestScoreAngle = -1;
            m_strErrorMessage = "";

            // Check pattern file exist
            if (m_arrMatcher.Count == 0)
                return m_intAngleResult;

            // Check are all patterns learnt
            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                if (!m_arrMatcher[i].PatternLearnt)
                    return m_intAngleResult;
            }
            EImageBW8 orientImage = new EImageBW8(grabbedImage.ref_objMainImage.Width, grabbedImage.ref_objMainImage.Height);
            EROIBW8 ROI4ImageRotated = new EROIBW8();
            EasyImage.Copy(grabbedImage.ref_objMainImage, orientImage);
            objSearchROI.ref_ROI.Detach();
            objSearchROI.ref_ROI.Attach(grabbedImage.ref_objMainImage);
            ROI4ImageRotated.Detach();
            ROI4ImageRotated.Attach(orientImage);
            ROI4ImageRotated.SetPlacement(objSearchROI.ref_ROI.OrgX, objSearchROI.ref_ROI.OrgY, objSearchROI.ref_ROI.Width, objSearchROI.ref_ROI.Height);
            // Init data
            float fHighScore = 0;
            float fUnitAngle = 0;
            int intSelectedHighestScoreIndex = -1;
            for (int i = 0; i < 4; i++)
            {
                if (m_intDirections != 4)
                {
                    if (i == 1 || i == 3)
                        continue;
                }
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[i].FinalReduction != intFinalReduction)
                    m_arrMatcher[i].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[i].MinAngle != -15)
                    m_arrMatcher[i].MinAngle = -15;
                if (m_arrMatcher[i].MaxAngle != 15)
                    m_arrMatcher[i].MaxAngle = 15;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[i].FinalReduction != intFinalReduction)
                    m_arrMatcher[i].FinalReduction = intFinalReduction;
                if (m_arrMatcher[i].MinAngle != -15)
                    m_arrMatcher[i].MinAngle = -15;
                if (m_arrMatcher[i].MaxAngle != 15)
                    m_arrMatcher[i].MaxAngle = 15;

#endif

                // Start pattern match
                m_arrMatcher[i].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[i].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    float fScore = Math.Max(0, m_arrMatcher[i].GetPosition(0).Score);
                    if (fScore > fHighScore)
                    {
                        intSelectedHighestScoreIndex = i;
                        fHighScore = fScore;
                        fUnitAngle = m_arrMatcher[i].GetPosition(0).Angle;
                    }
                }
            }

            EasyImage.ScaleRotate(objSearchROI.ref_ROI, objSearchROI.ref_ROI.Width / 2f, objSearchROI.ref_ROI.Height / 2f,
                ROI4ImageRotated.Width / 2f, ROI4ImageRotated.Height / 2f, 1, 1, fUnitAngle, ROI4ImageRotated, 4);
            int intMatcherIndex = 0;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 704");
                    break;
                }
                // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#endif

                // Start pattern match
                m_arrMatcher[intMatcherIndex].Match(ROI4ImageRotated);

                if (m_arrMatcher[intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    // Matcher score must >= OrientMinScore, then only can go to submatcher.
                    // 

                    float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex].GetPosition(0).Score);
                    bool blnMatcherScoreHighestNow = false;
                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                        intHighestScoreAngle = intMatcherIndex;

                        if (fScore >= m_fOrientMinScore)
                        {
                            // Collect matcher center point and angle
                            if (intMatcherIndex == 0)
                            {
                                m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            }
                            m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            m_intAngleResult = intMatcherIndex;
                            blnMatcherScoreHighestNow = true;   // 2019 07 12 - Tell sub matcher that the current matcher is the highest score now.
                        }
                    }

                    if (fScore >= m_fOrientMinScore)
                    {
                        // Check sub matcher if matcher score >= orientMinScore
                        if (blnSubMatcher && m_objSubMatcher.PatternLearnt)
                        {
                            EROIBW8 objSubROI = new EROIBW8();
                            objSubROI.Detach();
                            objSubROI.Attach(ROI4ImageRotated);
                            objSubROI.SetPlacement((int)Math.Round(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX - m_arrMatcher[intMatcherIndex].PatternWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY - m_arrMatcher[intMatcherIndex].PatternHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                     m_arrMatcher[intMatcherIndex].PatternWidth, m_arrMatcher[intMatcherIndex].PatternHeight);
                            m_objSubMatcher.FinalReduction = 0;
                            m_objSubMatcher.MaxPositions = 3;   // set 3 position in order to get the best matching result. 

                            // rotate sub matcher angle for different angle. (Matcher not need to set angle because matcher have 4 different direction matcher already but sub matcher have one only.
                            switch (intMatcherIndex)
                            {
                                case 0:
                                    m_objSubMatcher.MinAngle = 0;
                                    m_objSubMatcher.MaxAngle = 0;
                                    break;
                                case 1:
                                    m_objSubMatcher.MinAngle = 90;
                                    m_objSubMatcher.MaxAngle = 90;
                                    break;
                                case 2:
                                    m_objSubMatcher.MinAngle = 180;
                                    m_objSubMatcher.MaxAngle = 180;
                                    break;
                                case 3:
                                    m_objSubMatcher.MinAngle = 270;
                                    m_objSubMatcher.MaxAngle = 270;
                                    break;
                            }
                            m_objSubMatcher.Match(objSubROI);

                            fScore = m_objSubMatcher.GetPosition(0).Score;

                            if (fScore > m_fHighestScore)
                            {
                                m_fHighestScore = fScore;
                                intHighestScoreAngle = intMatcherIndex;

                                // Collect matcher center point, not sub matcher center point.
                                m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX;
                                m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY;
                                m_intAngleResult = intMatcherIndex;
                            }
                            else if (blnMatcherScoreHighestNow)
                            {
                                // When matcher score higher than sub matcher score, correct the submatcher object center point for sub matcher drawing.
                                m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX;
                                m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY;
                            }
                        }
                    }
                }

                if (m_intDirections == 4)
                    intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                else
                    intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg


            } while (intMatcherIndex < 4);

            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------


            if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            // Get object deg angle result
            if (blnWantDegAngleResult && m_intAngleResult < 4)
            {
                if (m_arrMatcher[m_intAngleResult].FinalReduction != 0)
                    m_arrMatcher[m_intAngleResult].FinalReduction = 0;
                // Make sure basic setting for matcher file
                if (m_arrMatcher[m_intAngleResult].MinAngle != -m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MinAngle = -m_intCheckMarkAngleMinMaxTolerance;
                if (m_arrMatcher[m_intAngleResult].MaxAngle != m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MaxAngle = m_intCheckMarkAngleMinMaxTolerance;
                // Start pattern match
                m_arrMatcher[m_intAngleResult].Match(ROI4ImageRotated);

                if (m_arrMatcher[m_intAngleResult].NumPositions > 0)
                {
                    m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX;
                    m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY;
                    m_fDegAngleResult = fUnitAngle; // m_arrMatcher[m_intAngleResult].GetPosition(0).Angle;
                }
                else
                    m_fDegAngleResult = 0;
            }
            else
            {
                if (m_fDegAngleResult != 0)
                    m_fDegAngleResult = 0;
            }

            orientImage.Dispose();
            ROI4ImageRotated.Dispose();

            return m_intAngleResult;
        }

        public int DoOrientationInspection_WithSubMatcher3(ImageDrawing grabbedImage, ROI objSearchROI, ROI objUnitOriROI, ROI objSubOriROI, int intFinalReduction, bool blnWantDegAngleResult, bool blnSubMatcher)
        {
            // Reset inspection result
            m_intAngleResult = 4;
            m_fHighestScore = -1.0f;
            int intHighestScoreAngle = -1;
            m_strErrorMessage = "";

            // Check pattern file exist
            if (m_arrMatcher.Count == 0)
                return m_intAngleResult;

            // Check are all patterns learnt
            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                if (!m_arrMatcher[i].PatternLearnt)
                    return m_intAngleResult;
            }
            EImageBW8 orientImage = new EImageBW8(grabbedImage.ref_objMainImage.Width, grabbedImage.ref_objMainImage.Height);
            EROIBW8 ROI4ImageRotated = new EROIBW8();
            EROIBW8 objUnitROI = new EROIBW8();
            EROIBW8 objSubROI = new EROIBW8();
            EasyImage.Copy(grabbedImage.ref_objMainImage, orientImage);
            objSearchROI.ref_ROI.Detach();
            objSearchROI.ref_ROI.Attach(grabbedImage.ref_objMainImage);
            ROI4ImageRotated.Detach();
            ROI4ImageRotated.Attach(orientImage);
            ROI4ImageRotated.SetPlacement(objSearchROI.ref_ROI.OrgX, objSearchROI.ref_ROI.OrgY, objSearchROI.ref_ROI.Width, objSearchROI.ref_ROI.Height);
            // Init data
            float fHighScore = 0;
            float fUnitAngle = 0;
            int intSelectedHighestScoreIndex = -1;
            for (int i = 0; i < 4; i++)
            {
                if (m_intDirections != 4)
                {
                    if (i == 1 || i == 3)
                        continue;
                }
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[i].FinalReduction != intFinalReduction)
                    m_arrMatcher[i].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[i].MinAngle != -15)
                    m_arrMatcher[i].MinAngle = -15;
                if (m_arrMatcher[i].MaxAngle != 15)
                    m_arrMatcher[i].MaxAngle = 15;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[i].FinalReduction != intFinalReduction)
                    m_arrMatcher[i].FinalReduction = intFinalReduction;
                if (m_arrMatcher[i].MinAngle != -15)
                    m_arrMatcher[i].MinAngle = -15;
                if (m_arrMatcher[i].MaxAngle != 15)
                    m_arrMatcher[i].MaxAngle = 15;

#endif

                // Start pattern match
                m_arrMatcher[i].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[i].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    float fScore = Math.Max(0, m_arrMatcher[i].GetPosition(0).Score);
                    if (fScore > fHighScore)
                    {
                        intSelectedHighestScoreIndex = i;
                        fHighScore = fScore;
                        fUnitAngle = m_arrMatcher[i].GetPosition(0).Angle;
                    }
                }
            }

            EasyImage.ScaleRotate(objSearchROI.ref_ROI, objSearchROI.ref_ROI.Width / 2f, objSearchROI.ref_ROI.Height / 2f,
                ROI4ImageRotated.Width / 2f, ROI4ImageRotated.Height / 2f, 1, 1, fUnitAngle, ROI4ImageRotated, 4);
            int intMatcherIndex = 0;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 702");
                    break;
                }
                // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#endif

                // Start pattern match
                m_arrMatcher[intMatcherIndex].Match(ROI4ImageRotated);

                if (m_arrMatcher[intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    // Matcher score must >= OrientMinScore, then only can go to submatcher.
                    // 

                    float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex].GetPosition(0).Score);
                    bool blnMatcherScoreHighestNow = false;
                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                        intHighestScoreAngle = intMatcherIndex;

                        if (fScore >= m_fOrientMinScore)
                        {
                            // Collect matcher center point and angle
                            if (intMatcherIndex == 0)
                            {
                                m_fObjectOriCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                m_fObjectOriCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            }
                            m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            m_intAngleResult = intMatcherIndex;
                            blnMatcherScoreHighestNow = true;   // 2019 07 12 - Tell sub matcher that the current matcher is the highest score now.
                        }
                    }

                    if (fScore >= m_fOrientMinScore)
                    {
                        // Check sub matcher if matcher score >= orientMinScore
                        if (blnSubMatcher && m_objSubMatcher.PatternLearnt)
                        {
                            //ROI4ImageRotated.Save("D:\\TS\\RotateROI.bmp");
                            objUnitROI.Detach();
                            objUnitROI.Attach(ROI4ImageRotated);
                            objUnitROI.SetPlacement((int)(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX - m_arrMatcher[intMatcherIndex].PatternWidth / 2),
                                (int)(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY - m_arrMatcher[intMatcherIndex].PatternHeight / 2),
                                m_arrMatcher[intMatcherIndex].PatternWidth, m_arrMatcher[intMatcherIndex].PatternHeight);

                            //objUnitROI.Save("D:\\TS\\UnitROI.bmp");

                            objSubROI.Detach();
                            objSubROI.Attach(objUnitROI);

                            float fSubROICenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                            float fSubROICenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                            float fSubROIOffsetX = objUnitOriROI.ref_ROICenterX - objSubOriROI.ref_ROICenterX;
                            float fSubROIOffsetY = objUnitOriROI.ref_ROICenterY - objSubOriROI.ref_ROICenterY;

                            switch (intMatcherIndex)
                            {
                                case 0:
                                    objSubROI.SetPlacement(objSubOriROI.ref_ROICenterX - objSubOriROI.ref_ROIWidth,
                                        objSubOriROI.ref_ROICenterY - objSubOriROI.ref_ROIHeight,
                                        objSubOriROI.ref_ROIWidth * 2, objSubOriROI.ref_ROIHeight * 2);

                                    //objSubROI.Save("D:\\TS\\objSubROI1.bmp");
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    objSubROI.SetPlacement(objUnitROI.Width - objSubOriROI.ref_ROICenterX - objSubOriROI.ref_ROIWidth,
                                            objUnitROI.Height - objSubOriROI.ref_ROICenterY - objSubOriROI.ref_ROIHeight,
                                            objSubOriROI.ref_ROIWidth * 2, objSubOriROI.ref_ROIHeight * 2);

                                    //objSubROI.Save("D:\\TS\\objSubROI2.bmp");
                                    break;
                                case 3:
                                    break;
                            }

                            //objSubROI.SetPlacement((int)Math.Round(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX - m_arrMatcher[intMatcherIndex].PatternWidth / 2, 0, MidpointRounding.AwayFromZero),
                            //                       (int)Math.Round(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY - m_arrMatcher[intMatcherIndex].PatternHeight / 2, 0, MidpointRounding.AwayFromZero),
                            //                         m_arrMatcher[intMatcherIndex].PatternWidth, m_arrMatcher[intMatcherIndex].PatternHeight);
                            m_objSubMatcher.FinalReduction = 0;
                            m_objSubMatcher.MaxPositions = 3;   // set 3 position in order to get the best matching result. 

                            // rotate sub matcher angle for different angle. (Matcher not need to set angle because matcher have 4 different direction matcher already but sub matcher have one only.
                            switch (intMatcherIndex)
                            {
                                case 0:
                                    m_objSubMatcher.MinAngle = 0;
                                    m_objSubMatcher.MaxAngle = 0;
                                    break;
                                case 1:
                                    m_objSubMatcher.MinAngle = 90;
                                    m_objSubMatcher.MaxAngle = 90;
                                    break;
                                case 2:
                                    m_objSubMatcher.MinAngle = 180;
                                    m_objSubMatcher.MaxAngle = 180;
                                    break;
                                case 3:
                                    m_objSubMatcher.MinAngle = 270;
                                    m_objSubMatcher.MaxAngle = 270;
                                    break;
                            }
                            m_objSubMatcher.Match(objSubROI);

                            fScore = m_objSubMatcher.GetPosition(0).Score;

                            if (fScore > m_fHighestScore)
                            {
                                m_fHighestScore = fScore;
                                intHighestScoreAngle = intMatcherIndex;

                                // Collect matcher center point, not sub matcher center point.
                                m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX + (objSubROI.TotalOrgX - ROI4ImageRotated.TotalOrgX - objUnitROI.OrgX);
                                m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY + (objSubROI.TotalOrgY - ROI4ImageRotated.TotalOrgY - objUnitROI.OrgY);
                                m_intAngleResult = intMatcherIndex;
                            }
                            else if (blnMatcherScoreHighestNow)
                            {
                                //// When matcher score higher than sub matcher score, correct the submatcher object center point for sub matcher drawing.
                                //m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX;
                                //m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY;
                                m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX + (objSubROI.TotalOrgX - ROI4ImageRotated.TotalOrgX - objUnitROI.OrgX);
                                m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY + (objSubROI.TotalOrgY - ROI4ImageRotated.TotalOrgY - objUnitROI.OrgY);
                            }
                        }
                    }
                }

                if (m_intDirections == 4)
                    intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                else
                    intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg

            } while (intMatcherIndex < 4);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            // Get object deg angle result
            if (blnWantDegAngleResult && m_intAngleResult < 4)
            {
                if (m_arrMatcher[m_intAngleResult].FinalReduction != 0)
                    m_arrMatcher[m_intAngleResult].FinalReduction = 0;
                // Make sure basic setting for matcher file
                if (m_arrMatcher[m_intAngleResult].MinAngle != -m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MinAngle = -m_intCheckMarkAngleMinMaxTolerance;
                if (m_arrMatcher[m_intAngleResult].MaxAngle != m_intCheckMarkAngleMinMaxTolerance)
                    m_arrMatcher[m_intAngleResult].MaxAngle = m_intCheckMarkAngleMinMaxTolerance;
                // Start pattern match
                m_arrMatcher[m_intAngleResult].Match(ROI4ImageRotated);

                if (m_arrMatcher[m_intAngleResult].NumPositions > 0)
                {
                    m_fObjectCenterX = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterX;
                    m_fObjectCenterY = m_arrMatcher[m_intAngleResult].GetPosition(0).CenterY;
                    m_fDegAngleResult = fUnitAngle; // m_arrMatcher[m_intAngleResult].GetPosition(0).Angle;
                }
                else
                    m_fDegAngleResult = 0;
            }
            else
            {
                if (m_fDegAngleResult != 0)
                    m_fDegAngleResult = 0;
            }

            objUnitROI.Dispose();
            objUnitROI = null;
            objSubROI.Dispose();
            objSubROI = null;
            orientImage.Dispose();
            orientImage = null;
            ROI4ImageRotated.Dispose();
            ROI4ImageRotated = null;

            return m_intAngleResult;
        }

        public int DoOrientationInspection_WithSubMatcher4(ImageDrawing grabbedImage, ROI objSearchROI, ROI objUnitOriROI, ROI objSubOriROI, int intFinalReduction)
        {
            // Reset inspection result
            m_intAngleResult = 4;
            m_fHighestScore = 0f;   // -1.0f;   // Return at least 0 score to prevent at -100 score.
            m_strErrorMessage = "";

            // Check pattern file exist
            if (m_arrMatcher.Count == 0)
                return m_intAngleResult;

            // Check are all patterns learnt
            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                if (!m_arrMatcher[i].PatternLearnt)
                    return m_intAngleResult;
            }
            EImageBW8 orientImage = new EImageBW8(grabbedImage.ref_objMainImage.Width, grabbedImage.ref_objMainImage.Height);
            EROIBW8 ROI4ImageRotated = new EROIBW8();
            EROIBW8 objUnitROI = new EROIBW8();
            EROIBW8 objSubROI = new EROIBW8();
            EasyImage.Copy(grabbedImage.ref_objMainImage, orientImage);
            objSearchROI.ref_ROI.Detach();
            objSearchROI.ref_ROI.Attach(grabbedImage.ref_objMainImage);
            ROI4ImageRotated.Detach();
            ROI4ImageRotated.Attach(orientImage);
            ROI4ImageRotated.SetPlacement(objSearchROI.ref_ROI.OrgX, objSearchROI.ref_ROI.OrgY, objSearchROI.ref_ROI.Width, objSearchROI.ref_ROI.Height);

            // ----- Get unit angle, then rotate it to 0 degree -------------------------- 
            float fHighScore = 0;
            float fUnitAngle = 0;
            for (int i = 0; i < 4; i++)
            {
                if (m_intDirections != 4)
                {
                    if (i == 1 || i == 3)
                        continue;
                }
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[i].FinalReduction != intFinalReduction)
                    m_arrMatcher[i].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[i].MinAngle != -15)
                    m_arrMatcher[i].MinAngle = -15;
                if (m_arrMatcher[i].MaxAngle != 15)
                    m_arrMatcher[i].MaxAngle = 15;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[i].FinalReduction != intFinalReduction)
                    m_arrMatcher[i].FinalReduction = intFinalReduction;
                if (m_arrMatcher[i].MinAngle != -15)
                    m_arrMatcher[i].MinAngle = -15;
                if (m_arrMatcher[i].MaxAngle != 15)
                    m_arrMatcher[i].MaxAngle = 15;

#endif

                // Start pattern match
                m_arrMatcher[i].Match(objSearchROI.ref_ROI);

                if (m_arrMatcher[i].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    float fScore = Math.Max(0, m_arrMatcher[i].GetPosition(0).Score);
                    if (fScore > fHighScore)
                    {
                        fHighScore = fScore;
                        fUnitAngle = m_arrMatcher[i].GetPosition(0).Angle;
                    }
                }
            }

            // Rotate unit to 0 degree
            EasyImage.ScaleRotate(objSearchROI.ref_ROI, objSearchROI.ref_ROI.Width / 2f, objSearchROI.ref_ROI.Height / 2f,
                ROI4ImageRotated.Width / 2f, ROI4ImageRotated.Height / 2f, 1, 1, fUnitAngle, ROI4ImageRotated, 4);
            int intMatcherIndex = 0;
           
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            // Scan sub matcher 
            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 701");
                    break;
                }
                // ---------------------------------------------------------------------------------------------
#if (Debug_2_12 || Release_2_12)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = (uint)intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Make sure basic setting for matcher file
                if (m_arrMatcher[intMatcherIndex].FinalReduction != intFinalReduction)
                    m_arrMatcher[intMatcherIndex].FinalReduction = intFinalReduction;
                if (m_arrMatcher[intMatcherIndex].MinAngle != 0)
                    m_arrMatcher[intMatcherIndex].MinAngle = 0;
                if (m_arrMatcher[intMatcherIndex].MaxAngle != 0)
                    m_arrMatcher[intMatcherIndex].MaxAngle = 0;

#endif
                
                // Start pattern match
                m_arrMatcher[intMatcherIndex].Match(ROI4ImageRotated);

                if (m_arrMatcher[intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    // Matcher score must >= OrientMinScore, then only can go to submatcher.
                    //float fScore = Math.Max(0, m_arrMatcher[intMatcherIndex].GetPosition(0).Score);
                    //if (fScore >= m_fOrientMinScore)  // 2019 09 17 - CCENG: Not need to check matcher score because it is useless and also if all direction of matcher score are lower than setting, then the score result not able to display.
                    {
                        // Check sub matcher if matcher score >= orientMinScore
                        if (m_objSubMatcher.PatternLearnt)
                        {
                            objUnitROI.Detach();
                            objUnitROI.Attach(ROI4ImageRotated);
                            objUnitROI.SetPlacement((int)(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX - m_arrMatcher[intMatcherIndex].PatternWidth / 2),
                                (int)(m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY - m_arrMatcher[intMatcherIndex].PatternHeight / 2),
                                m_arrMatcher[intMatcherIndex].PatternWidth, m_arrMatcher[intMatcherIndex].PatternHeight);
                           
                            objSubROI.Detach();
                            objSubROI.Attach(objUnitROI);

                            float fSubROICenterX = objUnitROI.Width / 2;//m_arrMatcher[intMatcherIndex].PatternWidth / 2; //GetPosition(0).CenterX
                            float fSubROICenterY = objUnitROI.Height / 2;//m_arrMatcher[intMatcherIndex].PatternHeight / 2; //GetPosition(0).CenterY
                            float fSubROIOffsetX = m_intMatcherOffSetCenterX;// objUnitOriROI.ref_ROICenterX - objSubOriROI.ref_ROICenterX; // 2020-04-08 ZJYEOH : Use template offset because the unit and sub Orient ROI position might change 
                            float fSubROIOffsetY = m_intMatcherOffSetCenterY;// objUnitOriROI.ref_ROICenterY - objSubOriROI.ref_ROICenterY; 

                            switch (intMatcherIndex)
                            {
                                case 0:
                                    {
                                        // Make sure child ROI is not place out of parent ROI range.
                                        //int intOriX2 = objSubOriROI.ref_ROICenterX - objSubOriROI.ref_ROIWidth;
                                        //int intOriY2 = objSubOriROI.ref_ROICenterY - objSubOriROI.ref_ROIHeight;
                                        //int intWidth2 = objSubOriROI.ref_ROIWidth * 2;
                                        //int intHeight2 = objSubOriROI.ref_ROIHeight * 2;

                                        // 2020-04-08 ZJYEOH : Use pattern parameter because the unit and sub Orient ROI position might change 
                                        int intOriX2 = (int)(fSubROICenterX + fSubROIOffsetX - m_objSubMatcher.PatternWidth);
                                        int intOriY2 = (int)(fSubROICenterY + fSubROIOffsetY - m_objSubMatcher.PatternHeight);
                                        int intWidth2 = m_objSubMatcher.PatternWidth * 2;
                                        int intHeight2 = m_objSubMatcher.PatternHeight * 2;

                                        if (intOriX2 < 0)
                                        {
                                            intOriX2 = 0;
                                        }
                                        if (intOriY2 < 0)
                                        {
                                            intOriY2 = 0;
                                        }
                                        if ((intOriX2 + intWidth2) > objUnitROI.Width)
                                        {
                                            intWidth2 = objUnitROI.Width - intOriX2;
                                        }
                                        if ((intOriY2 + intHeight2) > objUnitROI.Height)
                                        {
                                            intHeight2 = objUnitROI.Height - intOriY2;
                                        }
                                        objSubROI.SetPlacement(intOriX2, intOriY2, intWidth2, intHeight2);

                                        //objSubROI.SetPlacement(objSubOriROI.ref_ROICenterX - objSubOriROI.ref_ROIWidth,
                                        //    objSubOriROI.ref_ROICenterY - objSubOriROI.ref_ROIHeight,
                                        //    objSubOriROI.ref_ROIWidth * 2, objSubOriROI.ref_ROIHeight * 2);
                                        break;
                                    }
                                case 1:
                                    {
                                        //2020-09-24 ZJYEOH : Condition for 90 deg
                                        int intOriX2 = (int)(fSubROICenterX - fSubROIOffsetY - m_objSubMatcher.PatternHeight);//PatternWidth
                                        int intOriY2 = (int)(fSubROICenterY + fSubROIOffsetX - m_objSubMatcher.PatternWidth);// PatternHeight
                                        int intWidth2 = m_objSubMatcher.PatternHeight * 2;//PatternWidth
                                        int intHeight2 = m_objSubMatcher.PatternWidth * 2;//PatternHeight

                                        if (intOriX2 < 0)
                                        {
                                            intOriX2 = 0;
                                        }
                                        if (intOriY2 < 0)
                                        {
                                            intOriY2 = 0;
                                        }
                                        if ((intOriX2 + intWidth2) > objUnitROI.Width)
                                        {
                                            intWidth2 = objUnitROI.Width - intOriX2;
                                        }
                                        if ((intOriY2 + intHeight2) > objUnitROI.Height)
                                        {
                                            intHeight2 = objUnitROI.Height - intOriY2;
                                        }
                                        objSubROI.SetPlacement(intOriX2, intOriY2, intWidth2, intHeight2);
                                    }
                                    break;
                                case 2:
                                    {
                                        // Make sure child ROI is not place out of parent ROI range.
                                        //int intOriX2 = objUnitROI.Width - objSubOriROI.ref_ROICenterX - objSubOriROI.ref_ROIWidth;
                                        //int intOriY2 = objUnitROI.Height - objSubOriROI.ref_ROICenterY - objSubOriROI.ref_ROIHeight;
                                        //int intWidth2 = objSubOriROI.ref_ROIWidth * 2;
                                        //int intHeight2 = objSubOriROI.ref_ROIHeight * 2;

                                        // 2020-04-08 ZJYEOH : Use pattern parameter because the unit and sub Orient ROI position might change 
                                        int intOriX2 = (int)(objUnitROI.Width - (fSubROICenterX + fSubROIOffsetX) - m_objSubMatcher.PatternWidth);
                                        int intOriY2 = (int)(objUnitROI.Height - (fSubROICenterY + fSubROIOffsetY) - m_objSubMatcher.PatternHeight);
                                        int intWidth2 = m_objSubMatcher.PatternWidth * 2;
                                        int intHeight2 = m_objSubMatcher.PatternHeight * 2;

                                        if (intOriX2 < 0)
                                        {
                                            intOriX2 = 0;
                                        }
                                        if (intOriY2 < 0)
                                        {
                                            intOriY2 = 0;
                                        }
                                        if ((intOriX2 + intWidth2) > objUnitROI.Width)
                                        {
                                            intWidth2 = objUnitROI.Width - intOriX2;
                                        }
                                        if ((intOriY2 + intHeight2) > objUnitROI.Height)
                                        {
                                            intHeight2 = objUnitROI.Height - intOriY2;
                                        }
                                        objSubROI.SetPlacement(intOriX2, intOriY2, intWidth2, intHeight2);

                                        //objSubROI.SetPlacement(objUnitROI.Width - objSubOriROI.ref_ROICenterX - objSubOriROI.ref_ROIWidth,
                                        //        objUnitROI.Height - objSubOriROI.ref_ROICenterY - objSubOriROI.ref_ROIHeight,
                                        //        objSubOriROI.ref_ROIWidth * 2, objSubOriROI.ref_ROIHeight * 2);
                                    }
                                    break;
                                case 3:
                                    {
                                        //2020-09-24 ZJYEOH : Condition for -90 deg
                                        int intOriX2 = (int)(fSubROICenterX + fSubROIOffsetY - m_objSubMatcher.PatternHeight);//PatternWidth
                                        int intOriY2 = (int)(fSubROICenterY - fSubROIOffsetX - m_objSubMatcher.PatternWidth);//PatternHeight
                                        int intWidth2 = m_objSubMatcher.PatternHeight * 2;//PatternWidth
                                        int intHeight2 = m_objSubMatcher.PatternWidth * 2;//PatternHeight

                                        if (intOriX2 < 0)
                                        {
                                            intOriX2 = 0;
                                        }
                                        if (intOriY2 < 0)
                                        {
                                            intOriY2 = 0;
                                        }
                                        if ((intOriX2 + intWidth2) > objUnitROI.Width)
                                        {
                                            intWidth2 = objUnitROI.Width - intOriX2;
                                        }
                                        if ((intOriY2 + intHeight2) > objUnitROI.Height)
                                        {
                                            intHeight2 = objUnitROI.Height - intOriY2;
                                        }
                                        objSubROI.SetPlacement(intOriX2, intOriY2, intWidth2, intHeight2);
                                    }
                                    break;
                            }

                            m_objSubMatcher.FinalReduction = 0;
                            m_objSubMatcher.MaxPositions = 3;   // set 3 position in order to get the best matching result. 

                            // rotate sub matcher angle for different angle. (Matcher not need to set angle because matcher have 4 different direction matcher already but sub matcher have one only.
                            switch (intMatcherIndex)
                            {
                                case 0:
                                    m_objSubMatcher.MinAngle = 0;
                                    m_objSubMatcher.MaxAngle = 0;
                                    break;
                                case 1:
                                    m_objSubMatcher.MinAngle = 90;
                                    m_objSubMatcher.MaxAngle = 90;
                                    break;
                                case 2:
                                    m_objSubMatcher.MinAngle = 180;
                                    m_objSubMatcher.MaxAngle = 180;
                                    break;
                                case 3:
                                    m_objSubMatcher.MinAngle = 270;
                                    m_objSubMatcher.MaxAngle = 270;
                                    break;
                            }
                            if (objSubROI.OrgX >= 0 && objSubROI.OrgY >= 0 && objSubROI.Width > 0 && objSubROI.Height > 0)
                            {
                                m_objSubMatcher.Match(objSubROI);
                            }
                            //objUnitROI.Save("D:\\objUnitROI"+intMatcherIndex+".bmp");
                            //objSubROI.Save("D:\\objSubROI" + intMatcherIndex + ".bmp");
                            if (m_objSubMatcher.NumPositions > 0)
                            {
                                float fScore = m_objSubMatcher.GetPosition(0).Score;

                                if (fScore > m_fHighestScore)
                                {
                                    m_fHighestScore = fScore;

                                    // Collect matcher center point, not sub matcher center point.
                                    m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                    m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                    m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX + (objSubROI.TotalOrgX - ROI4ImageRotated.TotalOrgX - objUnitROI.OrgX);
                                    m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY + (objSubROI.TotalOrgY - ROI4ImageRotated.TotalOrgY - objUnitROI.OrgY);

                                    if (fScore >= m_fOrientMinScore)
                                        m_intAngleResult = intMatcherIndex;
                                }
                            }
                            else
                            {
                                // 2020 12 08 - CCENG: if cannot find object, put tolerance +-3 to doublec check pattern result.
                                //                     Euresys have weakness not able to find pattern when min max angle set at no zero range.
                                float fMinAngle = m_objSubMatcher.MinAngle;
                                float fMaxAngle = m_objSubMatcher.MaxAngle;
                                m_objSubMatcher.MinAngle = fMinAngle - 3;
                                m_objSubMatcher.MaxAngle = fMaxAngle + 3;

                                if (objSubROI.OrgX >= 0 && objSubROI.OrgY >= 0 && objSubROI.Width > 0 && objSubROI.Height > 0)
                                {
                                    m_objSubMatcher.Match(objSubROI);
                                }

                                if (m_objSubMatcher.NumPositions > 0)
                                {
                                    float fScore = m_objSubMatcher.GetPosition(0).Score;

                                    if (fScore > m_fHighestScore)
                                    {
                                        m_fHighestScore = fScore;

                                        // Collect matcher center point, not sub matcher center point.
                                        m_fObjectCenterX = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterX;
                                        m_fObjectCenterY = m_arrMatcher[intMatcherIndex].GetPosition(0).CenterY;
                                        m_fSubObjectCenterX = m_objSubMatcher.GetPosition(0).CenterX + (objSubROI.TotalOrgX - ROI4ImageRotated.TotalOrgX - objUnitROI.OrgX);
                                        m_fSubObjectCenterY = m_objSubMatcher.GetPosition(0).CenterY + (objSubROI.TotalOrgY - ROI4ImageRotated.TotalOrgY - objUnitROI.OrgY);

                                        if (fScore >= m_fOrientMinScore)
                                            m_intAngleResult = intMatcherIndex;
                                    }
                                }

                            }
                        }
                    }
                }

                if (m_intDirections == 4)
                    intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                else
                    intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg

            } while (intMatcherIndex < 4);

            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            // Get object deg angle result
            if (m_intAngleResult < 4)
            {
                m_fDegAngleResult = fUnitAngle;
            }
            else
            {
                if (m_fDegAngleResult != 0)
                    m_fDegAngleResult = 0;
            }

            objUnitROI.Dispose();
            objSubROI.Dispose();
            orientImage.Dispose();
            ROI4ImageRotated.Dispose();

            return m_intAngleResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchROI">ROI for orient test</param>
        /// <param name="grabbedImage">Grab Image</param>
        /// <param name="blnRotate"></param>
        /// <param name="fHighestScore"></param>
        /// <param name="blnPreciseAngleCheck">Extra check angle with more precise. Default is True.</param>
        /// <param name="blnSubMatcher">Want sub matcher test</param>
        /// <returns></returns>
        private int MatchOrient(EROIBW8 searchROI, EImageBW8 grabbedImage, bool blnRotate, float fHighestScore, bool blnPreciseAngleCheck, bool blnSubMatcher)
        {
            m_intAngleResult = 4;
            m_fHighestScore = -1.0f;
            int intHighestScoreAngle = -1;
            m_strErrorMessage = "";

            if (!IsPatternLearnt())
                return m_intAngleResult;

            int intTestCount = 0;      // Firstly, image not need to be rotated. If it is fail, then rotate image to angle 270 and continue on.
            int intRotateAngle = 0;

            EImageBW8 orientImage = new EImageBW8(grabbedImage.Width, grabbedImage.Height);
            EROIBW8 ROI4ImageRotated = new EROIBW8();
            EasyImage.Copy(grabbedImage, orientImage);
            searchROI.Detach();
            searchROI.Attach(grabbedImage);
            ROI4ImageRotated.Detach();
            ROI4ImageRotated.Attach(orientImage);
            ROI4ImageRotated.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);
            m_objMatcher.MinAngle = 0;
            m_objMatcher.MaxAngle = 0;
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 700");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                if (intTestCount > 0)
                {
                    switch (intTestCount)
                    {
                        case 1:
                            intRotateAngle = 90;
                            break;
                        case 2:
                            intRotateAngle = 180;
                            break;
                        case 3:
                            intRotateAngle = 270;
                            break;
                    }

                    Rotate(searchROI, ref ROI4ImageRotated, intRotateAngle);
                }

                m_objMatcher.FinalReduction = 0;

                m_objMatcher.Match(ROI4ImageRotated);

                if (m_objMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    float fScore;

                    if (m_objMatcher.GetPosition(0).Score < 0)
                        fScore = 0;
                    else
                        fScore = m_objMatcher.GetPosition(0).Score;

                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;
                        intHighestScoreAngle = intTestCount;

                        m_fObjectCenterX = m_objMatcher.GetPosition(0).CenterX;
                        m_fObjectCenterY = m_objMatcher.GetPosition(0).CenterY;
                        if (intTestCount == 0)
                        {
                            m_fObjectOriCenterX = m_objMatcher.GetPosition(0).CenterX;
                            m_fObjectOriCenterY = m_objMatcher.GetPosition(0).CenterY;
                        }

                        if (fScore >= m_fOrientMinScore)
                        {
                            if (blnSubMatcher && m_objSubMatcher.PatternLearnt)
                            {
                                EROIBW8 objSubROI = new EROIBW8();
                                objSubROI.Detach();
                                objSubROI.Attach(ROI4ImageRotated);
                                objSubROI.SetPlacement((int)Math.Round(m_objMatcher.GetPosition(0).CenterX - m_objMatcher.PatternWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(m_objMatcher.GetPosition(0).CenterY - m_objMatcher.PatternHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                         m_objMatcher.PatternWidth, m_objMatcher.PatternHeight);
                                m_objSubMatcher.FinalReduction = 0;
                                m_objSubMatcher.Match(objSubROI);

                                fScore = m_objSubMatcher.GetPosition(0).Score;
                            }

                            if (fScore >= m_fOrientMinScore)
                            {
                                m_fObjectCenterX = m_objMatcher.GetPosition(0).CenterX;
                                m_fObjectCenterY = m_objMatcher.GetPosition(0).CenterY;
                                if (intTestCount == 0)
                                {
                                    m_fObjectOriCenterX = m_objMatcher.GetPosition(0).CenterX;
                                    m_fObjectOriCenterY = m_objMatcher.GetPosition(0).CenterY;
                                }
                                if (blnRotate && (m_fHighestScore > fHighestScore))
                                    EasyImage.Copy(orientImage, m_objRotateImage.ref_objMainImage);

                                m_intAngleResult = intTestCount;
                            }
                            else
                            {
                                if (intTestCount == 0)
                                {
                                    m_fObjectOriCenterX = -1;
                                    m_fObjectOriCenterY = -1;
                                }
                            }
                        }
                    }
                }

                if (m_intDirections == 4)
                    intTestCount++;
                else
                    intTestCount += 2;


            } while (intTestCount < 4);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            //-------------- Comfirm angle and rotate it to 0 degree ----------------------------------
            if (blnPreciseAngleCheck)
            {
                Rotate(searchROI, ref ROI4ImageRotated, 90 * intHighestScoreAngle);
                float fOriMinAngle = m_objMatcher.MinAngle;
                float fOriMaxAngle = m_objMatcher.MaxAngle;
                m_objMatcher.MinAngle = -10;
                m_objMatcher.MaxAngle = 10;
                m_objMatcher.Match(ROI4ImageRotated);

                if (m_objMatcher.NumPositions > 0)
                {
                    float fScore = m_objMatcher.GetPosition(0).Score;
                    if (fScore > m_fHighestScore)
                    {
                        m_fHighestScore = fScore;

                        if (fScore >= m_fOrientMinScore)
                        {
                            m_fObjectCenterX = m_objMatcher.GetPosition(0).CenterX;
                            m_fObjectCenterY = m_objMatcher.GetPosition(0).CenterY;
                            if (blnRotate && (m_fHighestScore > fHighestScore))
                                EasyImage.Copy(orientImage, m_objRotateImage.ref_objMainImage);
                            float fAngle = m_objMatcher.GetPosition(0).Angle + 90 * intHighestScoreAngle;

                            ROI4ImageRotated.Detach();
                            ROI4ImageRotated.Attach(m_objRotateImage.ref_objMainImage);
                            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f, searchROI.Height / 2f, ROI4ImageRotated.Width / 2f, ROI4ImageRotated.Height / 2f, 1, 1, fAngle, ROI4ImageRotated, 8);
                            m_intAngleResult = intHighestScoreAngle;
                        }
                    }
                }
                m_objMatcher.MinAngle = fOriMinAngle;
                m_objMatcher.MaxAngle = fOriMaxAngle;
            }

            if (m_intAngleResult == 4)
            {
                m_strErrorMessage = "Fail Orient - Not fulfill Min Setting : Set = " + (m_fOrientMinScore * 100).ToString() +
                    " Score = " + (m_fHighestScore * 100).ToString("f2");
            }

            orientImage.Dispose();
            return m_intAngleResult;
        }

        private void Rotate(EROIBW8 searchROI, ref EROIBW8 destinationROI, int intRotateAngle)
        {
            switch (intRotateAngle)
            {
                case 0:
                case 2:
                    destinationROI.SetSize(searchROI.Width, searchROI.Height);
                    break;
                case 1:
                case 3:
                    destinationROI.SetSize(searchROI.Height, searchROI.Width);
                    break;
            }
            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f, searchROI.Height / 2f, destinationROI.Width / 2f, destinationROI.Height / 2f, 1, 1, intRotateAngle, destinationROI, 0);
        }

        public bool MatchWithTemplateUnitPR(ROI objSearchROI)
        {
            if (m_objUnitPRMatcher == null)
            {
                STTrackLog.WriteLine("MatchWithTemplateUnitPR > m_objUnitPRMatcher is null");
            }

            if (objSearchROI.ref_ROI == null)
            {
                STTrackLog.WriteLine("MatchWithTemplateUnitPR > objSearchROI is null");
            }

            if (!m_objUnitPRMatcher.PatternLearnt)
                return false;

            // 2020 03 10 - Set the MaxPositions to 2 will stabilize the pattern matching result.
            if (m_objUnitPRMatcher.MaxPositions < 2)
                m_objUnitPRMatcher.MaxPositions = 2;

            #region (DEBUG)
            //m_objUnitPRMatcher.Save("D:\\TS\\UnitPRMatcher.mch");
            //objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI.bmp");
            #endregion
            m_objUnitPRMatcher.Match(objSearchROI.ref_ROI);
        
            if (m_objUnitPRMatcher.NumPositions > 0 && m_objUnitPRMatcher.GetPosition(0).Score > 0.5)     // if macthing result hit the min score, its position will be 1 or more
            {
                return true;
            }

            return false;
        }

        public bool MatchWithTemplateUnitPR(ROI objSearchROI, float fLimitScore)
        {
            if (!m_objUnitPRMatcher.PatternLearnt)
                return false;

            // 2020 03 10 - Set the MaxPositions to 2 will stabilize the pattern matching result.
            if (m_objUnitPRMatcher.MaxPositions < 2)
                m_objUnitPRMatcher.MaxPositions = 2;

            #region (DEBUG)
            //m_objUnitPRMatcher.Save("D:\\TS\\UnitPRMatcher.mch");
            //objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI.bmp");
            #endregion
            m_objUnitPRMatcher.Match(objSearchROI.ref_ROI);

            if (m_objUnitPRMatcher.NumPositions > 0 && m_objUnitPRMatcher.GetPosition(0).Score > fLimitScore)     // if macthing result hit the min score, its position will be 1 or more
            {
                return true;
            }

            return false;
        }

        public float GetResultAngle()
        {
            if (m_objMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                return m_objMatcher.GetPosition(0).Angle;
            }
            else
                return 0;
        }

        public float GetUnitPRResultAngle()
        {
            if (m_objUnitPRMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                return m_objUnitPRMatcher.GetPosition(0).Angle;
            }
            else
                return 0;
        }

        public float GetUnitPRResultCenterX()
        {
            if (m_objUnitPRMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                return m_objUnitPRMatcher.GetPosition(0).CenterX;
            }
            else
                return 0;
        }

        public float GetUnitPRResultCenterY()
        {
            if (m_objUnitPRMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                return m_objUnitPRMatcher.GetPosition(0).CenterY;
            }
            else
                return 0;
        }

        public int GetUnitPRWidth()
        {
            if (!m_objUnitPRMatcher.PatternLearnt)
                return 0;

            return m_objUnitPRMatcher.PatternWidth;
        }

        public int GetUnitPRHeight()
        {
            if (!m_objUnitPRMatcher.PatternLearnt)
                return 0;

            return m_objUnitPRMatcher.PatternHeight;
        }
        public int GetMatcherWidth(int intMatcherIndex)
        {
            if ((m_arrMatcher.Count <= intMatcherIndex) || (!m_arrMatcher[intMatcherIndex].PatternLearnt))
                return 0;

            return m_arrMatcher[intMatcherIndex].PatternWidth;
        }

        public int GetMatcherHeight(int intMatcherIndex)
        {
            if ((m_arrMatcher.Count <= intMatcherIndex) || (!m_arrMatcher[intMatcherIndex].PatternLearnt))
                return 0;

            return m_arrMatcher[intMatcherIndex].PatternHeight;
        }

        public void Dispose()
        {
            if (m_objRotateImage != null)
                m_objRotateImage.Dispose();

            if (m_objUnitPRMatcher != null)
                m_objUnitPRMatcher.Dispose();

            if (m_objMatcher != null)
                m_objMatcher.Dispose();

            if (m_objSubMatcher != null)
                m_objSubMatcher.Dispose();

            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
                m_arrMatcher[i].Dispose();
            }
        }

        public void SetCalibrationData(float fPixelPerMMX, float fPixelPerMMY, int intUnitMode)
        {
            // MM to Pixel formula
            m_fMMPerPixelX = 1 / fPixelPerMMX;
            m_fMMPerPixelY = 1 / fPixelPerMMY;
            m_fMMToPixelXValue = fPixelPerMMX;
            m_fMMToPixelYValue = fPixelPerMMY;
            m_fMMtoPixelAreaValue = fPixelPerMMX * fPixelPerMMY;

            // MM to Micron or Mil formula
            switch (intUnitMode)
            {
                case 1: // mm
                    m_fMMToUnitValue = 1;
                    m_fMMToUnitValueArea = 1;
                    break;
                case 2: // mil
                    m_fMMToUnitValue = 1 / 0.0254f;
                    m_fMMToUnitValueArea = 1 / (0.0254f * 0.0254f);
                    break;
                case 3: // micron
                    m_fMMToUnitValue = 1000;
                    m_fMMToUnitValueArea = 1000000;
                    break;

            }


        }


        public float GetCenterXDiff(float fRotatedCenterX, int intSearchROIX)
        {

            return (intSearchROIX + m_fObjectCenterX - fRotatedCenterX) * m_fMMPerPixelX * m_fMMToUnitValue;
        }

        public float GetCenterYDiff(float fRotatedCenterY, int intSearchROIY)
        {

            return (intSearchROIY + m_fObjectCenterY - fRotatedCenterY) * m_fMMPerPixelY * m_fMMToUnitValue;
        }
    }
}
