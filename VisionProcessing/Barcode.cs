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
using System.IO;
using System.Runtime.InteropServices;

namespace VisionProcessing
{
    public class Barcode
    {
        #region DllImport

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        #endregion
        #region Member Variables
        private float m_fBarcodeOrientationAngle = 0;

        private EBlobs m_objEblob = new EBlobs();
        private float m_fDontCareScale = 1f;
        private int m_intPatternTemplateCenterX = 0;
        private int m_intPatternTemplateCenterY = 0;
        private Color[] m_Color = new Color[]{Color.MediumSpringGreen, Color.Yellow, Color.Cyan, Color.Orange, Color.Magenta, Color.DarkKhaki, Color.Pink, Color.YellowGreen, Color.Plum, Color.Gold};
        EWorldShape m_objWorldShape;
        private int m_intTemplateCount = 0;
        private int m_intCodeType = 0;//0:Barcode, 1: QR Code, 2: Matrix Code
        private IntPtr[] m_arrPen = new IntPtr[10] { CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.MediumSpringGreen)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Yellow)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Cyan))
        , CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Orange)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Magenta)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.DarkKhaki)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Pink))
        , CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.YellowGreen)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Plum)), CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Gold))};
        private IntPtr m_objLimePen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Lime));
        private IntPtr m_objRedPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Red));
        private IntPtr m_objBluePen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
        private bool[] m_blnCodeNotMatched = new bool[10];
        private bool m_blnTestDone = false;
        private bool[] m_blnTested = new bool[10];
        private EMatcher m_objMatcher = new EMatcher();
        private bool[] m_blnCodePassed = new bool[10];
        private object m_objLock = new object();
        private string[] m_strTemplateCode = new string[10];
        private string[] m_strResultCode = new string[10];
        private string m_strErrorMessage = "";
        private ERGBColor m_EROIRedColor = new ERGBColor(255, 0, 0);
        private float m_fCodeOrgX = 0;
        private float m_fCodeOrgY = 0;
        private float m_fCodeSizeX = 0;
        private float m_fCodeSizeY = 0;
        private bool[] m_blnCodeFound = new bool[10];
        private EBarCode[] m_objBarcode = new EBarCode[10] { new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode(), new EBarCode()};
        //private EQRCodeReader m_objQRCode = new EQRCodeReader();
        //private EQRCode[] m_objQRCodeResult;
#if (Debug_2_12 || Release_2_12)
        private Euresys.Open_eVision_2_12.EasyMatrixCode2.EMatrixCodeReader m_objMatrixCode = new Euresys.Open_eVision_2_12.EasyMatrixCode2.EMatrixCodeReader();
        private Euresys.Open_eVision_2_12.EasyMatrixCode2.EMatrixCode[] m_objMatrixCodeResult;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        private EMatrixCodeReader m_objMatrixCode = new EMatrixCodeReader();
        private EMatrixCode[] m_objMatrixCodeResult;
