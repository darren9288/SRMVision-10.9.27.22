using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;
using System.IO;

namespace VisionProcessForm
{
    public partial class OCRSettingForm : Form
    {
        //2021-08-05 ZJYEOH : User able to set char type
        #region enum
        public enum CharType
        {
            Any = 0, //represents any character (not including a space).
            Letter = 1, //represents an alphabetic character.
            LetterUC = 2, //represents an uppercase alphabetic character.
            LetterLC = 3, //represents a lowercase alphabetic character.
            Digit = 4, //represents a digit.
            Punctuation = 5, //represents the punctuation characters: ! “ # % & ‘ ( ) * , - . / : ; < > ? @ [ \ ] _ { | } ~
            Symbol = 6, //represents the symbols: $ + - < = > | ~
            LineBreak = 7, //represents a line break.
            Space = 8//(space) represents a space between two words.
        };
        #endregion
        private string[] m_strCharType = new string[9] { ".", "L", "Lu", "Ll", "N", "P", "S", "\n", " " };
        
        private bool m_blnHitChar = false;
        private bool m_blnPaintDone = true;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private int LineCount = 1;
        private int WordCount = 1;
        private int CharHeight = 0;
        private int CharMaxWidth = 0;
        private int CharMinWidth = 0;
        private int SelectedIndex = 0;
        private int Word_SelectedIndex = 0;
        private int Char_SelectedIndex = 0;
        private bool LineChanges = false;
        private bool blnInitdone = false;
        private string TopologyConverted = ".";
        private string m_strSelectedRecipe = "";
        private List<List<int>> CharCount = new List<List<int>>();
        private List<List<List<string>>> arrCharType = new List<List<List<string>>>();
        private List<List<List<PointF>>> arrCharStartPoint = new List<List<List<PointF>>>();

        public OCRSettingForm(VisionInfo smVisionInfo, ProductionInfo smProductionInfo,string selectedRecipe)
        {
            m_smVisionInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = selectedRecipe;
            InitializeComponent();
            UpdateGUI();
            blnInitdone = true;
        }

        private void UpdateGUI()
        {
            string[] arrLine = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue.Split('\n');
            for (int i =0; i < arrLine.Length; i++)
            {
                CharCount.Add(new List<int>());
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
                    CharCount[i].Add(arrCharType[i][j].Count);
                }
            }

            LineCount = Math.Max(CharCount.Count, 1);
            if (CharCount.Count > 0)
                WordCount = Math.Max(CharCount[0].Count, 1);

            //char[] a = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue.ToCharArray();
            //int CharacterCount = 0;
            //for (int i=0; i < a.Length;i++)
            //{
            //    if (a[i] == '\n')
            //    {
            //        CharCount.Add(new List<int>());
            //        CharCount[LineCount - 1].Add(CharacterCount);
            //        LineCount++;
            //        WordCount = 1;
            //        CharacterCount = 0;
            //    }
            //    else if (a[i] == ' ')
            //    {
            //        if (CharCount.Count <= LineCount - 1)
            //            CharCount.Add(new List<int>());

            //        WordCount++;
            //        CharCount[LineCount - 1].Add(CharacterCount);
            //        CharacterCount = 0;
            //    }
            //    else if (a[i] == '.')
            //    {
            //        CharacterCount++;
            //        if (i == a.Length - 1)
            //        {
            //            if (CharCount.Count <= LineCount - 1)
            //                CharCount.Add(new List<int>());

            //            CharCount[LineCount - 1].Add(CharacterCount);
            //        }
            //    }
            //    else if (a[i] == '{')
            //    {
            //        CharacterCount = 0;
            //    }
            //    else if (char.IsDigit(a[i]))
            //        CharacterCount = Int16.Parse(a[i].ToString());
            //    else
            //    {
            //        if (i == a.Length - 1)
            //        {
            //            if(CharCount.Count <= LineCount - 1)
            //                CharCount.Add(new List<int>());

            //            CharCount[LineCount - 1].Add(CharacterCount);
            //        }
            //    }
            //}

            txt_CharMinWidth.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth.ToString();
            txt_CharMaxWidth.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth.ToString();
            txt_CharHeight.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight.ToString();
            txt_LineCount.Text = CharCount.Count.ToString();
            if (CharCount.Count > 0)
                txt_WordCount.Text = CharCount[0].Count.ToString();
            else
                txt_WordCount.Text = "0";
            if (CharCount.Count > 0 && CharCount[0].Count > 0)
                txt_CharCount.Text = CharCount[0][0].ToString();
            else
                txt_CharCount.Text = "0";

