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
using System.IO;

namespace VisionProcessForm
{
    public partial class CalibrationCameraSettingsImageView : Form
    {
        #region Member Variables
        private int m_intImageDisplayMode = 0;// 2020-11-23 ZJYEOH : to replace m_smVisionInfo.g_intImageDisplayMode and always set to 0 as calibration need all lighting to get desired image
        private int m_intTotalLightSourceNo = 0;
        private int m_intGrabCount; // 1: Normal Calibration will only grab one image, 2: Calibration5S will grab two image and merge
        private int m_intUserGroup = 5;
        private int m_intSelectedImage = 0;
        private bool m_blnRepaint = false;
        private bool m_blnInitDone = false;
        private bool m_blnLEDi;
        private string m_strFilePath;
        private string m_strFolderPath;
        private string m_strSelectedRecipe = "";
        private int m_intMergeType_Ori;
        private int m_intMergeType;
        private AVTVimba m_objAVTFireGrab;
        private IDSuEyeCamera m_objIDSCamera;
        private TeliCamera m_objTeliCamera;
        private Graphics m_Graphic;
        private VisionInfo m_smVisionInfo;
        private List<uint> m_arrCameraGainPrev = new List<uint>();
        private List<float> m_arrImageGainPrev = new List<float>();
        private List<float> m_arrCameraShuttlePrev = new List<float>();
        private List<LightSource> m_arrLightSourcePrev = new List<LightSource>();
        private List<SequenceArray> m_arrSeq = new List<SequenceArray>();
        #endregion


        public class SequenceArray
        {
            public int ref_imageNo;
            public int ref_LightSourceNo;
            public int ref_LightSourceArray;
        }

