using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class ColorGuideLine : Form
    {
        #region Member Variables

        private bool m_blnPaint = false;
        private bool m_blnNextButton = false;
        private bool m_blnInitDone = false;

        private int m_intColorPanelWidth, m_intColorPanelHeight;

        private int m_intRMin = 255;
        private int m_intRMax = 0;
        private int m_intGMin = 255;
        private int m_intGMax = 0;
        private int m_intBMin = 255;
        private int m_intBMax = 0;

        private int[] m_intColor = new int[3];
        private int[] m_intColorTolerance = new int[3];

        private int m_intStepNo = 2;
        private List<int[]> m_arrColorRange = new List<int[]>();
        private List<List<int[]>> m_arrPicture = new List<List<int[]>>();
        private List<List<int>> m_arrColumn = new List<List<int>>();
        private Size m_colorUnit;
        private VisionInfo m_smVisionInfo;

        #endregion

        public ColorGuideLine(VisionInfo smVisionInfo)
        {           
            m_smVisionInfo = smVisionInfo;
            m_intColor = m_smVisionInfo.g_intColorThreshold;
            m_intColorTolerance = m_smVisionInfo.g_intColorTolerance;

            InitializeComponent();

            m_intColorPanelWidth = ColorPanel.Width;
            m_intColorPanelHeight = ColorPanel.Height;

            m_intRMin = m_intColor[0] - m_intColorTolerance[0];
            if (m_intRMin < 0)
                m_intRMin = 0;
            m_intRMax = m_intColor[0] + m_intColorTolerance[0];
            if (m_intRMax > 255)
                m_intRMax = 255;
            m_intGMin = m_intColor[1] - m_intColorTolerance[1];
            if (m_intGMin < 0)
                m_intGMin = 0;
            m_intGMax = m_intColor[1] + m_intColorTolerance[1];
            if (m_intGMax > 255)
                m_intGMax = 255;
            m_intBMin = m_intColor[2] - m_intColorTolerance[2];
            if (m_intBMin < 0)
                m_intBMin = 0;
            m_intBMax = m_intColor[2] + m_intColorTolerance[2];
            if (m_intBMax > 255)
                m_intBMax = 255;
            
            m_smVisionInfo.g_objColorROI.AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_objColorROI.LoadROISetting(10, 10, 100, 100);

            SetupStep();
            cbo_Action.SelectedIndex = 0;
            m_blnInitDone = true;
        }



        private bool CompareColor(int[] intRGB)
        {
            for (int i = 0; i < m_arrColorRange.Count; i++)
            {
                if (m_arrColorRange[i] == intRGB)
                    return true;
            }

            return false;
        }

        private void RepaintColorBar()
        {
            int intTotal = (m_intRMax  - m_intRMin+1) * (m_intGMax - m_intGMin+1) * (1+m_intBMax - m_intBMin);
            if (intTotal == 0)
                return;
            int intX = (panel_ColorBar.Width * panel_ColorBar.Height) / intTotal ;

            int intWidth =1;            
            if (intX > 0)
                intWidth = Convert.ToInt32(Math.Sqrt(intX));

            Bitmap colorBMP = new Bitmap(panel_ColorBar.Width, panel_ColorBar.Height);
            Graphics colorGraphic = Graphics.FromImage(colorBMP);
                 
            int intXNo = 0;
            int intYNo = 0;
            for (int i = m_intRMin; i <= m_intRMax; i++)
            {
                for (int j = m_intGMin; j <= m_intGMax; j++)
                {
                    for (int k = m_intBMin; k <= m_intBMax; k++)
                    {
                        Rectangle rect = new Rectangle(intXNo, intYNo, intWidth, intWidth);
                        Color colorBackground = Color.FromArgb(i,j,k);
                        colorGraphic.FillRectangle(new SolidBrush(colorBackground), rect);
             
                        if (intXNo >= (panel_ColorBar.Width - intWidth))
                        {
                            intYNo += intWidth;
                            intXNo = 0;
                        }
                        else
                        {
                            intXNo += intWidth;
                        }                   
                    }
                }
            }

            panel_ColorBar.BackgroundImage = colorBMP;
        }

        private void RepaintGraphic()
        {
            Bitmap colorBMP = new Bitmap(ColorPanel.Width, ColorPanel.Height);
            Graphics colorGraphic = Graphics.FromImage(colorBMP);
                        
            for (int i = 0; i < m_arrPicture.Count; i++)
            {
                for (int j = 0; j < m_arrPicture[i].Count; j++)
                {
                    int startX = i * m_colorUnit.Width;
                    int startY = j * m_colorUnit.Height;

                    Rectangle rect = new Rectangle(startX, startY, m_colorUnit.Width, m_colorUnit.Height);

                    int[] intRGB = m_arrPicture[i][j];                     

                    Color colorBackground = Color.FromArgb(intRGB[0], intRGB[1], intRGB[2]);

                    colorGraphic.FillRectangle(new SolidBrush(colorBackground), rect);
                    colorGraphic.DrawRectangle(new Pen(new SolidBrush(Color.Black)), rect);
                }
            }

            for (int i = 0; i < m_arrPicture.Count; i++)
            {
                for (int j = 0; j < m_arrPicture[i].Count; j++)
                {
                    int startX = i * m_colorUnit.Width;
                    int startY = j * m_colorUnit.Height;

                    Rectangle rect = new Rectangle(startX, startY, m_colorUnit.Width, m_colorUnit.Height);
                    if (m_arrColumn[i][j] == 1)
                        colorGraphic.DrawRectangle(new Pen(new SolidBrush(Color.Red)), rect);
                }
            }
           
            ColorPanel.BackgroundImage = colorBMP;
        }

        private void SetupStep()
        {
            switch (m_intStepNo)
            {
                case 0:
                    m_smVisionInfo.g_blnDragROI = true;

                    tabCtrl_Lean.SelectedTab = tp_Step1;
                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    break;
                case 1:
                    m_smVisionInfo.g_blnDragROI = false;

                    if (m_blnNextButton)
                    {
                        m_colorUnit = new Size(m_intColorPanelWidth / m_smVisionInfo.g_objColorROI.ref_ROIWidth, m_intColorPanelHeight / m_smVisionInfo.g_objColorROI.ref_ROIHeight);
                        ColorPanel.Width = m_colorUnit.Width * m_smVisionInfo.g_objColorROI.ref_ROIWidth;
                        ColorPanel.Height = m_colorUnit.Height * m_smVisionInfo.g_objColorROI.ref_ROIHeight;
                        
                        m_arrPicture = new List<List<int[]>>();
                        m_arrColumn = new List<List<int>>();
                        for (int i = 0; i < m_smVisionInfo.g_objColorROI.ref_ROIWidth; i++)
                        {
                            m_arrPicture.Add(new List<int[]>());
                            m_arrColumn.Add(new List<int>());
                            for (int j = 0; j < m_smVisionInfo.g_objColorROI.ref_ROIHeight; j++)
                            {
                                m_arrPicture[i].Add(new int[3]);
                                m_arrPicture[i][j] = m_smVisionInfo.g_objColorROI.GetRGBPixelValue(i, j);

                                m_arrColumn[i].Add(new int());
                                m_arrColumn[i][j] = 0;
                            }
                        }
                    }

                    m_blnPaint = true;
                    tabCtrl_Lean.SelectedTab = tp_Step2;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;  
                    break;
                case 2:

                    if (m_blnNextButton)
                    {
                        for (int i = 0; i < m_arrPicture.Count; i++)
                        {
                            for (int j = 0; j < m_arrPicture[i].Count; j++)
                            {
                                int[] intRGB = m_arrPicture[i][j];
                                if (m_intRMin > intRGB[0])
                                    m_intRMin = intRGB[0];
                                if (m_intRMax < intRGB[0])
                                    m_intRMax = intRGB[0];

                                if (m_intGMin > intRGB[1])
                                    m_intGMin = intRGB[1];
                                if (m_intGMax < intRGB[1])
                                    m_intGMax = intRGB[1];

                                if (m_intBMin > intRGB[2])
                                    m_intBMin = intRGB[2];
                                if (m_intBMax < intRGB[2])
                                    m_intBMax = intRGB[2];
                            }
                        }
                    }

                    txt_RedMin.Text = m_intRMin.ToString();
                    txt_RedMax.Text = m_intRMax.ToString();
                    txt_GreenMin.Text = m_intGMin.ToString();
                    txt_GreenMax.Text = m_intGMax.ToString();
                    txt_BlueMin.Text = m_intBMin.ToString();
                    txt_BlueMax.Text = m_intBMax.ToString();
                    RepaintColorBar();

                    tabCtrl_Lean.SelectedTab = tp_Step3;
                    btn_Next.Enabled = false;  
                    break;
            }
        }

     




        private void txt_ColorTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_intRMin = Convert.ToInt32(txt_RedMin.Text);
            m_intRMax = Convert.ToInt32(txt_RedMax.Text);
            m_intGMin = Convert.ToInt32(txt_GreenMin.Text);
            m_intGMax = Convert.ToInt32(txt_GreenMax.Text);
            m_intBMin = Convert.ToInt32(txt_BlueMin.Text);
            m_intBMax = Convert.ToInt32(txt_BlueMax.Text);

            RepaintColorBar();
        }



        private void ColorPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int intColumnX = (e.X / m_colorUnit.Width);
            int intColumnY = (e.Y / m_colorUnit.Height);
            switch (cbo_Action.SelectedIndex)
            {
                case 0:
                    if (m_arrColumn[intColumnX][intColumnY] == 0)
                    {
                        m_arrColumn[intColumnX][intColumnY] = 1;
                    }
                    else
                    {
                        m_arrColumn[intColumnX][intColumnY] = 0;
                    }
                    break;
                case 1:
                    {
                        int intValue = 0;
                        if (m_arrColumn[intColumnX][intColumnY] == 0)
                            intValue = 1;

                        for (int i = 0; i < m_arrPicture[intColumnX].Count; i++)
                        {
                            m_arrColumn[intColumnX][i] = intValue;
                        }
                    }
                    break;
                case 2:
                    {
                        int intValue = 0;
                        if (m_arrColumn[intColumnX][intColumnY] == 0)
                            intValue = 1;

                        for (int i = 0; i < m_arrPicture.Count; i++)
                        {
                            m_arrColumn[i][intColumnY] = intValue;
                        }
                    }
                    break;
            }

        
            m_blnPaint = true;
        }



       

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            m_blnNextButton = true;
            m_intStepNo++;
            SetupStep();
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            m_blnNextButton = false;
            m_intStepNo--;
            SetupStep();
        }

        private void btn_Redo_Click(object sender, EventArgs e)
        {
            m_intStepNo = 0;
            SetupStep();
        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            int intMaxX = 0;
            int intMaxY = 0;
            
            List<List<int[]>> arrTemp = new List<List<int[]>>();
            for (int i = 0; i < m_arrPicture.Count; i++)
            {
                arrTemp.Add(new List<int[]>());
                for (int j = 0; j < m_arrPicture[i].Count; j++)
                {
                    arrTemp[i].Add(new int[3]);
                    arrTemp[i][j] = m_arrPicture[i][j];
                }
            
            }
            m_arrPicture.Clear();
            bool blnUse = true;
            int intXLength = -1;
            int intYLength = -1;
            for (int i = 0; i < arrTemp.Count; i++)
            {
                if (blnUse)
                {
                    m_arrPicture.Add(new List<int[]>());
                    blnUse = false;
                    intXLength++;
                    intYLength = -1;
                }
                for (int j = 0; j < arrTemp[i].Count; j++)
                {
                    if (m_arrColumn[i][j] == 0)
                    {
                        intYLength++;
                        m_arrPicture[intXLength].Add(new int[3]);
                        m_arrPicture[intXLength][intYLength] = arrTemp[i][j];

                        if (intMaxX < intXLength)
                            intMaxX = intXLength;
                        if (intMaxY < intYLength)
                            intMaxY = intYLength;
                        if (!blnUse)
                            blnUse = true;
                    }
                    else
                        m_arrColumn[i][j] = 0;
                }
            }

            if (!blnUse)
            {
                m_arrPicture.RemoveAt((m_arrPicture.Count - 1));
                intMaxX--;
            }

            m_colorUnit = new Size(m_intColorPanelWidth / intMaxX, m_intColorPanelHeight / (intMaxY + 1));
            ColorPanel.Width = m_colorUnit.Width * intMaxX;
            ColorPanel.Height = m_colorUnit.Height * intMaxY;

            m_blnPaint = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            m_intColor[0] = (m_intRMin + m_intRMax) / 2;
            m_intColorTolerance[0] = (m_intRMax - m_intRMin)/2;
            m_intColor[1] = (m_intGMin + m_intGMax) / 2;
            m_intColorTolerance[1] = (m_intGMax - m_intGMin)/2;
            m_intColor[2] = (m_intBMin + m_intBMax) / 2;
            m_intColorTolerance[2] = (m_intBMax - m_intBMin)/2;

            m_smVisionInfo.g_intColorThreshold = m_intColor;
            m_smVisionInfo.g_intColorTolerance = m_intColorTolerance;

            Close();
            Dispose();
        }


        



        private void ColorGuideLine_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.VM_AT_ColorGuideline = true;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void ColorGuideLine_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.VM_AT_ColorGuideline = false;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void ColorPanel_Paint(object sender, PaintEventArgs e)
        {
          //  m_blnPaint = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_blnPaint)
            {
                RepaintGraphic();
                m_blnPaint = false;
            }
        }

     
        
    
       
    }
}