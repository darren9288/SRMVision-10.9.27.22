using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using ImageAcquisition;
using Lighting;
using SharedMemory;
using VisionProcessing;
using System.Threading;

namespace VisionProcessForm
{
    public partial class TestLightningForm : Form
    {
        public class SequenceArray
        {
            public int ref_imageNo;
            public int ref_LightSourceNo;
            public int ref_LightSourceArray;
        }

        //variable
        private int m_intTotalLightSourceNo = 0;
        private bool m_blnLEDi;
        private bool[] m_blnSelected = new bool[8];
        private bool m_blnRunTest = false;
        private bool m_blnLiveAtBegin = false;
        private VisionInfo m_smVisionInfo;
        private List<uint> m_arrCameraGainPrev = new List<uint>();
        private List<float> m_arrImageGainPrev = new List<float>();
        private List<float> m_arrCameraShuttlePrev = new List<float>();
        private List<LightSource> m_arrLightSourcePrev = new List<LightSource>();
        private List<SequenceArray> m_arrSeq = new List<SequenceArray>();
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private string m_strFilePath = "";
        private string m_strSelectedRecipe = "";
        private int intValue1 = 0;
        private int intValue2 = 0;
        private int intValue3 = 0;
        private int intValue4 = 0;
        private int intImageCount = 0;
        private int m_intMergeType_Ori;
        private int m_intGrabCount = 0;
        private LightSource objLight;
        private int intSelectedImage = 0;           
        private float m_fShutterPrev;
        private List<int> arrCOMList = new List<int>();


        public TestLightningForm(VisionInfo smVisionInfo, bool blnLEDi, bool blnVT,
               string strSelectedRecipe, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera,
               ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;

            m_blnLEDi = blnLEDi;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\Camera.xml";
            m_smVisionInfo = smVisionInfo;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            InitializeComponent();
            DisableAllPanel();
            m_intMergeType_Ori = m_smVisionInfo.g_intImageMergeType;
            switch (m_intMergeType_Ori)
            {
                case 0: // No Merge 
                default:
                    m_intGrabCount = 1;
                    break;
                case 1: // Merge Grab 1 & 2
                case 3: // Merge Grab 1 & 2, Grab 3 & 4
                    m_intGrabCount = 2;
                    break;
                case 2: // Merge All
                case 4: // Merge Grab 1 & 2 & 3, Grab 4 & 5
                    m_intGrabCount = 3;
                    break;
            }
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);
            grpBox_Lighting.Text = "Lighting (1-" + intMaxLimit.ToString() + ")";
            // Get camera grab delay, shuttle and gain setting from camera file to keep as previous setting
            objFile = new XmlParser(m_strFilePath);
            m_fShutterPrev = objFile.GetValueAsFloat("Shutter", 200f);
            m_smVisionInfo.g_fCameraShuttle = m_fShutterPrev;

            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                m_arrCameraShuttlePrev.Add(new float());
                m_arrCameraShuttlePrev[i] = m_smVisionInfo.g_arrCameraShuttle[i];
            }

            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
            {
                m_arrCameraGainPrev.Add(new uint());
                m_arrCameraGainPrev[i] = m_smVisionInfo.g_arrCameraGain[i];
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_arrImageGainPrev.Add(new float());
                m_arrImageGainPrev[i] = m_smVisionInfo.g_arrImageGain[i];
            }



            // Keep intensity setting as previous record
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                m_arrLightSourcePrev.Add(new LightSource());
                m_arrLightSourcePrev[i].ref_intChannel = m_smVisionInfo.g_arrLightSource[i].ref_intChannel;
                m_arrLightSourcePrev[i].ref_intPortNo = m_smVisionInfo.g_arrLightSource[i].ref_intPortNo;
                m_arrLightSourcePrev[i].ref_intSeqNo = m_smVisionInfo.g_arrLightSource[i].ref_intSeqNo;
                m_arrLightSourcePrev[i].ref_intValue = m_smVisionInfo.g_arrLightSource[i].ref_intValue;
                m_arrLightSourcePrev[i].ref_PHValue = m_smVisionInfo.g_arrLightSource[i].ref_PHValue;
                m_arrLightSourcePrev[i].ref_EmptyValue = m_smVisionInfo.g_arrLightSource[i].ref_EmptyValue;