#endif


        private float[] m_fPatternReferenceOffsetX = new float[10];
        private float[] m_fPatternReferenceOffsetY = new float[10];
        private int m_intMinMatchingScore = 50;
        private int m_intPatternAngleRangeTolerance = 50;
        private float m_floatBarcodeAngleRangeTolerance = 0;
        private float m_fGainRangeTolerance = 0;
        private float m_fMatcherCenterX = 0;
        private float m_fMatcherCenterY = 0;
        private float[] m_fBarcodeAngle = new float[10];
        private float m_fPatternAngle = 0;

        private bool m_blnWantUseAngleRange = false;
        private bool m_blnWantUseGainRange = false;

        private bool m_blnWantUseReferenceImage = false;
        private bool m_blnWantUseUniformize3x3 = false;

        private float[] m_fTemplateBarcodeWidth = new float[10];
        private float[] m_fTemplateBarcodeHeight = new float[10];
        private float[] m_fTemplateBarcodeAngle = new float[10];
        private float[] m_fTemplateBarcodeCenterX = new float[10];
        private float[] m_fTemplateBarcodeCenterY = new float[10];

        private int m_intBarcodeDetectionAreaTolerance = 250;

        private int m_intPatternDetectionAreaTolerance_Top = 20;
        private int m_intPatternDetectionAreaTolerance_Right = 20;
        private int m_intPatternDetectionAreaTolerance_Bottom = 20;
        private int m_intPatternDetectionAreaTolerance_Left = 20;

        private int m_intRetestCount = 0;
        private int m_intDelayTimeAfterPass = 1000;

        private int m_intUniformizeGain = 1;
        private float m_fImageGain = 0.5f;
        #endregion


        #region Properties
        public float ref_fBarcodeOrientationAngle { get { return m_fBarcodeOrientationAngle; } set { m_fBarcodeOrientationAngle = value; } }
        public float ref_fDontCareScale { get { return m_fDontCareScale; } set { m_fDontCareScale = value; } }
        public int ref_intPatternTemplateCenterX { get { return m_intPatternTemplateCenterX; } set { m_intPatternTemplateCenterX = value; } }
        public int ref_intPatternTemplateCenterY { get { return m_intPatternTemplateCenterY; } set { m_intPatternTemplateCenterY = value; } }
        public Color[] ref_Color { get { return m_Color; } set { m_Color = value; } }
        public int ref_intTemplateCount { get { return m_intTemplateCount; } set { m_intTemplateCount = value; } }
        public int ref_intCodeType { get { return m_intCodeType; } set { m_intCodeType = value; } }
        public int ref_intUniformizeGain { get { return m_intUniformizeGain; } set { m_intUniformizeGain = value; } }
        public float ref_fImageGain { get { return m_fImageGain; } set { m_fImageGain = value; } }
        public int ref_intDelayTimeAfterPass { get { return m_intDelayTimeAfterPass; } set { m_intDelayTimeAfterPass = value; } }
        public int ref_intRetestCount { get { return m_intRetestCount; } set { m_intRetestCount = value; } }
        public int ref_intBarcodeDetectionAreaTolerance { get { return m_intBarcodeDetectionAreaTolerance; } set { m_intBarcodeDetectionAreaTolerance = value; } }
        public int ref_intPatternDetectionAreaTolerance_Top { get { return m_intPatternDetectionAreaTolerance_Top; } set { m_intPatternDetectionAreaTolerance_Top = value; } }
        public int ref_intPatternDetectionAreaTolerance_Right { get { return m_intPatternDetectionAreaTolerance_Right; } set { m_intPatternDetectionAreaTolerance_Right = value; } }
        public int ref_intPatternDetectionAreaTolerance_Bottom { get { return m_intPatternDetectionAreaTolerance_Bottom; } set { m_intPatternDetectionAreaTolerance_Bottom = value; } }
        public int ref_intPatternDetectionAreaTolerance_Left { get { return m_intPatternDetectionAreaTolerance_Left; } set { m_intPatternDetectionAreaTolerance_Left = value; } }
        public bool[] ref_blnCodeNotMatched { get { return m_blnCodeNotMatched; } set { m_blnCodeNotMatched = value; } }
        public bool ref_blnTestDone { get { return m_blnTestDone; } set { m_blnTestDone = value; } }
        public float[] ref_fTemplateBarcodeCenterX { get { return m_fTemplateBarcodeCenterX; } set { m_fTemplateBarcodeCenterX = value; } }
        public float[] ref_fTemplateBarcodeCenterY { get { return m_fTemplateBarcodeCenterY; } set { m_fTemplateBarcodeCenterY = value; } }
        public float[] ref_fTemplateBarcodeWidth { get { return m_fTemplateBarcodeWidth; } set { m_fTemplateBarcodeWidth = value; } }
        public float[] ref_fTemplateBarcodeHeight { get { return m_fTemplateBarcodeHeight; } set { m_fTemplateBarcodeHeight = value; } }
        public float[] ref_fTemplateBarcodeAngle { get { return m_fTemplateBarcodeAngle; } set { m_fTemplateBarcodeAngle = value; } }
        public bool ref_blnWantUseAngleRange { get { return m_blnWantUseAngleRange; } set { m_blnWantUseAngleRange = value; } }
        public bool ref_blnWantUseGainRange { get { return m_blnWantUseGainRange; } set { m_blnWantUseGainRange = value; } }
        public bool ref_blnWantUseReferenceImage { get { return m_blnWantUseReferenceImage; } set { m_blnWantUseReferenceImage = value; } }
        public bool ref_blnWantUseUniformize3x3 { get { return m_blnWantUseUniformize3x3; } set { m_blnWantUseUniformize3x3 = value; } }
        public float ref_fGainRangeTolerance { get { return m_fGainRangeTolerance; } set { m_fGainRangeTolerance = value; } }
        public int ref_intPatternAngleRangeTolerance { get { return m_intPatternAngleRangeTolerance; } set { m_intPatternAngleRangeTolerance = value; } }
        public float ref_fBarcodeAngleRangeTolerance { get { return m_floatBarcodeAngleRangeTolerance; } set { m_floatBarcodeAngleRangeTolerance = value; } }
        public int ref_intMinMatchingScore { get { return m_intMinMatchingScore; } set { m_intMinMatchingScore = value; } }
        public float[] ref_fPatternReferenceOffsetX { get { return m_fPatternReferenceOffsetX; } set { m_fPatternReferenceOffsetX = value; } }
        public float[] ref_fPatternReferenceOffsetY { get { return m_fPatternReferenceOffsetY; } set { m_fPatternReferenceOffsetY = value; } }
        public float[] ref_fBarcodeAngle { get { return m_fBarcodeAngle; } set { m_fBarcodeAngle = value; } }
        public string[] ref_strTemplateCode { get { return m_strTemplateCode; } set { m_strTemplateCode = value; } }
        public string[] ref_strResultCode { get { return m_strResultCode; } set { m_strResultCode = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } set { m_strErrorMessage = value; } }
        public bool[] ref_blnCodeFound { get { return m_blnCodeFound; } set { m_blnCodeFound = value; } }
        public bool[] ref_blnCodePassed { get { return m_blnCodePassed; } set { m_blnCodePassed = value; } }
        public float ref_fOrgX { get { return m_fCodeOrgX; } }
        public float ref_fOrgY { get { return m_fCodeOrgY; } }
        public float ref_fSizeX { get { return m_fCodeSizeX; } }
        public float ref_fSizeY { get { return m_fCodeSizeY; } }
        #endregion

        public Barcode(EWorldShape objWorldShape)
        {
            m_objWorldShape = objWorldShape;
            //Barcode
            for (int i = 0; i < 10; i++)
            {
                m_objBarcode[i].StandardSymbologies = (int)ESymbologies.Standard;
                //m_objBarcode.AdditionalSymbologies = (int)ESymbologies.Additional;
                m_objBarcode[i].VerifyChecksum = true;
                m_objBarcode[i].Attach(objWorldShape);
            }

//#if (Debug_2_12 || Release_2_12)
//            m_objQRCode.TimeOut = 5000000;
//            m_objQRCode.DetectionTradeOff = EQRDetectionTradeOff.FavorReliability;
//#endif
            m_objMatrixCode.TimeOut = 5000000;
#if (Debug_2_12 || Release_2_12)
            m_objMatrixCode.ReadMode = Euresys.Open_eVision_2_12.EasyMatrixCode2.EReadMode.Quality;
#endif
        }
        public void ResetInspectionData_Inspection()
        {
            
            m_blnTestDone = false;
            for (int i = 0; i < 10; i++)
            {
                m_blnTested[i] = false;
                m_blnCodeNotMatched[i] = false;
                m_blnCodeFound[i] = false;
                m_blnCodePassed[i] = false;
                m_strResultCode[i] = "";
                m_fBarcodeAngle[i] = 0;
            }
            m_fPatternAngle = 0;
        }
        public void ResetInspectionData_Learn(int intTemplateIndex)
        {
            
            m_blnTestDone = false;
            if (intTemplateIndex < 10)
            {
                m_blnTested[intTemplateIndex] = false;
                m_blnCodeNotMatched[intTemplateIndex] = false;
                m_blnCodeFound[intTemplateIndex] = false;
                m_blnCodePassed[intTemplateIndex] = false;
                m_strResultCode[intTemplateIndex] = "";
                m_fBarcodeAngle[intTemplateIndex] = 0;
                m_fPatternReferenceOffsetX[intTemplateIndex] = 0;
                m_fPatternReferenceOffsetY[intTemplateIndex] = 0;
                m_strTemplateCode[intTemplateIndex] = "";
                m_fTemplateBarcodeWidth[intTemplateIndex] = 0;
                m_fTemplateBarcodeHeight[intTemplateIndex] = 0;
                m_fTemplateBarcodeCenterX[intTemplateIndex] = 0;
                m_fTemplateBarcodeCenterY[intTemplateIndex] = 0;
                m_fTemplateBarcodeAngle[intTemplateIndex] = 0;
            }
            m_fPatternAngle = 0;
        }
        public void ResetInspectionDataToPrevious_Learn(int intTemplateIndex)
        {
            
            //m_blnTestDone = false;
            if (intTemplateIndex == 9)
            {
                m_blnTested[intTemplateIndex] = false;
                m_blnCodeNotMatched[intTemplateIndex] = false;
                m_blnCodeFound[intTemplateIndex] = false;
                m_blnCodePassed[intTemplateIndex] = false;
                m_strResultCode[intTemplateIndex] = "";
                m_fBarcodeAngle[intTemplateIndex] = 0;
                m_fPatternReferenceOffsetX[intTemplateIndex] = 0;
                m_fPatternReferenceOffsetY[intTemplateIndex] = 0;
                m_strTemplateCode[intTemplateIndex] = "";
                m_fTemplateBarcodeWidth[intTemplateIndex] = 0;
                m_fTemplateBarcodeHeight[intTemplateIndex] = 0;
                m_fTemplateBarcodeCenterX[intTemplateIndex] = 0;
                m_fTemplateBarcodeCenterY[intTemplateIndex] = 0;
                m_fTemplateBarcodeAngle[intTemplateIndex] = 0;
                m_objBarcode[intTemplateIndex] = new EBarCode();
                m_objBarcode[intTemplateIndex].StandardSymbologies = (int)ESymbologies.Standard;
                m_objBarcode[intTemplateIndex].VerifyChecksum = true;
                m_objBarcode[intTemplateIndex].Attach(m_objWorldShape);
            }
            else
            {
                for (int i = intTemplateIndex; i < m_intTemplateCount; i++)
                {
                    if (i < 9)
                    {
                        m_blnTested[i] = m_blnTested[i + 1];
                        m_blnCodeNotMatched[i] = m_blnCodeNotMatched[i + 1];
                        m_blnCodeFound[i] = m_blnCodeFound[i + 1];
                        m_blnCodePassed[i] = m_blnCodePassed[i + 1];
                        m_strResultCode[i] = m_strResultCode[i + 1];
                        m_fBarcodeAngle[i] = m_fBarcodeAngle[i + 1];
                        m_objBarcode[i] = m_objBarcode[i + 1];
                        m_fPatternReferenceOffsetX[i] = m_fPatternReferenceOffsetX[i + 1];
                        m_fPatternReferenceOffsetY[i] = m_fPatternReferenceOffsetY[i + 1];
                        m_strTemplateCode[i] = m_strTemplateCode[i + 1];
                        m_fTemplateBarcodeWidth[i] = m_fTemplateBarcodeWidth[i + 1];
                        m_fTemplateBarcodeHeight[i] = m_fTemplateBarcodeHeight[i + 1];
                        m_fTemplateBarcodeCenterX[i] = m_fTemplateBarcodeCenterX[i + 1];
                        m_fTemplateBarcodeCenterY[i] = m_fTemplateBarcodeCenterY[i + 1];
                        m_fTemplateBarcodeAngle[i] = m_fTemplateBarcodeAngle[i + 1];
                        if (((i + 1) >= m_intTemplateCount) && i < 9)
                        {
                            m_objBarcode[i + 1] = new EBarCode();
                            m_objBarcode[i + 1].StandardSymbologies = (int)ESymbologies.Standard;
                            m_objBarcode[i + 1].VerifyChecksum = true;
                            m_objBarcode[i + 1].Attach(m_objWorldShape);
                        }
                    }
                    else
                    {
                        m_blnTested[i] = false;
                        m_blnCodeNotMatched[i] = false;
                        m_blnCodeFound[i] = false;
                        m_blnCodePassed[i] = false;
                        m_strResultCode[i] = "";
                        m_fBarcodeAngle[i] = 0;
                        m_fPatternReferenceOffsetX[i] = 0;
                        m_fPatternReferenceOffsetY[i] = 0;
                        m_strTemplateCode[i] = "";
                        m_fTemplateBarcodeWidth[i] = 0;
                        m_fTemplateBarcodeHeight[i] = 0;
                        m_fTemplateBarcodeCenterX[i] = 0;
                        m_fTemplateBarcodeCenterY[i] = 0;
                        m_fTemplateBarcodeAngle[i] = 0;
                        m_objBarcode[i] = new EBarCode();
                        m_objBarcode[i].StandardSymbologies = (int)ESymbologies.Standard;
                        m_objBarcode[i].VerifyChecksum = true;
                        m_objBarcode[i].Attach(m_objWorldShape);
                    }
                }
            }
            m_fPatternAngle = 0;
        }
        public void ResetBarcodeObject()
        {
            for (int i = 0; i < 10; i++)
            {
                m_objBarcode[i] = new EBarCode();
                m_objBarcode[i].StandardSymbologies = (int)ESymbologies.Standard;
                m_objBarcode[i].VerifyChecksum = true;
                m_objBarcode[i].Attach(m_objWorldShape);
            }
        }

        public void SetBarcodePosition(float CenterX, float CenterY, float Width, float Height, int intIndex, float fAngle)
        {
            m_objBarcode[intIndex].KnownLocation = true;
            m_objBarcode[intIndex].SetCenterXY(CenterX, CenterY);
            m_objBarcode[intIndex].SetSize(Width, Height);
            m_objBarcode[intIndex].SetReadingCenter(0, 0);
            m_objBarcode[intIndex].SetReadingSize(1.1f, 0.8f);
            m_objBarcode[intIndex].Angle = -fAngle;
        }

        public int ReadBarcodeObjects(ROI objROI, bool blnLearn, int intTemplateIndex, bool blnPreTest, float fAngle, ImageDrawing objWhiteImage, ImageDrawing objBlackImage)
        {
            //objROI.SaveImage("D:\\objROI" + intTemplateIndex.ToString() + ".bmp");

            int intResultType = 0;
            int intInspectMode = 0; // 0: Normal, 1: With Gain only, 2: With Angle Only, 3: With Gain + Angle

            if (m_blnWantUseGainRange && m_fGainRangeTolerance != 0)
                intInspectMode |= 0x01;
            if (m_blnWantUseAngleRange && m_floatBarcodeAngleRangeTolerance != 0)
                intInspectMode |= 0x02;

            bool blnFailInTryCatch = false;
            ROI objGainROI = new ROI();
            ImageDrawing objImg = new ImageDrawing(true, objROI.ref_ROI.TotalWidth, objROI.ref_ROI.TotalHeight);
            objImg.SetImageToBlack();
            objGainROI.AttachImage(objImg);
            objGainROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

            ROI objBlobROI = new ROI();
            ImageDrawing objBlobImg = new ImageDrawing(true, objROI.ref_ROI.TotalWidth, objROI.ref_ROI.TotalHeight);
            objBlobImg.SetImageToBlack();
            objBlobROI.AttachImage(objBlobImg);
            objBlobROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

            objROI.CopyImage_Bigger(ref objGainROI);
            objROI.CopyImage_Bigger(ref objBlobROI);

            m_objEblob.BuildObjects_Filter_GetElement_BlobLimit(objROI, true, true, 0, 180,
                    0, objROI.ref_ROIWidth * objROI.ref_ROIHeight + 1, 0xFF);

            m_objEblob.DrawBlobOnImage(objBlobROI, objWhiteImage, objBlackImage);

            //objBlobROI.SaveImage("D:\\objBlobROI" + intTemplateIndex.ToString() + ".bmp");

            if (m_blnWantUseUniformize3x3)
            {
                EasyImage.ConvolUniform(objGainROI.ref_ROI, objGainROI.ref_ROI);
            }

            m_objBarcode[intTemplateIndex].KnownLocation = false;
            for (int i = 0; i < 4; i++)
            {
                if (i == 1)
                {
                    EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
                    //EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
                }
                else if (i == 2)
                {
                    EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
                }
                else if (i == 3)
                {
                    EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);
                }

                try
                {
                    m_objBarcode[intTemplateIndex].Read(objGainROI.ref_ROI);

                    if (m_objBarcode[intTemplateIndex].NumDecodedSymbologies != 0)
                    {
                        m_objBarcode[intTemplateIndex].GetDecodedAngle(out m_fBarcodeAngle[intTemplateIndex]);
                        m_strResultCode[intTemplateIndex] = m_objBarcode[intTemplateIndex].Decode(m_objBarcode[intTemplateIndex].GetDecodedSymbology(0));
                        m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                        m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                        m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                        m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                        m_blnCodeFound[intTemplateIndex] = true;

                        if (m_strResultCode[intTemplateIndex] != m_strTemplateCode[intTemplateIndex] && m_strTemplateCode[intTemplateIndex] != "" && !blnLearn)
                        {
                            m_blnCodeNotMatched[intTemplateIndex] = true;
                            m_blnCodePassed[intTemplateIndex] = false;
                            m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() +" Barcode result = " + m_strResultCode[intTemplateIndex] + " does not match with template " + m_strTemplateCode[intTemplateIndex];
                            intResultType = 1;
                        }
                        else
                        {
                            if (blnLearn || blnPreTest)
                            {
                                m_fTemplateBarcodeWidth[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeX;
                                m_fTemplateBarcodeHeight[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeY;
                                m_fTemplateBarcodeAngle[intTemplateIndex] = m_fBarcodeAngle[intTemplateIndex];
                                m_fTemplateBarcodeCenterX[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterX;
                                m_fTemplateBarcodeCenterY[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterY;
                            }
                            blnFailInTryCatch = false;
                            if (m_strErrorMessage != "")
                                m_strErrorMessage = "";
                            m_blnCodePassed[intTemplateIndex] = true;
                            intResultType = 0;
                        }
                    }
                    else
                    {
                        m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                        m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                        m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                        m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                        m_blnCodeFound[intTemplateIndex] = true;
                        m_blnCodePassed[intTemplateIndex] = false;
                        m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode pattern found but unable to decode.";

                        intResultType = 2;
                    }
                }
                catch (Exception ex)
                {
                    intResultType = 3;
                    blnFailInTryCatch = true;
                    m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " " + ex.Message.ToString();
                }

                if (intResultType < 2 && m_blnCodeFound[intTemplateIndex])
                    break;
            }

            if (intResultType > 1 && intInspectMode > 0)
            {
                switch (intInspectMode)
                {
                    case 1: // Gain Only
                        {
                            bool blnAddGainNegative = false;
                            for (float fGainValue = 0; fGainValue <= m_fGainRangeTolerance; fGainValue+=0.1f)
                            {
                            RetestGainOnly:
                                if (fGainValue == 0)
                                {
                                    fGainValue = 0.1f;
                                }
                                objBlobROI.CopyImage_Bigger(ref objGainROI);
                                if (fGainValue != 0)
                                {
                                    if (!blnAddGainNegative)
                                    {
                                        if (fGainValue < 1) // for less than 1 gain value, max allow is 0.1 only because 0 gain will result black image
                                            blnAddGainNegative = true;
                                        objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 + fGainValue);
                                    }
                                    else
                                    {
                                        blnAddGainNegative = false;
                                        objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 - fGainValue);
                                    }
                                }
                                //objGainROI.SaveImage("D:\\objGainROI.bmp");

                                if (m_blnWantUseUniformize3x3)
                                {
                                    EasyImage.ConvolUniform(objGainROI.ref_ROI, objGainROI.ref_ROI);
                                }

                                EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
                                EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
                                EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);

                                try
                                {
                                    m_objBarcode[intTemplateIndex].Read(objGainROI.ref_ROI);

                                    if (m_objBarcode[intTemplateIndex].NumDecodedSymbologies != 0)
                                    {
                                        m_objBarcode[intTemplateIndex].GetDecodedAngle(out m_fBarcodeAngle[intTemplateIndex]);
                                        m_strResultCode[intTemplateIndex] = m_objBarcode[intTemplateIndex].Decode(m_objBarcode[intTemplateIndex].GetDecodedSymbology(0));
                                        m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                                        m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                                        m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                                        m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                                        m_blnCodeFound[intTemplateIndex] = true;

                                        if (m_strResultCode[intTemplateIndex] != m_strTemplateCode[intTemplateIndex] && m_strTemplateCode[intTemplateIndex] != "" && !blnLearn)
                                        {
                                            m_blnCodeNotMatched[intTemplateIndex] = true;
                                            m_blnCodePassed[intTemplateIndex] = false;
                                            m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode result = " + m_strResultCode[intTemplateIndex] + " does not match with template " + m_strTemplateCode[intTemplateIndex];
                                            intResultType = 1;
                                        }
                                        else
                                        {
                                            if (blnLearn || blnPreTest)
                                            {
                                                m_fTemplateBarcodeWidth[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeX;
                                                m_fTemplateBarcodeHeight[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeY;
                                                m_fTemplateBarcodeAngle[intTemplateIndex] = m_fBarcodeAngle[intTemplateIndex];
                                                m_fTemplateBarcodeCenterX[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterX;
                                                m_fTemplateBarcodeCenterY[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterY;
                                            }
                                            blnFailInTryCatch = false;
                                            if (m_strErrorMessage != "")
                                                m_strErrorMessage = "";
                                            m_blnCodePassed[intTemplateIndex] = true;
                                            intResultType = 0;
                                        }
                                    }
                                    else
                                    {
                                        m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                                        m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                                        m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                                        m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                                        m_blnCodeFound[intTemplateIndex] = true;
                                        m_blnCodePassed[intTemplateIndex] = false;
                                        m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode pattern found but unable to decode.";

                                        intResultType = 2;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    blnFailInTryCatch = true;
                                    m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " " + ex.Message.ToString();
                                }

                                if (intResultType < 2)
                                    break;

                                if (blnAddGainNegative)
                                    goto RetestGainOnly;
                            }

                        }
                        break;
                    case 2: // Angle Only
                        {
                            float fOriAngle = 0;
                            bool blnAddAngleNegative = false;
                            for (float fAngleValue = 0; fAngleValue <= m_floatBarcodeAngleRangeTolerance; fAngleValue += 0.1f)
                            {
                            RetestAngleOnly:
                                if (fAngleValue == 0)
                                {
                                    fAngleValue = 0.1f;
                                }
                                if (fAngleValue == 0.1f && !blnAddAngleNegative)
                                {
                                    fOriAngle = m_objBarcode[intTemplateIndex].Angle;
                                    m_objBarcode[intTemplateIndex].KnownLocation = true;
                                    m_objBarcode[intTemplateIndex].SetCenterXY(m_objBarcode[intTemplateIndex].CenterX, m_objBarcode[intTemplateIndex].CenterY);
                                    m_objBarcode[intTemplateIndex].SetSize(m_objBarcode[intTemplateIndex].SizeX, m_objBarcode[intTemplateIndex].SizeY);
                                    m_objBarcode[intTemplateIndex].SetReadingCenter(0, 0);
                                    m_objBarcode[intTemplateIndex].SetReadingSize(1.1f, 0.8f);
                                }
                                if (!blnAddAngleNegative)
                                {
                                    blnAddAngleNegative = true;
                                    m_objBarcode[intTemplateIndex].Angle = fOriAngle + fAngleValue;
                                }
                                else
                                {
                                    blnAddAngleNegative = false;
                                    m_objBarcode[intTemplateIndex].Angle = fOriAngle - fAngleValue;
                                }
                                //objGainROI.SaveImage("D:\\objGainROI.bmp");
                                try
                                {
                                    m_objBarcode[intTemplateIndex].Detect(objGainROI.ref_ROI);

                                    if (m_objBarcode[intTemplateIndex].NumDecodedSymbologies != 0)
                                    {
                                        m_objBarcode[intTemplateIndex].GetDecodedAngle(out m_fBarcodeAngle[intTemplateIndex]);
                                        m_strResultCode[intTemplateIndex] = m_objBarcode[intTemplateIndex].Decode(m_objBarcode[intTemplateIndex].GetDecodedSymbology(0));
                                        m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                                        m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                                        m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                                        m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                                        m_blnCodeFound[intTemplateIndex] = true;

                                        if (m_strResultCode[intTemplateIndex] != m_strTemplateCode[intTemplateIndex] && m_strTemplateCode[intTemplateIndex] != "" && !blnLearn)
                                        {
                                            m_blnCodeNotMatched[intTemplateIndex] = true;
                                            m_blnCodePassed[intTemplateIndex] = false;
                                            m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode result = " + m_strResultCode[intTemplateIndex] + " does not match with template " + m_strTemplateCode[intTemplateIndex];
                                            intResultType = 1;
                                        }
                                        else
                                        {
                                            if (blnLearn || blnPreTest)
                                            {
                                                m_fTemplateBarcodeWidth[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeX;
                                                m_fTemplateBarcodeHeight[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeY;
                                                m_fTemplateBarcodeAngle[intTemplateIndex] = m_fBarcodeAngle[intTemplateIndex];
                                                m_fTemplateBarcodeCenterX[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterX;
                                                m_fTemplateBarcodeCenterY[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterY;
                                            }
                                            blnFailInTryCatch = false;
                                            if (m_strErrorMessage != "")
                                                m_strErrorMessage = "";
                                            m_blnCodePassed[intTemplateIndex] = true;
                                            intResultType = 0;
                                        }
                                    }
                                    else
                                    {
                                        m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                                        m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                                        m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                                        m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                                        m_blnCodeFound[intTemplateIndex] = true;
                                        m_blnCodePassed[intTemplateIndex] = false;
                                        m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode pattern found but unable to decode.";

                                        intResultType = 2;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    blnFailInTryCatch = true;
                                    m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " " + ex.Message.ToString();
                                }

                                if (intResultType < 2)
                                    break;

                                if (blnAddAngleNegative)
                                    goto RetestAngleOnly;
                            }

                        }
                        break;
                    case 3: // Gain + Angle
                        {
                            bool blnTestGainFirst = true;
                            bool blnAddGainNegative = false;
                            for (float fGainValue = 0; fGainValue <= m_fGainRangeTolerance; fGainValue += 0.1f)
                            {
                            RetestGainOnly:
                                m_objBarcode[intTemplateIndex].KnownLocation = false;
                                if (fGainValue == 0)
                                {
                                    fGainValue = 0.1f;
                                }
                                objBlobROI.CopyImage_Bigger(ref objGainROI);
                                if (fGainValue != 0)
                                {
                                    if (!blnAddGainNegative)
                                    {
                                        if (fGainValue < 1) // for less than 1 gain value, max allow is 0.1 only because 0 gain will result black image
                                            blnAddGainNegative = true;
                                        objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 + fGainValue);
                                    }
                                    else
                                    {
                                        blnAddGainNegative = false;
                                        objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 - fGainValue);
                                    }
                                }
                                blnTestGainFirst = true;
                                //objGainROI.SaveImage("D:\\objGainROI.bmp");

                                if (m_blnWantUseUniformize3x3)
                                {
                                    EasyImage.ConvolUniform(objGainROI.ref_ROI, objGainROI.ref_ROI);
                                }

                                EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
                                EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
                                EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);

                                float fOriAngle = 0;
                                bool blnAddAngleNegative = false;
                                for (float fAngleValue = 0; fAngleValue <= m_floatBarcodeAngleRangeTolerance; fAngleValue += 0.1f)
                                {
                                RetestAngleOnly:
                                    if (fAngleValue == 0 && !blnTestGainFirst)
                                    {
                                        fAngleValue = 0.1f;
                                    }
                                    if (fAngleValue == 0.1f && !blnAddAngleNegative && !blnTestGainFirst)
                                    {
                                        fOriAngle = m_objBarcode[intTemplateIndex].Angle;
                                        m_objBarcode[intTemplateIndex].KnownLocation = true;
                                        m_objBarcode[intTemplateIndex].SetCenterXY(m_objBarcode[intTemplateIndex].CenterX, m_objBarcode[intTemplateIndex].CenterY);
                                        m_objBarcode[intTemplateIndex].SetSize(m_objBarcode[intTemplateIndex].SizeX, m_objBarcode[intTemplateIndex].SizeY);
                                        m_objBarcode[intTemplateIndex].SetReadingCenter(0, 0);
                                        m_objBarcode[intTemplateIndex].SetReadingSize(1.1f, 0.8f);
                                    }
                                    if (!blnTestGainFirst)
                                    {
                                        if (!blnAddAngleNegative)
                                        {
                                            blnAddAngleNegative = true;
                                            m_objBarcode[intTemplateIndex].Angle = fOriAngle + fAngleValue;
                                        }
                                        else
                                        {
                                            blnAddAngleNegative = false;
                                            m_objBarcode[intTemplateIndex].Angle = fOriAngle - fAngleValue;
                                        }
                                    }
                                    //objGainROI.SaveImage("D:\\objGainROI2.bmp");
                                    try
                                    {
                                        m_objBarcode[intTemplateIndex].Detect(objGainROI.ref_ROI);

                                        if (m_objBarcode[intTemplateIndex].NumDecodedSymbologies != 0)
                                        {
                                            m_objBarcode[intTemplateIndex].GetDecodedAngle(out m_fBarcodeAngle[intTemplateIndex]);
                                            m_strResultCode[intTemplateIndex] = m_objBarcode[intTemplateIndex].Decode(m_objBarcode[intTemplateIndex].GetDecodedSymbology(0));
                                            m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                                            m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                                            m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                                            m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                                            m_blnCodeFound[intTemplateIndex] = true;

                                            if (m_strResultCode[intTemplateIndex] != m_strTemplateCode[intTemplateIndex] && m_strTemplateCode[intTemplateIndex] != "" && !blnLearn)
                                            {
                                                m_blnCodeNotMatched[intTemplateIndex] = true;
                                                m_blnCodePassed[intTemplateIndex] = false;
                                                m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode result = " + m_strResultCode[intTemplateIndex] + " does not match with template " + m_strTemplateCode[intTemplateIndex];
                                                intResultType = 1;
                                            }
                                            else
                                            {
                                                if (blnLearn || blnPreTest)
                                                {
                                                    m_fTemplateBarcodeWidth[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeX;
                                                    m_fTemplateBarcodeHeight[intTemplateIndex] = m_objBarcode[intTemplateIndex].SizeY;
                                                    m_fTemplateBarcodeAngle[intTemplateIndex] = m_fBarcodeAngle[intTemplateIndex];
                                                    m_fTemplateBarcodeCenterX[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterX;
                                                    m_fTemplateBarcodeCenterY[intTemplateIndex] = m_objBarcode[intTemplateIndex].CenterY;
                                                }
                                                blnFailInTryCatch = false;
                                                if (m_strErrorMessage != "")
                                                    m_strErrorMessage = "";
                                                m_blnCodePassed[intTemplateIndex] = true;
                                                intResultType = 0;
                                            }
                                        }
                                        else
                                        {
                                            m_fCodeSizeX = m_objBarcode[intTemplateIndex].SizeX;
                                            m_fCodeSizeY = m_objBarcode[intTemplateIndex].SizeY;
                                            m_fCodeOrgX = m_objBarcode[intTemplateIndex].CenterX - m_fCodeSizeX / 2;
                                            m_fCodeOrgY = m_objBarcode[intTemplateIndex].CenterY - m_fCodeSizeY / 2;
                                            m_blnCodeFound[intTemplateIndex] = true;
                                            m_blnCodePassed[intTemplateIndex] = false;
                                            m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " Barcode pattern found but unable to decode.";

                                            intResultType = 2;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        blnFailInTryCatch = true;
                                        m_strErrorMessage = GetCodeType() + " Template " + (intTemplateIndex + 1).ToString() + " " + ex.Message.ToString();
                                    }

                                    if (intResultType < 2)
                                        break;

                                    if (blnTestGainFirst)
                                    {
                                        blnTestGainFirst = false;
                                        goto RetestAngleOnly;
                                    }

                                    if (blnAddAngleNegative)
                                        goto RetestAngleOnly;
                                }

                                if (intResultType < 2)
                                    break;

                                if (blnAddGainNegative)
                                    goto RetestGainOnly;
                            }

                        }
                        break;
                }
            }

            if (blnFailInTryCatch && intResultType == 0)
            {
                m_blnCodeFound[intTemplateIndex] = true;
                intResultType = 3;
            }

            if (m_strErrorMessage.Contains("Could not locate"))
            {
                SetBarcodePosition(objROI.ref_ROITotalCenterX, objROI.ref_ROITotalCenterY, objROI.ref_ROIWidth, objROI.ref_ROIHeight, intTemplateIndex, fAngle);
            }
            else
                m_blnTested[intTemplateIndex] = true;
            objGainROI.Dispose();
            objBlobROI.Dispose();
            objImg.Dispose();
            objBlobImg.Dispose();
            m_blnTestDone = true;
            return intResultType;
        }
      
        public int ReadQRCodeObjects(ROI objROI, bool blnLearn)
        {
            int intResultType = 0;
            //            int intInspectMode = 0; // 0: Normal, 1: With Gain only

            //            if (m_blnWantUseGainRange && m_fGainRangeTolerance != 0)
            //                intInspectMode |= 0x01;

            //            bool blnFailInTryCatch = false;
            //            ROI objGainROI = new ROI();
            //            ImageDrawing objImg = new ImageDrawing(true, objROI.ref_ROI.TotalWidth, objROI.ref_ROI.TotalHeight);
            //            objImg.SetImageToBlack();
            //            objGainROI.AttachImage(objImg);
            //            objGainROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

            //            objROI.CopyImage_Bigger(ref objGainROI);
            //            for (int i = 0; i < 3; i++)
            //            {
            //                if (i == 1)
            //                {
            //                    EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
            //                    EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
            //                }
            //                else if (i == 2)
            //                {
            //                    EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);
            //                }

            //                try
            //                {
            //                    m_objQRCode.SearchField = objGainROI.ref_ROI;
            //                    m_objQRCodeResult = m_objQRCode.Read();

            //                    if (m_objQRCodeResult.Length > 0 && m_objQRCodeResult[0].IsDecodingReliable)
            //                    {
            //#if (Debug_2_12 || Release_2_12)
            //                        m_strResultCode = m_objQRCodeResult[0].GetDecodedString(EByteInterpretationMode.Auto);
            //#endif
            //                        m_blnCodeFound = true;

            //                        if (m_strResultCode != m_strTemplateCode && m_strTemplateCode != "" && !blnLearn)
            //                        {
            //                            m_blnCodeNotMatched = true;
            //                            m_blnCodePassed = false;
            //                            m_strErrorMessage = "QR Code result = " + m_strResultCode + " does not match with template " + m_strTemplateCode;
            //                            intResultType = 1;
            //                        }
            //                        else
            //                        {
            //                            if (blnLearn)
            //                            {
            //                                EPoint[] arrPoints = m_objQRCodeResult[0].Geometry.Position.Corners;//0:Top Right, 1: Top Left, 2:Bottom Left, 3:Bottom Right
            //                                float arrSideAngle1 = 0;
            //#if (Debug_2_12 || Release_2_12)
            //                                arrSideAngle1 = m_objQRCodeResult[0].Geometry.Position.GetSideAngle(0);
            //#endif
            //                                float Angle1 = arrSideAngle1;
            //                                if (arrSideAngle1 >= 0)
            //                                    Angle1 = arrSideAngle1 - 180;
            //                                else //if (arrSideAngle1 < 0)//<-90)
            //                                    Angle1 = arrSideAngle1 + 180;

            //                                float fCenterX = 0;
            //                                float fCenterY = 0;

            //                                for(int a = 0; a < arrPoints.Length; a++)
            //                                {
            //                                    fCenterX += arrPoints[a].X;
            //                                    fCenterY += arrPoints[a].Y;
            //                                }
            //                                fCenterX /= 4;
            //                                fCenterY /= 4;
            //                                fCenterX += objGainROI.ref_ROITotalX;
            //                                fCenterY += objGainROI.ref_ROITotalY;

            //                                float fXAfterRotated1 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) - 
            //                                                   ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));
            //                                float fXAfterRotated2 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[1].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
            //                                                   ((objGainROI.ref_ROITotalY + arrPoints[1].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));


            //                                float fYAfterRotated1 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
            //                                                    ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));
            //                                float fYAfterRotated2 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[2].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
            //                                                    ((objGainROI.ref_ROITotalY + arrPoints[2].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));


            //                                m_fTemplateBarcodeWidth = fXAfterRotated1 - fXAfterRotated2;
            //                                m_fTemplateBarcodeHeight = fYAfterRotated2 - fYAfterRotated1;
            //                                m_fTemplateBarcodeAngle = Angle1;
            //                                m_fTemplateBarcodeCenterX = fCenterX;
            //                                m_fTemplateBarcodeCenterY = fCenterY;
            //                            }
            //                            blnFailInTryCatch = false;
            //                            if (m_strErrorMessage != "")
            //                                m_strErrorMessage = "";
            //                            m_blnCodePassed = true;
            //                            intResultType = 0;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        m_blnCodeFound = true;
            //                        m_blnCodePassed = false;
            //                        m_strErrorMessage = "QR Code pattern found but unable to decode.";

            //                        intResultType = 2;
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    intResultType = 3;
            //                    blnFailInTryCatch = true;
            //                    m_strErrorMessage = ex.Message.ToString();
            //                }

            //                if (intResultType < 2 && m_blnCodeFound)
            //                    break;
            //            }

            //            if (intResultType > 1 && intInspectMode == 1)
            //            {
            //                bool blnAddGainNegative = false;
            //                for (float fGainValue = 0; fGainValue <= m_fGainRangeTolerance; fGainValue += 0.1f)
            //                {
            //                RetestGainOnly:
            //                    if (fGainValue == 0)
            //                    {
            //                        fGainValue = 0.1f;
            //                    }
            //                    objROI.CopyImage_Bigger(ref objGainROI);
            //                    if (fGainValue != 0)
            //                    {
            //                        if (!blnAddGainNegative)
            //                        {
            //                            if (fGainValue < 1) // for less than 1 gain value, max allow is 0.1 only because 0 gain will result black image
            //                                blnAddGainNegative = true;
            //                            objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 + fGainValue);
            //                        }
            //                        else
            //                        {
            //                            blnAddGainNegative = false;
            //                            objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 - fGainValue);
            //                        }
            //                    }
            //                    //objGainROI.SaveImage("D:\\objGainROI.bmp");
            //                    EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
            //                    EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
            //                    EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);

            //                    try
            //                    {
            //                        m_objQRCode.SearchField = objGainROI.ref_ROI;
            //                        m_objQRCodeResult = m_objQRCode.Read();

            //                        if (m_objQRCodeResult.Length > 0 && m_objQRCodeResult[0].IsDecodingReliable)
            //                        {
            //#if (Debug_2_12 || Release_2_12)
            //                        m_strResultCode = m_objQRCodeResult[0].GetDecodedString(EByteInterpretationMode.Auto);
            //#endif
            //                            m_blnCodeFound = true;

            //                            if (m_strResultCode != m_strTemplateCode && m_strTemplateCode != "" && !blnLearn)
            //                            {
            //                                m_blnCodeNotMatched = true;
            //                                m_blnCodePassed = false;
            //                                m_strErrorMessage = "QR Code result = " + m_strResultCode + " does not match with template " + m_strTemplateCode;
            //                                intResultType = 1;
            //                            }
            //                            else
            //                            {
            //                                if (blnLearn)
            //                                {
            //                                    EPoint[] arrPoints = m_objQRCodeResult[0].Geometry.Position.Corners;//0:Top Right, 1: Top Left, 2:Bottom Left, 3:Bottom Right
            //                                    float arrSideAngle1 = 0;
            //#if (Debug_2_12 || Release_2_12)
            //                                    arrSideAngle1 = m_objQRCodeResult[0].Geometry.Position.GetSideAngle(0);
            //#endif
            //                                    float Angle1 = arrSideAngle1;
            //                                    if (arrSideAngle1 >= 0)
            //                                        Angle1 = arrSideAngle1 - 180;
            //                                    else //if (arrSideAngle1 < 0)//<-90)
            //                                        Angle1 = arrSideAngle1 + 180;

            //                                    float fCenterX = 0;
            //                                    float fCenterY = 0;

            //                                    for (int a = 0; a < arrPoints.Length; a++)
            //                                    {
            //                                        fCenterX += arrPoints[a].X;
            //                                        fCenterY += arrPoints[a].Y;
            //                                    }
            //                                    fCenterX /= 4;
            //                                    fCenterY /= 4;
            //                                    fCenterX += objGainROI.ref_ROITotalX;
            //                                    fCenterY += objGainROI.ref_ROITotalY;

            //                                    float fXAfterRotated1 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
            //                                                       ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));
            //                                    float fXAfterRotated2 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[1].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
            //                                                       ((objGainROI.ref_ROITotalY + arrPoints[1].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));


            //                                    float fYAfterRotated1 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
            //                                                        ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));
            //                                    float fYAfterRotated2 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[2].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
            //                                                        ((objGainROI.ref_ROITotalY + arrPoints[2].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));


            //                                    m_fTemplateBarcodeWidth = fXAfterRotated2 - fXAfterRotated1;
            //                                    m_fTemplateBarcodeHeight = fYAfterRotated2 - fYAfterRotated1;
            //                                    m_fTemplateBarcodeAngle = Angle1;
            //                                    m_fTemplateBarcodeCenterX = fCenterX;
            //                                    m_fTemplateBarcodeCenterY = fCenterY;
            //                                }
            //                                blnFailInTryCatch = false;
            //                                if (m_strErrorMessage != "")
            //                                    m_strErrorMessage = "";
            //                                m_blnCodePassed = true;
            //                                intResultType = 0;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            m_blnCodeFound = true;
            //                            m_blnCodePassed = false;
            //                            m_strErrorMessage = "QR Code pattern found but unable to decode.";

            //                            intResultType = 2;
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        blnFailInTryCatch = true;
            //                        m_strErrorMessage = ex.Message.ToString();
            //                    }

            //                    if (intResultType < 2)
            //                        break;

            //                    if (blnAddGainNegative)
            //                        goto RetestGainOnly;
            //                }
            //            }

            //            if (blnFailInTryCatch && intResultType == 0)
            //            {
            //                m_blnCodeFound = true;
            //                intResultType = 3;
            //            }
            //            objGainROI.Dispose();
            //            objImg.Dispose();

            //            m_blnTestDone = true;
            return intResultType;
        }
        public int ReadMatrixCodeObjects(ROI objROI, bool blnLearn)
        {
            int intResultType = 0;
//            int intInspectMode = 0; // 0: Normal, 1: With Gain only

//            if (m_blnWantUseGainRange && m_fGainRangeTolerance != 0)
//                intInspectMode |= 0x01;

//            bool blnFailInTryCatch = false;
//            ROI objGainROI = new ROI();
//            ImageDrawing objImg = new ImageDrawing(true, objROI.ref_ROI.TotalWidth, objROI.ref_ROI.TotalHeight);
//            objImg.SetImageToBlack();
//            objGainROI.AttachImage(objImg);
//            objGainROI.LoadROISetting(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

//            objROI.CopyImage_Bigger(ref objGainROI);
//            for (int i = 0; i < 3; i++)
//            {
//                if (i == 1)
//                {
//                    EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
//                    EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
//                }
//                else if (i == 2)
//                {
//                    EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);
//                }

//                try
//                {
//                    m_objMatrixCode.Read(objGainROI.ref_ROI);
//#if (Debug_2_12 || Release_2_12)
//                    m_objMatrixCodeResult = m_objMatrixCode.ReadResults;
//#endif
//                    if (m_objMatrixCodeResult.Length > 0)
//                    {
//                        m_strResultCode = m_objMatrixCodeResult[0].DecodedString;
//                        m_blnCodeFound = true;

//                        if (m_strResultCode != m_strTemplateCode && m_strTemplateCode != "" && !blnLearn)
//                        {
//                            m_blnCodeNotMatched = true;
//                            m_blnCodePassed = false;
//                            m_strErrorMessage = "Matrix Code result = " + m_strResultCode + " does not match with template " + m_strTemplateCode;
//                            intResultType = 1;
//                        }
//                        else
//                        {
//                            if (blnLearn)
//                            {
//                                EPoint[] arrPoints = new EPoint[0];
//                                float arrSideAngle1 = 0;
//#if (Debug_2_12 || Release_2_12)
//                                arrPoints = m_objMatrixCodeResult[0].Position.Corners;//0:Top Right, 1: Top Left, 2:Bottom Left, 3:Bottom Right
//                                arrSideAngle1 = m_objMatrixCodeResult[0].Position.GetSideAngle(0);
//#endif
//                                float Angle1 = arrSideAngle1;
//                                //if (arrSideAngle1 >= 0)
//                                //    Angle1 = arrSideAngle1 - 180;
//                                //else //if (arrSideAngle1 < 0)//<-90)
//                                //    Angle1 = arrSideAngle1 + 180;

//                                float fCenterX = 0;
//                                float fCenterY = 0;

//                                for (int a = 0; a < arrPoints.Length; a++)
//                                {
//                                    fCenterX += arrPoints[a].X;
//                                    fCenterY += arrPoints[a].Y;
//                                }
//                                fCenterX /= 4;
//                                fCenterY /= 4;
//                                fCenterX += objGainROI.ref_ROITotalX;
//                                fCenterY += objGainROI.ref_ROITotalY;

//                                float fXAfterRotated1 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
//                                                   ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));
//                                float fXAfterRotated2 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[1].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
//                                                   ((objGainROI.ref_ROITotalY + arrPoints[1].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));


//                                float fYAfterRotated1 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
//                                                    ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));
//                                float fYAfterRotated2 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[3].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
//                                                    ((objGainROI.ref_ROITotalY + arrPoints[3].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));


//                                m_fTemplateBarcodeWidth = fXAfterRotated2 - fXAfterRotated1;
//                                m_fTemplateBarcodeHeight = fYAfterRotated2 - fYAfterRotated1;
//                                m_fTemplateBarcodeAngle = Angle1;
//                                m_fTemplateBarcodeCenterX = fCenterX;
//                                m_fTemplateBarcodeCenterY = fCenterY;
//                            }
//                            blnFailInTryCatch = false;
//                            if (m_strErrorMessage != "")
//                                m_strErrorMessage = "";
//                            m_blnCodePassed = true;
//                            intResultType = 0;
//                        }
//                    }
//                    else
//                    {
//                        m_blnCodeFound = true;
//                        m_blnCodePassed = false;
//                        m_strErrorMessage = "Matrix Code pattern found but unable to decode.";

//                        intResultType = 2;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    intResultType = 3;
//                    blnFailInTryCatch = true;
//                    m_strErrorMessage = ex.Message.ToString();
//                }

//                if (intResultType < 2 && m_blnCodeFound)
//                    break;
//            }

//            if (intResultType > 1 && intInspectMode == 1)
//            {
//                bool blnAddGainNegative = false;
//                for (float fGainValue = 0; fGainValue <= m_fGainRangeTolerance; fGainValue += 0.1f)
//                {
//                RetestGainOnly:
//                    if (fGainValue == 0)
//                    {
//                        fGainValue = 0.1f;
//                    }
//                    objROI.CopyImage_Bigger(ref objGainROI);
//                    if (fGainValue != 0)
//                    {
//                        if (!blnAddGainNegative)
//                        {
//                            if (fGainValue < 1) // for less than 1 gain value, max allow is 0.1 only because 0 gain will result black image
//                                blnAddGainNegative = true;
//                            objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 + fGainValue);
//                        }
//                        else
//                        {
//                            blnAddGainNegative = false;
//                            objGainROI.GainTo_ROIToROISamePosition(ref objImg, 1 - fGainValue);
//                        }
//                    }
//                    //objGainROI.SaveImage("D:\\objGainROI.bmp");
//                    EasyImage.Median(objGainROI.ref_ROI, objGainROI.ref_ROI);
//                    EasyImage.ConvolHighpass2(objGainROI.ref_ROI, objGainROI.ref_ROI);
//                    EasyImage.Threshold(objGainROI.ref_ROI, objGainROI.ref_ROI);

//                    try
//                    {
//                        m_objMatrixCode.Read(objGainROI.ref_ROI);
//#if (Debug_2_12 || Release_2_12)
//                    m_objMatrixCodeResult = m_objMatrixCode.ReadResults;
//#endif
//                        if (m_objMatrixCodeResult.Length > 0)
//                        {
//                            m_strResultCode = m_objMatrixCodeResult[0].DecodedString;
//                            m_blnCodeFound = true;

//                            if (m_strResultCode != m_strTemplateCode && m_strTemplateCode != "" && !blnLearn)
//                            {
//                                m_blnCodeNotMatched = true;
//                                m_blnCodePassed = false;
//                                m_strErrorMessage = "Matrix Code result = " + m_strResultCode + " does not match with template " + m_strTemplateCode;
//                                intResultType = 1;
//                            }
//                            else
//                            {
//                                if (blnLearn)
//                                {
//                                    EPoint[] arrPoints = new EPoint[0];
//                                    float arrSideAngle1 = 0;
//#if (Debug_2_12 || Release_2_12)
//                                arrPoints = m_objMatrixCodeResult[0].Position.Corners;//0:Top Right, 1: Top Left, 2:Bottom Left, 3:Bottom Right
//                                arrSideAngle1 = m_objMatrixCodeResult[0].Position.GetSideAngle(0);
//#endif
//                                    float Angle1 = arrSideAngle1;
//                                    if (arrSideAngle1 >= 0)
//                                        Angle1 = arrSideAngle1 - 180;
//                                    else //if (arrSideAngle1 < 0)//<-90)
//                                        Angle1 = arrSideAngle1 + 180;

//                                    float fCenterX = 0;
//                                    float fCenterY = 0;

//                                    for (int a = 0; a < arrPoints.Length; a++)
//                                    {
//                                        fCenterX += arrPoints[a].X;
//                                        fCenterY += arrPoints[a].Y;
//                                    }
//                                    fCenterX /= 4;
//                                    fCenterY /= 4;
//                                    fCenterX += objGainROI.ref_ROITotalX;
//                                    fCenterY += objGainROI.ref_ROITotalY;

//                                    float fXAfterRotated1 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
//                                                       ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));
//                                    float fXAfterRotated2 = (float)((fCenterX) + ((objGainROI.ref_ROITotalX + arrPoints[1].X - fCenterX) * Math.Cos((Angle1) * Math.PI / 180)) -
//                                                       ((objGainROI.ref_ROITotalY + arrPoints[1].Y - fCenterY) * Math.Sin((Angle1) * Math.PI / 180)));


//                                    float fYAfterRotated1 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[0].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
//                                                        ((objGainROI.ref_ROITotalY + arrPoints[0].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));
//                                    float fYAfterRotated2 = (float)((fCenterY) + ((objGainROI.ref_ROITotalX + arrPoints[2].X - fCenterX) * Math.Sin((Angle1) * Math.PI / 180)) +
//                                                        ((objGainROI.ref_ROITotalY + arrPoints[2].Y - fCenterY) * Math.Cos((Angle1) * Math.PI / 180)));


//                                    m_fTemplateBarcodeWidth = fXAfterRotated2 - fXAfterRotated1;
//                                    m_fTemplateBarcodeHeight = fYAfterRotated2 - fYAfterRotated1;
//                                    m_fTemplateBarcodeAngle = Angle1;
//                                    m_fTemplateBarcodeCenterX = fCenterX;
//                                    m_fTemplateBarcodeCenterY = fCenterY;
//                                }
//                                blnFailInTryCatch = false;
//                                if (m_strErrorMessage != "")
//                                    m_strErrorMessage = "";
//                                m_blnCodePassed = true;
//                                intResultType = 0;
//                            }
//                        }
//                        else
//                        {
//                            m_blnCodeFound = true;
//                            m_blnCodePassed = false;
//                            m_strErrorMessage = "Matrix Code pattern found but unable to decode.";

//                            intResultType = 2;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        blnFailInTryCatch = true;
//                        m_strErrorMessage = ex.Message.ToString();
//                    }

//                    if (intResultType < 2)
//                        break;

//                    if (blnAddGainNegative)
//                        goto RetestGainOnly;
//                }
//            }

//            if (blnFailInTryCatch && intResultType == 0)
//            {
//                m_blnCodeFound = true;
//                intResultType = 3;
//            }
//            objGainROI.Dispose();
//            objImg.Dispose();

//            m_blnTestDone = true;
            return intResultType;
        }
        public void DrawBarcodeResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY, ROI objROI, bool blnLearn)
        {
            lock (m_objLock) //Lock feature is here
            {
                try
                {
                    if (blnLearn)
                    {
                        if (m_intCodeType == 0)
                        {
                            m_objLimePen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Lime));
                            m_objRedPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Red));
                            m_objBluePen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
                        }
                        else
                        {
                            m_objLimePen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Lime));
                            m_objRedPen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Red));
                            m_objBluePen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Blue));
                        }
                    }

                    for (int i = 0; i < m_intTemplateCount; i++)
                    {
                       
                        switch (m_intCodeType)
                        {
                            case 0:
                                if (m_blnCodeFound[i])
                                {
                                    if (m_blnCodePassed[i])
                                    {
                                        IntPtr ptrHdc = g.GetHdc();
                                        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Lime));
                                        SelectObject(ptrHdc, m_arrPen[i]);
                                        m_objBarcode[i].DrawWithCurrentPen(ptrHdc, EDrawingMode.Actual);
                                        g.ReleaseHdc(ptrHdc);
                                        //m_objBarcode.Draw(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EDrawingMode.Actual, true);
                                    }
                                    else
                                    {
                                        IntPtr ptrHdc = g.GetHdc();
                                        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Red));
                                        SelectObject(ptrHdc, m_objRedPen);
                                        m_objBarcode[i].DrawWithCurrentPen(ptrHdc, EDrawingMode.Actual);
                                        g.ReleaseHdc(ptrHdc);
                                        //m_objBarcode.Draw(g, m_EROIRedColor, EDrawingMode.Actual, true);
                                    }
                                }
                                else if(!blnLearn)
                                {
                                    if (m_blnTested[i])
                                    {
                                        IntPtr ptrHdc = g.GetHdc();
                                        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
                                        SelectObject(ptrHdc, m_objRedPen);
                                        m_objBarcode[i].DrawWithCurrentPen(ptrHdc, EDrawingMode.InvalidSampledPoints);
                                        g.ReleaseHdc(ptrHdc);
                                        //m_objBarcode.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Position, true);
                                    }
                                    else
                                    {
                                        IntPtr ptrHdc = g.GetHdc();
                                        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
                                        SelectObject(ptrHdc, m_objRedPen);
                                        m_objBarcode[i].DrawWithCurrentPen(ptrHdc, EDrawingMode.Position);
                                        g.ReleaseHdc(ptrHdc);
                                        //m_objBarcode.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Position, true);
                                    }
                                }

                                float CenterX = m_objBarcode[i].CenterX;
                                float CenterY = m_objBarcode[i].CenterY;

                                float fXAfterRotated = (float)((CenterX) + (((m_objBarcode[i].CenterX - m_objBarcode[i].SizeX / 2) - CenterX) * Math.Cos(m_objBarcode[i].Angle * Math.PI / 180)) -
                                                   (((m_objBarcode[i].CenterY - m_objBarcode[i].SizeY / 2) - CenterY) * Math.Sin(m_objBarcode[i].Angle * Math.PI / 180)));

                                float fYAfterRotated = (float)((CenterY) + (((m_objBarcode[i].CenterX - m_objBarcode[i].SizeX / 2) - CenterX) * Math.Sin(m_objBarcode[i].Angle * Math.PI / 180)) +
                                                    (((m_objBarcode[i].CenterY - m_objBarcode[i].SizeY / 2) - CenterY) * Math.Cos(m_objBarcode[i].Angle * Math.PI / 180)));


                                g.DrawString((i + 1).ToString(), new Font("Verdana", 17, FontStyle.Bold), new SolidBrush(m_Color[i]), fXAfterRotated * fDrawingScaleX - 20, fYAfterRotated * fDrawingScaleY - 20);
                                break;
                            case 1:
                                //if (m_blnCodeFound[i] && m_objQRCodeResult.Length > 0)
                                //{
                                //    if (m_blnCodePassed[i])
                                //    {
                                //        IntPtr ptrHdc = g.GetHdc();
                                //        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Lime));
                                //        SelectObject(ptrHdc, m_objLimePen);
                                //        m_objQRCodeResult[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
                                //        g.ReleaseHdc(ptrHdc);
                                //        //m_objBarcode.Draw(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EDrawingMode.Actual, true);
                                //    }
                                //    else
                                //    {
                                //        IntPtr ptrHdc = g.GetHdc();
                                //        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Red));
                                //        SelectObject(ptrHdc, m_objRedPen);
                                //        m_objQRCodeResult[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
                                //        g.ReleaseHdc(ptrHdc);
                                //        //m_objBarcode.Draw(g, m_EROIRedColor, EDrawingMode.Actual, true);
                                //    }
                                //}
                                //else if (m_objQRCodeResult.Length > 0)
                                //{
                                //    IntPtr ptrHdc = g.GetHdc();
                                //    //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
                                //    SelectObject(ptrHdc, m_objBluePen);
                                //    m_objQRCodeResult[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
                                //    g.ReleaseHdc(ptrHdc);
                                //    //m_objBarcode.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Position, true);
                                //}
                                break;
                            case 2:
                                if (m_blnCodeFound[i] && m_objMatrixCodeResult.Length > 0)
                                {
                                    if (m_blnCodePassed[i])
                                    {
                                        IntPtr ptrHdc = g.GetHdc();
                                        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Lime));
                                        SelectObject(ptrHdc, m_objLimePen);
