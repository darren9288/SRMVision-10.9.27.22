using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Common
{
    public class LanguageLibrary
    {
        private static string[] m_arrTextLibrary_ENG = new string[999];
        private static string[] m_arrTextLibrary_CHS = new string[999];
        private static string[] m_arrTextLibrary_CHT = new string[999];
        private static List<List<string>> m_arrText_ENG = new List<List<string>>();
        private static List<List<string>> m_arrText_CHS = new List<List<string>>();
        private static List<List<string>> m_arrText_CHT = new List<List<string>>();

        public static string Convert(int intLanguage, string strText)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            int TableSelectedIndex = 0;
            int intIndex = 0;
            int counter2 = 0;
            int tempindex = -1;
            int length = 0;
            string strNewText = strText;
            string strNewTextCompare = strText;
            bool bln_WholeWord = false;
            bool bln_allCH = false;

            if (intLanguage <= 1)
                return strText;

            switch (culture.Name)
            {
                case "zh-CHS":
                    intLanguage = 2;
                    break;
                case "zh-CHT":
                    intLanguage = 4;
                    break;
            }

            for (int i = 0; i < m_arrText_ENG.Count; i++)
            {
                TableSelectedIndex = 0;
                bln_WholeWord = false;
                strNewTextCompare = strText;

                if (bln_allCH)
                    break;

                for (int j = 0; j < m_arrText_ENG[i].Count; j++)
                {
                    intIndex = strText.IndexOf(m_arrText_ENG[i][j], StringComparison.OrdinalIgnoreCase);
                    string[] str = strText.Split(' ');
                    string symbol = "";
                    string sub = " ";
                    length = 0;

                    if (intIndex != -1)
                    {
                        sub = strText.Substring(intIndex, m_arrText_ENG[i][j].Length);

                        if (sub.Contains(":"))
                        {
                            symbol = ":";
                            sub = sub.Remove(sub.IndexOf(symbol), 1);
                        }

                        if (sub.Contains("*"))
                        {
                            symbol = "*";
                            sub = sub.Remove(sub.IndexOf(symbol), 1);
                        }

                        if (sub.Contains("*"))  //sometimes some word contain two **
                        {
                            symbol = "*";
                            sub = sub.Remove(sub.IndexOf(symbol), 1);
                        }

                        if (sub.Contains(","))
                        {
                            symbol = ",";
                            sub = sub.Remove(sub.IndexOf(symbol), 1);
                        }
                        if (sub.Contains("!"))
                        {
                            symbol = "!";
                            sub = sub.Remove(sub.IndexOf(symbol), 1);
                        }

                        if (sub.Contains("."))
                        {
                            symbol = ".";
                            sub = sub.Remove(sub.IndexOf(symbol), 1);
                        }

                        if (sub.ToLower().Equals(m_arrText_ENG[i][j].ToLower())) //filtering
                        {
                            if (length == 0)
                            {
                                length = m_arrText_ENG[i][j].Length;
                                TableSelectedIndex = j;
                            }
                            else
                            {
                                if (length < m_arrText_ENG[i][j].Length)  //find the nearest word match to it eg. [fail to find camera] will use "fail to find" instead of "fail"
                                {
                                    TableSelectedIndex = j;
                                    length = m_arrText_ENG[i][j].Length;
                                }
                                else
                                    continue;
                            }
                        }
                        else
                            continue;
                    }
                    else
                        continue;

                    tempindex = strText.IndexOf(m_arrText_ENG[i][TableSelectedIndex], StringComparison.OrdinalIgnoreCase);

                    foreach (string s in str) //check case "st(and) off" contained "and"
                    {
                        if (s.Contains(":") || s.Contains("*") || s.Contains(",") || s.Contains("!") || s.Contains("."))
                        {
                            bln_WholeWord = true;
                            continue;
                        }
                        else if (s.IndexOf(m_arrText_ENG[i][j], StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (s.Length == m_arrText_ENG[i][j].Length)
                            {
                                bln_WholeWord = true;
                                break;
                            }
                            else
                            {
                                bln_WholeWord = false;
                                break;
                            }
                        }
                        else
                            continue;
                    }

                    if (j != m_arrText_ENG[i].Count - 1)
                        continue;
                }

                   intIndex = tempindex;

                    if (intIndex == -1)
                        continue;

                if (bln_WholeWord)
                {
                    if (intLanguage == 2)
                    {
                        strNewText = strNewText.Replace(strText.Substring(intIndex, m_arrText_ENG[i][TableSelectedIndex].Length), m_arrText_CHS[i][TableSelectedIndex]);
                        strNewTextCompare = strText.Replace(strText.Substring(intIndex, m_arrText_ENG[i][TableSelectedIndex].Length), m_arrText_CHS[i][TableSelectedIndex]);
                    }
                    else
                    {
                        strNewText = strNewText.Replace(strText.Substring(intIndex, m_arrText_ENG[i][TableSelectedIndex].Length), m_arrText_CHT[i][TableSelectedIndex]);
                        strNewTextCompare = strText.Replace(strText.Substring(intIndex, m_arrText_ENG[i][TableSelectedIndex].Length), m_arrText_CHT[i][TableSelectedIndex]);
                    }

                }


                if (!bln_allCH)   //check all word is chinese anot 
                {
                    string[] split1 = strNewText.Split(' ');
                    string symbol = "";
                    counter2 = 0;
                    for (int k = 0; k < split1.Length; k++)
                    {
                        if (Regex.IsMatch(split1[k], "^[a-zA-Z0-9]*$"))
                            bln_allCH = false;
                        else if (split1[k].Contains(":") || split1[k].Contains("*") || split1[k].Contains(",") || split1[k].Contains("!") || split1[k].Contains("."))
                        {
                            string strRemove = split1[k];
                            if (strRemove.Contains(":"))
                            {
                                symbol = ":";
                                strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                            }

                            if (strRemove.Contains("*"))
                            {
                                symbol = "*";
                                strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                            }

                            if (strRemove.Contains("*"))
                            {
                                symbol = "*";
                                strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                            }


                            if (strRemove.Contains(","))
                            {
                                symbol = ",";
                                strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                            }
                            if (strRemove.Contains("!"))
                            {
                                symbol = "!";
                                strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                            }

                            if (strRemove.Contains("."))
                            {
                                symbol = ".";
                                strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                            }

                            if (Regex.IsMatch(strRemove, "^[a-zA-Z0-9]*$"))
                                bln_allCH = false;
                        }
                        else
                            counter2++;

                        if (counter2 == split1.Length)
                            bln_allCH = true;
                    }
                }
            }
            return strNewText;
        }

        private static string ResultComparison(string str1 , string str2 ,int counter2)
        {
            string[] split1 = str2.Split(' ');
            string symbol = "";
            string result = "";
            int counter = 0;

            for (int k = 0; k < split1.Length; k++)
            {
                if (Regex.IsMatch(split1[k], "^[a-zA-Z0-9]*$"))
                     continue;
                else if (split1[k].Contains(":") || split1[k].Contains("*") || split1[k].Contains(",") || split1[k].Contains("!") || split1[k].Contains("."))
                {
                    string strRemove = split1[k];
                    if (strRemove.Contains(":"))
                    {
                        symbol = ":";
                        strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                    }

                    if (split1[k].Contains("*"))
                    {
                        symbol = "*";
                        strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                    }

                    if (split1[k].Contains("*"))
                    {
                        symbol = "*";
                        strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                    }


                    if (strRemove.Contains(","))
                    {
                        symbol = ",";
                        strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                    }
                    if (split1[k].Contains("!"))
                    {
                        symbol = "!";
                        strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                    }

                    if (strRemove.Contains("."))
                    {
                        symbol = ".";
                        strRemove = strRemove.Remove(strRemove.IndexOf(symbol), 1);
                    }

                    if (Regex.IsMatch(strRemove, "^[a-zA-Z0-9]*$"))
                        continue;
                    else
                        counter++;
                }
                else
                    counter++;
            }

            if (counter > counter2)
                result = str2;
            else
                result = str1;

            return result;
        }

        public static string Convert(string strText)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            int intLanguage = 1;
            int count = 0;
            int intIndex = -1;
            int index2 = -1;
            int TableSelectedIndex = 0;
            int indexCH_temp;
            int LengthCompare;
            int Length_Temp;
            int compare;
            int counter = 0;
            string StrCH_Temp;
            string StrEng_Temp;
            string strCompare;
            string strCompareEng;
            string strText2 = strText;
            bool bln_NoRepeat = true;
            bool bln_WholeWord = false;
            List<int> index3 = new List<int>();
            List<int> Length = new List<int>();          
            List<string> strCH = new List<string>();
            List<string> strEng = new List<string>();

            switch (culture.Name)
            {
                case "zh-CHS":
                    intLanguage = 2;
                    break;
                case "zh-CHT":
                    intLanguage = 4;
                    break;
            }

            if (intLanguage <= 1)
                return strText;

            for (int i = 0; i < m_arrText_ENG.Count; i++)
            {
                strText = strText2;
                index2 = -1;
                bln_WholeWord = false;

                for (int j = 0; j < m_arrText_ENG[i].Count; j++)
                {
                    intIndex = strText.IndexOf(m_arrText_ENG[i][j], StringComparison.OrdinalIgnoreCase);

                    if (intIndex >= 0)
                    {
                        if (!m_arrText_ENG[i][j].Contains(" "))
                        {
                            string[] split = strText.Split(' ');

                            for (int k = 0; k < split.Length; k++)
                            {
                                if (split[k].Contains(":") || split[k].Contains("*") || split[k].Contains(",") || split[k].Contains("!") || split[k].Contains("."))
                                {
                                    if (split[k].IndexOf(m_arrText_ENG[i][j], StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        if (split[k].Length == m_arrText_ENG[i][j].Length)
                                            bln_WholeWord = true;
                                    }
                                }
                                else if (split[k].IndexOf(m_arrText_ENG[i][j], StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    if (split[k].Length == m_arrText_ENG[i][j].Length)
                                        bln_WholeWord = true;
                                }
                                else
                                    continue;
                            }
                        }
                        else
                        {
                            count = 0;
                            string[] split = strText.Split(' ');
                            string[] split2 = m_arrText_ENG[i][j].Split(' ');

                            for (int k = 0; k < split.Length; k++)
                            {
                                if (split[k].Contains(":") || split[k].Contains("*") || split[k].Contains(",") || split[k].Contains("!") || split[k].Contains("."))
                                {
                                    if (split[k].IndexOf(m_arrText_ENG[i][j], StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        if (split[k].Length == m_arrText_ENG[i][j].Length)
                                            bln_WholeWord = true;
                                    }
                                }

                                foreach (string str in split2)
                                {
                                    if (split[k].IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        if (split[k].Length == str.Length)
                                            count++;
                                        else
                                            continue;

                                        if (count == split2.Length)
                                            bln_WholeWord = true;
                                        else
                                            continue;
                                    }
                                }
                            }
                        }
                        if (intIndex >= 0)
                        {
                            TableSelectedIndex = j;
                            index2 = intIndex;
                        }


                        if (j != m_arrText_ENG[i].Count - 1)
                            continue;
                    }
                    else
                    {
                        if (j != m_arrText_ENG[i].Count - 1)
                            continue;
                    }

                    if (index2 != -1)
                        intIndex = index2;
                    else if (intIndex == -1)
                        break;

                    if (bln_WholeWord)
                    {
                        if (intLanguage == 2)
                            strText = strText.Replace(strText.Substring(intIndex, m_arrText_ENG[i][TableSelectedIndex].Length), m_arrText_CHS[i][TableSelectedIndex]);
                        else
                            strText = strText.Replace(strText.Substring(intIndex, m_arrText_ENG[i][TableSelectedIndex].Length), m_arrText_CHT[i][TableSelectedIndex]);


                        if (strEng.Count != 0) // check gt repeat word stored inside array
                        {
                            for (int k = 0; k < strEng.Count; k++)
                            {
                                if (m_arrText_ENG[i][TableSelectedIndex].ToLower().Contains(strEng[k].ToLower()))
                                {
                                    if (strEng[k].Length >= (m_arrText_ENG[i][TableSelectedIndex].Length))
                                        bln_NoRepeat = true;
                                    else
                                    {
                                        if (strEng.Count == 1)
                                            counter = 0;

                                        strEng.RemoveAt(k);
                                        strCH.RemoveAt(k);
                                        index3.RemoveAt(k);
                                        Length.RemoveAt(k);
                                        break;
                                    }

                                }
                                else
                                    bln_NoRepeat = true;
                            }
                        }

                        if (index3.Count != 0)
                        {
                            for (int k = 0; k < index3.Count; k++)
                            {
                                if (index3[k] == intIndex)
                                {
                                    bln_NoRepeat = false;
                                    break;
                                }
                                else
                                    bln_NoRepeat = true;
                            }
                        }

                        if (bln_NoRepeat)
                        {
                            if (counter == 0)
                            {
                                strCH.Add(m_arrText_CHS[i][TableSelectedIndex]);
                                index3.Add(intIndex);
                                strEng.Add(m_arrText_ENG[i][TableSelectedIndex]);
                                Length.Add(m_arrText_ENG[i][TableSelectedIndex].Length);
                                counter++;
                            }
                            else
                            {
                                compare = index3[0];
                                strCompare = strCH[0];
                                strCompareEng = strEng[0];
                                LengthCompare = Length[0];
                                index3.Add(intIndex);
                                strCH.Add(m_arrText_CHS[i][TableSelectedIndex]);
                                strEng.Add(m_arrText_ENG[i][TableSelectedIndex]);
                                Length.Add(m_arrText_ENG[i][TableSelectedIndex].Length);

                                if (index3[0] > intIndex)
                                {
                                    for (int k = 1; k < index3.Count; k++)
                                    {
                                        if (compare <= index3[k])
                                        {
                                            indexCH_temp = index3[k];
                                            StrCH_Temp = strCH[k];
                                            StrEng_Temp = strEng[k];
                                            Length_Temp = Length[k];
                                            index3[k] = compare;
                                            strCH[k] = strCompare;
                                            strEng[k] = strCompareEng;
                                            Length[k] = LengthCompare;
                                            compare = indexCH_temp;
                                            strCompare = StrCH_Temp;
                                            LengthCompare = Length_Temp;
                                            strCompareEng = StrEng_Temp;
                                        }
                                        else
                                        {
                                            index3[index3.Count - 1] = compare;
                                            strCH[index3.Count - 1] = strCompare;
                                            strEng[index3.Count - 1] = strCompareEng;
                                            Length[index3.Count - 1] = LengthCompare;
                                        }

                                    }
                                    index3[0] = intIndex;
                                    strCH[0] = m_arrText_CHS[i][TableSelectedIndex];
                                    strEng[0] = m_arrText_ENG[i][TableSelectedIndex];
                                    Length[0] = m_arrText_ENG[i][TableSelectedIndex].Length;
                                    counter++;
                                }
                                else if (index3[0] == 0)
                                {
                                    compare = intIndex;
                                    strCompare = m_arrText_CHS[i][TableSelectedIndex];
                                    strCompareEng = m_arrText_ENG[i][TableSelectedIndex];
                                    LengthCompare = m_arrText_ENG[i][TableSelectedIndex].Length;

                                    for (int k = 1; k < index3.Count; k++)
                                    {
                                        if (compare <= index3[k])
                                        {

                                            indexCH_temp = index3[k];
                                            StrCH_Temp = strCH[k];
                                            StrEng_Temp = strEng[k];
                                            Length_Temp = Length[k];
                                            index3[k] = compare;
                                            strCH[k] = strCompare;
                                            strEng[k] = strCompareEng;
                                            Length[k] = LengthCompare;
                                            compare = indexCH_temp;
                                            strCompare = StrCH_Temp;
                                            LengthCompare = Length_Temp;
                                            strCompareEng = StrEng_Temp;
                                        }
                                        else
                                        {
                                            index3[index3.Count - 1] = compare;
                                            strCH[index3.Count - 1] = strCompare;
                                            Length[index3.Count - 1] = LengthCompare;
                                            strEng[index3.Count - 1] = strCompareEng;
                                        }
                                    }
                                    counter++;
                                }
                                else
                                {
                                    if (counter < strCH.Count)
                                    {
                                        strCH[counter] = m_arrText_CHS[i][TableSelectedIndex];
                                        strEng[counter] = m_arrText_ENG[i][TableSelectedIndex];
                                        index3[counter] = intIndex;
                                        Length[counter] = m_arrText_ENG[i][TableSelectedIndex].Length;
                                        counter++;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }

            strText = strText2;
            int counter2 = 0;
            bool bln_allCh = false;

            for (int i = 0; i < strCH.Count; i++)
            {
                if (!bln_allCh)
                {
                    int diff = strText.Length - strText2.Length;

                    if (strCH[i] == null)
                        break;

                    try
                    {
                        if (diff > 0)
                        {
                            int intStartIndex = index3[i] - diff;
                            if (intStartIndex >= 0 && (intStartIndex + Length[i] <= strText.Length))
                                strText = strText.Replace(strText.Substring(index3[i] - diff, Length[i]), strCH[i]);
                        }
                        else
                        {
                            int intStartIndex = index3[i] + diff;
                            if (intStartIndex >= 0 && (intStartIndex + Length[i] <= strText.Length))
                                strText = strText.Replace(strText.Substring(index3[i] + diff, Length[i]), strCH[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        STTrackLog.WriteLine("LanguageLibrary > Convert Text = " + strText + ". Exception = " + ex.ToString());
                    }
                }
                else
                    break;

                string[] split = strText.Split(' ');
                counter2 = 0;

                for (int k = 0; k < split.Length; k++)
                {
                    if (Regex.IsMatch(split[k], "^[a-zA-Z0-9]*$"))
                        bln_allCh = false;
                    else if (split[k].Contains(":") || split[k].Contains("*") || split[k].Contains(",") || split[k].Contains("!") || split[k].Contains(".")|| split[k].Contains("?"))
                        bln_allCh = false;
                    else
                        counter2++;

                    if (counter2 == split.Length)
                        bln_allCh = true;
                }
            }
            return strText;
        }

        public static void InitLibrary()
        {
            m_arrTextLibrary_ENG[0] = "image";
            m_arrTextLibrary_CHS[0] = "图片";
            m_arrTextLibrary_CHT[0] = "图片";

            m_arrTextLibrary_ENG[1] = "offline";
            m_arrTextLibrary_CHS[1] = "离线";
            m_arrTextLibrary_CHT[1] = "离线";

            m_arrTextLibrary_ENG[2] = "test";
            m_arrTextLibrary_CHS[2] = "测试";
            m_arrTextLibrary_CHT[2] = "测试";

            m_arrTextLibrary_ENG[3] = "pass";
            m_arrTextLibrary_CHS[3] = "合格";
            m_arrTextLibrary_CHT[3] = "合格";

            m_arrTextLibrary_ENG[4] = "unit";
            m_arrTextLibrary_CHS[4] = "元件";
            m_arrTextLibrary_CHT[4] = "元件";

            m_arrTextLibrary_ENG[5] = "extra";
            m_arrTextLibrary_CHS[5] = "多余";
            m_arrTextLibrary_CHT[5] = "多余";

            m_arrTextLibrary_ENG[6] = "result";
            m_arrTextLibrary_CHS[6] = "结果";
            m_arrTextLibrary_CHT[6] = "结果";

            m_arrTextLibrary_ENG[7] = "range";
            m_arrTextLibrary_CHS[7] = "范围";
            m_arrTextLibrary_CHT[7] = "范围";

            m_arrTextLibrary_ENG[8] = "from";
            m_arrTextLibrary_CHS[8] = "从";
            m_arrTextLibrary_CHT[8] = "从";

            m_arrTextLibrary_ENG[9] = "top";
            m_arrTextLibrary_CHS[9] = "上面";
            m_arrTextLibrary_CHT[9] = "上面";

            m_arrTextLibrary_ENG[10] = "pixel";
            m_arrTextLibrary_CHS[10] = "像素";
            m_arrTextLibrary_CHT[10] = "像素";

            m_arrTextLibrary_ENG[11] = "set";
            m_arrTextLibrary_CHS[11] = "设置";
            m_arrTextLibrary_CHT[11] = "设置";

            m_arrTextLibrary_ENG[12] = "group";
            m_arrTextLibrary_CHS[12] = "组合";
            m_arrTextLibrary_CHT[12] = "组合";

            m_arrTextLibrary_ENG[13] = "calibration";
            m_arrTextLibrary_CHS[13] = "校准";
            m_arrTextLibrary_CHT[13] = "校准";

            m_arrTextLibrary_ENG[14] = "fail";
            m_arrTextLibrary_CHS[14] = "不合格";
            m_arrTextLibrary_CHT[14] = "不合格";

            m_arrTextLibrary_ENG[15] = "gauge";
            m_arrTextLibrary_CHS[15] = "仪器";
            m_arrTextLibrary_CHT[15] = "仪器";

            m_arrTextLibrary_ENG[16] = "continuous";
            m_arrTextLibrary_CHS[16] = "连续";
            m_arrTextLibrary_CHT[16] = "连续";

            m_arrTextLibrary_ENG[17] = "low yield fail!";
            m_arrTextLibrary_CHS[17] = "产量过低！";
            m_arrTextLibrary_CHT[17] = "产量过低！";

            m_arrTextLibrary_ENG[18] = "no sample found";
            m_arrTextLibrary_CHS[18] = "样本不存在";
            m_arrTextLibrary_CHT[18] = "样本不存在";

            m_arrTextLibrary_ENG[19] = "package";
            m_arrTextLibrary_CHS[19] = "塑封体";
            m_arrTextLibrary_CHT[19] = "塑封体";

            m_arrTextLibrary_ENG[20] = "no template found";
            m_arrTextLibrary_CHS[20] = "模板不存在";
            m_arrTextLibrary_CHT[20] = "模板不存在";

            m_arrTextLibrary_ENG[21] = "angle";
            m_arrTextLibrary_CHS[21] = "角度";
            m_arrTextLibrary_CHT[21] = "角度";

            m_arrTextLibrary_ENG[22] = "tolerance";
            m_arrTextLibrary_CHS[22] = "容许度";
            m_arrTextLibrary_CHT[22] = "容許度";

            m_arrTextLibrary_ENG[23] = "bottom";
            m_arrTextLibrary_CHS[23] = "底部";
            m_arrTextLibrary_CHT[23] = "底部";

            m_arrTextLibrary_ENG[24] = "side";
            m_arrTextLibrary_CHS[24] = "侧面";
            m_arrTextLibrary_CHT[24] = "侧面";

            m_arrTextLibrary_ENG[25] = "Pad";
            m_arrTextLibrary_CHS[25] = "衬垫";
            m_arrTextLibrary_CHT[25] = "衬垫";

            m_arrTextLibrary_ENG[26] = "joint";
            m_arrTextLibrary_CHS[26] = "桥接";
            m_arrTextLibrary_CHT[26] = "桥接";

            m_arrTextLibrary_ENG[27] = "bridge";
            m_arrTextLibrary_CHS[27] = "桥接";
            m_arrTextLibrary_CHT[27] = "桥接";

            m_arrTextLibrary_ENG[28] = "excess";
            m_arrTextLibrary_CHS[28] = "外延";
            m_arrTextLibrary_CHT[28] = "外延";

            m_arrTextLibrary_ENG[29] = "threshold";
            m_arrTextLibrary_CHS[29] = "二进制化";
            m_arrTextLibrary_CHT[29] = "二进制化";

            m_arrTextLibrary_ENG[30] = "gain";
            m_arrTextLibrary_CHS[30] = "图片亮度增益";
            m_arrTextLibrary_CHT[30] = "图片亮度增益";

            m_arrTextLibrary_ENG[31] = "edge";
            m_arrTextLibrary_CHS[31] = "边缘";
            m_arrTextLibrary_CHT[31] = "边缘";

            m_arrTextLibrary_ENG[32] = "setting";
            m_arrTextLibrary_CHS[32] = "设置";
            m_arrTextLibrary_CHT[32] = "設置";

            m_arrTextLibrary_ENG[33] = "contamination";
            m_arrTextLibrary_CHS[33] = "异物";
            m_arrTextLibrary_CHT[33] = "异物";

            m_arrTextLibrary_ENG[34] = "crack";
            m_arrTextLibrary_CHS[34] = "裂缝";
            m_arrTextLibrary_CHT[34] = "裂缝";

            m_arrTextLibrary_ENG[35] = "corrupted";
            m_arrTextLibrary_CHS[35] = "损坏";
            m_arrTextLibrary_CHT[35] = "损坏";

            m_arrTextLibrary_ENG[36] = "recipe";
            m_arrTextLibrary_CHS[36] = "菜单";
            m_arrTextLibrary_CHT[36] = "菜单";

            m_arrTextLibrary_ENG[37] = "please";
            m_arrTextLibrary_CHS[37] = "请";
            m_arrTextLibrary_CHT[37] = "請";

            m_arrTextLibrary_ENG[38] = "relearn";
            m_arrTextLibrary_CHS[38] = "重新学习";
            m_arrTextLibrary_CHT[38] = "重新学习";

            m_arrTextLibrary_ENG[39] = "template unit size min max setting cannot be zero.";
            m_arrTextLibrary_CHS[39] = "模板元件最大与最小尺寸设置不可设为0。";
            m_arrTextLibrary_CHT[39] = "模板元件最大与最小尺寸设置不可设为0。";

            m_arrTextLibrary_ENG[40] = "Calibration Pass!";
            m_arrTextLibrary_CHS[40] = "校准成功！";
            m_arrTextLibrary_CHT[40] = "校准成功！";

            m_arrTextLibrary_ENG[41] = "Calibration Fail!";
            m_arrTextLibrary_CHS[41] = "校准失败！";
            m_arrTextLibrary_CHT[41] = "校准失败！";

            m_arrTextLibrary_ENG[42] = "Gauge Fail";
            m_arrTextLibrary_CHS[42] = "仪器不合格";
            m_arrTextLibrary_CHT[42] = "仪器不合格";

            m_arrTextLibrary_ENG[43] = "Fail Orient Angle Tolerance";
            m_arrTextLibrary_CHS[43] = "Orient角度容许度不合格";
            m_arrTextLibrary_CHT[43] = "Orient角度容许度不合格";

            m_arrTextLibrary_ENG[44] = "Fail Orient X Tolerance";
            m_arrTextLibrary_CHS[44] = "Orient X 容许度不合格";
            m_arrTextLibrary_CHT[44] = "Orient X 容许度不合格";

            m_arrTextLibrary_ENG[45] = "Fail Orient Y Tolerance";
            m_arrTextLibrary_CHS[45] = "Orient Y 容许度不合格";
            m_arrTextLibrary_CHT[45] = "Orient Y 容许度不合格";

            m_arrTextLibrary_ENG[46] = "Recipe is corrupted. Please relearn.";
            m_arrTextLibrary_CHS[46] = "菜单已损坏。请重新学习。";
            m_arrTextLibrary_CHT[46] = "菜单已损坏。请重新学习。";

            m_arrTextLibrary_ENG[47] = "center";
            m_arrTextLibrary_CHS[47] = "中间";
            m_arrTextLibrary_CHT[47] = "中间";

            m_arrTextLibrary_ENG[48] = "*Total";
            m_arrTextLibrary_CHS[48] = "*总";
            m_arrTextLibrary_CHT[48] = "*總";

            m_arrTextLibrary_ENG[49] = "bright";
            m_arrTextLibrary_CHS[49] = "亮色";
            m_arrTextLibrary_CHT[49] = "亮色";

            m_arrTextLibrary_ENG[50] = "dark";
            m_arrTextLibrary_CHS[50] = "暗色";
            m_arrTextLibrary_CHT[50] = "暗色";

            m_arrTextLibrary_ENG[51] = "Fail Mark";
            m_arrTextLibrary_CHS[51] = "字模不合格";
            m_arrTextLibrary_CHT[51] = "字模不合格";

            m_arrTextLibrary_ENG[52] = "Fail Package";
            m_arrTextLibrary_CHS[52] = "塑封体不合格";
            m_arrTextLibrary_CHT[52] = "塑封体不合格";

            m_arrTextLibrary_ENG[53] = "Fail Pad";
            m_arrTextLibrary_CHS[53] = "衬垫不合格";
            m_arrTextLibrary_CHT[53] = "衬垫不合格";

            m_arrTextLibrary_ENG[54] = "up";
            m_arrTextLibrary_CHS[54] = "上面";
            m_arrTextLibrary_CHT[54] = "上面";

            m_arrTextLibrary_ENG[55] = "length";
            m_arrTextLibrary_CHS[55] = "长度";
            m_arrTextLibrary_CHT[55] = "长度";

            m_arrTextLibrary_ENG[56] = "width";
            m_arrTextLibrary_CHS[56] = "宽度";
            m_arrTextLibrary_CHT[56] = "宽度";

            m_arrTextLibrary_ENG[57] = "height";
            m_arrTextLibrary_CHS[57] = "高度";
            m_arrTextLibrary_CHT[57] = "高度";

            m_arrTextLibrary_ENG[58] = "area";
            m_arrTextLibrary_CHS[58] = "面积";
            m_arrTextLibrary_CHT[58] = "面积";

            m_arrTextLibrary_ENG[59] = "size";
            m_arrTextLibrary_CHS[59] = "尺寸";
            m_arrTextLibrary_CHT[59] = "尺寸";

            m_arrTextLibrary_ENG[60] = "Package : ";
            m_arrTextLibrary_CHS[60] = "塑封体 ： ";
            m_arrTextLibrary_CHT[60] = "塑封体 ： ";

            m_arrTextLibrary_ENG[61] = "Pad : ";
            m_arrTextLibrary_CHS[61] = "衬垫 ： ";
            m_arrTextLibrary_CHT[61] = "衬垫 ： ";

            m_arrTextLibrary_ENG[62] = "Wrong setting for package size image number.";
            m_arrTextLibrary_CHS[62] = "用于测量塑封体尺寸的照片号码设置不正确";
            m_arrTextLibrary_CHT[62] = "用于测量塑封体尺寸的照片号码设置不正确";

            m_arrTextLibrary_ENG[63] = "Wrong setting for package (Side Light View) image number.";
            m_arrTextLibrary_CHS[63] = "用于测量塑封体（侧灯视角）的照片号码设置不正确";
            m_arrTextLibrary_CHT[63] = "用于测量塑封体（侧灯视角）的照片号码设置不正确";

            m_arrTextLibrary_ENG[64] = "Wrong setting for package (Top Light View) image number.";
            m_arrTextLibrary_CHS[64] = "用于测量塑封体（直灯视角）的照片号码设置不正确";
            m_arrTextLibrary_CHT[64] = "用于测量塑封体（直灯视角）的照片号码设置不正确";

            m_arrTextLibrary_ENG[65] = "Change";
            m_arrTextLibrary_CHS[65] = "更换";
            m_arrTextLibrary_CHT[65] = "更換";

            m_arrTextLibrary_ENG[66] = "Fail to find unit.";
            m_arrTextLibrary_CHS[66] = "无法侦测元件";
            m_arrTextLibrary_CHT[66] = "无法侦测元件";

            m_arrTextLibrary_ENG[67] = "**Mark Shifted";
            m_arrTextLibrary_CHS[67] = "字模偏移";
            m_arrTextLibrary_CHT[67] = "字模偏移";

            m_arrTextLibrary_ENG[68] = "Min Tol";
            m_arrTextLibrary_CHS[68] = "最小容许度";
            m_arrTextLibrary_CHT[68] = "最小容许度";

            m_arrTextLibrary_ENG[69] = "Max Tol";
            m_arrTextLibrary_CHS[69] = "最大容许度";
            m_arrTextLibrary_CHT[69] = "最大容许度";

            m_arrTextLibrary_ENG[70] = "thickness";
            m_arrTextLibrary_CHS[70] = "厚度";
            m_arrTextLibrary_CHT[70] = "厚度";

            m_arrTextLibrary_ENG[71] = "sample";
            m_arrTextLibrary_CHS[71] = "样本";
            m_arrTextLibrary_CHT[71] = "样本";

            m_arrTextLibrary_ENG[72] = "yield";
            m_arrTextLibrary_CHS[72] = "产量";
            m_arrTextLibrary_CHT[72] = "产量";

            m_arrTextLibrary_ENG[73] = "low";
            m_arrTextLibrary_CHS[73] = "低";
            m_arrTextLibrary_CHT[73] = "低";

            m_arrTextLibrary_ENG[74] = "high";
            m_arrTextLibrary_CHS[74] = "高";
            m_arrTextLibrary_CHT[74] = "高";

            m_arrTextLibrary_ENG[75] = "down";
            m_arrTextLibrary_CHS[75] = "下面";
            m_arrTextLibrary_CHT[75] = "下面";

            m_arrTextLibrary_ENG[76] = "wait";
            m_arrTextLibrary_CHS[76] = "等待";
            m_arrTextLibrary_CHT[76] = "等待";

            m_arrTextLibrary_ENG[77] = "time out";
            m_arrTextLibrary_CHS[77] = "超时";
            m_arrTextLibrary_CHT[77] = "超时";

            m_arrTextLibrary_ENG[78] = "inspection";
            m_arrTextLibrary_CHS[78] = "检测";
            m_arrTextLibrary_CHT[78] = "检测";

            m_arrTextLibrary_ENG[79] = "bright field";
            m_arrTextLibrary_CHS[79] = "亮色";
            m_arrTextLibrary_CHT[79] = "亮色";

            m_arrTextLibrary_ENG[80] = "dark field";
            m_arrTextLibrary_CHS[80] = "暗色";
            m_arrTextLibrary_CHT[80] = "暗色";

            m_arrTextLibrary_ENG[81] = "smear";
            m_arrTextLibrary_CHS[81] = "边缘漏铜";
            m_arrTextLibrary_CHT[81] = "边缘漏铜";

            m_arrTextLibrary_ENG[82] = "excess";
            m_arrTextLibrary_CHS[82] = "过量";
            m_arrTextLibrary_CHT[82] = "过量";

            m_arrTextLibrary_ENG[83] = "text Shifted";
            m_arrTextLibrary_CHS[83] = "字体偏移";
            m_arrTextLibrary_CHT[83] = "字体偏移";

            m_arrTextLibrary_ENG[84] = "Mark : ";
            m_arrTextLibrary_CHS[84] = "字模 ： ";
            m_arrTextLibrary_CHT[84] = "字模 ： ";

            m_arrTextLibrary_ENG[85] = "missing";
            m_arrTextLibrary_CHS[85] = "缺少";
            m_arrTextLibrary_CHT[85] = "缺少";

            m_arrTextLibrary_ENG[86] = "joint";
            m_arrTextLibrary_CHS[86] = "合并";
            m_arrTextLibrary_CHT[86] = "合并";

            m_arrTextLibrary_ENG[87] = "total area";
            m_arrTextLibrary_CHS[87] = "总面积";
            m_arrTextLibrary_CHT[87] = "总面积";

            m_arrTextLibrary_ENG[88] = "gap";
            m_arrTextLibrary_CHS[88] = "间距";
            m_arrTextLibrary_CHT[88] = "间距";

            m_arrTextLibrary_ENG[89] = "excess pad";
            m_arrTextLibrary_CHS[89] = "衬垫外延/漏铜";
            m_arrTextLibrary_CHT[89] = "衬垫外延/漏铜";

            m_arrTextLibrary_ENG[90] = "Scratch";
            m_arrTextLibrary_CHS[90] = "刮伤";
            m_arrTextLibrary_CHT[90] = "刮伤";

            m_arrTextLibrary_ENG[91] = "Missing Mark";
            m_arrTextLibrary_CHS[91] = "破损字模 ";
            m_arrTextLibrary_CHT[91] = "破損字模";

            m_arrTextLibrary_ENG[92] = "right";
            m_arrTextLibrary_CHS[92] = "右面";
            m_arrTextLibrary_CHT[92] = "右面";

            m_arrTextLibrary_ENG[93] = "left";
            m_arrTextLibrary_CHS[93] = "左面";
            m_arrTextLibrary_CHT[93] = "左面";

            m_arrTextLibrary_ENG[94] = "Serial No.";
            m_arrTextLibrary_CHS[94] = "编号：";
            m_arrTextLibrary_CHT[94] = "编号：";

            m_arrTextLibrary_ENG[95] = "Fail Camera";
            m_arrTextLibrary_CHS[95] = "相机错误";
            m_arrTextLibrary_CHT[95] = "相机错误";

            m_arrTextLibrary_ENG[96] = "Unable to find any camera(s) connected";
            m_arrTextLibrary_CHS[96] = "未连接此编号相机";
            m_arrTextLibrary_CHT[96] = "未连接此编号相机";

            m_arrTextLibrary_ENG[97] = "has valid result";
            m_arrTextLibrary_CHS[97] = "是正确的";
            m_arrTextLibrary_CHT[97] = "是正确的";

            m_arrTextLibrary_ENG[98] = "Only";
            m_arrTextLibrary_CHS[98] = "只有";
            m_arrTextLibrary_CHT[98] = "只有";

            m_arrTextLibrary_ENG[99] = "please build object first";
            m_arrTextLibrary_CHS[99] = "请建立至少一个物体！";
            m_arrTextLibrary_CHT[99] = "请建立至少一个物体！";

            m_arrTextLibrary_ENG[100] = "Please learn at least one pattern before save!";
            m_arrTextLibrary_CHS[100] = "储存之前请学习至少一个图案！";
            m_arrTextLibrary_CHT[100] = "储存之前请学习至少一个图案！";

            m_arrTextLibrary_ENG[101] = "Please build characters first";
            m_arrTextLibrary_CHS[101] = "请建立至少一个字模！";
            m_arrTextLibrary_CHT[101] = "请建立至少一个字模！";

            m_arrTextLibrary_ENG[102] = "InPocket";
            m_arrTextLibrary_CHS[102] = "捲帶口袋";
            m_arrTextLibrary_CHT[102] = "捲帶口袋";

            m_arrTextLibrary_ENG[103] = "Are you sure you want to delete all patterns?";
            m_arrTextLibrary_CHS[103] = "是否确定删除全部图案？";
            m_arrTextLibrary_CHT[103] = "是否确定删除全部图案？";

            m_arrTextLibrary_ENG[104] = "Please select pattern on grid table first!";
            m_arrTextLibrary_CHS[104] = "请首先选择表格中的图案！";
            m_arrTextLibrary_CHT[104] = "请首先选择表格中的图案！";

            m_arrTextLibrary_ENG[105] = "Are you sure you want to delete the selected pattern?";
            m_arrTextLibrary_CHS[105] = "是否确定删除选择的图案？";
            m_arrTextLibrary_CHT[105] = "是否确定删除选择的图案？";

            m_arrTextLibrary_ENG[106] = "Please learn at least one pattern before save!";
            m_arrTextLibrary_CHS[106] = "储存之前请学习至少一个图案！";
            m_arrTextLibrary_CHT[106] = "储存之前请学习至少一个图案！";

            m_arrTextLibrary_ENG[107] = "No Char is selected";
            m_arrTextLibrary_CHS[107] = "未选择字模！";
            m_arrTextLibrary_CHT[107] = "未选择字模！";

            m_arrTextLibrary_ENG[108] = "Maximum only 1 sub roi can be added.";
            m_arrTextLibrary_CHS[108] = "最多可添加一个sub ROI。";
            m_arrTextLibrary_CHT[108] = "最多可添加一个sub ROI。";

            m_arrTextLibrary_ENG[109] = "Cannot remove because there is no Sub ROI.";
            m_arrTextLibrary_CHS[109] = "sub ROI不存在无法清除。";
            m_arrTextLibrary_CHT[109] = "sub ROI不存在无法清除。";

            m_arrTextLibrary_ENG[110] = "Please select Sub ROI before press remove button.";
            m_arrTextLibrary_CHS[110] = "清除之前请选择sub ROI。";
            m_arrTextLibrary_CHT[110] = "清除之前请选择sub ROI。";

            m_arrTextLibrary_ENG[111] = "Fail to unit surface position using Unit PR. Please relearn PR using Unit PR using Learn Mark Wizard.";
            m_arrTextLibrary_CHS[111] = "图片配对失败。请学习Mark页面中重新学习图片配对模板。";
            m_arrTextLibrary_CHT[111] = "图片配对失败。请学习Mark页面中重新学习图片配对模板。";

            m_arrTextLibrary_ENG[112] = "Edge tolerance can only goto half of unit size.";
            m_arrTextLibrary_CHS[112] = "边缘容许度必须是元件尺寸的一半或以上。";
            m_arrTextLibrary_CHT[112] = "边缘容许度必须是元件尺寸的一半或以上。";

            m_arrTextLibrary_ENG[113] = "Width or Height cannot be zero.";
            m_arrTextLibrary_CHS[113] = "长宽不可设定为0。";
            m_arrTextLibrary_CHT[113] = "长宽不可设定为0。";

            m_arrTextLibrary_ENG[114] = "Min Width cannot be larger than or equal to Max Width.";
            m_arrTextLibrary_CHS[114] = "宽度最低下限不可超过或等于宽度最高上限。";
            m_arrTextLibrary_CHT[114] = "宽度最低下限不可超过或等于宽度最高上限。";

            m_arrTextLibrary_ENG[115] = "Min Height cannot be larger than or equal to Max Height.";
            m_arrTextLibrary_CHS[115] = "长度最低下限不可超过或等于长度最高上限。";
            m_arrTextLibrary_CHT[115] = "长度最低下限不可超过或等于长度最高上限。";

            m_arrTextLibrary_ENG[116] = "Max Width cannot be smaller than or equal to measured Width.";
            m_arrTextLibrary_CHS[116] = "宽度最高上限不可小过或等于测量宽度。";
            m_arrTextLibrary_CHT[116] = "宽度最高上限不可小过或等于测量宽度。";

            m_arrTextLibrary_ENG[117] = "Max Height cannot be smaller than or equal to measured Height.";
            m_arrTextLibrary_CHS[117] = "长度最高上限不可小过或等于测量长度。";
            m_arrTextLibrary_CHT[117] = "长度最高上限不可小过或等于测量长度。";

            m_arrTextLibrary_ENG[118] = "Min Width cannot be larger than or equal to measured Width.";
            m_arrTextLibrary_CHS[118] = "宽度最低下限不可超过或等于测量宽度。";
            m_arrTextLibrary_CHT[118] = "宽度最低下限不可超过或等于测量宽度。";

            m_arrTextLibrary_ENG[119] = "Min Height cannot be larger than or equal to measured Height.";
            m_arrTextLibrary_CHS[119] = "长度最低下限不可超过或等于测量长度。";
            m_arrTextLibrary_CHT[119] = "长度最低下限不可超过或等于测量长度。";

            m_arrTextLibrary_ENG[120] = "Edge tolerance cannot goes beyond Search ROI.";
            m_arrTextLibrary_CHS[120] = "边缘容许度不可超过Search ROI。";
            m_arrTextLibrary_CHT[120] = "边缘容许度不可超过Search ROI。";

            m_arrTextLibrary_ENG[121] = "Search ROI no exist!";
            m_arrTextLibrary_CHS[121] = "Search ROI不存在！";
            m_arrTextLibrary_CHT[121] = "Search ROI不存在！";

            m_arrTextLibrary_ENG[122] = "Unit ROI no exist!";
            m_arrTextLibrary_CHS[122] = "Unit ROI不存在！";
            m_arrTextLibrary_CHT[122] = "Unit ROI不存在！";

            m_arrTextLibrary_ENG[123] = "Package ROI no exist!";
            m_arrTextLibrary_CHS[123] = "Package ROI不存在！";
            m_arrTextLibrary_CHT[123] = "Package ROI不存在！";

            m_arrTextLibrary_ENG[124] = "Set minimum value or maximum value is not corrects in";
            m_arrTextLibrary_CHS[124] = "设定的最低值或最高数值不正确于";
            m_arrTextLibrary_CHT[124] = "设定的最低值或最高数值不正确于";

            m_arrTextLibrary_ENG[125] = "Please check the red highlight value is correct or not.";
            m_arrTextLibrary_CHS[125] = "请检查红色框数值是否正确。";
            m_arrTextLibrary_CHT[125] = "请检查红色框数值是否正确。";

            m_arrTextLibrary_ENG[126] = "Can not select same pad number!";
            m_arrTextLibrary_CHS[126] = "不可选择相同的衬垫。";
            m_arrTextLibrary_CHT[126] = "不可选择相同的衬垫。";

            m_arrTextLibrary_ENG[127] = "This Pitch/Gap link already exist.";
            m_arrTextLibrary_CHS[127] = "该Pitch/Gap连接已存在。";
            m_arrTextLibrary_CHT[127] = "该Pitch/Gap连接已存在。";

            m_arrTextLibrary_ENG[128] = "Pitch/Gap already defined in pad number";
            m_arrTextLibrary_CHS[128] = "该衬垫的Pitch/Gap已被建立。";
            m_arrTextLibrary_CHT[128] = "该衬垫的Pitch/Gap已被建立。";

            m_arrTextLibrary_ENG[129] = "Pitch/Gap cannot be created in between pad no";
            m_arrTextLibrary_CHS[129] = "Pitch/Gap不可被建立于衬垫";
            m_arrTextLibrary_CHT[129] = "Pitch/Gap不可被建立于衬垫";

            m_arrTextLibrary_ENG[130] = "Gauge measurement in";
            m_arrTextLibrary_CHS[130] = "仪器测量在于";
            m_arrTextLibrary_CHT[130] = "仪器测量在于";

            m_arrTextLibrary_ENG[131] = "ROI is not good. Please adjust the ROI or gauge setting.";
            m_arrTextLibrary_CHS[131] = "ROI不适合。请调整ROI或仪器设定。";
            m_arrTextLibrary_CHT[131] = "ROI不适合。请调整ROI或仪器设定。";

            m_arrTextLibrary_ENG[132] = "Minimum 1 pad is required.";
            m_arrTextLibrary_CHS[132] = "至少必须建立一个衬垫。";
            m_arrTextLibrary_CHT[132] = "至少必须建立一个衬垫";

            m_arrTextLibrary_ENG[133] = "Selected pads is not tally with previous pads record in";
            m_arrTextLibrary_CHS[133] = "被选择的衬垫和之前的衬垫数量不符合";
            m_arrTextLibrary_CHT[133] = "被选择的衬垫和之前的衬垫数量不符合";

            m_arrTextLibrary_ENG[134] = ". There are";
            m_arrTextLibrary_CHS[134] = "。总共有";
            m_arrTextLibrary_CHT[134] = "。总共有";

            m_arrTextLibrary_ENG[135] = "and pad no";
            m_arrTextLibrary_CHS[135] = "和衬垫";
            m_arrTextLibrary_CHT[135] = "和衬垫";

            m_arrTextLibrary_ENG[136] = "measurement";
            m_arrTextLibrary_CHS[136] = "测量";
            m_arrTextLibrary_CHT[136] = "测量";

            m_arrTextLibrary_ENG[137] = "measure";
            m_arrTextLibrary_CHS[137] = "测量";
            m_arrTextLibrary_CHT[137] = "测量";

            m_arrTextLibrary_ENG[138] = "Measure edge fail";
            m_arrTextLibrary_CHS[138] = "测量元件边缘失败";
            m_arrTextLibrary_CHT[138] = "測量元件邊緣失敗";

            m_arrTextLibrary_ENG[139] = "No valid vertical line can be refered";
            m_arrTextLibrary_CHS[139] = "没有适合的纵线可参考";
            m_arrTextLibrary_CHT[139] = "没有适合的纵线可参考";

            m_arrTextLibrary_ENG[140] = "Only 1 gauge has valid result";
            m_arrTextLibrary_CHS[140] = "可参考的元件边缘少于2";
            m_arrTextLibrary_CHT[140] = "可参考的元件边缘少于2";

            m_arrTextLibrary_ENG[141] = "tolerance";
            m_arrTextLibrary_CHS[141] = "容许度";
            m_arrTextLibrary_CHT[141] = "容許度";

            m_arrTextLibrary_ENG[142] = "mark";
            m_arrTextLibrary_CHS[142] = "字模";
            m_arrTextLibrary_CHT[142] = "字模";

            m_arrTextLibrary_ENG[143] = "template";
            m_arrTextLibrary_CHS[143] = "模板";
            m_arrTextLibrary_CHT[143] = "模板";

            m_arrTextLibrary_ENG[144] = "score";
            m_arrTextLibrary_CHS[144] = "相似度";
            m_arrTextLibrary_CHT[144] = "相似度";

            m_arrTextLibrary_ENG[145] = "result";
            m_arrTextLibrary_CHS[145] = "结果";
            m_arrTextLibrary_CHT[145] = "結果";

            m_arrTextLibrary_ENG[146] = "orient";
            m_arrTextLibrary_CHS[146] = "取向";
            m_arrTextLibrary_CHT[146] = "取向";

            m_arrTextLibrary_ENG[147] = "not fulfill min Setting";
            m_arrTextLibrary_CHS[147] = "没有达到最低设置";
            m_arrTextLibrary_CHT[147] = "沒有達到最低设置";

            m_arrTextLibrary_ENG[148] = "Total Extra Mark";
            m_arrTextLibrary_CHS[148] = "额外字模总面积";
            m_arrTextLibrary_CHT[148] = "额外字模总面积";

            m_arrTextLibrary_ENG[149] = "Extra Mark";
            m_arrTextLibrary_CHS[149] = "额外字模面积";
            m_arrTextLibrary_CHT[149] = "额外字模面积";

            m_arrTextLibrary_ENG[150] = "Invalid";
            m_arrTextLibrary_CHS[150] = "不合格";
            m_arrTextLibrary_CHT[150] = "不合格";

            m_arrTextLibrary_ENG[151] = "Min";
            m_arrTextLibrary_CHS[151] = "最小";
            m_arrTextLibrary_CHT[151] = "最小";

            m_arrTextLibrary_ENG[152] = "Max";
            m_arrTextLibrary_CHS[152] = "最大";
            m_arrTextLibrary_CHT[152] = "最大";

            m_arrTextLibrary_ENG[153] = "Empty";
            m_arrTextLibrary_CHS[153] = "无料";
            m_arrTextLibrary_CHT[153] = "無料";

            m_arrTextLibrary_ENG[154] = "No Empty";
            m_arrTextLibrary_CHS[154] = "有料";
            m_arrTextLibrary_CHT[154] = "有料";

            m_arrTextLibrary_ENG[155] = "inside";
            m_arrTextLibrary_CHS[155] = "里面";
            m_arrTextLibrary_CHT[155] = "裏面";

            m_arrTextLibrary_ENG[156] = "Off Set";
            m_arrTextLibrary_CHS[156] = "偏移";
            m_arrTextLibrary_CHT[156] = "偏移";

            m_arrTextLibrary_ENG[157] = "Extra Area";
            m_arrTextLibrary_CHS[157] = "额外面积";
            m_arrTextLibrary_CHT[157] = "額外面積";

            m_arrTextLibrary_ENG[158] = "Total Extra Area";
            m_arrTextLibrary_CHS[158] = "总额外面积";
            m_arrTextLibrary_CHT[158] = "總額外面積";

            m_arrTextLibrary_ENG[159] = "Top Edge Distance";
            m_arrTextLibrary_CHS[159] = "上边缘偏移";
            m_arrTextLibrary_CHT[159] = "上邊緣偏移";

            m_arrTextLibrary_ENG[160] = "Btm Edge Distance";
            m_arrTextLibrary_CHS[160] = "下边缘偏移";
            m_arrTextLibrary_CHT[160] = "下邊緣偏移";

            m_arrTextLibrary_ENG[161] = "Left Edge Distance";
            m_arrTextLibrary_CHS[161] = "左边缘偏移";
            m_arrTextLibrary_CHT[161] = "左邊緣偏移";

            m_arrTextLibrary_ENG[162] = "Right Edge Distance";
            m_arrTextLibrary_CHS[162] = "右边缘偏移";
            m_arrTextLibrary_CHT[162] = "右邊緣偏移";

            m_arrTextLibrary_ENG[163] = "Please close below vision's setting form or test form before exit";
            m_arrTextLibrary_CHS[163] = "请在退出前关闭以下vision的设置表格或测试表格";
            m_arrTextLibrary_CHT[163] = "請在退出前關閉以下vision的設置表格或測試表格";

            m_arrTextLibrary_ENG[164] = "Orient";
            m_arrTextLibrary_CHS[164] = "取向";
            m_arrTextLibrary_CHT[164] = "取向";

            m_arrTextLibrary_ENG[165] = "Package";
            m_arrTextLibrary_CHS[165] = "塑封体";
            m_arrTextLibrary_CHT[165] = "塑封体";

            m_arrTextLibrary_ENG[166] = "sure";
            m_arrTextLibrary_CHS[166] = "确定";
            m_arrTextLibrary_CHT[166] = "確定";

            m_arrTextLibrary_ENG[167] = "Please make sure inspection option";
            m_arrTextLibrary_CHS[167] = "请确保检测选项";
            m_arrTextLibrary_CHT[167] = "請確保檢測選項";

            m_arrTextLibrary_ENG[168] = "are selected before running production";
            m_arrTextLibrary_CHS[168] = "在生产之前选择";
            m_arrTextLibrary_CHT[168] = "在生產之前選擇";

            m_arrTextLibrary_ENG[169] = "Continue Production";
            m_arrTextLibrary_CHS[169] = "继续生产";
            m_arrTextLibrary_CHT[169] = "繼續生產";

            m_arrTextLibrary_ENG[170] = "inspection option";
            m_arrTextLibrary_CHS[170] = "检测选项";
            m_arrTextLibrary_CHT[170] = "檢測選項";

            m_arrTextLibrary_ENG[171] = "not tally with previous inspection option";
            m_arrTextLibrary_CHS[171] = "与之前的检测选项不相同";
            m_arrTextLibrary_CHT[171] = "與之前的檢測選項不相同";

            m_arrTextLibrary_ENG[172] = "is not tally with previous inspection option";
            m_arrTextLibrary_CHS[172] = "与之前的检测选项不相同";
            m_arrTextLibrary_CHT[172] = "與之前的檢測選項不相同";

            m_arrTextLibrary_ENG[173] = "are not tally with previous inspection option";
            m_arrTextLibrary_CHS[173] = "与之前的检测选项不相同";
            m_arrTextLibrary_CHT[173] = "與之前的檢測選項不相同";

            m_arrTextLibrary_ENG[174] = "All";
            m_arrTextLibrary_CHS[174] = "全部";
            m_arrTextLibrary_CHT[174] = "全部";

            m_arrTextLibrary_ENG[175] = "is Unchecked";
            m_arrTextLibrary_CHS[175] = "未选中";
            m_arrTextLibrary_CHT[175] = "未選中";

            m_arrTextLibrary_ENG[176] = "are Unchecked";
            m_arrTextLibrary_CHS[176] = "未选中";
            m_arrTextLibrary_CHT[176] = "未選中";

            m_arrTextLibrary_ENG[177] = "and";
            m_arrTextLibrary_CHS[177] = "以及";
            m_arrTextLibrary_CHT[177] = "以及";

            m_arrTextLibrary_ENG[178] = "Check";
            m_arrTextLibrary_CHS[178] = "检测";
            m_arrTextLibrary_CHT[178] = "檢測";

            m_arrTextLibrary_ENG[179] = "Defect";
            m_arrTextLibrary_CHS[179] = "缺陷";
            m_arrTextLibrary_CHT[179] = "缺陷";

            m_arrTextLibrary_ENG[180] = "Sorry, You can only bypass the fail unit.";
            m_arrTextLibrary_CHS[180] = "抱歉，您只能省略不良元件";
            m_arrTextLibrary_CHT[180] = "抱歉，您只能省略不良元件";

            m_arrTextLibrary_ENG[181] = "Are you sure you want to";
            m_arrTextLibrary_CHS[181] = "您是否确定您要";
            m_arrTextLibrary_CHT[181] = "您是否確定您要";

            m_arrTextLibrary_ENG[182] = "Exit";
            m_arrTextLibrary_CHS[182] = "退出";
            m_arrTextLibrary_CHT[182] = "退出";

            m_arrTextLibrary_ENG[183] = "file format";
            m_arrTextLibrary_CHS[183] = "文件格式";
            m_arrTextLibrary_CHT[183] = "文件格式";

            m_arrTextLibrary_ENG[184] = "Login as";
            m_arrTextLibrary_CHS[184] = "登录";
            m_arrTextLibrary_CHT[184] = "登錄";

            m_arrTextLibrary_ENG[185] = "Sorry";
            m_arrTextLibrary_CHS[185] = "抱歉";
            m_arrTextLibrary_CHT[185] = "抱歉";

            m_arrTextLibrary_ENG[186] = "computer";
            m_arrTextLibrary_CHS[186] = "计算机";
            m_arrTextLibrary_CHT[186] = "電腦";

            m_arrTextLibrary_ENG[187] = "Please Login as higher level";
            m_arrTextLibrary_CHS[187] = "请您登录更高权限的账户";
            m_arrTextLibrary_CHT[187] = "請您登錄更高權限的賬戶";

            m_arrTextLibrary_ENG[188] = " Sorry, You don't have privilege to bypass this unit.";
            m_arrTextLibrary_CHS[188] = "抱歉，您并没有权限省略此元件。";
            m_arrTextLibrary_CHT[188] = "抱歉，您並沒有權限省略此元件。";

            m_arrTextLibrary_ENG[189] = "Program?";
            m_arrTextLibrary_CHS[189] = "程序?";
            m_arrTextLibrary_CHT[189] = "程序?";

            m_arrTextLibrary_ENG[190] = "this";
            m_arrTextLibrary_CHS[190] = "此";
            m_arrTextLibrary_CHT[190] = "此";

            m_arrTextLibrary_ENG[191] = "to bypass";
            m_arrTextLibrary_CHS[191] = "以省略";
            m_arrTextLibrary_CHT[191] = "以省略";

            m_arrTextLibrary_ENG[192] = "Minimum";
            m_arrTextLibrary_CHS[192] = "最低";
            m_arrTextLibrary_CHT[192] = "最低";

            m_arrTextLibrary_ENG[193] = "Maximum";
            m_arrTextLibrary_CHS[193] = "最高值";
            m_arrTextLibrary_CHT[193] = "最高值";

            m_arrTextLibrary_ENG[194] = "Wrong password";
            m_arrTextLibrary_CHS[194] = "密码错误";
            m_arrTextLibrary_CHT[194] = "密碼錯誤";

            m_arrTextLibrary_ENG[195] = "end the lot";
            m_arrTextLibrary_CHS[195] = "结束批次";
            m_arrTextLibrary_CHT[195] = "結束批次";

            m_arrTextLibrary_ENG[196] = "distance";
            m_arrTextLibrary_CHS[196] = "距离";
            m_arrTextLibrary_CHT[196] = "距離";

            m_arrTextLibrary_ENG[197] = "Over Heat during seal";
            m_arrTextLibrary_CHS[197] = "seal 发生过热";
            m_arrTextLibrary_CHT[197] = "seal 發生過熱";

            m_arrTextLibrary_ENG[198] = "start the lot";
            m_arrTextLibrary_CHS[198] = "开始批次";
            m_arrTextLibrary_CHT[198] = "開始批次";

            m_arrTextLibrary_ENG[199] = "start";
            m_arrTextLibrary_CHS[199] = "开始";
            m_arrTextLibrary_CHT[199] = "開始";

            m_arrTextLibrary_ENG[200] = "bypass this unit";
            m_arrTextLibrary_CHS[200] = "省略此元件";
            m_arrTextLibrary_CHT[200] = "省略此元件";

            m_arrTextLibrary_ENG[201] = "out of";
            m_arrTextLibrary_CHS[201] = "超出";
            m_arrTextLibrary_CHT[201] = "超出";

            m_arrTextLibrary_ENG[202] = "make sure";
            m_arrTextLibrary_CHS[202] = "确保";
            m_arrTextLibrary_CHT[202] = "確保";

            m_arrTextLibrary_ENG[203] = "the";
            m_arrTextLibrary_CHS[203] = "这个";
            m_arrTextLibrary_CHT[203] = "這個";

            m_arrTextLibrary_ENG[204] = "value";
            m_arrTextLibrary_CHS[204] = "数值";
            m_arrTextLibrary_CHT[204] = "數值";

            m_arrTextLibrary_ENG[205] = "Vision";
            m_arrTextLibrary_CHS[205] = "Vision";
            m_arrTextLibrary_CHT[205] = "Vision";

            m_arrTextLibrary_ENG[206] = "correct";
            m_arrTextLibrary_CHS[206] = "真确";
            m_arrTextLibrary_CHT[206] = "真確";

            m_arrTextLibrary_ENG[207] = "Bubble";
            m_arrTextLibrary_CHS[207] = "泡泡";
            m_arrTextLibrary_CHT[207] = "泡泡";

            m_arrTextLibrary_ENG[208] = "present in";
            m_arrTextLibrary_CHS[208] = "存在";
            m_arrTextLibrary_CHT[208] = "存在";

            m_arrTextLibrary_ENG[209] = "Fail to find seal Distance blob.";
            m_arrTextLibrary_CHS[209] = "未找到seal距离blob";
            m_arrTextLibrary_CHT[209] = "未找到seal距離blob";

            m_arrTextLibrary_ENG[210] = "White";
            m_arrTextLibrary_CHS[210] = "白色";
            m_arrTextLibrary_CHT[210] = "白色";

            m_arrTextLibrary_ENG[211] = "Black";
            m_arrTextLibrary_CHS[211] = "黑色";
            m_arrTextLibrary_CHT[211] = "黑色";

            m_arrTextLibrary_ENG[212] = "Fail to find";
            m_arrTextLibrary_CHS[212] = "未找到";
            m_arrTextLibrary_CHT[212] = "未找到";

            m_arrTextLibrary_ENG[213] = "Mark!";
            m_arrTextLibrary_CHS[213] = "字模!";
            m_arrTextLibrary_CHT[213] = "字模!";

            m_arrTextLibrary_ENG[214] = "key in";
            m_arrTextLibrary_CHS[214] = "输入";
            m_arrTextLibrary_CHT[214] = "輸入";

            m_arrTextLibrary_ENG[215] = "new recipe";
            m_arrTextLibrary_CHS[215] = "新产品";
            m_arrTextLibrary_CHT[215] = "新產品";

            m_arrTextLibrary_ENG[216] = "name!";
            m_arrTextLibrary_CHS[216] = "名字!";
            m_arrTextLibrary_CHT[216] = "名字!";

            m_arrTextLibrary_ENG[217] = "cannot";
            m_arrTextLibrary_CHS[217] = "不能";
            m_arrTextLibrary_CHT[217] = "不能";

            m_arrTextLibrary_ENG[218] = "Modify";
            m_arrTextLibrary_CHS[218] = "更改";
            m_arrTextLibrary_CHT[218] = "更改";

            m_arrTextLibrary_ENG[219] = "Current Selected Recipe";
            m_arrTextLibrary_CHS[219] = "当前的产品";
            m_arrTextLibrary_CHT[219] = "當前的產品";

            m_arrTextLibrary_ENG[220] = "insufficient at";
            m_arrTextLibrary_CHS[220] = "缺少";
            m_arrTextLibrary_CHT[220] = "缺少";

            m_arrTextLibrary_ENG[221] = "Width is";
            m_arrTextLibrary_CHS[221] = "寬度";
            m_arrTextLibrary_CHT[221] = "寬度";

            m_arrTextLibrary_ENG[222] = "No seal was found in";
            m_arrTextLibrary_CHS[222] = "并没有找到任何seal在";
            m_arrTextLibrary_CHT[222] = "並沒有找到任何seal在";

            m_arrTextLibrary_ENG[223] = "Build Area Set";
            m_arrTextLibrary_CHS[223] = "的设定面积";
            m_arrTextLibrary_CHT[223] = "的設定面積";

            m_arrTextLibrary_ENG[224] = "delete";
            m_arrTextLibrary_CHS[224] = "刪除";
            m_arrTextLibrary_CHT[224] = "删除";

            m_arrTextLibrary_ENG[225] = "Please close";
            m_arrTextLibrary_CHS[225] = "请先关闭";
            m_arrTextLibrary_CHT[225] = "請先關閉";

            m_arrTextLibrary_ENG[226] = "below";
            m_arrTextLibrary_CHS[226] = "下列";
            m_arrTextLibrary_CHT[226] = "下列";

            m_arrTextLibrary_ENG[227] = "Please select character on image first!";
            m_arrTextLibrary_CHS[227] = "请首先选择照片中的字模！";
            m_arrTextLibrary_CHT[227] = "请首先选择照片中的字模！";

            m_arrTextLibrary_ENG[228] = "test form";
            m_arrTextLibrary_CHS[228] = "测试表单";
            m_arrTextLibrary_CHT[228] = "測試表單";

            m_arrTextLibrary_ENG[229] = "before Exit";
            m_arrTextLibrary_CHS[229] = "才退出";
            m_arrTextLibrary_CHT[229] = "才退出";

            m_arrTextLibrary_ENG[230] = "form";
            m_arrTextLibrary_CHS[230] = "表单";
            m_arrTextLibrary_CHT[230] = "表單";

            m_arrTextLibrary_ENG[231] = "before create new lot / end lot";
            m_arrTextLibrary_CHS[231] = "才创建或結束批次";
            m_arrTextLibrary_CHT[231] = "才創建或結束批次";

            m_arrTextLibrary_ENG[232] = "before running production";
            m_arrTextLibrary_CHS[232] = "才开始生产";
            m_arrTextLibrary_CHT[232] = "才開始生產";

            m_arrTextLibrary_ENG[233] = "before login";
            m_arrTextLibrary_CHS[233] = "才登录";
            m_arrTextLibrary_CHT[233] = "才登錄";

            m_arrTextLibrary_ENG[234] = "start new lot before running production.";
            m_arrTextLibrary_CHS[234] = "创建批次才开始生产。";
            m_arrTextLibrary_CHT[234] = "創建批次才開始生產。";

            m_arrTextLibrary_ENG[235] = "imum";
            m_arrTextLibrary_CHS[235] = "";
            m_arrTextLibrary_CHT[235] = "";

            m_arrTextLibrary_ENG[236] = "fail find";
            m_arrTextLibrary_CHS[236] = "未找到";
            m_arrTextLibrary_CHT[236] = "未找到";

            m_arrTextLibrary_ENG[237] = "failed";
            m_arrTextLibrary_CHS[237] = "不合格";
            m_arrTextLibrary_CHT[237] = "不合格";

            m_arrTextLibrary_ENG[238] = "mm";
            m_arrTextLibrary_CHS[238] = "毫米";
            m_arrTextLibrary_CHT[238] = "毫米";

            m_arrTextLibrary_ENG[239] = "you are not a valid user";
            m_arrTextLibrary_CHS[239] = "您不是有效用户";
            m_arrTextLibrary_CHT[239] = "您不是有效用戶";

            m_arrTextLibrary_ENG[240] = "Broken";
            m_arrTextLibrary_CHS[240] = "断开";
            m_arrTextLibrary_CHT[240] = "斷開";

            m_arrTextLibrary_ENG[241] = "Too Many images in this folder.";
            m_arrTextLibrary_CHS[241] = "太多照片存在这文件夾里。";
            m_arrTextLibrary_CHT[241] = "太多照片存在這文件夾里。";

            m_arrTextLibrary_ENG[242] = "Do you want to load all the images? \n";
            m_arrTextLibrary_CHS[242] = "请问您要读取所有的照片吗?   ";
            m_arrTextLibrary_CHT[242] = "請問您要讀取所有的照片嗎?   ";

            m_arrTextLibrary_ENG[243] = "Press No will load 30 images only.";
            m_arrTextLibrary_CHS[243] = "按否会读取30张照片罢了";
            m_arrTextLibrary_CHT[243] = "按否會讀取30張照片罷了";

            m_arrTextLibrary_ENG[244] = "Current";
            m_arrTextLibrary_CHS[244] = "現在的";
            m_arrTextLibrary_CHT[244] = "现在的";

            m_arrTextLibrary_ENG[245] = "Not Found";
            m_arrTextLibrary_CHS[245] = "未找到";
            m_arrTextLibrary_CHT[245] = "未找到";

            m_arrTextLibrary_ENG[246] = "be zero";
            m_arrTextLibrary_CHS[246] = "为零";
            m_arrTextLibrary_CHT[246] = "為零";

            m_arrTextLibrary_ENG[247] = "the following";
            m_arrTextLibrary_CHS[247] = "以下的";
            m_arrTextLibrary_CHT[247] = "以下的";

            m_arrTextLibrary_ENG[248] = "NOT assigned:";
            m_arrTextLibrary_CHS[248] = "没分配：";
            m_arrTextLibrary_CHT[248] = "没分配：";

            m_arrTextLibrary_ENG[249] = "is";
            m_arrTextLibrary_CHS[249] = "是";
            m_arrTextLibrary_CHT[249] = "是";

            m_arrTextLibrary_ENG[250] = "Fail!";
            m_arrTextLibrary_CHS[250] = "不合格!";
            m_arrTextLibrary_CHT[250] = "不合格!";

            m_arrTextLibrary_ENG[251] = "To";
            m_arrTextLibrary_CHS[251] = "至";
            m_arrTextLibrary_CHT[251] = "至";

            m_arrTextLibrary_ENG[252] = "Stand Off";
            m_arrTextLibrary_CHS[252] = "站力度";
            m_arrTextLibrary_CHT[252] = "站力度";


            m_arrTextLibrary_ENG[253] = "to copy";
            m_arrTextLibrary_CHS[253] = "复制";
            m_arrTextLibrary_CHT[253] = "複製";

            m_arrTextLibrary_ENG[254] = "Broken Pad";
            m_arrTextLibrary_CHS[254] = "破损衬垫 ";
            m_arrTextLibrary_CHT[254] = "破損衬垫";

            m_arrTextLibrary_ENG[255] = "Tol Pad";
            m_arrTextLibrary_CHS[255] = "容许度衬垫";
            m_arrTextLibrary_CHT[255] = "容許度衬垫";

            //Rearrange array to put the longest sentence on top
            m_arrText_ENG.Clear();
            m_arrText_CHS.Clear();
            m_arrText_CHT.Clear();
   
            int counter = 0;

            for (int i = 0; i < m_arrTextLibrary_ENG.Length; i++)
            {
                if (m_arrTextLibrary_ENG[i] == null)
                    break;

                m_arrText_ENG.Add(new List<string>());
                m_arrText_CHS.Add(new List<string>());
                m_arrText_CHT.Add(new List<string>());
                m_arrText_ENG[i].Add("");
                m_arrText_CHS[i].Add("");
                m_arrText_CHT[i].Add("");

                for (int j = 0; j < m_arrTextLibrary_ENG.Length; j++)
                {
                    if (m_arrTextLibrary_ENG[j] == null)
                    {
                        counter++;
                        break;
                    }

                    if (m_arrText_ENG[i][0] == "" || m_arrText_CHS[i][0] == "" || m_arrText_CHT[i][0] == "")
                    {
                        m_arrText_ENG[i][0] = m_arrTextLibrary_ENG[counter];
                        m_arrText_CHS[i][0] = m_arrTextLibrary_CHS[counter];
                        m_arrText_CHT[i][0] = m_arrTextLibrary_CHT[counter];
                    }
                    else if (m_arrTextLibrary_ENG[j].IndexOf(m_arrText_ENG[i][0], StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        string[] splitstring = m_arrTextLibrary_ENG[j].Split(' ');

                        for (int k = 0; k < splitstring.Length; k++)
                        {
                            if (splitstring[k].IndexOf(m_arrText_ENG[i][0], StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                if (splitstring[k].Length == m_arrText_ENG[i][0].Length)
                                {
                                    if (m_arrTextLibrary_ENG[j].Length > m_arrText_ENG[i][0].Length)
                                    {
                                        m_arrText_ENG[i].Add(m_arrTextLibrary_ENG[j]);
                                        m_arrText_CHS[i].Add(m_arrTextLibrary_CHS[j]);
                                        m_arrText_CHT[i].Add(m_arrTextLibrary_CHT[j]);
                                        break;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    break;
                            }
                            else
                                continue;
                        }
                    }
                    else
                        continue;
                }
            }
        }
    }
}