            cbo_LineNo.Items.Clear();
            for (int i = 0; i < Int32.Parse(txt_LineCount.Text); i++)
            {
                cbo_LineNo.Items.Add(i + 1);
            }

            cbo_WordNo.Items.Clear();
            for (int i = 0; i < Int32.Parse(txt_WordCount.Text); i++)
            {
                cbo_WordNo.Items.Add(i + 1);
            }

            if (cbo_LineNo.Items.Count > 0)
                cbo_LineNo.SelectedIndex = 0;
            if (cbo_WordNo.Items.Count > 0)
                cbo_WordNo.SelectedIndex = 0;

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            blnInitdone = false;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
           // m_smVisionInfo.g_strVisionFolderName + "\\Mark\\OCRSettings.xml";
            
           // m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].LoadTemplateOCR(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\OCRTemplate.xml");
            //LoadOCRSettings(strPath);
            this.Dispose();
            this.Close();
        }

        private void txt_CharMaxWidth_TextChanged(object sender, EventArgs e)
        {
            if (txt_CharMaxWidth.Text != "")
            {
                int s = 0;
                if (Int32.TryParse(txt_CharMaxWidth.Text, out s))
                   CharMaxWidth = s;
                else
                    SRMMessageBox.Show("Please Enter Valid Input");
            }
        }

        private void txt_CharMinWidth_TextChanged(object sender, EventArgs e)
        {
            if (txt_CharMinWidth.Text != "")
            {
                int s = 0;
                if (Int32.TryParse(txt_CharMinWidth.Text, out s))
                    CharMinWidth = s;
                else
                    SRMMessageBox.Show("Please Enter Valid Input");
            }
        }

        private void txt_CharHeight_TextChanged(object sender, EventArgs e)
        {
            if (txt_CharHeight.Text != "")
            {
                int s = 0;
                if (Int32.TryParse(txt_CharHeight.Text, out s))
                    CharHeight = s;
                else
                    SRMMessageBox.Show("Please Enter Valid Input");
            }
        }

        private void txt_LineCount_TextChanged(object sender, EventArgs e)
        {
            if (txt_LineCount.Text != "")
            {
                if (!blnInitdone)
                    return;

                int s = 0;

                if (Int32.TryParse(txt_LineCount.Text, out s))
                {
                    if (LineCount < s)
                    {
                        for (int i = LineCount + 1; i <= s; i++)
                        {
                            cbo_LineNo.Items.Add(i);
                            CharCount.Add(new List<int>());
                            CharCount[CharCount.Count - 1].Add(1);
                        }
                        for (int i = LineCount + 1; i <= s; i++)
                        {
                            arrCharType.Add(new List<List<string>>());
                            arrCharType[arrCharType.Count -1].Add(new List<string>());
                            arrCharType[arrCharType.Count -1][arrCharType[arrCharType.Count - 1].Count - 1].Add(".");
                        }
                    }
                    else
                    {
                        for (int i = LineCount; s < i; i--)
                        {
                            cbo_LineNo.Items.Remove(i);
                            CharCount.RemoveAt(i - 1);
                        }
                        for (int i = LineCount; s < i; i--)
                        {
                            arrCharType.RemoveAt(i - 1);
                        }
                    }
                    LineCount = s;
                }
                else
                    SRMMessageBox.Show("Please Enter Valid Input");
            }

            if (cbo_LineNo.SelectedIndex == -1 && cbo_LineNo.Items.Count > 0)
                cbo_LineNo.SelectedIndex = 0;
            pnl_pic.Size = new Size(pnl_pic.Size.Width, 450);
            pnl_pic.Refresh();
        }