                m_arrLightSourcePrev[i].ref_arrValue = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[i].ref_arrValue.Count; j++)
                {
                    m_arrLightSourcePrev[i].ref_arrValue.Add(m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j]);
                }

                m_arrLightSourcePrev[i].ref_arrImageNo = new List<int>();
                for (int k = 0; k < m_smVisionInfo.g_arrLightSource[i].ref_arrImageNo.Count; k++)
                {
                    m_arrLightSourcePrev[i].ref_arrImageNo.Add(m_smVisionInfo.g_arrLightSource[i].ref_arrImageNo[k]);
                }

                m_arrLightSourcePrev[i].ref_strCommPort = m_smVisionInfo.g_arrLightSource[i].ref_strCommPort;
                m_arrLightSourcePrev[i].ref_strType = m_smVisionInfo.g_arrLightSource[i].ref_strType;
            }

            if (m_smVisionInfo.g_arrColorImages == null)
                intImageCount = m_smVisionInfo.g_arrImages.Count;
            else if (m_smVisionInfo.g_arrImages == null)
                intImageCount = m_smVisionInfo.g_arrColorImages.Count;
            else
                intImageCount = Math.Max(m_smVisionInfo.g_arrColorImages.Count, m_smVisionInfo.g_arrImages.Count);

            //if (m_smVisionInfo.g_arrColorImages == null)
            //    intImageCount = m_smVisionInfo.g_arrImages.Count;

            //if (m_smVisionInfo.g_intImageMergeType != 0)
            //{
            //    intImageCount = m_smVisionInfo.g_arrImages.Count - m_smVisionInfo.g_intImageMergeType;

            //    if (m_smVisionInfo.g_intImageMergeType == 3)
            //    {
            //        // 2020 03 25 - CCENG: cannot fix image count at 3 because total grab may be 3 or 4.
            //        //intViewImageCount = 3;
            //        intImageCount = m_smVisionInfo.g_arrImages.Count;
            //        if (m_smVisionInfo.g_arrImages.Count >= 2)
            //            intImageCount--;
            //        if (m_smVisionInfo.g_arrImages.Count >= 4)
            //            intImageCount--;
            //    }

            //if (m_smVisionInfo.g_intImageMergeType == 4)
            //{
            //    //intViewImageCount = 2;
            //    intViewImageCount = m_smVisionInfo.g_arrImages.Count;
            //    if (m_smVisionInfo.g_arrImages.Count <= 3)
            //        intViewImageCount = 1;
            //    else if (m_smVisionInfo.g_arrImages.Count <= 5)
            //        intViewImageCount = 2;
            //    else
            //        intViewImageCount = intViewImageCount - 3;
            //}
            //}

            for (int i = 0; i < m_intGrabCount; i++)
            {
                int intLightSourceNo = 0;
                // add image
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                {
                    // Check is image i using the light source
                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                    {
                        intLightSourceNo++;

                        for(int k =0; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count;k++)
                        {
                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[k] = 0;
                        }

                        SequenceArray objSeq = new SequenceArray();
                        objSeq.ref_imageNo = i;
                        objSeq.ref_LightSourceNo = j;


                        switch (i)
                        {
                            case 0:  // first image view
                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light1.Text = 125.ToString();
                                        pnl_Lighting1.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Text = 125.ToString();
                                        pnl_Lighting2.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Text = 125.ToString();
                                        pnl_Lighting3.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Text = 125.ToString();
                                        pnl_Lighting4.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Text = 125.ToString();
                                        pnl_Lighting5.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Text = 125.ToString();
                                        pnl_Lighting6.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Text = 125.ToString();
                                        pnl_Lighting7.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Text = 125.ToString();
                                        pnl_Lighting8.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        break;
                                }
                                break;
                            case 1:
                                int intLightSourceArrayNo = 0;
                                for (int x = 0; x < m_arrSeq.Count; x++)
                                {
                                    if (m_arrSeq[x].ref_LightSourceNo == j)   // check whether this light source is used in previous image
                                    {
                                        intLightSourceArrayNo++;
                                    }
                                }

                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light1.Text = 125.ToString();
                                        pnl_Lighting1.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Text = 125.ToString();
                                        pnl_Lighting.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Text = 125.ToString();
                                        pnl_Lighting3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Text = 125.ToString();
                                        pnl_Lighting4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Text = 125.ToString();
                                        pnl_Lighting5.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Text = 125.ToString();
                                        pnl_Lighting6.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Text = 125.ToString();
                                        pnl_Lighting7.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Text = 125.ToString();
                                        pnl_Lighting8.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                }
                                break;
                            case 2:
                                intLightSourceArrayNo = 0;
                                for (int x = 0; x < m_arrSeq.Count; x++)
                                {
                                    if (m_arrSeq[x].ref_LightSourceNo == j)   // check whether this light source is used in previous image
                                    {
                                        intLightSourceArrayNo++;
                                    }
                                }

                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light1.Text = 125.ToString();
                                        pnl_Lighting1.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Text = 125.ToString();
                                        pnl_Lighting2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Text = 125.ToString();
                                        pnl_Lighting3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Text = 125.ToString();
                                        pnl_Lighting4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Text = 125.ToString();
                                        pnl_Lighting5.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Text = 125.ToString();
                                        pnl_Lighting6.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Text = 125.ToString();
                                        pnl_Lighting7.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Text = 125.ToString();
                                        pnl_Lighting8.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                }
                                break;
                            case 3:
                                intLightSourceArrayNo = 0;
                                for (int x = 0; x < m_arrSeq.Count; x++)
                                {
                                    if (m_arrSeq[x].ref_LightSourceNo == j)   // check whether this light source is used in previous image
                                    {
                                        intLightSourceArrayNo++;
                                    }
                                }

                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light1.Text = 125.ToString();
                                        pnl_Lighting1.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Text = 125.ToString();
                                        pnl_Lighting2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Text = 125.ToString();
                                        pnl_Lighting3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Text = 125.ToString();
                                        pnl_Lighting4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Text = 125.ToString();
                                        pnl_Lighting5.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Text = 125.ToString();
                                        pnl_Lighting6.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Text = 125.ToString();
                                        pnl_Lighting7.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Text = 125.ToString();
                                        pnl_Lighting8.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                }
                                break;
                            case 4:
                                intLightSourceArrayNo = 0;
                                for (int x = 0; x < m_arrSeq.Count; x++)
                                {
                                    if (m_arrSeq[x].ref_LightSourceNo == j)   // check whether this light source is used in previous image
                                    {
                                        intLightSourceArrayNo++;
                                    }
                                }

                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light1.Text = 125.ToString();
                                        pnl_Lighting1.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Text = 125.ToString();
                                        pnl_Lighting2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Text = 125.ToString();
                                        pnl_Lighting3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Text = 125.ToString();
                                        pnl_Lighting4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Text = 125.ToString();
                                        pnl_Lighting5.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;

                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Text = 125.ToString();
                                        pnl_Lighting6.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;

                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Text = 125.ToString();
                                        pnl_Lighting7.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;

                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Text = 125.ToString();
                                        pnl_Lighting8.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                }
                                break;
                        }
                        if (intLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4)) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                        {
                            if (i == 0)
                                m_intTotalLightSourceNo = intLightSourceNo;
                        }
                        m_arrSeq.Add(objSeq);
                    }
                }
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4)) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                {
                    switch (m_smVisionInfo.g_intImageDisplayMode)
                    {
                        case 0: // Standard

                            break;
                        case 1: // Pad
                            {
                                switch (i)
                                {
                                    case 0: // Center
                                    default: // Other
                                             //Channel 1 & 2 Combine
                                        lbl_Light1.Text = "Side Blue All";
                                        txt_Light2.Text = txt_Light1.Text;
                                        pnl_Lighting2.Visible = false;

                                        //Channel 3 & 4 Combine
                                        lbl_Light3.Text = "Side Red All";
                                        txt_Light4.Text = txt_Light3.Text;
                                        pnl_Lighting4.Visible = false;

                                        //Channel 5 & 6 Combine
                                        lbl_Light5.Text = "Direct All";
                                        txt_Light6.Text = txt_Light5.Text;
                                        pnl_Lighting6.Visible = false;

                                        //Channel 7 & 8 Combine
                                        lbl_Light7.Text = "Coaxial All";
                                        txt_Light8.Text = txt_Light7.Text;
                                        pnl_Lighting8.Visible = false;
                                        break;
                                }
                            }
                            break;
                        case 2: // Lead
                            {
                                switch (i)
                                {
                                    // 2020-10-10 ZJYEOH : Side Light will trigger in opposite direction for Lead3D
                                    case 0:
                                    default:
                                        //Channel 1 & 2 & 3 & 4 Combine
                                        lbl_Light1.Text = "Side All";
                                        txt_Light2.Text = txt_Light1.Text;
                                        pnl_Lighting2.Visible = false;
                                        txt_Light3.Text = txt_Light1.Text;
                                        pnl_Lighting3.Visible = false;
                                        txt_Light4.Text = txt_Light1.Text;
                                        pnl_Lighting4.Visible = false;

                                        //Channel 5 & 6 Combine
                                        lbl_Light5.Text = "Direct All";
                                        txt_Light6.Text = txt_Light5.Text;
                                        pnl_Lighting6.Visible = false;

                                        //Channel 7 & 8 Combine
                                        lbl_Light7.Text = "Coaxial All";
                                        txt_Light8.Text = txt_Light7.Text;
                                        pnl_Lighting8.Visible = false;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void DisableAllPanel()
        {
            pnl_Lighting1.Visible = false;
            pnl_Lighting2.Visible = false;
            pnl_Lighting3.Visible = false;
            pnl_Lighting4.Visible = false;
            pnl_Lighting5.Visible = false;
            pnl_Lighting6.Visible = false;
            pnl_Lighting7.Visible = false;
            pnl_Lighting8.Visible = false;
            pnl_Lighting9.Visible = false;
        }

        private void Btn_TestLightCancel_Click(object sender, EventArgs e)
        {
            if(m_blnRunTest)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_PauseLiveImage = true;
                m_blnRunTest = false;
            }

            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewNormalImage = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_blnSeparateGrab = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            ReadSettings();
            m_smVisionInfo.AT_PR_PauseLiveImage = false;

            if (m_blnLiveAtBegin)
                m_smVisionInfo.AT_PR_StartLiveImage = true;

            this.Close();
            this.Dispose();
        }

        private void Btn_TestLightningRun_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.AT_PR_StartLiveImage = true;
            int intLightSourceUsed = 0;
            m_blnRunTest = true;
            m_smVisionInfo.AT_PR_PauseLiveImage = true;
            int selected = Convert.ToInt32(((Button)sender).Tag);
            intValue1 = intValue2 = intValue3 = intValue4 = 0;
            int intSelectedCOMIndex = -1;
            int intSeq = 0;
            txt_Light1.Text = txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 125.ToString();

            if (m_smVisionInfo.g_intImageDisplayMode == 0)
            {
                switch (selected)
                {
                    case 0:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 1:
                        txt_Light1.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 2:
                        txt_Light2.Text = txt_Light1.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 3:
                        txt_Light2.Text = txt_Light3.Text = txt_Light1.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 4:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light1.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 5:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light1.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 6:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light1.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 7:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light1.Text = 0.ToString();
                        break;
                }
            }
            else
            {
                switch (selected)
                {
                    case 0:
                        txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 4:
                        txt_Light1.Text = txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 6:
                        txt_Light1.Text = txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = 0.ToString();
                        break;
                }
            }
            for (int i = 0; i < m_arrSeq.Count; i++)
            {
                if (selected == m_arrSeq[i].ref_LightSourceNo)
                {
                    intSeq = i;
                    break;
                }
            }

            objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intSeq].ref_LightSourceNo];

            for (int c = 0; c < arrCOMList.Count; c++)
            {
                if (arrCOMList[c] == objLight.ref_intPortNo)
                    intSelectedCOMIndex = c;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is selected image using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << intSelectedImage)) > 0)
                {
                    if (intSelectedCOMIndex == 0)
                    {
                        switch (j)  // light source
                        {
                            case 0:
                                intValue1 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                            case 1:
                                if (intLightSourceUsed > 0)
                                    intValue2 = Int32.Parse(txt_Light2.Text);
                                else
                                    intValue2 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                            case 2:
                                if (intLightSourceUsed > 1)
                                    intValue3 = Int32.Parse(txt_Light3.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue3 = Int32.Parse(txt_Light2.Text);
                                else
                                    intValue3 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                            case 3:
                                if (intLightSourceUsed > 2)
                                    intValue4 = Int32.Parse(txt_Light4.Text);
                                else if (intLightSourceUsed > 1)
                                    intValue4 = Int32.Parse(txt_Light3.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue4 = Int32.Parse(txt_Light2.Text);
                                else
                                    intValue4 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                        }
                    }
                    else // Second Controller
                    {
                        switch (j)  // light source
                        {
                            case 4:
                                intValue1 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                            case 5:
                                if (intLightSourceUsed > 0)
                                    intValue2 = Int32.Parse(txt_Light6.Text);
                                else
                                    intValue2 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                            case 6:
                                if (intLightSourceUsed > 1)
                                    intValue3 = Int32.Parse(txt_Light7.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue3 = Int32.Parse(txt_Light6.Text);
                                else
                                    intValue3 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                            case 7:
                                if (intLightSourceUsed > 2)
                                    intValue4 = Int32.Parse(txt_Light8.Text);
                                else if (intLightSourceUsed > 1)
                                    intValue4 = Int32.Parse(txt_Light7.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue4 = Int32.Parse(txt_Light6.Text);
                                else
                                    intValue4 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                        }
                    }
                }
            }

            //set all light off
            for (int k = 0; k < intImageCount; k++)
            {
                for (int i = 0; i < arrCOMList.Count; i++)
                {
                    LEDi_Control.RunStop(objLight.ref_intPortNo, 0, false);
                    Thread.Sleep(10);
                    LEDi_Control.SetSeqIntensity(arrCOMList[i], 0, k, 0, 0, 0, 0);
                    Thread.Sleep(10);
                    LEDi_Control.SaveIntensity(arrCOMList[i], 0);
                    Thread.Sleep(100);
                    LEDi_Control.RunStop(objLight.ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                }
            }

            LEDi_Control.RunStop(objLight.ref_intPortNo, 0, false);
            Thread.Sleep(10);
            LEDi_Control.SetSeqIntensity(objLight.ref_intPortNo, 0, intSelectedImage, intValue1, intValue2, intValue3, intValue4);
            Thread.Sleep(10);
            LEDi_Control.SaveIntensity(objLight.ref_intPortNo, 0);
            Thread.Sleep(100);
            LEDi_Control.RunStop(objLight.ref_intPortNo, 0, true);
            Thread.Sleep(10);
            m_smVisionInfo.AT_PR_PauseLiveImage = false;
        }

        private void Btn_StopTest_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_blnRunTest = false;
        }

        private void TestLightningForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_blncboImageView = false;
            m_smVisionInfo.g_blnViewNormalImage = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (!m_smVisionInfo.AT_PR_StartLiveImage)
                m_blnLiveAtBegin = false;
            else
                m_blnLiveAtBegin = true;

                arrCOMList.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                bool blnFound = false;
                for (int c = 0; c < arrCOMList.Count; c++)
                {
                    if (arrCOMList[c] == m_smVisionInfo.g_arrLightSource[i].ref_intPortNo)
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (!blnFound)
                    arrCOMList.Add(m_smVisionInfo.g_arrLightSource[i].ref_intPortNo);
            }

            //set all light off
            for (int k = 0; k < intImageCount; k++)
            {
                for (int i = 0; i < arrCOMList.Count; i++)
                {
                    LEDi_Control.RunStop(arrCOMList[i], 0, false);
                    Thread.Sleep(10);
                    LEDi_Control.SetSeqIntensity(arrCOMList[i], 0, k, 0, 0, 0, 0);
                    Thread.Sleep(10);
                    LEDi_Control.SaveIntensity(arrCOMList[i], 0);
                    Thread.Sleep(100);
                    LEDi_Control.RunStop(arrCOMList[i], 0, true);
                    Thread.Sleep(10);
                    m_smVisionInfo.AT_PR_PauseLiveImage = false;
                }
            }
        }

        private void ReadSettings()
        {
            TrackLog objTL = new TrackLog();
            m_smVisionInfo.g_arrLightSource = m_arrLightSourcePrev;
            m_smVisionInfo.g_arrCameraGain = m_arrCameraGainPrev;
            m_smVisionInfo.g_arrImageGain = m_arrImageGainPrev;
            m_smVisionInfo.g_arrCameraShuttle = m_arrCameraShuttlePrev;
            m_smVisionInfo.g_fCameraShuttle = m_fShutterPrev;

            if (m_smVisionInfo.g_intLightControllerType == 1)
            {
                // Set for image 1 light source setting
                for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                {
                    LightSource objLight = m_smVisionInfo.g_arrLightSource[i];
                    if (m_blnLEDi)
                    {
                        LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(objLight.ref_arrValue[0]));
                    }
                    System.Threading.Thread.Sleep(5);
                }
            }
            else
            {
                if (m_blnLEDi)
                {
                    int intImageCount;
                    if (m_smVisionInfo.g_arrColorImages == null)
                        intImageCount = m_smVisionInfo.g_arrImages.Count;
                    else if (m_smVisionInfo.g_arrImages == null)
                        intImageCount = m_smVisionInfo.g_arrColorImages.Count;
                    else
                        intImageCount = Math.Max(m_smVisionInfo.g_arrColorImages.Count, m_smVisionInfo.g_arrImages.Count);

                    for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                    {
                        bool blnFound = false;
                        for (int c = 0; c < arrCOMList.Count; c++)
                        {
                            if (arrCOMList[c] == m_smVisionInfo.g_arrLightSource[i].ref_intPortNo)
                            {
                                blnFound = true;
                                break;
                            }
                        }

                        if (!blnFound)
                            arrCOMList.Add(m_smVisionInfo.g_arrLightSource[i].ref_intPortNo);
                    }

                    //Set to stop mode
                    for (int c = 0; c < arrCOMList.Count; c++)
                        LEDi_Control.RunStop(arrCOMList[c], 0, false);  //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);

                    Thread.Sleep(10);
                    for (int i = 0; i < intImageCount; i++)
                    {
                        int intValue1 = 0;
                        int intValue2 = 0;
                        int intValue3 = 0;
                        int intValue4 = 0;
                        int intValue5 = 0;
                        int intValue6 = 0;
                        int intValue7 = 0;
                        int intValue8 = 0;

                        if (intImageCount > 0)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                            {
                                int intValueNo = 0;
                                for (int k = 0; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                                {
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                                    {
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
                                        {
                                            intValueNo = k;

                                            switch (j)
                                            {
                                                case 0:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue1 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 1:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue2 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 2:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue3 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 3:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 4:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue5 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 5:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue6 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 6:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue7 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 7:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue8 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                            if (arrCOMList.Count > 0)
                                LEDi_Control.SetSeqIntensity(arrCOMList[0], 0, i, intValue1, intValue2, intValue3, intValue4);
                            if (arrCOMList.Count > 1)
                                LEDi_Control.SetSeqIntensity(arrCOMList[1], 0, i, intValue5, intValue6, intValue7, intValue8);

                            Thread.Sleep(10);

                        }
                    }

                    for (int c = 0; c < arrCOMList.Count; c++)
                        LEDi_Control.SaveIntensity(arrCOMList[c], 0); //LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                    Thread.Sleep(100);
                    //Set to run mode
                    for (int i = 0; i < arrCOMList.Count; i++)
                        LEDi_Control.RunStop(arrCOMList[i], 0, true);   // LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                }
            }
        }

        private void Btn_Grab1_Click(object sender, EventArgs e)
        {
            int intLightSourceUsed = 0;
            intValue1 = intValue2 = intValue3 = intValue4 = 0;
            int intSelectedCOMIndex = -1;
            int selected = Convert.ToInt32(((Button)sender).Tag);
            int intSeq = 0;
            txt_Light1.Text = txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 125.ToString();

            if (m_smVisionInfo.g_intImageDisplayMode == 0)
            {
                switch (selected)
                {
                    case 0:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 1:
                        txt_Light1.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 2:
                        txt_Light2.Text = txt_Light1.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 3:
                        txt_Light2.Text = txt_Light3.Text = txt_Light1.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 4:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light1.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 5:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light1.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 6:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light1.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 7:
                        txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light1.Text = 0.ToString();
                        break;
                }
            }
            else
            {
                switch (selected)
                {
                    case 0:
                        txt_Light5.Text = txt_Light6.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 4:
                        txt_Light1.Text = txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light7.Text = txt_Light8.Text = 0.ToString();
                        break;
                    case 6:
                        txt_Light1.Text = txt_Light2.Text = txt_Light3.Text = txt_Light4.Text = txt_Light5.Text = txt_Light6.Text = 0.ToString();
                        break;
                }
            }

            for (int i = 0; i < m_arrSeq.Count; i++)
            {
                if (selected == m_arrSeq[i].ref_LightSourceNo)
                {
                    intSeq = i;
                    break;
                }
            }

            objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intSeq].ref_LightSourceNo];


            for (int c = 0; c < arrCOMList.Count; c++)
            {
                if (arrCOMList[c] == objLight.ref_intPortNo)
                    intSelectedCOMIndex = c;
            }


            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is selected image using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << intSelectedImage)) > 0)
                {
                    if (intSelectedCOMIndex == 0)
                    {
                        switch (j)  // light source
                        {
                            case 0:
                                intValue1 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                            case 1:
                                if (intLightSourceUsed > 0)
                                    intValue2 = Int32.Parse(txt_Light2.Text);
                                else
                                    intValue2 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                            case 2:
                                if (intLightSourceUsed > 1)
                                    intValue3 = Int32.Parse(txt_Light3.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue3 = Int32.Parse(txt_Light2.Text);
                                else
                                    intValue3 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                            case 3:
                                if (intLightSourceUsed > 2)
                                    intValue4 = Int32.Parse(txt_Light4.Text);
                                else if (intLightSourceUsed > 1)
                                    intValue4 = Int32.Parse(txt_Light3.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue4 = Int32.Parse(txt_Light2.Text);
                                else
                                    intValue4 = Int32.Parse(txt_Light1.Text);
                                intLightSourceUsed++;
                                break;
                        }
                    }
                    else // Second Controller
                    {
                        switch (j)  // light source
                        {
                            case 4:
                                intValue1 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                            case 5:
                                if (intLightSourceUsed > 0)
                                    intValue2 = Int32.Parse(txt_Light6.Text);
                                else
                                    intValue2 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                            case 6:
                                if (intLightSourceUsed > 1)
                                    intValue3 = Int32.Parse(txt_Light7.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue3 = Int32.Parse(txt_Light6.Text);
                                else
                                    intValue3 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                            case 7:
                                if (intLightSourceUsed > 2)
                                    intValue4 = Int32.Parse(txt_Light8.Text);
                                else if (intLightSourceUsed > 1)
                                    intValue4 = Int32.Parse(txt_Light7.Text);
                                else if (intLightSourceUsed > 0)
                                    intValue4 = Int32.Parse(txt_Light6.Text);
                                else
                                    intValue4 = Int32.Parse(txt_Light5.Text);
                                intLightSourceUsed++;
                                break;
                        }
                    }
                }
            }


            LEDi_Control.RunStop(objLight.ref_intPortNo, 0, false);
            Thread.Sleep(10);
            LEDi_Control.SetSeqIntensity(objLight.ref_intPortNo, 0, intSelectedImage, intValue1, intValue2, intValue3, intValue4);
            Thread.Sleep(10);
            LEDi_Control.SaveIntensity(objLight.ref_intPortNo, 0);
            Thread.Sleep(100);
            LEDi_Control.RunStop(objLight.ref_intPortNo, 0, false);
            Thread.Sleep(10);
            m_smVisionInfo.AT_PR_GrabImage = true;
            m_smVisionInfo.AT_PR_PauseLiveImage = true;
        }
    }
}
