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
using System.Runtime.InteropServices;

namespace VisionProcessing
{
    public class Mark2DCode
    {
        #region DllImport

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        #endregion

        #region Member Variables

        private bool m_blnPassedCode = false;
        private object m_objLock = new object();
        private string m_strTemplateCode = "";
        private string m_strResultCode = "";
        private string m_strErrorMessage = "";
        private ERGBColor m_EROIRedColor = new ERGBColor(255, 0, 0);
        private float m_fCodeOrgX = 0;
        private float m_fCodeOrgY = 0;
        private float m_fCodeSizeX = 0;
        private float m_fCodeSizeY = 0;
        private bool m_blnCodeFound = false;
        private EBarCode m_objBarCode = new EBarCode();
        private EMatrixCodeReader m_objMatrixCodeReader = new EMatrixCodeReader();
        private EMatrixCode m_objMatrixCode = new EMatrixCode();
        private EQRCodeReader m_objQRCodeReader = new EQRCodeReader();
        private EQRCode[] m_objQRCode = new EQRCode[10];
        private int m_intCodeType = 0;

        #endregion


        #region Properties

        public string ref_strTemplateCode { get { return m_strTemplateCode; } set { m_strTemplateCode = value; } }
        public string ref_strResultCode { get { return m_strResultCode; } set { m_strResultCode = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public bool ref_blnCodeFound { get { return m_blnCodeFound; } set { m_blnCodeFound = value; } }
        public bool ref_blnPassedCode { get { return m_blnPassedCode; } set { m_blnPassedCode = value; } }
        public float ref_fOrgX { get { return m_fCodeOrgX; } }
        public float ref_fOrgY { get { return m_fCodeOrgY; } }
        public float ref_fSizeX { get { return m_fCodeSizeX; } }
        public float ref_fSizeY { get { return m_fCodeSizeY; } }
        #endregion

        public Mark2DCode()
        {
            //Barcode
            m_objBarCode.AdditionalSymbologies = (int)ESymbologies.Additional;

            //Matrix code
            m_objMatrixCodeReader.ComputeGrading = true;
            m_objMatrixCodeReader.SetLearnMaskElement(ELearnParam.LogicalSize, true);
            m_objMatrixCodeReader.SetLearnMaskElement(ELearnParam.ContrastType, true);
            m_objMatrixCodeReader.SetLearnMaskElement(ELearnParam.Flipping, true);
            m_objMatrixCodeReader.SetLearnMaskElement(ELearnParam.Family, true);
            m_objMatrixCodeReader.SetLearnMaskElement(ELearnParam.NumItems, true);

            //QR code
            m_objQRCodeReader.FilterOutUnreliablyDecodedQRCodes = true;
            m_objQRCodeReader.MinimumScore = 0.65f;
            m_objQRCodeReader.MinimumVersion = 1;
            m_objQRCodeReader.MaximumVersion = 40;
            m_objQRCodeReader.MinimumIsotropy = 0.8f;
            m_objQRCodeReader.ScanPrecision = EQRCodeScanPrecision.Automatic;
            m_objQRCodeReader.PerspectiveMode = EQRCodePerspectiveMode.Basic;
        }

        // =================================================Basic Function=================================================

        /// <summary>
        /// <param name="objROI">Source Image</param>
        /// </summary>
        /// <returns></returns>
        public int ReadCodeObjects(ROI objROI, int int2DCodetype)
        {
            lock (m_objLock) //Lock feature is here
            {
                int intResult = 0;
                switch (int2DCodetype)
                {
                    case 0:
                        intResult = ReadMatrixCodeObjects(objROI);
                        if (intResult != 0)
                        {
                            //Matrix code found
                            m_intCodeType = 2;
                        }
                        break;
                    case 1:
                        //Matrix code not found, proceed to check QR code
                        intResult = ReadQRCodeObjects(objROI);
                        if (intResult != 0)
                        {
                            //QR code found
                            m_intCodeType = 3;
                        }
                        break;
                }

                if (intResult == 0)
                    m_intCodeType = 0;

                return intResult;
            }
        }


        //private int ReadBarcodeObjects(ROI objROI)//, int intBarcodeLength)
        //{
        //    m_blnCodeFound = false;
        //    m_blnPassedCode = false;
        //    m_strResultCode = "";
        //    try
        //    {
        //        m_objBarCode.Detect(objROI.ref_ROI);

        //        if (m_objBarCode.NumDecodedSymbologies != 0)
        //        {
        //            m_strResultCode = m_objBarCode.Decode(m_objBarCode.GetDecodedSymbology(0));
        //            m_fCodeSizeX = m_objBarCode.SizeX;
        //            m_fCodeSizeY = m_objBarCode.SizeY;
        //            m_fCodeOrgX = m_objBarCode.CenterX - m_fCodeSizeX / 2;
        //            m_fCodeOrgY = m_objBarCode.CenterY - m_fCodeSizeY / 2;
        //            m_blnCodeFound = true;

        //            if (m_strResultCode.Length != intBarcodeLength)
        //            {
        //                m_blnPassedCode = false;
        //                m_strErrorMessage = "Barcode length is not correct.";
        //                return 0;
        //            }
        //            else
        //            {
        //                m_blnPassedCode = true;
        //                return 1;
        //            }
        //        }
        //        else
        //        {
        //            m_fCodeSizeX = m_objBarCode.SizeX;
        //            m_fCodeSizeY = m_objBarCode.SizeY;
        //            m_fCodeOrgX = m_objBarCode.CenterX - m_fCodeSizeX / 2;
        //            m_fCodeOrgY = m_objBarCode.CenterY - m_fCodeSizeY / 2;
        //            m_blnCodeFound = true;
        //            m_blnPassedCode = false;
        //            m_strErrorMessage = "Barcode pattern found but unable to decode.";
        //            return 2;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        m_strErrorMessage = ex.Message.ToString();
        //        return 0;
        //    }
        //}

        private int ReadMatrixCodeObjects(ROI objROI)
        {
            m_blnCodeFound = false;
            m_blnPassedCode = false;
            m_strResultCode = "";

            try
            {
                m_objMatrixCode = m_objMatrixCodeReader.Read(objROI.ref_ROI);
                if (m_objMatrixCode.Found)
                {
                    if (m_objMatrixCode.NumErrors > 0)
                    {
                        //Matrix code found but with error
                        m_strResultCode = m_objMatrixCode.DecodedString;

                        int intcellNumY = m_objMatrixCode.LogicalSizeHeight;
                        int intcellNumX = m_objMatrixCode.LogicalSizeWidth;
                        float fcellHeight = m_objMatrixCode.DataMatrixCellHeight;
                        float fcellWidth = m_objMatrixCode.DataMatrixCellWidth;
                        float fmatrixHeight = intcellNumY * fcellHeight;
                        float fmatrixWidth = intcellNumX * fcellWidth;
                        EPoint pfirstCorner = m_objMatrixCode.GetCorner(0);

                        m_fCodeOrgX = pfirstCorner.X - fcellWidth / 2;
                        m_fCodeOrgY = pfirstCorner.Y - fcellWidth / 2;
                        m_fCodeSizeX = fmatrixWidth;
                        m_fCodeSizeY = fmatrixHeight;

                        pfirstCorner.Dispose();
                        pfirstCorner = null;

                        m_blnCodeFound = true;
                        m_blnPassedCode = false;
                        m_strErrorMessage = "2DCode pattern found but with error. Number of error(s) = " + m_objMatrixCode.NumErrors.ToString();

                        return 2;
                    }
                    else
                    {
                        //Matrix code found without error
                        m_strResultCode = m_objMatrixCode.DecodedString;
                        int intcellNumY = m_objMatrixCode.LogicalSizeHeight;
                        int intcellNumX = m_objMatrixCode.LogicalSizeWidth;
                        float fcellHeight = m_objMatrixCode.DataMatrixCellHeight;
                        float fcellWidth = m_objMatrixCode.DataMatrixCellWidth;
                        float fmatrixHeight = intcellNumY * fcellHeight;
                        float fmatrixWidth = intcellNumX * fcellWidth;
                        EPoint pfirstCorner = m_objMatrixCode.GetCorner(0);

                        m_fCodeOrgX = pfirstCorner.X - fcellWidth / 2;
                        m_fCodeOrgY = pfirstCorner.Y - fcellWidth / 2;
                        m_fCodeSizeX = fmatrixWidth;
                        m_fCodeSizeY = fmatrixHeight;

                        pfirstCorner.Dispose();
                        pfirstCorner = null;

                        m_blnCodeFound = true;
                        m_blnPassedCode = true;
                        return 1;
                    }
                }
                else
                {
                    m_strResultCode = m_objMatrixCode.DecodedString;
                    int intcellNumY = m_objMatrixCode.LogicalSizeHeight;
                    int intcellNumX = m_objMatrixCode.LogicalSizeWidth;
                    float fcellHeight = m_objMatrixCode.DataMatrixCellHeight;
                    float fcellWidth = m_objMatrixCode.DataMatrixCellWidth;
                    float fmatrixHeight = intcellNumY * fcellHeight;
                    float fmatrixWidth = intcellNumX * fcellWidth;
                    EPoint pfirstCorner = m_objMatrixCode.GetCorner(0);

                    m_fCodeOrgX = pfirstCorner.X - fcellWidth / 2;
                    m_fCodeOrgY = pfirstCorner.Y - fcellWidth / 2;
                    m_fCodeSizeX = fmatrixWidth;
                    m_fCodeSizeY = fmatrixHeight;

                    pfirstCorner.Dispose();
                    pfirstCorner = null;

                    m_blnCodeFound = true;
                    m_blnPassedCode = false;
                    m_strErrorMessage = "2D Code pattern found but unable to decode.";
                    return 2;
                }
            }
            catch (Exception ex)
            {
                m_strErrorMessage = ex.Message.ToString();
                return 0;
            }
        }

        public int ReadQRCodeObjects(ROI objROI)
        {
            m_blnCodeFound = false;
            m_blnPassedCode = false;
            m_strResultCode = "";

            try
            {
                m_objQRCodeReader.SearchField = objROI.ref_ROI;
                m_objQRCode = m_objQRCodeReader.Read();

                if (m_objQRCode.Length > 0)
                {
                    if (m_objQRCode[0].IsDecodingReliable)
                    {
                        EQRCodeDecodedStream DecodedStream = m_objQRCode[0].DecodedStream;
                        EQRCodeDecodedStreamPart[] DecodedStreamPart = null;
                        DecodedStreamPart = DecodedStream.DecodedStreamParts;

                        for (int j = 0; j < DecodedStreamPart.Length; j ++)
                        {
                            byte[] bDecodedChar = DecodedStreamPart[j].DecodedData;
                            m_strResultCode = m_strResultCode + System.Text.Encoding.UTF8.GetString(bDecodedChar, 0, bDecodedChar.Length);
                            DecodedStreamPart[j].Dispose();
                            DecodedStreamPart[j] = null;
                        }
                        
                        DecodedStream.Dispose();
                        DecodedStream = null;

                        EQRCodeGeometry QRCodeGeometry = m_objQRCode[0].Geometry;
#if (Debug_2_12 || Release_2_12)
                        EQuadrangle Quadrilateral = QRCodeGeometry.Position;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EQuadrilateral Quadrilateral = QRCodeGeometry.Position;
#endif

                        EPoint pFirstCorner = Quadrilateral.Corners[1];
                        EPoint pSecondCorner = Quadrilateral.Corners[0];
                        EPoint pThirdCorner = Quadrilateral.Corners[3];
                        EPoint pFourthCorner = Quadrilateral.Corners[2];

                        m_fCodeOrgX = Math.Min(pFirstCorner.X - 1, pFourthCorner.X - 1);
                        m_fCodeOrgY = Math.Min(pFirstCorner.Y - 1, pSecondCorner.Y - 1);
                        m_fCodeSizeX = Math.Max(pThirdCorner.X - m_fCodeOrgX + 1, pSecondCorner.X - m_fCodeOrgX + 1);
                        m_fCodeSizeY = Math.Max(pThirdCorner.Y - m_fCodeOrgY + 1, pFourthCorner.Y - m_fCodeOrgY + 1);

                        QRCodeGeometry.Dispose();
                        Quadrilateral.Dispose();
                        pFirstCorner.Dispose();
                        pSecondCorner.Dispose();
                        pThirdCorner.Dispose();
                        pFourthCorner.Dispose();
                        QRCodeGeometry = null;
                        Quadrilateral = null;
                        pFirstCorner = null;
                        pSecondCorner = null;
                        pThirdCorner = null;
                        pFourthCorner = null;

                        m_blnCodeFound = true;
                        m_blnPassedCode = true;
                        return 1;
                    }
                    else
                    {
                        EQRCodeDecodedStream DecodedStream = m_objQRCode[0].DecodedStream;
                        EQRCodeDecodedStreamPart[] DecodedStreamPart = null;
                        DecodedStreamPart = DecodedStream.DecodedStreamParts;

                        for (int j = 0; j < DecodedStreamPart.Length; j++)
                        {
                            byte[] bDecodedChar = DecodedStreamPart[j].DecodedData;
                            m_strResultCode = m_strResultCode + System.Text.Encoding.UTF8.GetString(bDecodedChar, 0, bDecodedChar.Length);
                            DecodedStreamPart[j].Dispose();
                            DecodedStreamPart[j] = null;
                        }

                        DecodedStream.Dispose();
                        DecodedStream = null;

                        EQRCodeGeometry QRCodeGeometry = m_objQRCode[0].Geometry;
#if (Debug_2_12 || Release_2_12)
                        EQuadrangle Quadrilateral = QRCodeGeometry.Position;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EQuadrilateral Quadrilateral = QRCodeGeometry.Position;
#endif

                        EPoint pFirstCorner = Quadrilateral.Corners[1];
                        EPoint pSecondCorner = Quadrilateral.Corners[0];
                        EPoint pThirdCorner = Quadrilateral.Corners[3];
                        EPoint pFourthCorner = Quadrilateral.Corners[2];

                        m_fCodeOrgX = Math.Min(pFirstCorner.X - 1, pFourthCorner.X - 1);
                        m_fCodeOrgY = Math.Min(pFirstCorner.Y - 1, pSecondCorner.Y - 1);
                        m_fCodeSizeX = Math.Max(pThirdCorner.X - m_fCodeOrgX + 1, pSecondCorner.X - m_fCodeOrgX + 1);
                        m_fCodeSizeY = Math.Max(pThirdCorner.Y - m_fCodeOrgY + 1, pFourthCorner.Y - m_fCodeOrgY + 1);

                        QRCodeGeometry.Dispose();
                        Quadrilateral.Dispose();
                        pFirstCorner.Dispose();
                        pSecondCorner.Dispose();
                        pThirdCorner.Dispose();
                        pFourthCorner.Dispose();
                        QRCodeGeometry = null;
                        Quadrilateral = null;
                        pFirstCorner = null;
                        pSecondCorner = null;
                        pThirdCorner = null;
                        pFourthCorner = null;

                        m_blnCodeFound = true;
                        m_blnPassedCode = false;
                        m_strErrorMessage = "Barcode pattern found but not reliable.";
                        return 2;
                    }
                }
                else
                {
                    m_blnCodeFound = false;
                    m_blnPassedCode = false;
                    m_strErrorMessage = "Barcode pattern not found.";
                    return 0;
                }
            }
            catch (Exception ex)
            {
                m_strErrorMessage = ex.Message.ToString();
                return 0;
            }
        }


        public void DrawBarcodeResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            lock (m_objLock) //Lock feature is here
            {
                switch(m_intCodeType)
                {
                    case 1:
                        if (m_blnCodeFound)
                        {
                            if (m_blnPassedCode)
                                m_objBarCode.Draw(g);
                            else
                                m_objBarCode.Draw(g, m_EROIRedColor);
                        }
                        break;
                    case 2:
                        if (m_blnCodeFound)
                        {
                            if (m_blnPassedCode)
                            {
                                m_objMatrixCode.Draw(g, fDrawingScaleX, fDrawingScaleY);

                                if (m_objMatrixCode.NumErrors > 0)
                                    m_objMatrixCode.DrawErrors(g, fDrawingScaleX, fDrawingScaleY);
                            }
                            else
                            {
                                m_objMatrixCode.Draw(g, m_EROIRedColor, fDrawingScaleX, fDrawingScaleY);
                                m_objMatrixCode.DrawErrors(g, fDrawingScaleX, fDrawingScaleY);
                            }
                        }
                        break;
                    case 3:
                        if (m_blnCodeFound)
                        {
                            if (m_blnPassedCode)
                            {
                                IntPtr ptrHdc = g.GetHdc();
                                IntPtr ptrPen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Lime));
                                SelectObject(ptrHdc, ptrPen);
                                m_objQRCode[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 1f, 1f);
                                g.ReleaseHdc(ptrHdc);
                            }
                            else
                            {
                                IntPtr ptrHdc = g.GetHdc();
                                IntPtr ptrPen = CreatePen(0, 1, (uint)ColorTranslator.ToWin32(Color.Red));
                                SelectObject(ptrHdc, ptrPen);
                                m_objQRCode[0].DrawWithCurrentPen(ptrHdc, fDrawingScaleX, fDrawingScaleY, 1f, 1f);
                                g.ReleaseHdc(ptrHdc);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