        private void txt_WordCount_TextChanged(object sender, EventArgs e)
        {
            if (!LineChanges)
            {
                if (!blnInitdone)
                    return;

                if (txt_WordCount.Text != "")
                {
                    int s = 0;
                    if (cbo_WordNo.Items.Count != WordCount)
                        WordCount = cbo_WordNo.Items.Count;

                    if (Int32.TryParse(txt_WordCount.Text, out s))
                    {
                        if (WordCount < s)
                        {
                            for (int i = WordCount + 1; i <= s; i++)
                            {
                                cbo_WordNo.Items.Add(i);
                                CharCount[SelectedIndex].Add(1);
                            }
                            for (int i = WordCount + 1; i <= s; i++)
                            {
                                arrCharType[SelectedIndex].Add(new List<string>());
                                arrCharType[SelectedIndex][arrCharType[SelectedIndex].Count - 1].Add(".");
                            }
                        }
                        else
                        {
                            for (int i = WordCount; s < i; i--)
                            {
                                cbo_WordNo.Items.Remove(i);
                                CharCount[SelectedIndex].RemoveAt(i - 1);
                            }
                            for (int i = WordCount; s < i; i--)
                            {
                                arrCharType[SelectedIndex].RemoveAt(i - 1);
                            }
                        }
                        WordCount = s;
                    }
                    else
                        SRMMessageBox.Show("Please Enter Valid Input");
                }
            }

            if (cbo_WordNo.SelectedIndex == -1 && cbo_WordNo.Items.Count > 0)
                cbo_WordNo.SelectedIndex = 0;
            pnl_pic.Size = new Size(675, pnl_pic.Size.Height);
            pnl_pic.Refresh();
        }

        private void txt_CharCount_TextChanged(object sender, EventArgs e)
        {
            if (!LineChanges)
            {
                if (!blnInitdone)
                    return;

                if (txt_CharCount.Text != "")
                {
                    int s = 0;
                    if (Int32.TryParse(txt_CharCount.Text, out s))
                    {
                        if (CharCount[SelectedIndex][Word_SelectedIndex] < s)
                        {
                            for (int i = CharCount[SelectedIndex][Word_SelectedIndex] + 1; i <= s; i++)
                            {
                                arrCharType[SelectedIndex][Word_SelectedIndex].Add(".");
                            }
                        }
                        else
                        {
                            for (int i = CharCount[SelectedIndex][Word_SelectedIndex]; s < i; i--)
                            {
                                arrCharType[SelectedIndex][Word_SelectedIndex].RemoveAt(i - 1);
                            }
                        }

                        CharCount[SelectedIndex][Word_SelectedIndex] = s;
                    }
                }
            }
            else
                LineChanges = false;
            pnl_pic.Size = new Size(675, pnl_pic.Size.Height);
            pnl_pic.Refresh();
        }