        public CalibrationCameraSettingsImageView(VisionInfo smVisionInfo, bool blnLEDi,
            string strSelectedRecipe, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera, int intUserGroup, string strFolderPath)
        {
            InitializeComponent();

            m_blnLEDi = blnLEDi;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFolderPath = strFolderPath;
            m_smVisionInfo = smVisionInfo;
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                m_strFilePath = strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Camera.xml";
            else
                m_strFilePath = strFolderPath + "Camera.xml";
            m_objAVTFireGrab = objAVTFireGrab;
            m_objIDSCamera = objIDSCamera;
            m_objTeliCamera = objTeliCamera;
            m_intMergeType_Ori = m_smVisionInfo.g_intImageMergeType;
            m_intMergeType = m_smVisionInfo.g_intImageMergeType;
            if (m_intMergeType > 1) // 2019-11-12 ZJYEOH : Required image max is 2
                m_intMergeType = 1;
            //if (m_smVisionInfo.g_strVisionName.Contains("Pad5S"))
            //    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 1;
            //else
            m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;  // View image 1 first bcos selected tabpage is first image also.
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
            pnl_Lighting1.Visible = false;

            pnl_Lighting2.Visible = false;

            pnl_Lighting3.Visible = false;

            pnl_Lighting4.Visible = false;

            pnl_Lighting5.Visible = false;

            pnl_Lighting6.Visible = false;

            pnl_Lighting7.Visible = false;

            pnl_Lighting8.Visible = false;

            pnl_Lighting9.Visible = false;

            //m_intGrabCount = intGrabCount; 
            UpdateGUI();
            //GrabOneTime();

            if (tre_ImageView.SelectedNode == null)
            {
                grpBox_Camera.Enabled = false;
                grpBox_Lighting.Enabled = false;
            }

            m_blnInitDone = true;
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
            objFile.GetFirstSection(m_smVisionInfo.g_strVisionFolderName);
            for (int i = 0; i < m_intGrabCount; i++)
            {
                m_arrCameraShuttlePrev.Add(new float());
                m_arrCameraShuttlePrev[i] = objFile.GetValueAsFloat("Shutter" + i.ToString(), 200f);
            }
            for (int i = 0; i < m_intGrabCount; i++)
            {
                m_arrCameraGainPrev.Add(new uint());
                m_arrCameraGainPrev[i] = objFile.GetValueAsUInt("Gain" + i.ToString(), 20);
            }
            for (int i = 0; i < m_intGrabCount; i++)
            {
                m_arrImageGainPrev.Add(new float());
                m_arrImageGainPrev[i] = objFile.GetValueAsFloat("ImageGain" + i.ToString(), 1f);
            }

            // Keep intensity setting as previous record
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {


                m_arrLightSourcePrev.Add(new LightSource());

                objFile.GetSecondSection(m_smVisionInfo.g_arrLightSource[i].ref_strType);


                m_arrLightSourcePrev[i].ref_intChannel = m_smVisionInfo.g_arrLightSource[i].ref_intChannel;
                m_arrLightSourcePrev[i].ref_intPortNo = m_smVisionInfo.g_arrLightSource[i].ref_intPortNo;
                m_arrLightSourcePrev[i].ref_intSeqNo = m_smVisionInfo.g_arrLightSource[i].ref_intSeqNo;
                m_arrLightSourcePrev[i].ref_arrValue = new List<int>();
                for (int j = 0; j < m_intGrabCount; j++)
                    m_arrLightSourcePrev[i].ref_arrValue.Add(objFile.GetValueAsInt("Seq" + j.ToString(), 100, 2));

                m_arrLightSourcePrev[i].ref_arrImageNo = new List<int>();
                m_arrLightSourcePrev[i].ref_arrImageNo.Add(m_smVisionInfo.g_arrLightSource[i].ref_arrImageNo[0]);

                m_arrLightSourcePrev[i].ref_strCommPort = m_smVisionInfo.g_arrLightSource[i].ref_strCommPort;
                m_arrLightSourcePrev[i].ref_strType = m_smVisionInfo.g_arrLightSource[i].ref_strType;
            }

            for (int i = 0; i < 1; i++)
            {
                TreeNode ImageView = new TreeNode("View " + (i + 1).ToString());
                tre_ImageView.Nodes.Add(ImageView);

                string strImageViewChildName = "";

                // 0 = No Merge, 1 = Merge Grab 1 and Grab 2, 2 = Merge All, 3 = Merge Grab 1 & 2, Grab 3 & 4
                switch (m_smVisionInfo.g_intImageMergeType)
                {
                    default:
                    case 0: // No merge
                        {
                            strImageViewChildName = "View " + (i + 1).ToString();
                            TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                            ImageView.Nodes.Add(ImageViewChild);
                        }
                        break;
                    case 1: // Merge grab 1 center and grab 2 side 
                        {
                            if (i == 0)
                            {
                                strImageViewChildName = "Center Region";
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Side Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                            else
                            {
                                strImageViewChildName = "View " + (i + 1).ToString();
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                        }
                        break;
                    case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                        {
                            if (i == 0)
                            {
                                strImageViewChildName = "Center Region";
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Top Left Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Bottom Right Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                            else
                            {
                                strImageViewChildName = "View " + (i + 1).ToString();
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                        }
                        break;
                    case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                        {
                            if (i == 0)
                            {
                                strImageViewChildName = "Center Region";
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Side Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                            else if (i == 1)
                            {
                                strImageViewChildName = "Center Region";
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Side Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                            else
                            {
                                strImageViewChildName = "View " + (i + 1).ToString();
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                        }
                        break;
                    case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 4 side
                        {
                            if (i == 0)
                            {
                                strImageViewChildName = "Center Region";
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Top Left Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Bottom Right Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);

                            }
                            else if (i == 1)
                            {
                                strImageViewChildName = "Center Region";
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                                strImageViewChildName = "Side Region";
                                ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                            else
                            {
                                strImageViewChildName = "View " + (i + 1).ToString();
                                TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                                ImageView.Nodes.Add(ImageViewChild);
                            }
                        }
                        break;
                }
            }

            tre_ImageView.ExpandAll();

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
                                        txt_Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                        pnl_Lighting1.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light1.Tag = m_arrSeq.Count;
                                        trackBar_Light1.Tag = m_arrSeq.Count;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                        pnl_Lighting2.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light2.Tag = m_arrSeq.Count;
                                        trackBar_Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                        pnl_Lighting3.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light3.Tag = m_arrSeq.Count;
                                        trackBar_Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                        pnl_Lighting4.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light4.Tag = m_arrSeq.Count;
                                        trackBar_Light4.Tag = m_arrSeq.Count;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                        pnl_Lighting5.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light5.Tag = m_arrSeq.Count;
                                        trackBar_Light5.Tag = m_arrSeq.Count;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                        pnl_Lighting6.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light6.Tag = m_arrSeq.Count;
                                        trackBar_Light6.Tag = m_arrSeq.Count;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                        pnl_Lighting7.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light7.Tag = m_arrSeq.Count;
                                        trackBar_Light7.Tag = m_arrSeq.Count;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                        pnl_Lighting8.Visible = true;

                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Light8.Tag = m_arrSeq.Count;
                                        trackBar_Light8.Tag = m_arrSeq.Count;
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
                                        txt_Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                        pnl_Lighting1.Visible = true;
                                        txt_Light1.Tag = m_arrSeq.Count;
                                        trackBar_Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                        pnl_Lighting.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light2.Tag = m_arrSeq.Count;
                                        trackBar_Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                        pnl_Lighting3.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light3.Tag = m_arrSeq.Count;
                                        trackBar_Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                        pnl_Lighting4.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light4.Tag = m_arrSeq.Count;
                                        trackBar_Light4.Tag = m_arrSeq.Count;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                        pnl_Lighting5.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light5.Tag = m_arrSeq.Count;
                                        trackBar_Light5.Tag = m_arrSeq.Count;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                        pnl_Lighting6.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light6.Tag = m_arrSeq.Count;
                                        trackBar_Light6.Tag = m_arrSeq.Count;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                        pnl_Lighting7.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light7.Tag = m_arrSeq.Count;
                                        trackBar_Light7.Tag = m_arrSeq.Count;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                        pnl_Lighting8.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light8.Tag = m_arrSeq.Count;
                                        trackBar_Light8.Tag = m_arrSeq.Count;
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
                                        txt_Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                        pnl_Lighting1.Visible = true;
                                        txt_Light1.Tag = m_arrSeq.Count;
                                        trackBar_Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                        pnl_Lighting2.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light2.Tag = m_arrSeq.Count;
                                        trackBar_Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                        pnl_Lighting3.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light3.Tag = m_arrSeq.Count;
                                        trackBar_Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                        pnl_Lighting4.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light4.Tag = m_arrSeq.Count;
                                        trackBar_Light4.Tag = m_arrSeq.Count;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                        pnl_Lighting5.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light5.Tag = m_arrSeq.Count;
                                        trackBar_Light5.Tag = m_arrSeq.Count;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                        pnl_Lighting6.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light6.Tag = m_arrSeq.Count;
                                        trackBar_Light6.Tag = m_arrSeq.Count;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                        pnl_Lighting7.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light7.Tag = m_arrSeq.Count;
                                        trackBar_Light7.Tag = m_arrSeq.Count;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                        pnl_Lighting8.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light8.Tag = m_arrSeq.Count;
                                        trackBar_Light8.Tag = m_arrSeq.Count;
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
                                        txt_Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                        pnl_Lighting1.Visible = true;
                                        txt_Light1.Tag = m_arrSeq.Count;
                                        trackBar_Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                        pnl_Lighting2.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light2.Tag = m_arrSeq.Count;
                                        trackBar_Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];

                                        trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);
                                        pnl_Lighting3.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light3.Tag = m_arrSeq.Count;
                                        trackBar_Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];

                                        trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);
                                        pnl_Lighting4.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light4.Tag = m_arrSeq.Count;
                                        trackBar_Light4.Tag = m_arrSeq.Count;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];

                                        trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);
                                        pnl_Lighting5.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light5.Tag = m_arrSeq.Count;
                                        trackBar_Light5.Tag = m_arrSeq.Count;
                                        break;
                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];

                                        trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);
                                        pnl_Lighting6.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light6.Tag = m_arrSeq.Count;
                                        trackBar_Light6.Tag = m_arrSeq.Count;
                                        break;
                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];

