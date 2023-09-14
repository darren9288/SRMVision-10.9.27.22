using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text;
using Common;
using System.Windows.Forms;
using Ocr2Studio;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace VisionProcessing
{
    class OCR2
    {
        private int m_intHitLineNo = -1;
        private int m_intHitWordNo = -1;
        private int m_intHitCharNo = -1;
        private EOCR2 ocr2 = new EOCR2();
        private EOCR2Text DetectResult = new EOCR2Text();
        private bool hasHitText = false;
        private bool hasDetected = false;
        private object HitObj;
        private int RecognitionChrono = 0;
        private int DetectionChrono = 0;
        private EOCR2Char hitChar = new EOCR2Char();
        private EOCR2Word hitWord = new EOCR2Word();
        private EOCR2Line hitLine = new EOCR2Line();
        private EOCR2Text hitText = new EOCR2Text();
        private int detectionOrgX = 0;
        private int detectionOrgY = 0;
        private string errorMessage = "";
        private string strText = "";
        private EImageBW8 LearnROI = new EImageBW8();
        private EOCR2Char c;
        private EOCR2Word w;
        private EOCR2Line l;
        private EOCR2Text t;
        private List<int> Score = new List<int>();
        private Font m_Font = new Font("Verdana", 10);

        public string ref_Text { get { return strText; } set { strText = value; } }
        public int ref_CharDatabase { get { return ocr2.CharacterDatabase.Characters.Length; } }
        
        public bool OCRDetect(bool WhiteOnBlack, int CharMinWidth, int CharMaxWidth, int CharHeight, EROIBW8 MarkROI, string TopologyConvertValue, bool blnAuto)
        {
            try
            {
                ocr2.SegmentationMethod = EOCR2SegmentationMethod.Global;
                ocr2.CharsWidthRange.SetBounds(CharMinWidth, CharMaxWidth);
                //ocr2.CharsWidth = CharMaxWidth;
                ocr2.CharsHeight = CharHeight;

                if (WhiteOnBlack)
                    ocr2.TextPolarity = EasyOCR2TextPolarity.WhiteOnBlack;
                else
                    ocr2.TextPolarity = EasyOCR2TextPolarity.BlackOnWhite;

                if (blnAuto)
                    ocr2.DetectionMethod = EOCR2DetectionMethod.Proportional;
                else
                    ocr2.DetectionMethod = EOCR2DetectionMethod.FixedWidth;

                ocr2.Topology = TopologyConvertValue;
                //ocr2.CharsWidthTolerance = 1;
                ocr2.CharsWidthBias = (EasyOCR2CharWidthBias)Enum.Parse(typeof(EasyOCR2CharWidthBias), "Neutral");
                ocr2.CharsMaxFragmentation = (float)0.1;
                ocr2.MaxVariation = (float)0.5;
                ocr2.TextBaseAngle = 0;
                ocr2.TextAngleTolerance = 10;
                ocr2.DetectionDelta = 12;
                //Easy.StartTiming();
                DetectResult = ocr2.Detect(MarkROI);
                //DetectionChrono = Easy.StopTiming(1000000);
                detectionOrgX = MarkROI.TotalOrgX;
                detectionOrgY = MarkROI.TotalOrgY;
                hasDetected = true;
                return true;
            }
            catch (Exception ex)
            {
                hasDetected = false;
                errorMessage = ex.ToString() + " Please Enter OCR settings for adjusment";
                return false;
            }
        }
        public bool OCRDetect(EROIBW8 MarkROI)
        {
            try
            {
                //Easy.StartTiming();
                //DetectResult = ocr2.Detect(MarkROI);
                //DetectionChrono = Easy.StopTiming(1000000);
                detectionOrgX = MarkROI.TotalOrgX;
                detectionOrgY = MarkROI.TotalOrgY;
                hasDetected = true;
                return true;
            }
            catch (Exception ex)
            {
                hasDetected = false;
                errorMessage = ex.ToString() + " Please Enter OCR settings for adjusment";
                return false;
            }
        }
        public bool OCRLearn(object hitObject,string str)
        {
            try
            {
                str = str.Replace(Environment.NewLine, "\n");
                char[] a = str.ToCharArray();
                string strLen = ".";
                int counter = 0;
                bool notEnd = false;
                errorMessage = "";
                List<List<List<string>>> arrCharType = new List<List<List<string>>>();
                string[] arrLine = ocr2.Topology.Split('\n');
                for (int i = 0; i < arrLine.Length; i++)
                {
                    arrCharType.Add(new List<List<string>>());
                    string[] arrWord = arrLine[i].Split(' ');
                    for (int j = 0; j < arrWord.Length; j++)
                    {
                        arrCharType[i].Add(new List<string>());
                        char[] arrChar = arrWord[j].ToCharArray();
                        for (int k = 0; k < arrChar.Length; k++)
                        {
                            if (arrChar[k] != '{' && arrChar[k] != '}' && arrChar[k] != '[' && arrChar[k] != ']')
                            {
                                if (arrChar[k] == 'L' && arrChar.Length > (k + 1) && (arrChar[k + 1] == 'u' || arrChar[k + 1] == 'l'))
                                {
                                    arrCharType[i][j].Add((arrChar[k].ToString() + arrChar[k + 1].ToString()).ToString());
                                    k++;
                                }
                                else
                                    arrCharType[i][j].Add(arrChar[k].ToString());
                            }
                            else if (arrChar[k] == '{')
                            {
                                for (int x = 1; x < Convert.ToInt32(arrChar[k + 1].ToString()); x++)
                                {
                                    arrCharType[i][j].Add(arrChar[k - 1].ToString());
                                }
                                k++;
                            }
                            else if (arrChar[k] == '[')
                            {
                                string strCombined = "";
                                for (int x = k + 1; x < arrChar.Length; x++)
                                {
                                    if (arrChar[x] != ']')
                                        strCombined += arrChar[x].ToString();
                                    else
                                    {
                                        k++;
                                        break;
                                    }
                                    k++;
                                }
                                arrCharType[i][j].Add(strCombined);
                            }
                        }
                    }
                }

                if (hitObject is EOCR2Text)
                {
                    //for (int i = 0; i < a.Length; i++)
                    //{
                    //    //if (a[i] != '\n')
                    //    //    counter++;
                    //    //else
                    //    //{
                    //    //    strLen += "{" + counter + "}" + "\n.";
                    //    //    counter = 0;
                    //    //}

                    //    //if (i == a.Length - 1)
                    //    //{
                    //    //    if (notEnd)
                    //    //        strLen += "{" + counter + "}";
                    //    //    else
                    //    //        strLen.Remove(strLen.Length - 2, 2);
                    //    //}
                    //    //else
                    //    //{
                    //    //    notEnd = true;
                    //    //}
                    //}
                  
                    arrLine = str.Split('\n');
                    if (arrLine.Length != arrCharType.Count)
                    {
                        errorMessage = "Input Text Line Count Fail To Match With Topology Line Count!";
                        return false;
                    }
                    else
                    {
                        for (int i = 0; i < arrLine.Length; i++)
                        {
                            string[] arrWord = arrLine[i].Split(' ');
                            if (arrWord.Length != arrCharType[i].Count)
                            {
                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word Count Fail To Match With Topology Line " + (i + 1).ToString() + " Word Count!";
                                return false;
                            }
                            else
                            {
                                for (int j = 0; j < arrWord.Length; j++)
                                {
                                    char[] arrChar = arrWord[j].ToCharArray();
                                    if (arrChar.Length != arrCharType[i][j].Count)
                                    {
                                        errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char Count Fail To Match With Topology Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char Count!";
                                        return false;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < arrChar.Length; k++)
                                        {
                                            bool blnMatch = false;
                                            char[] chCharType = arrCharType[i][j][k].ToCharArray();
                                            for (int x = 0; x < chCharType.Length; x++)
                                            {
                                                string strCharType = chCharType[x].ToString();
                                                if (chCharType.Length > 1)
                                                {
                                                    if (chCharType[x] == 'L' && chCharType.Length > (x + 1) && (chCharType[x + 1] == 'u' || chCharType[x + 1] == 'l'))
                                                    {
                                                        strCharType = (chCharType[x].ToString() + chCharType[x + 1].ToString()).ToString();
                                                        x++;
                                                    }
                                                }

                                                switch (strCharType)
                                                {
                                                    case "L":
                                                        if (!char.IsLetter(arrChar[k]))
                                                        {
                                                            if (errorMessage == "")
                                                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char " + (k + 1).ToString() + ": " + arrChar[k].ToString() + " is not an Alphabet!";
                                                            else
                                                                errorMessage += " / Alphabet";
                                                        }
                                                        else
                                                        {
                                                            blnMatch = true;
                                                            break;
                                                        }
                                                        break;
                                                    case "Lu":
                                                        if (!char.IsUpper(arrChar[k]))
                                                        {
                                                            if (errorMessage == "")
                                                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char " + (k + 1).ToString() + ": " + arrChar[k].ToString() + " is not a Uppercase Alphabet!";
                                                            else
                                                                errorMessage += " / Uppercase Alphabet";
                                                        }
                                                        else
                                                        {
                                                            blnMatch = true;
                                                            break;
                                                        }
                                                        break;
                                                    case "Ll":
                                                        if (!char.IsLower(arrChar[k]))
                                                        {
                                                            if (errorMessage == "")
                                                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char " + (k + 1).ToString() + ": " + arrChar[k].ToString() + " is not a Lowercase Alphabet!";
                                                            else
                                                                errorMessage += " / Lowercase Alphabet";
                                                        }
                                                        else
                                                        {
                                                            blnMatch = true;
                                                            break;
                                                        }
                                                        break;
                                                    case "N":
                                                        if (!char.IsNumber(arrChar[k]))
                                                        {
                                                            if (errorMessage == "")
                                                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char " + (k + 1).ToString() + ": " + arrChar[k].ToString() + " is not a Number!";
                                                            else
                                                                errorMessage += " / Number";
                                                        }
                                                        else
                                                        {
                                                            blnMatch = true;
                                                            break;
                                                        }
                                                        break;
                                                    case "P":
                                                        if (!char.IsPunctuation(arrChar[k]))
                                                        {
                                                            if (errorMessage == "")
                                                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char " + (k + 1).ToString() + ": " + arrChar[k].ToString() + " is not a Punctuation!";
                                                            else
                                                                errorMessage += " / Punctuation";
                                                        }
                                                        else
                                                        {
                                                            blnMatch = true;
                                                            break;
                                                        }
                                                        break;
                                                    case "S":
                                                        if (!char.IsSymbol(arrChar[k]))
                                                        {
                                                            if (errorMessage == "")
                                                                errorMessage = "Input Text Line " + (i + 1).ToString() + " Word " + (j + 1).ToString() + " Char " + (k + 1).ToString() +": " + arrChar[k].ToString() + " is not a Symbol!";
                                                            else
                                                                errorMessage += " / Symbol";
                                                        }
                                                        else
                                                        {
                                                            blnMatch = true;
                                                            break;
                                                        }
                                                        break;
                                                    case ".":
                                                        {
                                                            blnMatch = true;
                                                        }
                                                        break;
                                                }
                                            }
                                            if (errorMessage != "")
                                                errorMessage += "!";
                                            if (!blnMatch)
                                            {
                                                return false;
                                            }
                                            else
                                                errorMessage = "";
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else if (hitObject is EOCR2Char)
                {
                    if (str.Length != 1)
                    {
                        errorMessage = "Input Char cannot more than one!";
                        return false;
                    }
                    char[] arrChar = str.ToCharArray();
                    bool blnMatch = false;
                    char[] chCharType = arrCharType[m_intHitLineNo][m_intHitWordNo][m_intHitCharNo].ToCharArray();
                    for (int x = 0; x < chCharType.Length; x++)
                    {
                        string strCharType = chCharType[x].ToString();
                        if (chCharType.Length > 1)
                        {
                            if (chCharType[x] == 'L' && chCharType.Length > (x + 1) && (chCharType[x + 1] == 'u' || chCharType[x + 1] == 'l'))
                            {
                                strCharType = (chCharType[x].ToString() + chCharType[x + 1].ToString()).ToString();
                                x++;
                            }
                        }

                        switch (strCharType)
                        {
                            case "L":
                                if (!char.IsLetter(arrChar[0]))
                                {
                                    if (errorMessage == "")
                                        errorMessage = "Input Char :" + arrChar[0].ToString() + " is not an Alphabet";
                                    else
                                        errorMessage += " / Alphabet";
                                }
                                else
                                {
                                    blnMatch = true;
                                    break;
                                }
                                break;
                            case "Lu":
                                if (!char.IsUpper(arrChar[0]))
                                {
                                    if (errorMessage == "")
                                        errorMessage = "Input Char :" + arrChar[0].ToString() + " is not an Uppercase Alphabet";
                                    else
                                        errorMessage += " / Uppercase Alphabet";
                                }
                                else
                                {
                                    blnMatch = true;
                                    break;
                                }
                                break;
                            case "Ll":
                                if (!char.IsLower(arrChar[0]))
                                {
                                    if (errorMessage == "")
                                        errorMessage = "Input Char :" + arrChar[0].ToString() + " is not an Lowercase Alphabet";
                                    else
                                        errorMessage += " / Lowercase Alphabet";
                                }
                                else
                                {
                                    blnMatch = true;
                                    break;
                                }
                                break;
                            case "N":
                                if (!char.IsNumber(arrChar[0]))
                                {
                                    if (errorMessage == "")
                                        errorMessage = "Input Char :" + arrChar[0].ToString() + " is not a Number";
                                    else
                                        errorMessage += " / Number";
                                }
                                else
                                {
                                    blnMatch = true;
                                    break;
                                }
                                break;
                            case "P":
                                if (!char.IsPunctuation(arrChar[0]))
                                {
                                    if (errorMessage == "")
                                        errorMessage = "Input Char :" + arrChar[0].ToString() + " is not a Punctuation";
                                    else
                                        errorMessage += " / Punctuation";
                                }
                                else
                                {
                                    blnMatch = true;
                                    break;
                                }
                                break;
                            case "S":
                                if (!char.IsSymbol(arrChar[0]))
                                {
                                    if (errorMessage == "")
                                        errorMessage = "Input Char :" + arrChar[0].ToString() + " is not a Symbol";
                                    else
                                        errorMessage += " / Symbol";
                                }
                                else
                                {
                                    blnMatch = true;
                                    break;
                                }
                                break;
                            case ".":
                                {
                                    blnMatch = true;
                                }
                                break;
                        }
                    }
                    if (errorMessage != "")
                        errorMessage += "!";
                    if (!blnMatch)
                    {
                        return false;
                    }
                    else
                        errorMessage = "";
                }
                else if (hitObject is EOCR2Word)
                {
                    w = (EOCR2Word)hitObject;

                    if (w.Characters.Length != str.Length)
                    {
                        errorMessage = "Input Word Count Fail To Match With Topology Word Count!";
                        return false;
                    }

                    EOCR2Char[] arrCharObject = w.Characters;
                    char[] arrChar = str.ToCharArray();
                    for (int i = 0; i < arrCharObject.Length; i++)
                    {
                        if (ocr2.HitTestChar(arrCharObject[i], ref m_intHitLineNo, ref m_intHitWordNo, ref m_intHitCharNo, (int)arrCharObject[i].BoundingBox.CenterX, (int)arrCharObject[i].BoundingBox.CenterY))
                        {
                            bool blnMatch = false;
                            char[] chCharType = arrCharType[m_intHitLineNo][m_intHitWordNo][m_intHitCharNo].ToCharArray();
                            for (int x = 0; x < chCharType.Length; x++)
                            {
                                string strCharType = chCharType[x].ToString();
                                if (chCharType.Length > 1)
                                {
                                    if (chCharType[x] == 'L' && chCharType.Length > (x + 1) && (chCharType[x + 1] == 'u' || chCharType[x + 1] == 'l'))
                                    {
                                        strCharType = (chCharType[x].ToString() + chCharType[x + 1].ToString()).ToString();
                                        x++;
                                    }
                                }

                                switch (strCharType)
                                {
                                    case "L":
                                        if (!char.IsLetter(arrChar[i]))
                                        {
                                            if (errorMessage == "")
                                                errorMessage = "Input Char " + (i + 1).ToString() + ": " + arrChar[i].ToString() + " is not an Alphabet";
                                            else
                                                errorMessage += " / Alphabet";
                                        }
                                        else
                                        {
                                            blnMatch = true;
                                            break;
                                        }
                                        break;
                                    case "Lu":
                                        if (!char.IsUpper(arrChar[i]))
                                        {
                                            if (errorMessage == "")
                                                errorMessage = "Input Char " + (i + 1).ToString() + ": " + arrChar[i].ToString() + "  is not an Uppercase Alphabet";
                                            else
                                                errorMessage += " / Uppercase Alphabet";
                                        }
                                        else
                                        {
                                            blnMatch = true;
                                            break;
                                        }
                                        break;
                                    case "Ll":
                                        if (!char.IsLower(arrChar[i]))
                                        {
                                            if (errorMessage == "")
                                                errorMessage = "Input Char " + (i + 1).ToString() + ": " + arrChar[i].ToString() + "  is not an Lowercase Alphabet";
                                            else
                                                errorMessage += " / Lowercase Alphabet";
                                        }
                                        else
                                        {
                                            blnMatch = true;
                                            break;
                                        }
                                        break;
                                    case "N":
                                        if (!char.IsNumber(arrChar[i]))
                                        {
                                            if (errorMessage == "")
                                                errorMessage = "Input Char " + (i + 1).ToString() + ": " + arrChar[i].ToString() + "  is not a Number";
                                            else
                                                errorMessage += " / Number";
                                        }
                                        else
                                        {
                                            blnMatch = true;
                                            break;
                                        }
                                        break;
                                    case "P":
                                        if (!char.IsPunctuation(arrChar[i]))
                                        {
                                            if (errorMessage == "")
                                                errorMessage = "Input Char " + (i + 1).ToString() + ": " + arrChar[i].ToString() + "  is not a Punctuation";
                                            else
                                                errorMessage += " / Punctuation";
                                        }
                                        else
                                        {
                                            blnMatch = true;
                                            break;
                                        }
                                        break;
                                    case "S":
                                        if (!char.IsSymbol(arrChar[i]))
                                        {
                                            if (errorMessage == "")
                                                errorMessage = "Input Char " + (i + 1).ToString() + ": " + arrChar[i].ToString() + "  is not a Symbol";
                                            else
                                                errorMessage += " / Symbol";
                                        }
                                        else
                                        {
                                            blnMatch = true;
                                            break;
                                        }
                                        break;
                                    case ".":
                                        {
                                            blnMatch = true;
                                        }
                                        break;
                                }
                            }
                            if (errorMessage != "")
                                errorMessage += "!";
                            if (!blnMatch)
                            {
                                return false;
                            }
                            else
                                errorMessage = "";
                        }
                    }
                }
                else if (hitObject is EOCR2Line)
                {
                    l = (EOCR2Line)hitObject;
                    EOCR2Word[] arrWordObject = l.Words;
                    EOCR2Char[][] arrCharObject = new EOCR2Char[arrWordObject.Length][];
                    string[] arrWord = str.Split(' ');
                    if (arrWordObject.Length != arrWord.Length)
                    {
                        errorMessage = "Input Word Count Fail To Match With Topology Word Count!";
                        return false;
                    }
                    int intTotalLength = arrWord.Length - 1;
                    for (int i =0;i< arrWordObject.Length;i++)
                    {
                        arrCharObject[i] = arrWordObject[i].Characters;
                        intTotalLength += arrWordObject[i].Characters.Length;
                    }
                    if (intTotalLength != str.Length)
                    {
                        errorMessage = "Input Line Fail To Match With Topology Line!";
                        return false;
                    }


                    for (int i = 0; i < arrCharObject.Length; i++)
                    {
                        char[] arrChar = arrWord[i].ToCharArray();
                        for (int j = 0; j < arrCharObject[i].Length; j++)
                        {
                            if (ocr2.HitTestChar(arrCharObject[i][j], ref m_intHitLineNo, ref m_intHitWordNo, ref m_intHitCharNo, (int)arrCharObject[i][j].BoundingBox.CenterX, (int)arrCharObject[i][j].BoundingBox.CenterY))
                            {
                                bool blnMatch = false;
                                char[] chCharType = arrCharType[m_intHitLineNo][m_intHitWordNo][m_intHitCharNo].ToCharArray();
                                for (int x = 0; x < chCharType.Length; x++)
                                {
                                    string strCharType = chCharType[x].ToString();
                                    if (chCharType.Length > 1)
                                    {
                                        if (chCharType[x] == 'L' && chCharType.Length > (x + 1) && (chCharType[x + 1] == 'u' || chCharType[x + 1] == 'l'))
                                        {
                                            strCharType = (chCharType[x].ToString() + chCharType[x + 1].ToString()).ToString();
                                            x++;
                                        }
                                    }

                                    switch (strCharType)
                                    {
                                        case "L":
                                            if (!char.IsLetter(arrChar[j]))
                                            {
                                                if (errorMessage == "")
                                                    errorMessage = "Input Word " + (i + 1).ToString() + " Char " + (j + 1).ToString() + ": " + arrChar[j].ToString() + " is not an Alphabet";
                                                else
                                                    errorMessage += " / Alphabet";
                                            }
                                            else
                                            {
                                                blnMatch = true;
                                                break;
                                            }
                                            break;
                                        case "Lu":
                                            if (!char.IsUpper(arrChar[j]))
                                            {
                                                if (errorMessage == "")
                                                    errorMessage = "Input Word " + (i + 1).ToString() + " Char " + (j + 1).ToString() + ": " + arrChar[j].ToString() + "  is not an Uppercase Alphabet";
                                                else
                                                    errorMessage += " / Uppercase Alphabet";
                                            }
                                            else
                                            {
                                                blnMatch = true;
                                                break;
                                            }
                                            break;
                                        case "Ll":
                                            if (!char.IsLower(arrChar[j]))
                                            {
                                                if (errorMessage == "")
                                                    errorMessage = "Input Word " + (i + 1).ToString() + " Char " + (j + 1).ToString() + ": " + arrChar[j].ToString() + "  is not an Lowercase Alphabet";
                                                else
                                                    errorMessage += " / Lowercase Alphabet";
                                            }
                                            else
                                            {
                                                blnMatch = true;
                                                break;
                                            }
                                            break;
                                        case "N":
                                            if (!char.IsNumber(arrChar[j]))
                                            {
                                                if (errorMessage == "")
                                                    errorMessage = "Input Word " + (i + 1).ToString() + " Char " + (j + 1).ToString() + ": " + arrChar[j].ToString() + "  is not a Number";
                                                else
                                                    errorMessage += " / Number";
                                            }
                                            else
                                            {
                                                blnMatch = true;
                                                break;
                                            }
                                            break;
                                        case "P":
                                            if (!char.IsPunctuation(arrChar[j]))
                                            {
                                                if (errorMessage == "")
                                                    errorMessage = "Input Word " + (i + 1).ToString() + " Char " + (j + 1).ToString() + ": " + arrChar[j].ToString() + "  is not a Punctuation";
                                                else
                                                    errorMessage += " / Punctuation";
                                            }
                                            else
                                            {
                                                blnMatch = true;
                                                break;
                                            }
                                            break;
                                        case "S":
                                            if (!char.IsSymbol(arrChar[j]))
                                            {
                                                if (errorMessage == "")
                                                    errorMessage = "Input Word " + (i + 1).ToString() + " Char " + (j + 1).ToString() + ": " + arrChar[j].ToString() + "  is not a Symbol";
                                                else
                                                    errorMessage += " / Symbol";
                                            }
                                            else
                                            {
                                                blnMatch = true;
                                                break;
                                            }
                                            break;
                                        case ".":
                                            {
                                                blnMatch = true;
                                            }
                                            break;
                                    }
                                }
                                if (errorMessage != "")
                                    errorMessage += "!";
                                if (!blnMatch)
                                {
                                    return false;
                                }
                                else
                                    errorMessage = "";
                            }
                        }
                    }
                }


                if (hitObject is EOCR2Char)
                {
                    c = (EOCR2Char)hitObject;
                    c.Text = strText;
                    ocr2.Learn(c);
                }
                else if (hitObject is EOCR2Word)
                {
                    w = (EOCR2Word)hitObject;
                    w.Text = strText;
                    ocr2.Learn(w);
                }
                else if (hitObject is EOCR2Line)
                {
                    l = (EOCR2Line)hitObject;
                    l.Text = strText;
                    ocr2.Learn(l);
                }
                else if (hitObject is EOCR2Text)
                {
                    t = (EOCR2Text)hitObject;
                    t.Text = strText;
                    ocr2.Learn(t);
                }
                ocr2.Recognize(DetectResult);
                DetectResult = ocr2.ReadText;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }

        public int GetCharNum()
        {
            string s = ocr2.ReadText.Text.ToString();
            int counter = 0;
            s = s.Replace(Environment.NewLine, "\n");
            char[] a = s.ToCharArray();


            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != '\n')
                    counter++;
            }
            return counter;
        }

        public object HitTest(int x, int y, float scaleX, float scaleY)
        {
            m_intHitLineNo = -1;
            m_intHitWordNo = -1;
            m_intHitCharNo = -1;

            BitmapCreatorVisitor creatorVisitor = new BitmapCreatorVisitor(LearnROI); 

            try
            {
                if (hasHitText = ocr2.HitTestChar(hitChar, ref m_intHitLineNo, ref m_intHitWordNo, ref m_intHitCharNo, x, y, scaleX, scaleY, detectionOrgX, detectionOrgY))
                {
                    HitObj = hitChar;
                    creatorVisitor.Visit(hitChar);
                }
                else
                {
                    if (hasHitText = ocr2.HitTestWord(hitWord, x, y, scaleX, scaleY, detectionOrgX, detectionOrgY))
                    {
                        HitObj = hitWord;
                        creatorVisitor.Visit(hitWord);
                    }
                    else
                    {

                        if (hasHitText = ocr2.HitTestLine(hitLine, x, y, scaleX, scaleY, detectionOrgX, detectionOrgY))
                        {
                            HitObj = hitLine;
                            creatorVisitor.Visit(hitLine);
                        }
                        else
                        {

                            if (hasHitText = ocr2.HitTestText(hitText, x, y, scaleX, scaleY, detectionOrgX, detectionOrgY))
                            {
                                HitObj = hitText;
                                creatorVisitor.Visit(hitText);
                            }
                            else
                            {
                                HitObj = null;
                            }
                        }
                    }
                }
                return HitObj;
            }
            catch (Exception ex)
            {
                hasHitText = false;
                return null;
            }
        }
        public bool Recognize()
        {
            try
            {
                if (hasDetected)
                {
                    ocr2.ClearResult();
                    //Easy.StartTiming();
                    ocr2.Recognize(DetectResult);
                    //RecognitionChrono = Easy.StopTiming(1000000);
                    DetectResult = ocr2.ReadText;
                    return true;
                }
                else
                {
                    errorMessage = "Please Perform Character Detection before Recognise";
                    return false; 
                }
            }
            catch (Exception ex)
            {
                RecognitionChrono = -1;
                errorMessage = ex.ToString() + " Please Enter OCR settings for adjusment";
                return false;
            }
        }
        public bool Read(ROI objROI)
        {
            try
            {
                //if (hasDetected)
                {
                    ocr2.ClearResult();
                    //Easy.StartTiming();
                    ocr2.Read(objROI.ref_ROI);
                    //RecognitionChrono = Easy.StopTiming(1000000);
                    DetectResult = ocr2.ReadText;
                    return true;
                }
                //else
                //{
                //    errorMessage = "Please Perform Character Detection before Recognise";
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                RecognitionChrono = -1;
                errorMessage = ex.ToString() + " Please Enter OCR settings for adjusment";
                return false;
            }
        }

        public void DrawOCR2(Graphics g,float scaleX,float scaleY,int mode)
        {
            if (hasDetected)
            {
                try
                {
                    if (mode == 0) //draw detection
                    {
                        ocr2.DrawDetection(g, new ERGBColor(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B), EasyOCR2DrawDetectionStyle.DrawChars, scaleX, scaleY);
                        ocr2.DrawDetection(g, new ERGBColor(Color.LightBlue.R, Color.LightBlue.G, Color.LightBlue.B), EasyOCR2DrawDetectionStyle.DrawText, scaleX);
                        //ocr2.DrawDetection(g, new ERGBColor(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B), EasyOCR2DrawDetectionStyle.DrawWords, scaleX);
                        //ocr2.DrawDetection(g, new ERGBColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), EasyOCR2DrawDetectionStyle.DrawLines, scaleX);
                    }
                    else if (mode == 1) //draw segmentation
                    {
                        ocr2.DrawSegmentation(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EasyOCR2DrawSegmentationStyle.DrawBlobs, scaleX, scaleY);
                    }
                    else if (mode == 2) //draw recognisation
                    {
                        ocr2.DrawRecognition(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), new ERGBColor(Color.LightYellow.R, Color.LightYellow.G, Color.LightYellow.B), EasyOCR2DrawRecognitionStyle.TopLeft, (uint)(40 * scaleY), scaleX, scaleY);
                    }
                    else if (mode == 3) //draw template no.
                    {
                        int counter = 0;
                        for (int i = 0; i < DetectResult.Lines.Length; i++)
                        {
                            for (int j = 0; j < DetectResult.Lines[i].Words.Length; j++)
                            {
                                for (int k = 0; k < DetectResult.Lines[i].Words[j].Characters.Length; k++)
                                {
                                    EOCR2Char e = DetectResult.Lines[i].Words[j].Characters[k];

                                    g.DrawString(Convert.ToString(counter + 1), m_Font, new SolidBrush(Color.Red), (detectionOrgX + e.BoundingBox.CenterX - (e.BoundingBox.SizeX / 2)) * scaleX, (detectionOrgY + e.BoundingBox.CenterY - (e.BoundingBox.SizeY / 2)) * scaleY);
                                    counter++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void LoadModel(string StrPath)
        {
            if (StrPath != null)
            {
                //ocr2.AddCharactersToDatabase(StrPath, EasyOCR2CharacterFilter.Digits);
                //ocr2.AddCharactersToDatabase(StrPath, EasyOCR2CharacterFilter.Letters);
                ocr2.AddCharactersToDatabase(StrPath);
            }
        }
        public void LoadModel(string[] StrPath)
        {
            for (int i = 0; i < StrPath.Length; i++)
            {
                if (StrPath[i] != null)
                {
                    if (File.Exists(StrPath[i]) && StrPath[i].Contains(".o2d"))
                    {
                        ocr2.AddCharactersToDatabase(StrPath[i]);
                    }
                    if (File.Exists(StrPath[i]) && StrPath[i].Contains(".o2m"))
                    {
                        ocr2.Load(StrPath[i]);
                    }
                }
            }
        }
        public void SaveModel(string StrPath)
        {
            try
            {
               if(StrPath != null)
                {
                    ocr2.Save(StrPath + "OCR2.o2m");
                    ocr2.SaveCharacterDatabase(StrPath + "OCR2.o2d");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }
        }

        public string getMessage(int type)
        {
            if (type == 0)
            {
                string s = ocr2.ReadText.Text.ToString();
                s = s.Replace(Environment.NewLine, "\n");
                return s;
            }
            else
                return errorMessage;
        }

        public int getLearnROIWidth()
        {
            return LearnROI.Width;
        }

        public int getLearnROIHeight()
        {
            return LearnROI.Height;
        }

        public void LearnROIDraw(IntPtr hdc, float zoom)
        {
            LearnROI.Draw(hdc, zoom);
        }

        public List<int> GetCandidateResult()
        {
            Score.Clear();
            for(int i =0; i < DetectResult.Lines.Length;i++)
            {
                for(int j=0; j < DetectResult.Lines[i].Words.Length;j++)
                {
                    for(int k=0; k < DetectResult.Lines[i].Words[j].Characters.Length;k++)
                    {
                        EOCR2Char e = DetectResult.Lines[i].Words[j].Characters[k];

                        if(e.Candidates.Length !=0)
                            Score.Add((int)(e.Candidates[0].Score * 100));
                    }
                }
            }
            return Score;
        }

        public void RemoveSpecialCharacterFromDatabase()
        {
            int length = ocr2.CharacterDatabase.Characters.Length;

            for (int i= 0; i <length; i++)
            {
                EOCR2DatabaseCharacter a = ocr2.CharacterDatabase.GetCharacter(i);

                if (a.CharacterCode >= 48 && a.CharacterCode <= 57)
                    continue;

                if (a.CharacterCode >= 65 && a.CharacterCode <= 90)
                    continue;

                if (a.CharacterCode >= 97 && a.CharacterCode <= 122)
                    continue;
                else
                    ocr2.CharacterDatabase.RemoveCharacter(i);
            }
        }

        public void DrawResult(Graphics g ,int index ,float scaleX, float scaleY , bool Pass)
        {
            int counter = 0;

            for (int i = 0; i < DetectResult.Lines.Length; i++)
            {
                for (int j = 0; j < DetectResult.Lines[i].Words.Length; j++)
                {
                    for (int k = 0; k < DetectResult.Lines[i].Words[j].Characters.Length; k++)
                    {
                        EOCR2Char e = DetectResult.Lines[i].Words[j].Characters[k];

                        if (index == counter)
                        {
                            if(!Pass)
                                g.DrawRectangle(Pens.Red, (detectionOrgX + e.BoundingBox.CenterX - (e.BoundingBox.SizeX / 2)) * scaleX, (detectionOrgY + e.BoundingBox.CenterY - (e.BoundingBox.SizeY / 2)) * scaleY, e.BoundingBox.SizeX * scaleX, e.BoundingBox.SizeY * scaleY);
                            else
                                g.DrawRectangle(Pens.LightGreen, (detectionOrgX + e.BoundingBox.CenterX - (e.BoundingBox.SizeX / 2)) * scaleX, (detectionOrgY + e.BoundingBox.CenterY - (e.BoundingBox.SizeY / 2)) * scaleY, e.BoundingBox.SizeX * scaleX, e.BoundingBox.SizeY * scaleY);
                            return;
                        }
                        else
                        {
                            counter++;
                            continue;
                        }

                    }
                }
            }
        }

        public string getCode(int index)
        {
            for (int i = 0; i < DetectResult.Lines.Length; i++)
            {
                for (int j = 0; j < DetectResult.Lines[i].Words.Length; j++)
                {
                    for (int k = 0; k < DetectResult.Lines[i].Words[j].Characters.Length; k++)
                    {
                        if (index == k)
                        {
                            EOCR2Char e = DetectResult.Lines[i].Words[j].Characters[k];
                            return e.Candidates[0].Code.ToString();
                        }
                        else
                            continue;

                    }
                }
            }

            return (-1).ToString();
        }

        public void ClearDatabase()
        {
            ocr2.ClearResult();
            ocr2.ClearCharacterDatabase();
        }
    }
}
