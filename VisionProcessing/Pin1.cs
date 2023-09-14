using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Common;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace VisionProcessing
{
    public class Pin1
    {
        #region Member Variables

        private bool m_blnFinalResultPassFail = false;  // Keep final result of after inspect all template
        private int m_intFinalResultSelectedTemplate = 0;
        private string m_strErrorMessage = "";
        private ROI m_objSearchROI;
        private ROI m_objPin1ROI;
        private ROI m_objTestROI;
        private List<TemplateSetting> m_arrTemplateSetting = new List<TemplateSetting>();   // First Dimension = Template 
        private Pen penLime = new Pen(Color.Lime);
        private Pen penRed = new Pen(Color.Red);
        private object m_objLock = new object();
        
        public class TemplateSetting
        {
            public int intOffsetFromSearchROIX = 0;
            public int intOffsetFromSearchROIY = 0;
            public float fOffsetRefPosX = 0;
            public float fOffsetRefPosY = 0;
            public float fMinScore = 0.7f;
            public EMatcher objMatcher = new EMatcher();

            public float fResultScore = -1;     // Keep individual template result
            public float fResultPosX = 0;       // Keep individual template result
            public float fResultPosY = 0;       // Keep individual template result

            public int intDrawParentROIOriX = 0;
            public int intDrawParentROIOriY = 0;

            public bool blnTemplateOrientationIsLeft = false;
            public bool blnTemplateOrientationIsTop = false;

            public bool blnWantCheckPin1 = false;
        }

        private ImageDrawing m_objSampleImage = new ImageDrawing(true);

        #endregion

        #region Properties
        public int ref_intFinalResultSelectedTemplate { get { return m_intFinalResultSelectedTemplate; } set { m_intFinalResultSelectedTemplate = value; } }
        public bool ref_blnFinalResultPassFail { get { return m_blnFinalResultPassFail; } set { m_blnFinalResultPassFail = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public ROI ref_objSearchROI { get { return m_objSearchROI; } set { m_objSearchROI = value; } }
        public ROI ref_objPin1ROI { get { return m_objPin1ROI; } set { m_objPin1ROI = value; } }
        public ROI ref_objTestROI { get { return m_objTestROI; } set { m_objTestROI = value; } }
        public List<TemplateSetting> ref_arrTemplateSetting { get { return m_arrTemplateSetting; } set { m_arrTemplateSetting = value; } }

        #endregion


        public Pin1()
        {

        }

        public void DrawFinalResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            lock (m_objLock)
            {

                if (m_intFinalResultSelectedTemplate == -1)
                    return;

                if (m_intFinalResultSelectedTemplate >= m_arrTemplateSetting.Count)
                    return;

                Pen p;
                if (m_blnFinalResultPassFail)
                    p = penLime;
                else
                    p = penRed;

                g.DrawString("Pin 1", new Font("Verdana", 12, FontStyle.Regular), new SolidBrush(p.Color), (m_arrTemplateSetting[m_intFinalResultSelectedTemplate].intDrawParentROIOriX +
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].fResultPosX -
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].objMatcher.PatternWidth / 2) * fDrawingScaleX, (m_arrTemplateSetting[m_intFinalResultSelectedTemplate].intDrawParentROIOriY +
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].fResultPosY -
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].objMatcher.PatternHeight / 2) * fDrawingScaleY - 20);
                g.DrawRectangle(p,
                               (m_arrTemplateSetting[m_intFinalResultSelectedTemplate].intDrawParentROIOriX +
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].fResultPosX -
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].objMatcher.PatternWidth / 2) * fDrawingScaleX,
                               (m_arrTemplateSetting[m_intFinalResultSelectedTemplate].intDrawParentROIOriY +
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].fResultPosY -
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].objMatcher.PatternHeight / 2) * fDrawingScaleY,
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].objMatcher.PatternWidth * fDrawingScaleX,
                               m_arrTemplateSetting[m_intFinalResultSelectedTemplate].objMatcher.PatternHeight * fDrawingScaleY);
            }
        }
        public void DrawFinalResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intSelectedTemplate)
        {
            lock (m_objLock)
            {

                if (intSelectedTemplate == -1)
                    return;

                if (intSelectedTemplate >= m_arrTemplateSetting.Count)
                    return;

                Pen p;
                if (m_arrTemplateSetting[intSelectedTemplate].fResultScore >= m_arrTemplateSetting[intSelectedTemplate].fMinScore)
                    p = penLime;
                else
                    p = penRed;

                g.DrawString("Pin 1", new Font("Verdana", 12, FontStyle.Regular), new SolidBrush(p.Color), (m_arrTemplateSetting[intSelectedTemplate].intDrawParentROIOriX +
                               m_arrTemplateSetting[intSelectedTemplate].fResultPosX -
                               m_arrTemplateSetting[intSelectedTemplate].objMatcher.PatternWidth / 2) * fDrawingScaleX, (m_arrTemplateSetting[intSelectedTemplate].intDrawParentROIOriY +
                               m_arrTemplateSetting[intSelectedTemplate].fResultPosY -
                               m_arrTemplateSetting[intSelectedTemplate].objMatcher.PatternHeight / 2) * fDrawingScaleY - 20);
                g.DrawRectangle(p,
                               (m_arrTemplateSetting[intSelectedTemplate].intDrawParentROIOriX +
                               m_arrTemplateSetting[intSelectedTemplate].fResultPosX -
                               m_arrTemplateSetting[intSelectedTemplate].objMatcher.PatternWidth / 2) * fDrawingScaleX,
                               (m_arrTemplateSetting[intSelectedTemplate].intDrawParentROIOriY +
                               m_arrTemplateSetting[intSelectedTemplate].fResultPosY -
                               m_arrTemplateSetting[intSelectedTemplate].objMatcher.PatternHeight / 2) * fDrawingScaleY,
                               m_arrTemplateSetting[intSelectedTemplate].objMatcher.PatternWidth * fDrawingScaleX,
                               m_arrTemplateSetting[intSelectedTemplate].objMatcher.PatternHeight * fDrawingScaleY);
            }
        }
        public float GetFinalScore()
        {
            if (m_intFinalResultSelectedTemplate < 0 || m_intFinalResultSelectedTemplate >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[m_intFinalResultSelectedTemplate].fResultScore;
        }

        public float GetMinScoreSetting(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].fMinScore;
        }

        public int GetPin1PatternWidth(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].objMatcher.PatternWidth;
        }

        public int GetPin1PatternHeight(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].objMatcher.PatternHeight;
        }

        public float GetRefOffsetX(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].fOffsetRefPosX;
        }

        public float GetRefOffsetY(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].fOffsetRefPosY;
        }

        public float GetResultScore(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return -1;

            return m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore;
        }

        public float GetResultPosX(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].fResultPosX;
        }

        public float GetResultPosY(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return 0;

            return m_arrTemplateSetting[intSelectedTemplateIndex].fResultPosY;
        }

        public void LearnTemplate(int intSelectedTemplate, float fOffsetRefPosX, float fOffsetRefPosY, ROI objPin1ROI)
        {
            // 2019 09 10 - JBTAN: dont clear because will make setting like blnWantCheckPin1 back to default(false)
            //m_arrTemplateSetting.Clear();
            for (int i = m_arrTemplateSetting.Count; i <= intSelectedTemplate; i++)
            {
                m_arrTemplateSetting.Add(new TemplateSetting());
            }

            m_arrTemplateSetting[intSelectedTemplate].fOffsetRefPosX = fOffsetRefPosX;
            m_arrTemplateSetting[intSelectedTemplate].fOffsetRefPosY = fOffsetRefPosY;
#if (Debug_2_12 || Release_2_12)
            m_arrTemplateSetting[intSelectedTemplate].objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
            m_arrTemplateSetting[intSelectedTemplate].objMatcher.LearnPattern(objPin1ROI.ref_ROI);
        }
        public void LearnTemplate(int intSelectedTemplate, float fOffsetRefPosX, float fOffsetRefPosY, ROI objPin1ROI, ROI objSearchROI)
        {
            // 2019 09 10 - JBTAN: dont clear because will make setting like blnWantCheckPin1 back to default(false)
            //m_arrTemplateSetting.Clear();
            for (int i = m_arrTemplateSetting.Count; i <= intSelectedTemplate; i++)
            {
                m_arrTemplateSetting.Add(new TemplateSetting());
            }

            m_arrTemplateSetting[intSelectedTemplate].fOffsetRefPosX = fOffsetRefPosX;
            m_arrTemplateSetting[intSelectedTemplate].fOffsetRefPosY = fOffsetRefPosY;
#if (Debug_2_12 || Release_2_12)
            m_arrTemplateSetting[intSelectedTemplate].objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
            m_arrTemplateSetting[intSelectedTemplate].objMatcher.LearnPattern(objPin1ROI.ref_ROI);

            float fOrientationSeparatorX = (float)objSearchROI.ref_ROIWidth / 2;
            float fOrientationSeparatorY = (float)objSearchROI.ref_ROIHeight / 2;

            if ((objPin1ROI.ref_ROICenterX) < fOrientationSeparatorX)
                m_arrTemplateSetting[intSelectedTemplate].blnTemplateOrientationIsLeft = true;
            else
                m_arrTemplateSetting[intSelectedTemplate].blnTemplateOrientationIsLeft = false;

            if ((objPin1ROI.ref_ROICenterY) < fOrientationSeparatorY)
                m_arrTemplateSetting[intSelectedTemplate].blnTemplateOrientationIsTop = true;
            else
                m_arrTemplateSetting[intSelectedTemplate].blnTemplateOrientationIsTop = false;

            m_arrTemplateSetting[intSelectedTemplate].intOffsetFromSearchROIX = objSearchROI.ref_ROITotalCenterX - objPin1ROI.ref_ROITotalCenterX;
            m_arrTemplateSetting[intSelectedTemplate].intOffsetFromSearchROIY = objSearchROI.ref_ROITotalCenterY - objPin1ROI.ref_ROITotalCenterY;
        }
        public void SetPreviousTemplateSettingtoOtherTemplate(string strFolderPath, int intSelectedTemplate)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
            objFile.GetFirstSection("Pin1Settings");

            int intTemplateNum = objFile.GetValueAsInt("Pin1TemplateNum", 0);
            m_arrTemplateSetting.Clear();

            for (int j = 0; j < intTemplateNum; j++)
            {
                if (intSelectedTemplate == j)
                    objFile.GetSecondSection("Pin1Template" + (intSelectedTemplate - 1)); //j //2020-05-08 ZJYEOH : load previous template setting to set to other template
                else
                    objFile.GetSecondSection("Pin1Template" + j);

                TemplateSetting objTemplateSetting = new TemplateSetting();

                objTemplateSetting.blnWantCheckPin1 = objFile.GetValueAsBoolean("WantCheckPin1", false, 2);
                objTemplateSetting.fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f, 2);

                //2020-05-08 ZJYEOH : reset back to current template because finish set to tolerance setting
                objFile.GetSecondSection("Pin1Template" + j);

                objTemplateSetting.fOffsetRefPosX = objFile.GetValueAsFloat("OffsetRefPosX", 0, 2);
                objTemplateSetting.fOffsetRefPosY = objFile.GetValueAsFloat("OffsetRefPosY", 0, 2);

                objTemplateSetting.blnTemplateOrientationIsLeft = objFile.GetValueAsBoolean("TemplateOrientationIsLeft", false, 2);
                objTemplateSetting.blnTemplateOrientationIsTop = objFile.GetValueAsBoolean("TemplateOrientationIsTop", false, 2);

                objTemplateSetting.intOffsetFromSearchROIX = objFile.GetValueAsInt("OffsetFromSearchROIX", 0, 2);
                objTemplateSetting.intOffsetFromSearchROIY = objFile.GetValueAsInt("OffsetFromSearchROIY", 0, 2);

                if (objTemplateSetting.objMatcher == null)
                    objTemplateSetting.objMatcher = new EMatcher();

                if (System.IO.File.Exists(strFolderPath + "Pin1Template" + j.ToString() + ".mch"))
                    objTemplateSetting.objMatcher.Load(strFolderPath + "Pin1Template" + j.ToString() + ".mch");

                m_arrTemplateSetting.Add(objTemplateSetting);
            }

            objFile.GetSecondSection("SearchROI");
            if (m_objSearchROI == null)
                m_objSearchROI = new ROI();
            m_objSearchROI.LoadROISetting(
                objFile.GetValueAsInt("SearchROIStartX", 0, 2),
                objFile.GetValueAsInt("SearchROIStartY", 0, 2),
                objFile.GetValueAsInt("SearchROIWidth", 100, 2),
                objFile.GetValueAsInt("SearchROIHeight", 100, 2));

            objFile.GetSecondSection("Pin1ROI");
            if (m_objPin1ROI == null)
                m_objPin1ROI = new ROI();
            m_objPin1ROI.LoadROISetting(
                objFile.GetValueAsInt("Pin1ROIStartX", 0, 2),
                objFile.GetValueAsInt("Pin1ROIStartY", 0, 2),
                objFile.GetValueAsInt("Pin1ROIWidth", 100, 2),
                objFile.GetValueAsInt("Pin1ROIHeight", 100, 2));

        }
        public void SetTemplate1SettingtoOtherTemplate(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
            objFile.GetFirstSection("Pin1Settings");

            int intTemplateNum = objFile.GetValueAsInt("Pin1TemplateNum", 0);
            m_arrTemplateSetting.Clear();

            for (int j = 0; j < intTemplateNum; j++)
            {
                objFile.GetSecondSection("Pin1Template" + 0); //j //2020-05-08 ZJYEOH : Always load template 1 setting to set to other template
                TemplateSetting objTemplateSetting = new TemplateSetting();

                objTemplateSetting.blnWantCheckPin1 = objFile.GetValueAsBoolean("WantCheckPin1", false, 2);
                objTemplateSetting.fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f, 2);

                //2020-05-08 ZJYEOH : reset back to current template because finish set to tolerance setting
                objFile.GetSecondSection("Pin1Template" + j); 

                objTemplateSetting.fOffsetRefPosX = objFile.GetValueAsFloat("OffsetRefPosX", 0, 2);
                objTemplateSetting.fOffsetRefPosY = objFile.GetValueAsFloat("OffsetRefPosY", 0, 2);

                objTemplateSetting.blnTemplateOrientationIsLeft = objFile.GetValueAsBoolean("TemplateOrientationIsLeft", false, 2);
                objTemplateSetting.blnTemplateOrientationIsTop = objFile.GetValueAsBoolean("TemplateOrientationIsTop", false, 2);

                objTemplateSetting.intOffsetFromSearchROIX = objFile.GetValueAsInt("OffsetFromSearchROIX", 0, 2);
                objTemplateSetting.intOffsetFromSearchROIY = objFile.GetValueAsInt("OffsetFromSearchROIY", 0, 2);

                if (objTemplateSetting.objMatcher == null)
                    objTemplateSetting.objMatcher = new EMatcher();

                if (System.IO.File.Exists(strFolderPath + "Pin1Template" + j.ToString() + ".mch"))
                    objTemplateSetting.objMatcher.Load(strFolderPath + "Pin1Template" + j.ToString() + ".mch");

                m_arrTemplateSetting.Add(objTemplateSetting);
            }

            objFile.GetSecondSection("SearchROI");
            if (m_objSearchROI == null)
                m_objSearchROI = new ROI();
            m_objSearchROI.LoadROISetting(
                objFile.GetValueAsInt("SearchROIStartX", 0, 2),
                objFile.GetValueAsInt("SearchROIStartY", 0, 2),
                objFile.GetValueAsInt("SearchROIWidth", 100, 2),
                objFile.GetValueAsInt("SearchROIHeight", 100, 2));

            objFile.GetSecondSection("Pin1ROI");
            if (m_objPin1ROI == null)
                m_objPin1ROI = new ROI();
            m_objPin1ROI.LoadROISetting(
                objFile.GetValueAsInt("Pin1ROIStartX", 0, 2),
                objFile.GetValueAsInt("Pin1ROIStartY", 0, 2),
                objFile.GetValueAsInt("Pin1ROIWidth", 100, 2),
                objFile.GetValueAsInt("Pin1ROIHeight", 100, 2));

        }

        public void LoadTemplate(string strFolderPath)
        {
            try
            {
                XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
                objFile.GetFirstSection("Pin1Settings");

                int intTemplateNum = objFile.GetValueAsInt("Pin1TemplateNum", 0);
                m_arrTemplateSetting.Clear();

                for (int j = 0; j < intTemplateNum; j++)
                {
                    objFile.GetSecondSection("Pin1Template" + j);
                    TemplateSetting objTemplateSetting = new TemplateSetting();

                    objTemplateSetting.blnWantCheckPin1 = objFile.GetValueAsBoolean("WantCheckPin1", false, 2);

                    objTemplateSetting.fOffsetRefPosX = objFile.GetValueAsFloat("OffsetRefPosX", 0, 2);
                    objTemplateSetting.fOffsetRefPosY = objFile.GetValueAsFloat("OffsetRefPosY", 0, 2);
                    objTemplateSetting.fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f, 2);

                    objTemplateSetting.blnTemplateOrientationIsLeft = objFile.GetValueAsBoolean("TemplateOrientationIsLeft", false, 2);
                    objTemplateSetting.blnTemplateOrientationIsTop = objFile.GetValueAsBoolean("TemplateOrientationIsTop", false, 2);

                    objTemplateSetting.intOffsetFromSearchROIX = objFile.GetValueAsInt("OffsetFromSearchROIX", 0, 2);
                    objTemplateSetting.intOffsetFromSearchROIY = objFile.GetValueAsInt("OffsetFromSearchROIY", 0, 2);

                    if (objTemplateSetting.objMatcher == null)
                        objTemplateSetting.objMatcher = new EMatcher();

                    if (System.IO.File.Exists(strFolderPath + "Pin1Template" + j.ToString() + ".mch"))
                        objTemplateSetting.objMatcher.Load(strFolderPath + "Pin1Template" + j.ToString() + ".mch");

                    m_arrTemplateSetting.Add(objTemplateSetting);
                }

                objFile.GetSecondSection("SearchROI");
                if (m_objSearchROI == null)
                    m_objSearchROI = new ROI();
                m_objSearchROI.LoadROISetting(
                    objFile.GetValueAsInt("SearchROIStartX", 0, 2),
                    objFile.GetValueAsInt("SearchROIStartY", 0, 2),
                    objFile.GetValueAsInt("SearchROIWidth", 100, 2),
                    objFile.GetValueAsInt("SearchROIHeight", 100, 2));

                objFile.GetSecondSection("Pin1ROI");
                if (m_objPin1ROI == null)
                    m_objPin1ROI = new ROI();
                m_objPin1ROI.LoadROISetting(
                    objFile.GetValueAsInt("Pin1ROIStartX", 0, 2),
                    objFile.GetValueAsInt("Pin1ROIStartY", 0, 2),
                    objFile.GetValueAsInt("Pin1ROIWidth", 100, 2),
                    objFile.GetValueAsInt("Pin1ROIHeight", 100, 2));

            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("Pin1 > LoadTemplate > Exception = " + ex.ToString());
            }
        }

        public bool MatchWithTemplate(ROI objSampleROI, int intSelectedTemplateIndex)
        {
            EMatcher objMatcher = m_arrTemplateSetting[intSelectedTemplateIndex].objMatcher;

            // 2021 06 02 - CCENG: future need to add tolerance to pin 1 if pin 1 is special mark and not attach to marking.
            //objMatcher.MinAngle = -10;
            //objMatcher.MaxAngle = 10;
            if (objMatcher.MinAngle != 0)
                objMatcher.MinAngle = 0;
            if (objMatcher.MaxAngle != 0)
                objMatcher.MaxAngle = 0;

            objMatcher.Match(objSampleROI.ref_ROI);

            if (objMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                if (objMatcher.GetPosition(0).Score < 0)
                    m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore = 0;
                else
                    m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore = objMatcher.GetPosition(0).Score;

                m_arrTemplateSetting[intSelectedTemplateIndex].fResultPosX = objMatcher.GetPosition(0).CenterX;
                m_arrTemplateSetting[intSelectedTemplateIndex].fResultPosY = objMatcher.GetPosition(0).CenterY;
                m_arrTemplateSetting[intSelectedTemplateIndex].intDrawParentROIOriX = objSampleROI.ref_ROITotalX;
                m_arrTemplateSetting[intSelectedTemplateIndex].intDrawParentROIOriY = objSampleROI.ref_ROITotalY;

                if (m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore >= m_arrTemplateSetting[intSelectedTemplateIndex].fMinScore)
                {
                    return true;
                }
            }

            m_strErrorMessage = "Fail Pin 1 - Not fulfill Min Setting : Set = " + (m_arrTemplateSetting[intSelectedTemplateIndex].fMinScore * 100).ToString() + " Score = " + (m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore * 100).ToString("f2");
            return false;

        }

        public bool MatchWithTemplate(ROI objSampleROI, int intSelectedTemplateIndex, float fScore)
        {
            return MatchWithTemplate(objSampleROI, intSelectedTemplateIndex, fScore, 0, 0);
        }

        public bool MatchWithTemplate(ROI objSampleROI, int intSelectedTemplateIndex, float fScore, int intMinAngle, int intMaxAngle)
        {
            EMatcher objMatcher = m_arrTemplateSetting[intSelectedTemplateIndex].objMatcher;
            if (objMatcher.MaxPositions != 5)
                objMatcher.MaxPositions = 5; //2021-03-14 ZJYEOH : Set to 5 so that pattern can be found more accurate

            objMatcher.MinAngle = intMinAngle;
            objMatcher.MaxAngle = intMaxAngle;

            //objMatcher.Save("D:\\TS\\Pin1Matcher.mch");
            //objSampleROI.ref_ROI.Save("D:\\TS\\Pin1SampleROI.bmp");

            objMatcher.Match(objSampleROI.ref_ROI);

            if (objMatcher.NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                if (Math.Max(objMatcher.GetPosition(0).Score, 0) >= fScore)
                {
                    if (objMatcher.GetPosition(0).Score < 0)
                        m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore = 0;
                    else
                        m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore = objMatcher.GetPosition(0).Score;

                    m_arrTemplateSetting[intSelectedTemplateIndex].fResultPosX = objMatcher.GetPosition(0).CenterX;
                    m_arrTemplateSetting[intSelectedTemplateIndex].fResultPosY = objMatcher.GetPosition(0).CenterY;
                    m_arrTemplateSetting[intSelectedTemplateIndex].intDrawParentROIOriX = objSampleROI.ref_ROITotalX;
                    m_arrTemplateSetting[intSelectedTemplateIndex].intDrawParentROIOriY = objSampleROI.ref_ROITotalY;
                }
                if (Math.Max(objMatcher.GetPosition(0).Score, 0) >= m_arrTemplateSetting[intSelectedTemplateIndex].fMinScore)
                {
                    return true;
                }
            }

            m_strErrorMessage = "Fail Pin 1 - Not fulfill Min Setting : Set = " + (m_arrTemplateSetting[intSelectedTemplateIndex].fMinScore * 100).ToString() + " Score = " + (m_arrTemplateSetting[intSelectedTemplateIndex].fResultScore * 100).ToString("f2");
            return false;

        }

        /// <summary>
        /// Reset previous inspection result
        /// </summary>
        public void ResetInspectionData()
        {
            lock (m_objLock)
            {
                m_intFinalResultSelectedTemplate = -1;
                m_blnFinalResultPassFail = false;

                for(int i = 0; i < m_arrTemplateSetting.Count; i ++)
                m_arrTemplateSetting[i].fResultScore = -1;
            }
        }

        public void SavePin1Setting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
            objFile.WriteSectionElement("Pin1Settings");
            int intTemplateNum = m_arrTemplateSetting.Count;

            objFile.WriteElement1Value("Pin1TemplateNum", intTemplateNum);

            for (int j = 0; j < intTemplateNum; j++)
            {
                objFile.WriteElement1Value("Pin1Template" + j, "");
                objFile.WriteElement2Value("WantCheckPin1", m_arrTemplateSetting[j].blnWantCheckPin1);
                objFile.WriteElement2Value("OffsetRefPosX", m_arrTemplateSetting[j].fOffsetRefPosX);
                objFile.WriteElement2Value("OffsetRefPosY", m_arrTemplateSetting[j].fOffsetRefPosY);
                objFile.WriteElement2Value("MinScore", m_arrTemplateSetting[j].fMinScore);

                objFile.WriteElement2Value("TemplateOrientationIsLeft", m_arrTemplateSetting[j].blnTemplateOrientationIsLeft);
                objFile.WriteElement2Value("TemplateOrientationIsTop", m_arrTemplateSetting[j].blnTemplateOrientationIsTop);

                objFile.WriteElement2Value("OffsetFromSearchROIX", m_arrTemplateSetting[j].intOffsetFromSearchROIX);
                objFile.WriteElement2Value("OffsetFromSearchROIY", m_arrTemplateSetting[j].intOffsetFromSearchROIY);
            }
            objFile.WriteEndElement();
        }

        public void SaveTemplate(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
            objFile.WriteSectionElement("Pin1Settings");

            int intTemplateNum = m_arrTemplateSetting.Count;

            objFile.WriteElement1Value("Pin1TemplateNum", intTemplateNum);

            for (int j = 0; j < intTemplateNum; j++)
            {
                objFile.WriteElement1Value("Pin1Template" + j, "");
                objFile.WriteElement2Value("WantCheckPin1", m_arrTemplateSetting[j].blnWantCheckPin1);
                objFile.WriteElement2Value("OffsetRefPosX", m_arrTemplateSetting[j].fOffsetRefPosX);
                objFile.WriteElement2Value("OffsetRefPosY", m_arrTemplateSetting[j].fOffsetRefPosY);

                objFile.WriteElement2Value("TemplateOrientationIsLeft", m_arrTemplateSetting[j].blnTemplateOrientationIsLeft);
                objFile.WriteElement2Value("TemplateOrientationIsTop", m_arrTemplateSetting[j].blnTemplateOrientationIsTop);

                objFile.WriteElement2Value("OffsetFromSearchROIX", m_arrTemplateSetting[j].intOffsetFromSearchROIX);
                objFile.WriteElement2Value("OffsetFromSearchROIY", m_arrTemplateSetting[j].intOffsetFromSearchROIY);
                
                objFile.WriteElement2Value("MinScore", m_arrTemplateSetting[j].fMinScore);
                m_arrTemplateSetting[j].objMatcher.Save(strFolderPath + "Pin1Template" + j.ToString() + ".mch");
                if (m_objPin1ROI.ref_ROI != null)
                {
                    if (m_objPin1ROI.ref_ROI.TopParent != null)
                        m_objPin1ROI.SaveImage(strFolderPath + "Pin1Template" + j.ToString() + ".bmp");
                }
            }

            objFile.WriteElement1Value("SearchROI", "");
            objFile.WriteElement2Value("SearchROIStartX", m_objSearchROI.ref_ROIPositionX);
            objFile.WriteElement2Value("SearchROIStartY", m_objSearchROI.ref_ROIPositionY);
            objFile.WriteElement2Value("SearchROIWidth", m_objSearchROI.ref_ROIWidth);
            objFile.WriteElement2Value("SearchROIHeight", m_objSearchROI.ref_ROIHeight);

            if (m_objPin1ROI.ref_ROI != null)
            {
                objFile.WriteElement1Value("Pin1ROI", "");
                objFile.WriteElement2Value("Pin1ROIStartX", m_objPin1ROI.ref_ROIPositionX);
                objFile.WriteElement2Value("Pin1ROIStartY", m_objPin1ROI.ref_ROIPositionY);
                objFile.WriteElement2Value("Pin1ROIWidth", m_objPin1ROI.ref_ROIWidth);
                objFile.WriteElement2Value("Pin1ROIHeight", m_objPin1ROI.ref_ROIHeight);
            }

            objFile.WriteEndElement();
        }
        public void SaveTemplate(string strFolderPath, int intSelectedTemplate)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
            objFile.WriteSectionElement("Pin1Settings");

            int intTemplateNum = m_arrTemplateSetting.Count;

            objFile.WriteElement1Value("Pin1TemplateNum", intTemplateNum);

            //for (int j = 0; j < intTemplateNum; j++)
            //{
                objFile.WriteElement1Value("Pin1Template" + intSelectedTemplate, "");
                objFile.WriteElement2Value("WantCheckPin1", m_arrTemplateSetting[intSelectedTemplate].blnWantCheckPin1);
                objFile.WriteElement2Value("OffsetRefPosX", m_arrTemplateSetting[intSelectedTemplate].fOffsetRefPosX);
                objFile.WriteElement2Value("OffsetRefPosY", m_arrTemplateSetting[intSelectedTemplate].fOffsetRefPosY);

            objFile.WriteElement2Value("TemplateOrientationIsLeft", m_arrTemplateSetting[intSelectedTemplate].blnTemplateOrientationIsLeft);
            objFile.WriteElement2Value("TemplateOrientationIsTop", m_arrTemplateSetting[intSelectedTemplate].blnTemplateOrientationIsTop);

            objFile.WriteElement2Value("OffsetFromSearchROIX", m_arrTemplateSetting[intSelectedTemplate].intOffsetFromSearchROIX);
            objFile.WriteElement2Value("OffsetFromSearchROIY", m_arrTemplateSetting[intSelectedTemplate].intOffsetFromSearchROIY);

            objFile.WriteElement2Value("MinScore", m_arrTemplateSetting[intSelectedTemplate].fMinScore);
                m_arrTemplateSetting[intSelectedTemplate].objMatcher.Save(strFolderPath + "Pin1Template" + intSelectedTemplate.ToString() + ".mch");
                if (m_objPin1ROI.ref_ROI != null)
                {
                    if (m_objPin1ROI.ref_ROI.TopParent != null)
                        m_objPin1ROI.SaveImage(strFolderPath + "Pin1Template" + intSelectedTemplate.ToString() + ".bmp");
                }
            //}

            objFile.WriteElement1Value("SearchROI", "");
            objFile.WriteElement2Value("SearchROIStartX", m_objSearchROI.ref_ROIPositionX);
            objFile.WriteElement2Value("SearchROIStartY", m_objSearchROI.ref_ROIPositionY);
            objFile.WriteElement2Value("SearchROIWidth", m_objSearchROI.ref_ROIWidth);
            objFile.WriteElement2Value("SearchROIHeight", m_objSearchROI.ref_ROIHeight);

            if (m_objPin1ROI.ref_ROI != null)
            {
                objFile.WriteElement1Value("Pin1ROI", "");
                objFile.WriteElement2Value("Pin1ROIStartX", m_objPin1ROI.ref_ROIPositionX);
                objFile.WriteElement2Value("Pin1ROIStartY", m_objPin1ROI.ref_ROIPositionY);
                objFile.WriteElement2Value("Pin1ROIWidth", m_objPin1ROI.ref_ROIWidth);
                objFile.WriteElement2Value("Pin1ROIHeight", m_objPin1ROI.ref_ROIHeight);
            }

            objFile.WriteEndElement();
        }
        public void SaveTemplate_SECSGEM(string strPath, string strSectionName, string strVisionName, bool blnSECSGEMFileExist)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            //XmlParser objFile = new XmlParser(strFolderPath + "Pin1Template.xml");
            //objFile.WriteSectionElement("Pin1Settings");

            int intTemplateNum = m_arrTemplateSetting.Count;

            objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1TemplateNum", intTemplateNum);

            //max template no is 8
            for (int j = 0; j < 8; j++)
            {
                if (m_arrTemplateSetting.Count > j)
                {
                    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_WantCheckPin1", m_arrTemplateSetting[j].blnWantCheckPin1);
                    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_OffsetRefPosX", m_arrTemplateSetting[j].fOffsetRefPosX);
                    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_OffsetRefPosY", m_arrTemplateSetting[j].fOffsetRefPosY);
                    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_MinScore", m_arrTemplateSetting[j].fMinScore);
                }
                else
                {
                    if (!blnSECSGEMFileExist)
                    {
                        objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_WantCheckPin1", "NA");
                        objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_OffsetRefPosX", "NA");
                        objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_OffsetRefPosY", "NA");
                        objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1Template" + j + "_MinScore", "NA");
                    }
                }

            }

            ////objFile.WriteElementValue("SearchROI", "");
            //objFile.WriteElementValue(strVisionName + strSectionName + "_SearchROIStartX", m_objSearchROI.ref_ROIPositionX);
            //objFile.WriteElementValue(strVisionName + strSectionName + "_SearchROIStartY", m_objSearchROI.ref_ROIPositionY);
            //objFile.WriteElementValue(strVisionName + strSectionName + "_SearchROIWidth", m_objSearchROI.ref_ROIWidth);
            //objFile.WriteElementValue(strVisionName + strSectionName + "_SearchROIHeight", m_objSearchROI.ref_ROIHeight);

            //if (m_objPin1ROI.ref_ROI != null)
            //{
            //    //objFile.WriteElementValue("Pin1ROI", "");
            //    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1ROIStartX", m_objPin1ROI.ref_ROIPositionX);
            //    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1ROIStartY", m_objPin1ROI.ref_ROIPositionY);
            //    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1ROIWidth", m_objPin1ROI.ref_ROIWidth);
            //    objFile.WriteElementValue(strVisionName + strSectionName + "_Pin1ROIHeight", m_objPin1ROI.ref_ROIHeight);
            //}

            objFile.WriteEndElement();
        }
        public void SavePin1ToleranceToFile(string strPath, bool blnNewFile, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement("Pin1Settings", blnNewSection);
            
            int intTemplateNum = m_arrTemplateSetting.Count;

            objFile.WriteElement1Value("Pin1TemplateNum", intTemplateNum);

            for (int j = 0; j < intTemplateNum; j++)
            {
                objFile.WriteElement1Value("Pin1Template" + j, "");
                objFile.WriteElement2Value("MinScore", m_arrTemplateSetting[j].fMinScore);
            }
            
            objFile.WriteEndElement();
        }
        public void LoadPin1ToleranceFromFile(string strFilePath)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            objFile.GetFirstSection("Pin1Settings");

            int intTemplateNum = objFile.GetValueAsInt("Pin1TemplateNum", 0, 1);

            for (int j = 0; j < intTemplateNum; j++)
            {
                if (j > m_arrTemplateSetting.Count)
                    break;

                objFile.GetSecondSection("Pin1Template" + j.ToString());
                m_arrTemplateSetting[j].fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f, 2);
            }
        }
        public void SetMinScoreSetting(int intSelectedTemplateIndex, float fMinScore)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return;

            m_arrTemplateSetting[intSelectedTemplateIndex].fMinScore = fMinScore;
        }

        public void setWantCheckPin1(int intSelectedTemplateIndex, bool blnValue, bool blnSetToAllTemplate)
        {
            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return;

            if (blnSetToAllTemplate)
            {
                for (int i = 0; i < m_arrTemplateSetting.Count; i++)
                    m_arrTemplateSetting[i].blnWantCheckPin1 = blnValue;
            }
            else
                m_arrTemplateSetting[intSelectedTemplateIndex].blnWantCheckPin1 = blnValue;
        }

        public bool getWantCheckPin1(int intSelectedTemplateIndex)
        {
            if (intSelectedTemplateIndex < 0)
                return false;

            if (intSelectedTemplateIndex >= m_arrTemplateSetting.Count)
                return false;

            return m_arrTemplateSetting[intSelectedTemplateIndex].blnWantCheckPin1;
        }

        public void Dispose()
        {
            if (m_objSearchROI != null)
                m_objSearchROI.Dispose();

            if (m_objPin1ROI != null)
                m_objPin1ROI.Dispose();

            if (m_objTestROI != null)
                m_objTestROI.Dispose();

            penLime.Dispose();
            penRed.Dispose();

            for (int i = 0; i < m_arrTemplateSetting.Count; i++)
            {
                if (m_arrTemplateSetting[i].objMatcher != null)
                    m_arrTemplateSetting[i].objMatcher.Dispose();
            }
        }
    }
}