                                        trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);
                                        pnl_Lighting7.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light7.Tag = m_arrSeq.Count;
                                        trackBar_Light7.Tag = m_arrSeq.Count;
                                        break;
                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];

                                        trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);
                                        pnl_Lighting8.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light8.Tag = m_arrSeq.Count;
                                        trackBar_Light8.Tag = m_arrSeq.Count;
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
                                        txt_Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                        pnl_Lighting1.Visible = true;
                                        txt_Light1.Tag = m_arrSeq.Count;
                                        trackBar_Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                        pnl_Lighting2.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light2.Tag = m_arrSeq.Count;
                                        trackBar_Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                        pnl_Lighting3.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light3.Tag = m_arrSeq.Count;
                                        trackBar_Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                        pnl_Lighting4.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light4.Tag = m_arrSeq.Count;
                                        trackBar_Light4.Tag = m_arrSeq.Count;
                                        break;
                                    case 5:
                                        lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light5.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                        pnl_Lighting5.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light5.Tag = m_arrSeq.Count;
                                        trackBar_Light5.Tag = m_arrSeq.Count;
                                        break;

                                    case 6:
                                        lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light6.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                        pnl_Lighting6.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light6.Tag = m_arrSeq.Count;
                                        trackBar_Light6.Tag = m_arrSeq.Count;
                                        break;

                                    case 7:
                                        lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light7.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                        pnl_Lighting7.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light7.Tag = m_arrSeq.Count;
                                        trackBar_Light7.Tag = m_arrSeq.Count;
                                        break;

                                    case 8:
                                        lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Light8.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                        pnl_Lighting8.Visible = true;

                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Light8.Tag = m_arrSeq.Count;
                                        trackBar_Light8.Tag = m_arrSeq.Count;
                                        break;
                                }
                                break;
                        }
                        if (intLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                        {
                            if (i == 0)
                                m_intTotalLightSourceNo = intLightSourceNo;
                            //UpdateSideDisplayMode(i, intLightSourceNo);
                        }
                        m_arrSeq.Add(objSeq);
                    }
                }
                if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                    UpdateDisplayMode(i);
            }
        }

        private void UpdateSetting(int intImageIndex)
        {
            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

            if (m_objIDSCamera != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
                {
                    if (intImageIndex == i)

                        txt_Shutter.Value = Math.Min(100, (int)(m_smVisionInfo.g_arrCameraShuttle[i] * 10));
                    trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);
                }

                for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
                {
                    if (intImageIndex == i)
                        txt_Gain.Value = Math.Min(50, m_smVisionInfo.g_arrCameraGain[i]);
                    trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
                }

                for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
                {
                    if (intImageIndex == i)
                        if (txt_Gain.Value == 50)
                            txt_Gain.Value += Convert.ToUInt32((m_smVisionInfo.g_arrImageGain[i] - 1) / 0.18f);
                    trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
                }


            }
            else if (m_objTeliCamera != null)
            {
                int intShuttleValue = 0;

                for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
                {
                    if (intImageIndex == i)
                        intShuttleValue = (int)Math.Round(m_smVisionInfo.g_arrCameraShuttle[i] / 100f, 0, MidpointRounding.AwayFromZero);
                    if (intShuttleValue > 0 && intShuttleValue <= 100)
                        txt_Shutter.Value = intShuttleValue;
                    else
                        txt_Shutter.Value = 10;
                    trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);
                }

                for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
                {
                    if (intImageIndex == i)
                        txt_Gain.Value = Math.Min(50, m_smVisionInfo.g_arrCameraGain[i]);
                    trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
                }

                for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
                {
                    if (intImageIndex == i)
                        if (txt_Gain.Value == 50)
                            txt_Gain.Value += Convert.ToUInt32((m_smVisionInfo.g_arrImageGain[i] - 1) / 0.18f);
                    trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
                }
            }
            else if (m_objAVTFireGrab != null)
            {
                int intShuttleValue = 0;

                for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
                {
                    if (intImageIndex == i)
                        intShuttleValue = (int)Math.Round(m_smVisionInfo.g_arrCameraShuttle[i] / 5f, 0, MidpointRounding.AwayFromZero);
                    if (intShuttleValue > 0 && intShuttleValue <= 100)
                        txt_Shutter.Value = intShuttleValue;
                    else
                        txt_Shutter.Value = 10;
                    trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);
                }

                for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
                {
                    if (intImageIndex == i)
                        txt_Gain.Value = Math.Min(50, m_smVisionInfo.g_arrCameraGain[i]);
                    trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
                }

                for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
                {
                    if (intImageIndex == i)
                        if (txt_Gain.Value == 50)
                            txt_Gain.Value += Convert.ToUInt32((m_smVisionInfo.g_arrImageGain[i] - 1) / 0.18f);
                    trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
                }
            }

            int intLightSourceNo = 0;

            // add image
            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is image i using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << intImageIndex)) > 0)
                {
                    intLightSourceNo++;

                    switch (intImageIndex)
                    {
                        case 0:  // first image view
                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);

                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    pnl_Lighting1.Visible = true;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;


                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;


                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;


                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;


                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;


                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;


                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                        case 1:

                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    pnl_Lighting1.Visible = true;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;

                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;


                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;


                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;


                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;


                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[1];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;


                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                        case 2:

                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                    pnl_Lighting1.Visible = true;
                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;

                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;


                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;


                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;


                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;


                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[2];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;


                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                        case 3:

                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                    pnl_Lighting1.Visible = true;
                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;


                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;


                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;


                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;


                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;


                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[3];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;


                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                        case 4:

                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                    pnl_Lighting1.Visible = true;
                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;


                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;

                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;

                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;

                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;

                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[4];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;

                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                        case 5:

                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                    pnl_Lighting1.Visible = true;
                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;


                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;

                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;

                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;

                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;

                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[5];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;

                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                        case 6:

                            switch (intLightSourceNo)  // light source sequence
                            {
                                case 1:
                                    lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light1.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                                    pnl_Lighting1.Visible = true;
                                    txt_Light1.Tag = intLightSourceNo - 1;
                                    trackBar_Light1.Tag = intLightSourceNo - 1;
                                    break;
                                case 2:
                                    lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light2.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                                    pnl_Lighting2.Visible = true;


                                    txt_Light2.Tag = intLightSourceNo - 1;
                                    trackBar_Light2.Tag = intLightSourceNo - 1;
                                    break;
                                case 3:
                                    lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light3.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                                    pnl_Lighting3.Visible = true;


                                    txt_Light3.Tag = intLightSourceNo - 1;
                                    trackBar_Light3.Tag = intLightSourceNo - 1;
                                    break;
                                case 4:
                                    lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light4.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                                    pnl_Lighting4.Visible = true;

                                    txt_Light4.Tag = intLightSourceNo - 1;
                                    trackBar_Light4.Tag = intLightSourceNo - 1;
                                    break;
                                case 5:
                                    lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light5.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                                    pnl_Lighting5.Visible = true;

                                    txt_Light5.Tag = intLightSourceNo - 1;
                                    trackBar_Light5.Tag = intLightSourceNo - 1;
                                    break;
                                case 6:
                                    lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light6.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                                    pnl_Lighting6.Visible = true;

                                    txt_Light6.Tag = intLightSourceNo - 1;
                                    trackBar_Light6.Tag = intLightSourceNo - 1;
                                    break;
                                case 7:
                                    lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light7.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                                    pnl_Lighting7.Visible = true;

                                    txt_Light7.Tag = intLightSourceNo - 1;
                                    trackBar_Light7.Tag = intLightSourceNo - 1;
                                    break;
                                case 8:
                                    lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                    txt_Light8.Maximum = intMaxLimit;
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] > intMaxLimit)
                                        m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6] = intMaxLimit;
                                    txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[6];
                                    trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                                    pnl_Lighting8.Visible = true;

                                    txt_Light8.Tag = intLightSourceNo - 1;
                                    trackBar_Light8.Tag = intLightSourceNo - 1;
                                    break;
                            }
                            break;
                    }
                    
                }
            }
            if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                UpdateDisplayMode(intImageIndex);
            m_blnInitDone = true;
        }

        private int GetArrayImageIndex(int intUserSelectImageIndex)
        {
            if (intUserSelectImageIndex < 0)
                return 0;

            switch (m_smVisionInfo.g_intImageMergeType)
            {
                default:
                case 0: // No merge
                    {
                        return intUserSelectImageIndex;
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else
                            return (intUserSelectImageIndex + 1);
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                            return (intUserSelectImageIndex + 1);
                        else
                            return (intUserSelectImageIndex + 2);
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 2) // select image 2 which is grab 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select image 1 which is grab 3 and 4
                        {
                            if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 1);
                        }
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 1) // select image 1 which is grab 4 and 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select other than image 0 or 1
                        {
                            if (intUserSelectImageIndex + 3 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 3);
                        }
                    }
            }
        }
        private int GetGrabImageIndex(int intImageViewNo, int intNodeIndex)
        {
            if (intImageViewNo < 0)
                return 0;

            switch (m_smVisionInfo.g_intImageMergeType)
            {
                default:
                case 0: // No merge
                    {
                        return (intImageViewNo - 1);
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intImageViewNo == 1)
                            return intNodeIndex;
                        else
                            return (intImageViewNo);
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intImageViewNo == 1)
                            return intNodeIndex;
                        else
                            return (intImageViewNo + 1);
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intImageViewNo == 1)
                            return intNodeIndex;
                        else if (intImageViewNo == 2)
                            return intImageViewNo + intNodeIndex;
                        else
                            return (intImageViewNo + 1);
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (intImageViewNo == 1)
                            return intNodeIndex;
                        else if (intImageViewNo == 2)
                            return (3 + intNodeIndex);
                        else
                            return (intImageViewNo + 2);
                    }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            //ReadSettings();

            this.Close();
            this.Dispose();
        }
        private void ReadSettings()
        {
            TrackLog objTL = new TrackLog();
           
            m_smVisionInfo.g_arrLightSource = m_arrLightSourcePrev;
            m_smVisionInfo.g_arrCameraGain = m_arrCameraGainPrev;
            m_smVisionInfo.g_arrImageGain = m_arrImageGainPrev;
            m_smVisionInfo.g_arrCameraShuttle = m_arrCameraShuttlePrev;
            
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
                    else
                    {
                        if ((m_smVisionInfo.g_arrLightSource[i].ref_intSeqNo & 0x01) > 0)
                            TCOSIO_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(objLight.ref_arrValue[0]));
                        else
                            TCOSIO_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, 0);

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

                    //Set to stop mode
                    LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                    Thread.Sleep(10);
                    for (int i = 0; i < intImageCount; i++)
                    {
                        int intValue1 = 0;
                        int intValue2 = 0;
                        int intValue3 = 0;
                        int intValue4 = 0;

                        if (intImageCount > 0)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                            {
                                int intValueNo = 0;

                                // Due to some light source only ON for second image so its intensity value is at array no. 0.
                                // So we need to loop to find which array no. is for that image
                                for (int k = 0; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                                {
                                    // if this image no is in array k
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
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                            //Set all light source for sequence light controller for each grab
                            LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);
                            Thread.Sleep(10);
                        }
                    }
                    LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                    Thread.Sleep(100);
                    //Set to run mode
                    LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                }
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            SaveSettings();
            this.Close();
            this.Dispose();
        }
        private void SaveSettings()
        {
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);

            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                objFile.WriteElement1Value("Shutter" + i.ToString(), m_smVisionInfo.g_arrCameraShuttle[i], "Calibration Camera Setting-Image " + (i + 1).ToString(), true);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
            {
                objFile.WriteElement1Value("Gain" + i.ToString(), m_smVisionInfo.g_arrCameraGain[i], "Calibration Camera Setting-Image " + (i + 1).ToString(), true);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                objFile.WriteElement1Value("ImageGain" + i.ToString(), m_smVisionInfo.g_arrImageGain[i], "Calibration Camera Setting-Image " + (i + 1).ToString(), true);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                objFile.WriteElement1Value(m_smVisionInfo.g_arrLightSource[i].ref_strType, "", "Calibration Camera Setting-Image " + (i + 1).ToString(), true);
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[i].ref_arrValue.Count; j++)
                {
                    objFile.WriteElement2Value("Seq" + j.ToString(), m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j], "Calibration Camera Setting-Image " + (m_smVisionInfo.g_arrLightSource[i].ref_arrImageNo[j] + 1).ToString() + "-" + m_smVisionInfo.g_arrLightSource[i].ref_strType, true);
                }
            }

            objFile.WriteEndElement();