        private void cbo_LineNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!blnInitdone)
                return;
            Char_SelectedIndex = 0;
            Word_SelectedIndex = cbo_WordNo.SelectedIndex = 0;
            SelectedIndex = cbo_LineNo.SelectedIndex;
            int count = CharCount[SelectedIndex].Count;
            LineChanges = true;
           
            cbo_WordNo.Text = count.ToString();

            if (cbo_WordNo.Items.Count != count)
            {
                if (cbo_WordNo.Items.Count > count)
                {
                    for (int i = cbo_WordNo.Items.Count; i > count; i--)
                    {
                        cbo_WordNo.Items.Remove(i);
                    }
                }
                else
                {
                    for (int i = cbo_WordNo.Items.Count + 1; i <= count; i++)
                    {
                        cbo_WordNo.Items.Add(i);
                    }
                }
            }

            txt_WordCount.Text = count.ToString();
            txt_CharCount.Text = CharCount[SelectedIndex][Word_SelectedIndex].ToString();

            if (txt_CharCount.Text == CharCount[SelectedIndex][Word_SelectedIndex].ToString())
                LineChanges = false;

        }

        private void cbo_WordNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Word_SelectedIndex = cbo_WordNo.SelectedIndex;
            txt_CharCount.Text = CharCount[SelectedIndex][Word_SelectedIndex].ToString();
            Char_SelectedIndex = 0;
        }

        
        private void SaveOCRSettings(string strPath)
        {
            XmlParser objFileHandle = new XmlParser(strPath);
            int count1;
            int count2;
            objFileHandle.WriteSectionElement("OCR1",true);
            objFileHandle.WriteElement1Value("LineCount", Int32.Parse(txt_LineCount.Text));
            for (int i = 0; i < CharCount.Count; i++)
            {
                count1 = i + 1;
                objFileHandle.WriteElement1Value("Line" + count1, "");
                objFileHandle.WriteElement2Value("Line" + count1 + "_WordCount", CharCount[i].Count);

                for (int j = 0; j < CharCount[i].Count; j++)
                {
                    count2 = j + 1;
                    objFileHandle.WriteElement2Value("Word" + count2 + "_CharCount", CharCount[i][j]);
                }
            }

            objFileHandle.WriteEndElement();
        }

        private void LoadOCRSettings(string strPath)
        {
            int count1;
            int count2;
            CharCount.Clear();
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("OCR1");
            txt_CharMaxWidth.Text =  m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth.ToString();
            txt_CharMinWidth.Text =  m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth.ToString();
            txt_CharHeight.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight.ToString();
            txt_LineCount.Text = objFileHandle.GetValueAsInt("LineCount", 1).ToString();
            txt_WordCount.Text = objFileHandle.GetValueAsInt("Line1_WordCount", 1).ToString();
            txt_CharCount.Text = objFileHandle.GetValueAsInt("Word1_CharCount", 1).ToString();

            for(int i=0; i< Int32.Parse(txt_LineCount.Text);i++)
            {
                CharCount.Add(new List<int>()); //add line
            }

            for (int i = 0; i < CharCount.Count; i++)
            {
                count1 = i + 1;
                objFileHandle.GetSecondSection("Line" + count1);
                int temp2 = objFileHandle.GetValueAsInt("Line" + count1 + "_WordCount", 1,2);
                for (int j = 0; j < temp2; j++)
                {
                    count2 = j + 1;
                    int temp1 = objFileHandle.GetValueAsInt("Word" + count2 + "_CharCount", 1,2);
                    CharCount[i].Add(temp1);
                }
            }

            for (int i=0; i< (Int32.Parse(txt_LineCount.Text)) ; i++)
            {
                cbo_LineNo.Items.Add(i + 1);
            }

            for(int i =0; i< CharCount[0].Count;i++)
            {
                cbo_WordNo.Items.Add(i + 1);
            }

            cbo_LineNo.Text = 1.ToString();
            cbo_WordNo.Text = 1.ToString();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            bool BlnFirstLine = true;
            bool BlnFirstWord = true;

            //for(int i =0; i < CharCount.Count; i++)
            //{
            //    if (!BlnFirstLine)
            //        TopologyConverted = TopologyConverted + "\n.";

            //    for (int j=0; j<CharCount[i].Count;j++)
            //    {
            //        TopologyConverted = TopologyConverted + "{" + CharCount[i][j].ToString() + "}";
            //        BlnFirstLine = false;
            //    }
            //}

            //2021-08-05 ZJYEOH : Modified so that able to handle more than 1 word and different type topology
            string strTopology = "";
            //for (int i = 0; i < CharCount.Count; i++)
            //{
            //    if (!BlnFirstLine)
            //        strTopology = strTopology + "\n";
            //    for (int j = 0; j < CharCount[i].Count; j++)
            //    {
            //        //strTopology += m_strCharType[Convert.ToInt32(CharType.Any)];
            //        if(j != 0)
            //            strTopology = strTopology + " ";

            //        strTopology = strTopology + ".{" + CharCount[i][j].ToString() + "}";
            //        BlnFirstLine = false;
            //    }
            //}

            for (int i = 0; i < arrCharType.Count; i++)
            {
                if (!BlnFirstLine)
                    strTopology += "\n";
                BlnFirstWord = true;
                for (int j = 0; j < arrCharType[i].Count; j++)
                {
                    if (!BlnFirstWord)
                        strTopology += " ";
                    for (int k = 0; k < arrCharType[i][j].Count; k++)
                    {
                        if (arrCharType[i][j][k].Length == 1)
                            strTopology += arrCharType[i][j][k];
                        else if (arrCharType[i][j][k] == "Lu" || arrCharType[i][j][k] == "Ll")
                        {
                            strTopology += arrCharType[i][j][k];
                        }
                        else
                        {
                            strTopology += "[" + arrCharType[i][j][k] + "]";
                        }
                    }
                    BlnFirstWord = false;
                }
                BlnFirstLine = false;
            }
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //    m_smVisionInfo.g_strVisionFolderName + "\\Mark\\OCRSettings.xml";

            //SaveOCRSettings(strPath);
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SaveTemplateOCR(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\OCRTemplate.xml",true);
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth = CharMaxWidth;
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth = CharMinWidth;
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight = CharHeight;
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue = strTopology;// TopologyConverted;
            this.Close();
            this.Dispose();
        }

        private void pnl_pic_Paint(object sender, PaintEventArgs e)
        {
            m_blnPaintDone = false;
            int intGapWidth = 8;
            int intGapHeight = 8;
            int intCharWidth = 45;
            int intCharHeight = 80;
            int intWordHeight = intCharHeight + intGapHeight * 2;
            int intLineHeight = intWordHeight + intGapHeight * 2;
            int intLineStringHeight = 18;
            int intTotalHeightForLine = 140;
            int intLineStartY = 20;
            int intWordStartY = intLineStartY + intGapHeight;
            int intCharStartY = intWordStartY + intGapHeight;
            Font objFont = new Font("Verdana", 10, FontStyle.Bold);
            e.Graphics.Clear(Color.Black);
            arrCharStartPoint = new List<List<List<PointF>>>();
            for (int i = 0; i < CharCount.Count; i++)
            {
                arrCharStartPoint.Add(new List<List<PointF>>());
                if (i != 0)
                {
                    intLineStartY += intTotalHeightForLine;
                    intWordStartY += intTotalHeightForLine;
                    intCharStartY += intTotalHeightForLine;
                }
                int intLineStartX = 5;
                int intWordStartX = intLineStartX + intGapWidth;
                int intCharStartX = intWordStartX + intGapWidth;

                for (int j = 0; j < CharCount[i].Count; j++)
                {
                    arrCharStartPoint[i].Add(new List<PointF>());
                    e.Graphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle(intWordStartX, intWordStartY, (CharCount[i][j] * intCharWidth) + intGapWidth + intGapWidth, intWordHeight));
                    intWordStartX += (CharCount[i][j] * intCharWidth) + intGapWidth + intGapWidth;

                    for (int k = 0; k < CharCount[i][j]; k++)
                    {
                        arrCharStartPoint[i][j].Add(new PointF(intCharStartX, intCharStartY));
                        e.Graphics.DrawRectangle(new Pen(Color.LightGreen, 2), new Rectangle(intCharStartX, intCharStartY, intCharWidth, intCharHeight));
                        e.Graphics.DrawString(arrCharType[i][j][k], objFont, new SolidBrush(Color.LightGreen), intCharStartX + intCharWidth / 2 - (objFont.Size * arrCharType[i][j][k].Length) / 2, intCharStartY + intCharHeight / 2 - objFont.Height / 2);
                        intCharStartX += intCharWidth;
                    }
                    if (j == CharCount[i].Count - 1)
                    {
                        e.Graphics.DrawString("Line " + (i + 1), objFont, new SolidBrush(Color.Red), intLineStartX, intLineStartY - intLineStringHeight);
                        e.Graphics.DrawRectangle(new Pen(Color.Aquamarine, 2), new Rectangle(intLineStartX, intLineStartY, intWordStartX, intLineHeight));

                        if (intLineStartX + intWordStartX > pnl_pic.Size.Width)
                            pnl_pic.Size = new Size(675 + (intLineStartX + intWordStartX + intGapWidth + intGapWidth - pnl_pic.Size.Width) + intGapWidth, pnl_pic.Size.Height);
                    }
                    else if (CharCount[i].Count > 1)
                    {
                        intWordStartX += intGapWidth;
                        intCharStartX = intWordStartX + intGapWidth;
                    }
                }
            }

            if (CharCount.Count * (intTotalHeightForLine) + 20 > pnl_pic.Size.Height)
                pnl_pic.Size = new Size(pnl_pic.Size.Width, CharCount.Count * (intTotalHeightForLine) + 20);

            m_blnPaintDone = true;
        }

        private void ToolStripMenuItem_CharType_Click(object sender, EventArgs e)
        {
            if (arrCharType.Count > cbo_LineNo.SelectedIndex &&
                arrCharType[cbo_LineNo.SelectedIndex].Count > cbo_WordNo.SelectedIndex &&
                arrCharType[cbo_LineNo.SelectedIndex][cbo_WordNo.SelectedIndex].Count > Char_SelectedIndex)
            {
                //string strCharType = arrCharType[cbo_LineNo.SelectedIndex][cbo_WordNo.SelectedIndex][Char_SelectedIndex];
                //switch (((ToolStripMenuItem)sender).Name.ToString())
                //{
                //    case "ToolStripMenuItem_Any":
                //        strCharType = ".";
                //        break;
                //    case "ToolStripMenuItem_Letter":
                //        strCharType = "L";
                //        break;
                //    case "ToolStripMenuItem_LetterUppercase":
                //        strCharType = "Lu";
                //        break;
                //    case "ToolStripMenuItem_LetterLowercase":
                //        strCharType = "Ll";
                //        break;
                //    case "ToolStripMenuItem_Digit":
                //        strCharType = "N";
                //        break;
                //    case "ToolStripMenuItem_Punctuation":
                //        strCharType = "P";
                //        break;
                //    case "ToolStripMenuItem_Symbol":
                //        strCharType = "S";
                //        break;
                //}

                string strCharType_New = "";
                if (((ToolStripMenuItem)sender).Name == ToolStripMenuItem_Any.Name)
                {
                    strCharType_New = ".";
                }
                else
                {
                    if (ToolStripMenuItem_Letter.Checked && ((ToolStripMenuItem)sender).Name != ToolStripMenuItem_Letter.Name)
                        strCharType_New += "L";
                    else if (!ToolStripMenuItem_Letter.Checked && ((ToolStripMenuItem)sender).Name == ToolStripMenuItem_Letter.Name)
                        strCharType_New += "L";

                    if (ToolStripMenuItem_LetterUppercase.Checked && ((ToolStripMenuItem)sender).Name != ToolStripMenuItem_LetterUppercase.Name)
                        strCharType_New += "Lu";
                    else if (!ToolStripMenuItem_LetterUppercase.Checked && ((ToolStripMenuItem)sender).Name == ToolStripMenuItem_LetterUppercase.Name)
                        strCharType_New += "Lu";

                    if (ToolStripMenuItem_LetterLowercase.Checked && ((ToolStripMenuItem)sender).Name != ToolStripMenuItem_LetterLowercase.Name)
                        strCharType_New += "Ll";
                    else if (!ToolStripMenuItem_LetterLowercase.Checked && ((ToolStripMenuItem)sender).Name == ToolStripMenuItem_LetterLowercase.Name)
                        strCharType_New += "Ll";

                    if (strCharType_New == "LLu")
                    {
                        if (ToolStripMenuItem_Letter.Checked)
                            strCharType_New = "Lu";
                        else
                            strCharType_New = "L";
                    }
                    else if (strCharType_New == "LLl")
                    {
                        if (ToolStripMenuItem_Letter.Checked)
                            strCharType_New = "Ll";
                        else
                            strCharType_New = "L";
                    }
                    else if (strCharType_New == "LuLl")
                    {
                        if (ToolStripMenuItem_LetterUppercase.Checked)
                            strCharType_New = "Ll";
                        else
                            strCharType_New = "Lu";
                    }
                    else if (strCharType_New == "LLuLl")
                    {
                        strCharType_New = "L";
                    }

                    if (ToolStripMenuItem_Digit.Checked && ((ToolStripMenuItem)sender).Name != ToolStripMenuItem_Digit.Name)
                        strCharType_New += "N";
                    else if (!ToolStripMenuItem_Digit.Checked && ((ToolStripMenuItem)sender).Name == ToolStripMenuItem_Digit.Name)
                        strCharType_New += "N";

                    if (ToolStripMenuItem_Punctuation.Checked && ((ToolStripMenuItem)sender).Name != ToolStripMenuItem_Punctuation.Name)
                        strCharType_New += "P";
                    else if (!ToolStripMenuItem_Punctuation.Checked && ((ToolStripMenuItem)sender).Name == ToolStripMenuItem_Punctuation.Name)
                        strCharType_New += "P";

                    if (ToolStripMenuItem_Symbol.Checked && ((ToolStripMenuItem)sender).Name != ToolStripMenuItem_Symbol.Name)
                        strCharType_New += "S";
                    else if (!ToolStripMenuItem_Symbol.Checked && ((ToolStripMenuItem)sender).Name == ToolStripMenuItem_Symbol.Name)
                        strCharType_New += "S";
                }
                
                if (strCharType_New == "" || (strCharType_New.Contains("L") && strCharType_New.Contains("N") && strCharType_New.Contains("P") && strCharType_New.Contains("S") && !(strCharType_New.Contains("u") || strCharType_New.Contains("l"))))
                    strCharType_New = ".";

                arrCharType[cbo_LineNo.SelectedIndex][cbo_WordNo.SelectedIndex][Char_SelectedIndex] = strCharType_New;
            }
            pnl_pic.Refresh();
        }

        private void pnl_pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (!m_blnPaintDone)
                return;

            int intPositionX = e.X;
            int intPositionY = e.Y;
            m_blnHitChar = false;
            int intCharWidth = 45;
            int intCharHeight = 80;

            for (int i = 0; i < arrCharStartPoint.Count; i++)
            {
                for (int j = 0; j < arrCharStartPoint[i].Count; j++)
                {
                    for (int k = 0; k < arrCharStartPoint[i][j].Count; k++)
                    {
                        if (intPositionY >= arrCharStartPoint[i][j][k].Y &&
                                 intPositionY <= arrCharStartPoint[i][j][k].Y + intCharHeight &&
                                 intPositionX >= arrCharStartPoint[i][j][k].X &&
                                 intPositionX <= arrCharStartPoint[i][j][k].X + intCharWidth)
                        {
                            if (e.Button == MouseButtons.Right)
                                m_blnHitChar = true;
                            cbo_LineNo.SelectedIndex = i;
                            cbo_WordNo.SelectedIndex = j;
                            Char_SelectedIndex = k;
                            break;
                        }
                        else if (intPositionX > arrCharStartPoint[i][j][0].X + (intCharWidth * arrCharStartPoint[i][j].Count))
                        {
                            goto NextWord;
                        }
                        else if (intPositionY > arrCharStartPoint[i][j][k].Y + intCharHeight)
                        {
                            goto NextLine;
                        }
                    }
                        NextWord:;
                }
                NextLine:;
            }

        }

        private void contextMenuStrip_ModifyChar_Opening(object sender, CancelEventArgs e)
        {
            if (!m_blnPaintDone || !m_blnHitChar)
                e.Cancel = true;

        }

        private void contextMenuStrip_ModifyChar_Opened(object sender, EventArgs e)
        {
            UnTickAllContextMenuStrip();
            if (arrCharType.Count > cbo_LineNo.SelectedIndex &&
                arrCharType[cbo_LineNo.SelectedIndex].Count > cbo_WordNo.SelectedIndex &&
                arrCharType[cbo_LineNo.SelectedIndex][cbo_WordNo.SelectedIndex].Count > Char_SelectedIndex)
            {
                char[] arrChar = arrCharType[cbo_LineNo.SelectedIndex][cbo_WordNo.SelectedIndex][Char_SelectedIndex].ToCharArray();
                for (int k = 0; k < arrChar.Length; k++)
                {
                    if (arrChar[k] == 'L' && arrChar.Length > (k + 1) && (arrChar[k + 1] == 'u' || arrChar[k + 1] == 'l'))
                    {
                        TickContextMenuStrip((arrChar[k].ToString() + arrChar[k + 1].ToString()).ToString());
                        k++;
                    }
                    else
                        TickContextMenuStrip(arrChar[k].ToString());
                }
            }
        }
        private void UnTickAllContextMenuStrip()
        {
            ToolStripMenuItem_Any.Checked = false;
            ToolStripMenuItem_Letter.Checked = false;
            ToolStripMenuItem_LetterUppercase.Checked = false;
            ToolStripMenuItem_LetterLowercase.Checked = false;
            ToolStripMenuItem_Digit.Checked = false;
            ToolStripMenuItem_Punctuation.Checked = false;
            ToolStripMenuItem_Symbol.Checked = false;
        }
        private void TickContextMenuStrip(string strCharType)
        {
            switch (strCharType)
            {
                case ".":
                    ToolStripMenuItem_Any.Checked = true;
                    break;
                case "L":
                    ToolStripMenuItem_Letter.Checked = true;
                    break;
                case "Lu":
                    ToolStripMenuItem_LetterUppercase.Checked = true;
                    break;
                case "Ll":
                    ToolStripMenuItem_LetterLowercase.Checked = true;
                    break;
                case "N":
                    ToolStripMenuItem_Digit.Checked = true;
                    break;
                case "P":
                    ToolStripMenuItem_Punctuation.Checked = true;
                    break;
                case "S":
                    ToolStripMenuItem_Symbol.Checked = true;
                    break;
            }
        }
    }
}