#if (Debug_2_12 || Release_2_12)
                                        m_objMatrixCodeResult[0].DrawGridWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                    m_objMatrixCodeResult[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
#endif
                                        SelectObject(ptrHdc, m_objRedPen);
                                        m_objMatrixCodeResult[0].DrawErrorsWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
                                        g.ReleaseHdc(ptrHdc);
                                        //m_objBarcode.Draw(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EDrawingMode.Actual, true);
                                    }
                                    else
                                    {
                                        IntPtr ptrHdc = g.GetHdc();
                                        //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Red));
                                        SelectObject(ptrHdc, m_objRedPen);
#if (Debug_2_12 || Release_2_12)
                                        m_objMatrixCodeResult[0].DrawGridWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                    m_objMatrixCodeResult[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
#endif
                                        g.ReleaseHdc(ptrHdc);
                                        //m_objBarcode.Draw(g, m_EROIRedColor, EDrawingMode.Actual, true);
                                    }
                                }
                                else if (m_objMatrixCodeResult.Length > 0)
                                {
                                    IntPtr ptrHdc = g.GetHdc();
                                    //IntPtr ptrPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
                                    SelectObject(ptrHdc, m_objBluePen);
#if (Debug_2_12 || Release_2_12)
                                    m_objMatrixCodeResult[0].DrawPositionWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                m_objMatrixCodeResult[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 0, 0);
#endif
                                    g.ReleaseHdc(ptrHdc);
                                    //m_objBarcode.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Position, true);
                                }
                                break;
                        }
                    }
                }
                catch
                {
                    //g.ReleaseHdc();
                }
            }
        }
        public void SaveBarcode(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);
            objFile.WriteElement1Value("CodeType", m_intCodeType); 
            objFile.WriteElement1Value("GainRangeTolerance", m_fGainRangeTolerance);
            objFile.WriteElement1Value("BarcodeAngleRangeTolerance", m_floatBarcodeAngleRangeTolerance); 
            objFile.WriteElement1Value("PatternAngleRangeTolerance", m_intPatternAngleRangeTolerance);
            objFile.WriteElement1Value("MinMatchingScore", m_intMinMatchingScore);
            objFile.WriteElement1Value("BarcodeDetectionAreaTolerance", m_intBarcodeDetectionAreaTolerance);
            objFile.WriteElement1Value("DelayTimeAfterPass", m_intDelayTimeAfterPass);
            objFile.WriteElement1Value("RetestCount", m_intRetestCount); 
            objFile.WriteElement1Value("PatternDetectionAreaTolerance_Top", m_intPatternDetectionAreaTolerance_Top);
            objFile.WriteElement1Value("PatternDetectionAreaTolerance_Right", m_intPatternDetectionAreaTolerance_Right);
            objFile.WriteElement1Value("PatternDetectionAreaTolerance_Bottom", m_intPatternDetectionAreaTolerance_Bottom);
            objFile.WriteElement1Value("PatternDetectionAreaTolerance_Left", m_intPatternDetectionAreaTolerance_Left);
            objFile.WriteElement1Value("ImageGain", m_fImageGain);
            objFile.WriteElement1Value("UniformizeGain", m_intUniformizeGain);
            objFile.WriteElement1Value("PatternTemplateCenterX", m_intPatternTemplateCenterX);
            objFile.WriteElement1Value("PatternTemplateCenterY", m_intPatternTemplateCenterY);
            objFile.WriteElement1Value("DontCareScale", m_fDontCareScale);
            objFile.WriteElement1Value("BarcodeOrientationAngle", m_fBarcodeOrientationAngle);

            objFile.WriteElement1Value("TemplateCount", m_intTemplateCount);
            for (int i =0;i< m_intTemplateCount; i++)
            {
                objFile.WriteElement1Value("TemplateCode" + i.ToString(), m_strTemplateCode[i]);
                objFile.WriteElement1Value("PatternReferenceOffsetX" + i.ToString(), m_fPatternReferenceOffsetX[i]);
                objFile.WriteElement1Value("PatternReferenceOffsetY" + i.ToString(), m_fPatternReferenceOffsetY[i]);
                objFile.WriteElement1Value("TemplateBarcodeWidth" + i.ToString(), m_fTemplateBarcodeWidth[i]);
                objFile.WriteElement1Value("TemplateBarcodeHeight" + i.ToString(), m_fTemplateBarcodeHeight[i]);
                objFile.WriteElement1Value("TemplateBarcodeAngle" + i.ToString(), m_fTemplateBarcodeAngle[i]);
                objFile.WriteElement1Value("TemplateBarcodeCenterX" + i.ToString(), m_fTemplateBarcodeCenterX[i]);
                objFile.WriteElement1Value("TemplateBarcodeCenterY" + i.ToString(), m_fTemplateBarcodeCenterY[i]);
            }

            objFile.WriteEndElement();


        }
        public void LoadBarcode(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);
            m_intCodeType = objFile.GetValueAsInt("CodeType", 0);
            m_fGainRangeTolerance = objFile.GetValueAsFloat("GainRangeTolerance", 0);
            m_floatBarcodeAngleRangeTolerance = objFile.GetValueAsFloat("BarcodeAngleRangeTolerance", 0); 
            m_intPatternAngleRangeTolerance = objFile.GetValueAsInt("PatternAngleRangeTolerance", 50);
            m_intMinMatchingScore = objFile.GetValueAsInt("MinMatchingScore", 50);  
            m_intBarcodeDetectionAreaTolerance = objFile.GetValueAsInt("BarcodeDetectionAreaTolerance", 250);
            m_intDelayTimeAfterPass = objFile.GetValueAsInt("DelayTimeAfterPass", 1000);
            m_intRetestCount = objFile.GetValueAsInt("RetestCount", 0);
            m_intPatternDetectionAreaTolerance_Top = objFile.GetValueAsInt("PatternDetectionAreaTolerance_Top", 20);
            m_intPatternDetectionAreaTolerance_Right = objFile.GetValueAsInt("PatternDetectionAreaTolerance_Right", 20);
            m_intPatternDetectionAreaTolerance_Bottom = objFile.GetValueAsInt("PatternDetectionAreaTolerance_Bottom", 20);
            m_intPatternDetectionAreaTolerance_Left = objFile.GetValueAsInt("PatternDetectionAreaTolerance_Left", 20);
            m_fImageGain = objFile.GetValueAsFloat("ImageGain", 0.5f);
            m_intUniformizeGain = objFile.GetValueAsInt("UniformizeGain", 1);
            m_intPatternTemplateCenterX = objFile.GetValueAsInt("PatternTemplateCenterX", 1);
            m_intPatternTemplateCenterY = objFile.GetValueAsInt("PatternTemplateCenterY", 1);
            m_fDontCareScale = objFile.GetValueAsFloat("DontCareScale", 1f);
            m_fBarcodeOrientationAngle = objFile.GetValueAsFloat("BarcodeOrientationAngle", 0);

            m_intTemplateCount = objFile.GetValueAsInt("TemplateCount", 0);
            for (int i = 0; i < m_intTemplateCount; i++)
            {
                m_strTemplateCode[i] = objFile.GetValueAsString("TemplateCode" + i.ToString(), "");
                m_fPatternReferenceOffsetX[i] = objFile.GetValueAsFloat("PatternReferenceOffsetX" + i.ToString(), 0);
                m_fPatternReferenceOffsetY[i] = objFile.GetValueAsFloat("PatternReferenceOffsetY" + i.ToString(), 0);
                m_fTemplateBarcodeWidth[i] = objFile.GetValueAsFloat("TemplateBarcodeWidth" + i.ToString(), 0);
                m_fTemplateBarcodeHeight[i] = objFile.GetValueAsFloat("TemplateBarcodeHeight" + i.ToString(), 0);
                m_fTemplateBarcodeAngle[i] = objFile.GetValueAsFloat("TemplateBarcodeAngle" + i.ToString(), 0);
                m_fTemplateBarcodeCenterX[i] = objFile.GetValueAsFloat("TemplateBarcodeCenterX" + i.ToString(), 0);
                m_fTemplateBarcodeCenterY[i] = objFile.GetValueAsFloat("TemplateBarcodeCenterY" + i.ToString(), 0);
            }

            if (m_intCodeType == 0)
            {
                m_objLimePen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Lime));
                m_objRedPen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Red));
                m_objBluePen = CreatePen(0, 3, (uint)ColorTranslator.ToWin32(Color.Blue));
            }
            else
            {
                m_objLimePen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Lime));
                m_objRedPen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Red));
                m_objBluePen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Blue));
            }
        }
        public bool IsPatternLearnt()
        {
            return m_objMatcher.PatternLearnt;
        }
        public bool LearnPattern(ROI objROI)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif

                m_objMatcher.LearnPattern(objROI.ref_ROI);
                m_intPatternTemplateCenterX = objROI.ref_ROITotalCenterX;
                m_intPatternTemplateCenterY = objROI.ref_ROITotalCenterY;
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = GetCodeType() + " Learn Pattern Error: " + ex.ToString();
                return false;
            }
            return true;
        }

        public bool LoadPattern(string strFilePath)
        {
            try
            {
                m_objMatcher.Load(strFilePath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = GetCodeType() + " Load Pattern Error: " + ex.ToString() + " FilePath:" + strFilePath;
                return false;
            }
            return true;
        }

        public bool SavePattern(string strFilePath)
        {
            try
            {
                string strDirectoryName = System.IO.Path.GetDirectoryName(strFilePath);
                DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                if (!directory.Exists)
                    CreateUnexistDirectory(directory);

                m_objMatcher.Save(strFilePath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = GetCodeType() + " Save Pattern Error: " + ex.ToString();
                return false;
            }
            return true;
        }
        private void CreateUnexistDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
            {
                CreateUnexistDirectory(directory.Parent);
            }

            Directory.CreateDirectory(directory.FullName);

        }
        public bool MatchReferencePattern(ROI objROI)
        {
            try
            {

                if (m_objMatcher.PatternLearnt)
                {
                    if (m_objMatcher.MinAngle != -m_intPatternAngleRangeTolerance)
                        m_objMatcher.MinAngle = -m_intPatternAngleRangeTolerance;
                    if (m_objMatcher.MaxAngle != m_intPatternAngleRangeTolerance)
                        m_objMatcher.MaxAngle = m_intPatternAngleRangeTolerance;
                    if (m_objMatcher.FinalReduction != 1)
                        m_objMatcher.FinalReduction = 1;
                    if (m_objMatcher.MaxPositions < 2)
                        m_objMatcher.MaxPositions = 2;  // 2020 03 10 - Set the MaxPositions to 2 will stabilize the pattern matching result.
                    //objROI.ref_ROI.Save("D:\\objROI.bmp");
                    //m_objMatcher.Save("D:\\m_objMatcher.mch");
                    m_objMatcher.Match(objROI.ref_ROI);
                    if (m_objMatcher.NumPositions > 0)
                    {
                        if ((m_objMatcher.GetPosition(0).Score * 100) > m_intMinMatchingScore)
                        {
                            m_fMatcherCenterX = objROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX;
                            m_fMatcherCenterY = objROI.ref_ROITotalY + m_objMatcher.GetPosition(0).CenterY;
                            m_fPatternAngle = m_objMatcher.GetPosition(0).Angle;
                            return true;
                        }
                        else
                        {
                            m_strErrorMessage = "*" + GetCodeType() + " : Pattern Matching Score Fail. Set = " + m_intMinMatchingScore.ToString() + ", Result = " + Math.Max(0, m_objMatcher.GetPosition(0).Score) * 100;
                            return false;
                        }
                    }
                    else
                    {
                        m_strErrorMessage = "*" + GetCodeType() + " : No Pattern Found!";
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage = "*" + GetCodeType() + " : No Pattern Learned!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision7Process : MatchReferencePattern() -> Exception: " + ex.ToString());
                return false;
            }

            return false;
        }
        public float GetMatchingScore()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).Score * 100;

            return 0;
        }
        public float GetMatchingAngle()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).Angle;

            return 0;
        }
        public float GetMatchingCenterX()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).CenterX;

            return 0;
        }
        public float GetMatchingCenterY()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).CenterY;

            return 0;
        }
        public int GetMatchingTemplateWidth()
        {
            if (m_objMatcher.PatternLearnt)
                return m_objMatcher.PatternWidth;

            return 0;
        }
        public int GetMatchingTemplateHeight()
        {
            if (m_objMatcher.PatternLearnt)
                return m_objMatcher.PatternHeight;

            return 0;
        }
        private string GetCodeType()
        {
            switch (m_intCodeType)
            {
                case 0:
                    return "Barcode";
                case 1:
                    return "QR Code";
                case 2:
                    return "Matrix Code";
            }
            return "Barcode";
        }
    }

}