#if (RELEASE || Release_2_12 || RTXRelease)
                //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
                if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                {
                    File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Camera.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVisionInfo.g_strVisionFolderName + "Camera.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strFolderPath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                        {
                            File.Copy(m_strFolderPath + "\\" + m_smVisionInfo.g_strVisionFolderName + "Camera.xml", arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_smVisionInfo.g_strVisionFolderName + "Camera.xml", true);
                        }
                    }
                }
#elif (DEBUG || Debug_2_12 || RTXDebug)
            //2021-08-19 ZJYEOH : Overwrite global xml
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
            {
                File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Camera.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVisionInfo.g_strVisionFolderName + "Camera.xml", true);
            }
#endif
        }
        private void CalibrationCameraSettingsImageView_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            //m_smVisionInfo.VM_AT_DisableImageLoad = false;
            m_smVisionInfo.g_blnViewNormalImage = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void CalibrationCameraSettingsImageView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // m_smVisionInfo.VM_AT_DisableImageLoad = true;
            //m_smVisionInfo.g_blnViewNormalImage = false;
            //m_smVisionInfo.g_blnViewPackageImage = false;
            //m_smVisionInfo.g_blnDragROI = false;
            //m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_intImageMergeType = m_intMergeType_Ori;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void SetAllLightingPanelInvisible()
        {
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting1, 8);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting2, 7);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting3, 6);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting4, 5);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting5, 4);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting6, 3);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting7, 2);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting8, 1);
            pnl_Lighting.Controls.SetChildIndex(pnl_Lighting9, 0);

            if (pnl_Lighting1.Visible)
                pnl_Lighting1.Visible = false;
            if (pnl_Lighting2.Visible)
                pnl_Lighting2.Visible = false;
            if (pnl_Lighting3.Visible)
                pnl_Lighting3.Visible = false;
            if (pnl_Lighting4.Visible)
                pnl_Lighting4.Visible = false;
            if (pnl_Lighting5.Visible)
                pnl_Lighting5.Visible = false;
            if (pnl_Lighting6.Visible)
                pnl_Lighting6.Visible = false;
            if (pnl_Lighting7.Visible)
                pnl_Lighting7.Visible = false;
            if (pnl_Lighting8.Visible)
                pnl_Lighting8.Visible = false;
            if (pnl_Lighting9.Visible)
                pnl_Lighting9.Visible = false;
        }
        private void tre_ImageView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            //always expand tree node
            e.Cancel = true;
        }
        private void tre_ImageView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!m_blnInitDone)
                return;
            SetAllLightingPanelInvisible();
            TreeNode selectedNode, ParentNode;
            int intselectedNodeIndex;
            selectedNode = e.Node;
            intselectedNodeIndex = selectedNode.Index;
            ParentNode = selectedNode.Parent;
            if (ParentNode != null)
            {

                m_blnInitDone = false;
                grpBox_Camera.Enabled = true;
                grpBox_Lighting.Enabled = true;

                if (ParentNode.Nodes.Count == 1)
                {
                    int intImageViewNo = Convert.ToInt32(ParentNode.Text.ToString().Substring(ParentNode.Text.ToString().Length - 1, 1));
                    m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(intImageViewNo - 1);
                    UpdateSetting(GetGrabImageIndex(intImageViewNo, intselectedNodeIndex));
                    m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
                else
                {
                    int intImageViewNo = Convert.ToInt32(ParentNode.Text.ToString().Substring(ParentNode.Text.ToString().Length - 1, 1));
                    m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(intImageViewNo - 1);
                    UpdateSetting(GetGrabImageIndex(intImageViewNo, intselectedNodeIndex));
                    m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
            }
            else
            {
                m_blnInitDone = false;

                grpBox_Camera.Enabled = false;
                grpBox_Lighting.Enabled = false;

                int intImageViewNo = Convert.ToInt32(selectedNode.Text.ToString().Substring(selectedNode.Text.ToString().Length - 1, 1));
                m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(intImageViewNo - 1);
                UpdateSetting(GetGrabImageIndex(intImageViewNo, intselectedNodeIndex));
                m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void txt_Shutter_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (tre_ImageView.SelectedNode == null)
                return;
            int intSelectedImage = 0;

            int intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));

            intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);

            float fValue = Convert.ToSingle(((NumericUpDown)sender).Value);
            trackBar_Shutter.Value = Convert.ToInt32(((NumericUpDown)sender).Value);
            if (m_objIDSCamera != null)
            {
                //m_objIDSCamera.SetShuttle(Convert.ToSingle(((NumericUpDown)sender).Value));

                fValue = fValue * 0.1f;
            }
            else if (m_objAVTFireGrab != null)
            {
                fValue = fValue * 100f;   // 1% == Shuttle value 100. For AVTVimba, ST 1 == 100 microsecond. So, 100% == 100% * 100 = ST 10000 = 10ms

                if (intSelectedImage == 0 && !m_objAVTFireGrab.SetCameraParameter(1, (uint)fValue))
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }
            else if (m_objTeliCamera != null)
            {
                fValue = fValue * 100f;     // 1% == Shuttle value 100. For Teli, ST 1 == 1 microsecond. So, 100% == 100% * 100 = ST 10000 = 10000 * 1 microsecond = 10ms

                //2021-01-25 ZJYEOH : Dont set here, as we will set again during grabing
                //if (intSelectedImage == 0 && !m_objTeliCamera.SetCameraParameter(1, fValue))
                //    SRMMessageBox.Show("Fail to set camera's shuttle");
            }

            m_smVisionInfo.g_arrCameraShuttle[intSelectedImage] = fValue;
            
        }

        private void trackBar_Shutter_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Shutter.Value = Convert.ToInt32(trackBar_Shutter.Value);
        }

        private void txt_Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (tre_ImageView.SelectedNode == null)
                return;
            int intSelectedImage = 0;

            int intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));

            intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);

            UInt32 intValue = Convert.ToUInt32(((NumericUpDown)sender).Value);
            trackBar_Gain.Value = Convert.ToInt32(((NumericUpDown)sender).Value);
            if (intValue <= 50)
            {
                m_smVisionInfo.g_arrImageGain[intSelectedImage] = 1;

                if (m_objIDSCamera != null)
                {
                    m_objIDSCamera.SetGain(Convert.ToInt32(((NumericUpDown)sender).Value));
                }
                else if (m_objAVTFireGrab != null)
                {

                    //double dValue = (double)Math.Round((float)intValue * 0.46f, 0, MidpointRounding.AwayFromZero);
                    double dValue = intValue;

                    if (intSelectedImage == 0 && !m_objAVTFireGrab.SetCameraParameter(2, dValue))
                        SRMMessageBox.Show("Fail to set camera's gain");
                }
                else if (m_objTeliCamera != null)
                {
                    //TrackLog objTL = new TrackLog();
                    //objTL.WriteLine("User Set value =" + intValue.ToString());
                    //float fValue = (float)Math.Round((float)intValue * 0.46f, 0, MidpointRounding.AwayFromZero);
                    //objTL.WriteLine("User Set value After convert =" + intValue.ToString());

                    float fValue = intValue;

                    //2021-01-25 ZJYEOH : Dont set here, as we will set again during grabing
                    //if (intSelectedImage == 0 && !m_objTeliCamera.SetCameraParameter(2, fValue))
                    //    SRMMessageBox.Show("Fail to set camera's gain");
                }

                m_smVisionInfo.g_arrCameraGain[intSelectedImage] = intValue;
            }
            else
            {
                if (m_objIDSCamera != null)
                {
                    m_objIDSCamera.SetGain(50);
                }
                else if (m_objAVTFireGrab != null)
                {

                    //double dValue = (double)Math.Round((float)intValue * 0.46f, 0, MidpointRounding.AwayFromZero);
                    double dValue = 50;

                    if (intSelectedImage == 0 && !m_objAVTFireGrab.SetCameraParameter(2, dValue))
                        SRMMessageBox.Show("Fail to set camera's gain");
                }
                else if (m_objTeliCamera != null)
                {
                    //TrackLog objTL = new TrackLog();
                    //objTL.WriteLine("User Set value =" + intValue.ToString());
                    //float fValue = (float)Math.Round((float)intValue * 0.46f, 0, MidpointRounding.AwayFromZero);
                    //objTL.WriteLine("User Set value After convert =" + intValue.ToString());

                    float fValue = 50;

                    //2021-01-25 ZJYEOH : Dont set here, as we will set again during grabing
                    //if (intSelectedImage == 0 && !m_objTeliCamera.SetCameraParameter(2, fValue))
                    //    SRMMessageBox.Show("Fail to set camera's gain");
                }
                m_smVisionInfo.g_arrCameraGain[intSelectedImage] = 50;
                m_smVisionInfo.g_arrImageGain[intSelectedImage] = 1 + (intValue - 50) * 0.18f;
            }
        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Gain.Value = Convert.ToInt32(trackBar_Gain.Value);
        }

        private void txt_Light_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (tre_ImageView.SelectedNode == null)
                return;
            int intSelectedImage = 0;
            int intImageViewNo = 0;

            intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));
            STTrackLog.WriteLine("intImageViewNo = " + intImageViewNo);
            intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);

            STTrackLog.WriteLine("intSelectedImage = " + intSelectedImage);

            int intTag = Convert.ToInt32(((NumericUpDown)sender).Tag);
            STTrackLog.WriteLine("Sendor = " + ((NumericUpDown)sender).Name);
            STTrackLog.WriteLine("Tag = " + ((NumericUpDown)sender).Tag.ToString());

            int intSeq = 0;
            for (int i = 0; i < m_arrSeq.Count; i++)
            {
                if (intTag == m_arrSeq[i].ref_LightSourceNo && intSelectedImage == m_arrSeq[i].ref_imageNo)
                {
                    STTrackLog.WriteLine("m_arrSeq[i].ref_LightSourceNo = " + m_arrSeq[i].ref_LightSourceNo.ToString());
                    STTrackLog.WriteLine("m_arrSeq[i].ref_imageNo = " + m_arrSeq[i].ref_imageNo.ToString());
                    intSeq = i;
                    break;
                }
            }

            List<int> arrCOMList = new List<int>();
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

            LightSource objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intSeq].ref_LightSourceNo];

            int intSelectedCOMIndex = -1;
            for (int c = 0; c < arrCOMList.Count; c++)
            {
                if (arrCOMList[c] == objLight.ref_intPortNo)
                    intSelectedCOMIndex = c;
            }

            if (intSelectedCOMIndex < 0)
                return;

            STTrackLog.WriteLine("Port = " + objLight.ref_intPortNo.ToString());
            STTrackLog.WriteLine("m_intTotalLightSourceNo = " + m_intTotalLightSourceNo.ToString());
            int intValue = Convert.ToInt32(((NumericUpDown)sender).Value);
            if (Convert.ToInt32(trackBar_Light1.Tag) == intTag)
            {
                trackBar_Light1.Value = intValue;
                if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2)
                {
                    switch (intSelectedImage)
                    {
                        case 1: // Top Left
                            break;
                        case 2: // Bottom Right
                            if (m_intImageDisplayMode == 2) // Lead
                            {
                                txt_Light3.Value = intValue;
                            }
                            break;
                        case 0: // Center
                        default: // Other
                            if (m_intImageDisplayMode == 1) // Pad
                            {
                                txt_Light2.Value = intValue;
                            }
                            else if (m_intImageDisplayMode == 2) // Lead
                            {
                                txt_Light2.Value = intValue;
                                txt_Light3.Value = intValue;
                                txt_Light4.Value = intValue;
                            }
                            break;
                    }
                }
            }
            else if (Convert.ToInt32(trackBar_Light2.Tag) == intTag)
            {
                trackBar_Light2.Value = intValue;
                if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2)
                {
                    switch (intSelectedImage)
                    {
                        case 1: // Top Left
                            if (m_intImageDisplayMode == 2) // Lead
                            {
                                txt_Light4.Value = intValue;
                            }
                            break;
                    }
                }
            }
            else if (Convert.ToInt32(trackBar_Light3.Tag) == intTag)
            {
                trackBar_Light3.Value = intValue;
                if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2 && (intSelectedImage == 0 || intSelectedImage > 2))
                {
                    if (m_intImageDisplayMode == 1) // Pad
                    {
                        txt_Light4.Value = intValue;
                    }
                }
            }
            else if (Convert.ToInt32(trackBar_Light4.Tag) == intTag)
                trackBar_Light4.Value = intValue;
            else if (Convert.ToInt32(trackBar_Light5.Tag) == intTag)
            {
                trackBar_Light5.Value = intValue;
                if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2 && (intSelectedImage == 0 || intSelectedImage > 2))
                {
                    if (m_intImageDisplayMode == 1) // Pad
                    {
                        txt_Light6.Value = intValue;
                    }
                    else if (m_intImageDisplayMode == 2) // Lead
                    {
                        txt_Light6.Value = intValue;
                    }
                }
            }
            else if (Convert.ToInt32(trackBar_Light6.Tag) == intTag)
                trackBar_Light6.Value = intValue;
            else if (Convert.ToInt32(trackBar_Light7.Tag) == intTag)
            {
                trackBar_Light7.Value = intValue;
                if (m_intTotalLightSourceNo > 4 && m_smVisionInfo.g_intImageMergeType == 2 && (intSelectedImage == 0 || intSelectedImage > 2))
                {
                    if (m_intImageDisplayMode == 1) // Pad
                    {
                        txt_Light8.Value = intValue;
                    }
                    else if (m_intImageDisplayMode == 2) // Lead
                    {
                        txt_Light8.Value = intValue;
                    }
                }
            }
            else if (Convert.ToInt32(trackBar_Light8.Tag) == intTag)
                trackBar_Light8.Value = intValue;
            else if (Convert.ToInt32(trackBar_Light9.Tag) == intTag)
                trackBar_Light9.Value = intValue;

            objLight.ref_arrValue[m_arrSeq[intSeq].ref_LightSourceArray] = intValue;

            if (m_smVisionInfo.g_intLightControllerType == 1)
            {
                //if (m_smVisionInfo.g_arrImages.Count == 1)    // Prevent both side intensity during camera live (Warning: This condition should apply if intensity change during grab image)
                {
                    if (m_blnLEDi)
                    {
                        LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(intValue));
                        //Thread.Sleep(15);
                    }
                    else
                        TCOSIO_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, intValue);
                }
            }
            else
            {
                if (m_blnLEDi)
                {
                    m_smVisionInfo.AT_PR_PauseLiveImage = true;

                    while (m_smVisionInfo.g_blnGrabbing)
                    {
                        Thread.Sleep(1);
                    }

                    int intValue1 = 0;
                    int intValue2 = 0;
                    int intValue3 = 0;
                    int intValue4 = 0;
                    int intLightSourceUsed = 0;

                    //For sequential light controller
                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                    {
                        // Check is selected image using the light source
                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << intSelectedImage)) > 0)
                        {
                            if (intSelectedCOMIndex == 0)
                            {
                                switch (intSelectedImage)
                                {
                                    case 0:  // first image view
                                        switch (j)  // light source
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                    case 1:
                                        switch (j)
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                    case 2:
                                        switch (j)  // light source sequence
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                    case 3:
                                        switch (j)  // light source sequence
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                    case 4:
                                        switch (j)  // light source sequence
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                    case 5:
                                        switch (j)  // light source sequence
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                    case 6:
                                        switch (j)  // light source sequence
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                        }
                                        break;
                                }
                            }
                            else // Second Controller
                            {
                                switch (j)  // light source
                                {
                                    case 4:
                                        intValue1 = Convert.ToInt32(txt_Light5.Value);
                                        intLightSourceUsed++;
                                        break;
                                    case 5:
                                        if (intLightSourceUsed > 0)
                                            intValue2 = Convert.ToInt32(txt_Light6.Value);
                                        else
                                            intValue2 = Convert.ToInt32(txt_Light5.Value);
                                        intLightSourceUsed++;
                                        break;
                                    case 6:
                                        if (intLightSourceUsed > 1)
                                            intValue3 = Convert.ToInt32(txt_Light7.Value);
                                        else if (intLightSourceUsed > 0)
                                            intValue3 = Convert.ToInt32(txt_Light6.Value);
                                        else
                                            intValue3 = Convert.ToInt32(txt_Light5.Value);
                                        intLightSourceUsed++;
                                        break;
                                    case 7:
                                        if (intLightSourceUsed > 2)
                                            intValue4 = Convert.ToInt32(txt_Light8.Value);
                                        else if (intLightSourceUsed > 1)
                                            intValue4 = Convert.ToInt32(txt_Light7.Value);
                                        else if (intLightSourceUsed > 0)
                                            intValue4 = Convert.ToInt32(txt_Light6.Value);
                                        else
                                            intValue4 = Convert.ToInt32(txt_Light5.Value);
                                        intLightSourceUsed++;
                                        break;
                                }
                            }
                    }
                    }

                    //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                    //Thread.Sleep(10);
                    //LEDi_Control.SetSeqIntensity(objLight.ref_intPortNo, 0, intSelectedImage, intValue1, intValue2, intValue3, intValue4);
                    //Thread.Sleep(10);
                    //LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                    //Thread.Sleep(100);
                    //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    //Thread.Sleep(10);
                    //m_smVisionInfo.AT_PR_PauseLiveImage = false;
                    LEDi_Control.RunStop(objLight.ref_intPortNo, 0, false);
                    Thread.Sleep(10);
                    LEDi_Control.SetSeqIntensity(objLight.ref_intPortNo, 0, intSelectedImage, intValue1, intValue2, intValue3, intValue4);
                    STTrackLog.WriteLine("Port=" + objLight.ref_intPortNo.ToString() + ", " +
                                         "ImgeNo=" + intSelectedImage.ToString() + ", " +
                                         "v1=" + intValue1.ToString() + ", " +
                                         "v2=" + intValue2.ToString() + ", " +
                                         "v3=" + intValue3.ToString() + ", " +
                                         "v4=" + intValue4.ToString());
                    Thread.Sleep(10);
                    LEDi_Control.SaveIntensity(objLight.ref_intPortNo, 0);
                    Thread.Sleep(100);
                    LEDi_Control.RunStop(objLight.ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                    m_smVisionInfo.AT_PR_PauseLiveImage = false;
                }
            }
        }

        private void trackBar_Light_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            int intTag = Convert.ToInt32(((TrackBar)sender).Tag);

            if (Convert.ToInt32(txt_Light1.Tag) == intTag)
                txt_Light1.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light2.Tag) == intTag)
                txt_Light2.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light3.Tag) == intTag)
                txt_Light3.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light4.Tag) == intTag)
                txt_Light4.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light5.Tag) == intTag)
                txt_Light5.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light6.Tag) == intTag)
                txt_Light6.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light7.Tag) == intTag)
                txt_Light7.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light8.Tag) == intTag)
                txt_Light8.Value = Convert.ToInt32(((TrackBar)sender).Value);
            else if (Convert.ToInt32(txt_Light9.Tag) == intTag)
                txt_Light9.Value = Convert.ToInt32(((TrackBar)sender).Value);
        }

        private void tre_ImageView_Enter(object sender, EventArgs e)
        {
            if (tre_ImageView.SelectedNode != null)
            {
                tre_ImageView.SelectedNode.BackColor = Color.Empty;
                tre_ImageView.SelectedNode.ForeColor = Color.Empty;
            }
        }

        private void tre_ImageView_Leave(object sender, EventArgs e)
        {
            if (tre_ImageView.SelectedNode != null)
            {
                tre_ImageView.SelectedNode.BackColor = SystemColors.Highlight;
                tre_ImageView.SelectedNode.ForeColor = Color.White;
            }
        }
        private void UpdateDisplayMode(int intImageIndex)
        {
            // 2020-10-10 ZJYEOH : image view format refer Z:\Backup\SRM Optical Vision Systems\MISC Doc\SRMVision Programing Guidline\Lighting 8 Channel Rename.xls
            switch (m_intImageDisplayMode)
            {
                case 0: // Standard

                    break;
                case 1: // Pad
                    {
                        switch (intImageIndex)
                        {
                            case 1: // Top Left
                                //Channel 1 remain, Channel 2 Set zero
                                lbl_Light1.Text = "Side Blue TL";
                                txt_Light2.Value = 0;
                                pnl_Lighting2.Visible = false;

                                //Channel 3 remain, Channel 4 Set zero
                                lbl_Light3.Text = "Side Red TL";
                                txt_Light4.Value = 0;
                                pnl_Lighting4.Visible = false;

                                //Channel 5 remain, Channel 6 Set zero
                                lbl_Light5.Text = "Direct TL";
                                txt_Light6.Value = 0;
                                pnl_Lighting6.Visible = false;

                                //Channel 7 remain, Channel 8 Set zero
                                lbl_Light7.Text = "Coaxial TL";
                                txt_Light8.Value = 0;
                                pnl_Lighting8.Visible = false;
                                break;
                            case 2: // Bottom Right
                                //Channel 1 Set zero, Channel 2 remain
                                lbl_Light2.Text = "Side Blue BR";
                                txt_Light1.Value = 0;
                                pnl_Lighting1.Visible = false;

                                //Channel 3 Set zero, Channel 4 remain
                                lbl_Light4.Text = "Side Red BR";
                                txt_Light3.Value = 0;
                                pnl_Lighting3.Visible = false;

                                //Channel 5 Set zero, Channel 6 remain
                                lbl_Light6.Text = "Direct BR";
                                txt_Light5.Value = 0;
                                pnl_Lighting5.Visible = false;

                                //Channel 7 Set zero, Channel 8 remain
                                lbl_Light8.Text = "Coaxial BR";
                                txt_Light7.Value = 0;
                                pnl_Lighting7.Visible = false;
                                break;
                            case 0: // Center
                            default: // Other
                                //Channel 1 & 2 Combine
                                lbl_Light1.Text = "Side Blue All";
                                txt_Light2.Value = txt_Light1.Value;
                                pnl_Lighting2.Visible = false;

                                //Channel 3 & 4 Combine
                                lbl_Light3.Text = "Side Red All";
                                txt_Light4.Value = txt_Light3.Value;
                                pnl_Lighting4.Visible = false;

                                //Channel 5 & 6 Combine
                                lbl_Light5.Text = "Direct All";
                                txt_Light6.Value = txt_Light5.Value;
                                pnl_Lighting6.Visible = false;

                                //Channel 7 & 8 Combine
                                lbl_Light7.Text = "Coaxial All";
                                txt_Light8.Value = txt_Light7.Value;
                                pnl_Lighting8.Visible = false;
                                break;
                        }
                    }
                    break;
                case 2: // Lead
                    {
                        switch (intImageIndex)
                        {
                            // 2020-10-10 ZJYEOH : Side Light will trigger in opposite direction for Lead3D
                            case 1:
                                //Channel 2 & 4 Combine, Channel 1 & 3 set to zero
                                lbl_Light2.Text = "Side TL";
                                txt_Light4.Value = txt_Light2.Value;
                                pnl_Lighting4.Visible = false;
                                txt_Light1.Value = 0;
                                pnl_Lighting1.Visible = false;
                                txt_Light3.Value = 0;
                                pnl_Lighting3.Visible = false;

                                //Channel 5 remain, Channel 6 set to zero
                                lbl_Light5.Text = "Direct TL";
                                txt_Light6.Value = 0;
                                pnl_Lighting6.Visible = false;

                                //Channel 7 remain, Channel 8 set to zero
                                lbl_Light7.Text = "Coaxial TL";
                                txt_Light8.Value = 0;
                                pnl_Lighting8.Visible = false;
                                break;
                            case 2:
                                //Channel 1 & 3 Combine, Channel 2 & 4 set to zero
                                lbl_Light1.Text = "Side BR";
                                txt_Light3.Value = txt_Light1.Value;
                                pnl_Lighting3.Visible = false;
                                txt_Light2.Value = 0;
                                pnl_Lighting2.Visible = false;
                                txt_Light4.Value = 0;
                                pnl_Lighting4.Visible = false;

                                //Channel 6 remain, Channel 5 set to zero
                                lbl_Light6.Text = "Direct BR";
                                txt_Light5.Value = 0;
                                pnl_Lighting5.Visible = false;

                                //Channel 8 remain, Channel 7 set to zero
                                lbl_Light8.Text = "Coaxial BR";
                                txt_Light7.Value = 0;
                                pnl_Lighting7.Visible = false;
                                break;
                            case 0:
                            default:
                                //Channel 1 & 2 & 3 & 4 Combine
                                lbl_Light1.Text = "Side All";
                                txt_Light2.Value = txt_Light1.Value;
                                pnl_Lighting2.Visible = false;
                                txt_Light3.Value = txt_Light1.Value;
                                pnl_Lighting3.Visible = false;
                                txt_Light4.Value = txt_Light1.Value;
                                pnl_Lighting4.Visible = false;

                                //Channel 5 & 6 Combine
                                lbl_Light5.Text = "Direct All";
                                txt_Light6.Value = txt_Light5.Value;
                                pnl_Lighting6.Visible = false;

                                //Channel 7 & 8 Combine
                                lbl_Light7.Text = "Coaxial All";
                                txt_Light8.Value = txt_Light7.Value;
                                pnl_Lighting8.Visible = false;
                                break;
                        }
                    }
                    break;
            }

        }
    }
}
