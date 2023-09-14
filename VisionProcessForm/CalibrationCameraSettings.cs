using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
    public partial class CalibrationCameraSettings : Form
    {
        #region Member Variables
        private int m_intGrabCount; // 1: Normal Calibration will only grab one image, 2: Calibration5S will grab two image and merge
        private int m_intUserGroup = 5;
        private int m_intSelectedImage = 0;
        private bool m_blnRepaint = false;
        private bool m_blnInitDone = false;
        private bool m_blnLEDi;
        private string m_strFilePath;
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

        //public CalibrationCameraSettings(VisionInfo smVisionInfo, bool blnLEDi, 
        //    string strSelectedRecipe, AVTFireGrab objAVTFireGrab, int intUserGroup)
        //{
        //    m_blnLEDi = blnLEDi;
        //    m_intUserGroup = intUserGroup;          
        //    m_strSelectedRecipe = strSelectedRecipe;
        //    m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
        //         "\\Camera.xml";

        //    m_objAVTFireGrab = objAVTFireGrab;
        //    m_smVisionInfo = smVisionInfo;

        //    InitializeComponent();

        //    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;  // View image 1 first bcos selected tabpage is first image also.

        //    UpdateGUI();

        //    m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);

        //    m_blnInitDone = true;
        //    m_blnRepaint = true;
        //}

        public CalibrationCameraSettings(VisionInfo smVisionInfo, bool blnLEDi,
            string strSelectedRecipe, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera, int intUserGroup, string strFolderPath)//, int intGrabCount)
        {
            
            InitializeComponent();

            m_blnLEDi = blnLEDi;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFilePath = strFolderPath + "\\Camera.xml";
            m_objAVTFireGrab = objAVTFireGrab;
            m_objIDSCamera = objIDSCamera;
            m_objTeliCamera = objTeliCamera;
            m_smVisionInfo = smVisionInfo;
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
                    m_intGrabCount = 3;
                    break;
            }

            //m_intGrabCount = intGrabCount; 
            UpdateGUI();
            //GrabOneTime();
            m_blnInitDone = true;
            
        }



        private void ReadSettings()
        {
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

                    ////Set to stop mode
                    //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                    //Thread.Sleep(10);
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
                            //Set to stop mode
                            LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                            Thread.Sleep(10);
                            //Set all light source for sequence light controller for each grab
                            LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);
                            Thread.Sleep(10);
                            LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                            Thread.Sleep(100);
                            //Set to run mode
                            LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                            Thread.Sleep(10);
                        }
                    }
                    ////Set to run mode
                    //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    //Thread.Sleep(10);
                }
            }
        }

        private void SaveSettings()
        {
            if (txt_CameraGain1.Value.ToString() == null || txt_CameraGain1.Value.ToString() == "" ||
                txt_CameraShutter1.Value.ToString() == null || txt_CameraShutter1.Value.ToString() == "" ||
                txt_Image1Light1.Value.ToString() == null || txt_Image1Light1.Value.ToString() == "" ||
                txt_Image1Light2.Value.ToString() == null || txt_Image1Light2.Value.ToString() == "" ||
                txt_Image1Light3.Value.ToString() == null || txt_Image1Light3.Value.ToString() == ""||
                     txt_Image1Light4.Value.ToString() == null || txt_Image1Light4.Value.ToString() == ""||
                     ((m_intGrabCount==2) &&
                     (txt_CameraGain2.Value.ToString() == null || txt_CameraGain2.Value.ToString() == "" ||
                txt_CameraShutter2.Value.ToString() == null || txt_CameraShutter2.Value.ToString() == "" ||
                txt_Image2Light1.Value.ToString() == null || txt_Image2Light1.Value.ToString() == "" ||
                txt_Image2Light2.Value.ToString() == null || txt_Image2Light2.Value.ToString() == "" ||
                txt_Image2Light3.Value.ToString() == null || txt_Image2Light3.Value.ToString() == "" ||
                     txt_Image2Light4.Value.ToString() == null || txt_Image2Light4.Value.ToString() == "")) ||
                     ((m_intGrabCount == 3) &&
                     (txt_CameraGain3.Value.ToString() == null || txt_CameraGain3.Value.ToString() == "" ||
                txt_CameraShutter3.Value.ToString() == null || txt_CameraShutter3.Value.ToString() == "" ||
                txt_Image3Light1.Value.ToString() == null || txt_Image3Light1.Value.ToString() == "" ||
                txt_Image3Light2.Value.ToString() == null || txt_Image3Light2.Value.ToString() == "" ||
                txt_Image3Light3.Value.ToString() == null || txt_Image3Light3.Value.ToString() == "" ||
                     txt_Image3Light4.Value.ToString() == null || txt_Image3Light4.Value.ToString() == "")))
            {
              SRMMessageBox.Show("Value cannot be empty!");
              return;
            }


            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);

            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        objFile.WriteElement1Value("Shutter" + i.ToString(), ((int)(txt_CameraShutter1.Value) * 100).ToString());
                        objFile.WriteElement1Value("Gain" + i.ToString(), ((uint)Math.Round((float)txt_CameraGain1.Value, 0, MidpointRounding.AwayFromZero)).ToString());
                        objFile.WriteElement1Value("ImageGain" + i.ToString(), m_smVisionInfo.g_arrImageGain[i]);
                        break;
                    case 1:
                        objFile.WriteElement1Value("Shutter" + i.ToString(), ((int)(txt_CameraShutter2.Value) * 100).ToString());
                        objFile.WriteElement1Value("Gain" + i.ToString(), ((uint)Math.Round((float)txt_CameraGain2.Value, 0, MidpointRounding.AwayFromZero)).ToString());
                        objFile.WriteElement1Value("ImageGain" + i.ToString(), m_smVisionInfo.g_arrImageGain[i]);
                        break;
                }
            }
             
            
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                objFile.WriteElement1Value(m_smVisionInfo.g_arrLightSource[i].ref_strType, "");
                for (int j = 0; j < m_intGrabCount; j++)
                {
                    switch (j)
                    {
                        case 0:
                            switch (i)
                            {
                                case 0:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image1Light1.Value.ToString());
                                    break;
                                case 1:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image1Light2.Value.ToString());
                                    break;
                                case 2:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image1Light3.Value.ToString());
                                    break;
                                case 3:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image1Light4.Value.ToString());
                                    break;
                            }
                            break;
                        case 1:
                            switch (i)
                            {
                                case 0:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image2Light1.Value.ToString());
                                    break;
                                case 1:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image2Light2.Value.ToString());
                                    break;
                                case 2:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image2Light3.Value.ToString());
                                    break;
                                case 3:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image2Light4.Value.ToString());
                                    break;
                            }
                            break;
                        case 2:
                            switch (i)
                            {
                                case 0:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image3Light1.Value.ToString());
                                    break;
                                case 1:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image3Light2.Value.ToString());
                                    break;
                                case 2:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image3Light3.Value.ToString());
                                    break;
                                case 3:
                                    objFile.WriteElement2Value("Seq" + j.ToString(), txt_Image3Light4.Value.ToString());
                                    break;
                            }
                            break;
                    }
                }
            }

            objFile.WriteEndElement();
        }

        private void GrabOneTime()
        {
            /////Set Gain
            UInt32 intGainValue = Convert.ToUInt32(m_arrCameraGainPrev);
            if (intGainValue <= 50)
            {
                m_smVisionInfo.g_arrImageGain[0] = 1;
                if (m_objIDSCamera != null)
                {
                    m_objIDSCamera.SetGain((int)intGainValue);
                }
                else
                {

                    intGainValue = ((uint)Math.Round((float)intGainValue * 6.8, 0, MidpointRounding.AwayFromZero));

                    m_objAVTFireGrab.SetCameraParameter(2, intGainValue);
                   

                }
                m_smVisionInfo.g_arrCameraGain[0] = intGainValue;
            }
            else
            {
                m_smVisionInfo.g_arrImageGain[0] = 1 + (intGainValue - 50) * 0.18f;
            }


            ///////Set Shuttle
            float fShuttleValue = 0;//m_arrCameraShuttlePrev;

            if (m_objIDSCamera != null)
            {
                //m_objIDSCamera.SetShuttle(Convert.ToSingle(((NumericUpDown)sender).Value));

                fShuttleValue = fShuttleValue * 0.1f;
            }
            else
            {
                fShuttleValue = fShuttleValue * 5f;

                m_objAVTFireGrab.SetCameraParameter(1, (uint)fShuttleValue);
             
            }

            m_smVisionInfo.g_arrCameraShuttle[0] = fShuttleValue;

            /////Set Light source
            for (int i = 0; i < m_arrLightSourcePrev.Count;i++ )
            {
                int intTag = i;// Convert.ToInt32(((NumericUpDown)sender).Tag);

                LightSource objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intTag].ref_LightSourceNo];
                int intValue = objLight.ref_arrValue[m_arrSeq[intTag].ref_LightSourceArray];

                if (m_smVisionInfo.g_intLightControllerType == 1)
                {
                    //if (m_smVisionInfo.g_arrImages.Count == 1)    // Prevent both side intensity during camera live (Warning: This condition should apply if intensity change during grab image)
                    {
                        if (m_blnLEDi)
                            LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(intValue));
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
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << m_intSelectedImage)) > 0)
                            {
                                switch (m_intSelectedImage)
                                {
                                    case 0:  // first image view
                                        switch (j)  // light source
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Image1Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Image1Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Image1Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Image1Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Image1Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Image1Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Image1Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Image1Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Image1Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Image1Light1.Value);
                                                break;
                                        }
                                        break;
                                    case 1: 
                                        switch (j)  // light source
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Image2Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Image2Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Image2Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Image2Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Image2Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Image2Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Image2Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Image2Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Image2Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Image2Light1.Value);
                                                break;
                                        }
                                        break;
                                    case 2:
                                        switch (j)  // light source
                                        {
                                            case 0:
                                                intValue1 = Convert.ToInt32(txt_Image3Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 1:
                                                if (intLightSourceUsed > 0)
                                                    intValue2 = Convert.ToInt32(txt_Image3Light2.Value);
                                                else
                                                    intValue2 = Convert.ToInt32(txt_Image3Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 2:
                                                if (intLightSourceUsed > 1)
                                                    intValue3 = Convert.ToInt32(txt_Image3Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue3 = Convert.ToInt32(txt_Image3Light2.Value);
                                                else
                                                    intValue3 = Convert.ToInt32(txt_Image3Light1.Value);
                                                intLightSourceUsed++;
                                                break;
                                            case 3:
                                                if (intLightSourceUsed > 2)
                                                    intValue4 = Convert.ToInt32(txt_Image3Light4.Value);
                                                else if (intLightSourceUsed > 1)
                                                    intValue4 = Convert.ToInt32(txt_Image3Light3.Value);
                                                else if (intLightSourceUsed > 0)
                                                    intValue4 = Convert.ToInt32(txt_Image3Light2.Value);
                                                else
                                                    intValue4 = Convert.ToInt32(txt_Image3Light1.Value);
                                                break;
                                        }
                                        break;
                                }
                            }
                        }

                        LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                        Thread.Sleep(10);
                        LEDi_Control.SetSeqIntensity(objLight.ref_intPortNo, 0, m_intSelectedImage, intValue1, intValue2, intValue3, intValue4);
                        Thread.Sleep(10);
                        LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                        Thread.Sleep(100);
                        LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                        Thread.Sleep(10);
                        m_smVisionInfo.AT_PR_PauseLiveImage = false;
                    }
                }
            }
        }
        private void UpdateGUI()
        {
           
            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

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

            if (m_objIDSCamera != null)
            {
                for (int i = 0; i < m_arrCameraShuttlePrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraShutter1.Value = Math.Min(100, (int)(m_arrCameraShuttlePrev[i] * 10));
                            break;
                        case 1:
                            txt_CameraShutter2.Value = Math.Min(100, (int)(m_arrCameraShuttlePrev[i] * 10));
                            break;
                        case 2:
                            txt_CameraShutter3.Value = Math.Min(100, (int)(m_arrCameraShuttlePrev[i] * 10));
                            break;
                    }
                }

                for (int i = 0; i < m_arrCameraGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraGain1.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 1:
                            txt_CameraGain2.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 2:
                            txt_CameraGain3.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                    }
                }

                for (int i = 0; i < m_arrImageGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (txt_CameraGain1.Value == 50)
                                txt_CameraGain1.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 1:
                            if (txt_CameraGain2.Value == 50)
                                txt_CameraGain2.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 2:
                            if (txt_CameraGain3.Value == 50)
                                txt_CameraGain3.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                    }
                }   
            }
            else if (m_objAVTFireGrab != null)
            {
                int intShuttleValue;
                for (int i = 0; i < m_arrCameraShuttlePrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter1.Value = intShuttleValue;
                            else
                                txt_CameraShutter1.Value = 10;
                            break;
                        case 1:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter2.Value = intShuttleValue;
                            else
                                txt_CameraShutter2.Value = 10;
                            break;
                        case 2:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter3.Value = intShuttleValue;
                            else
                                txt_CameraShutter3.Value = 10;
                            break;
                    }
                }

                for (int i = 0; i < m_arrCameraGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraGain1.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 1:
                            txt_CameraGain2.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 2:
                            txt_CameraGain3.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                    }
                }

                for (int i = 0; i < m_arrImageGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (txt_CameraGain1.Value == 50)
                                txt_CameraGain1.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 1:
                            if (txt_CameraGain2.Value == 50)
                                txt_CameraGain2.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 2:
                            if (txt_CameraGain3.Value == 50)
                                txt_CameraGain3.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                    }
                }
            }
            else if (m_objTeliCamera != null)
            {
                int intShuttleValue;

                for (int i = 0; i < m_arrCameraShuttlePrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter1.Value = intShuttleValue;
                            else
                                txt_CameraShutter1.Value = 10;
                            break;
                        case 1:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter2.Value = intShuttleValue;
                            else
                                txt_CameraShutter2.Value = 10;
                            break;
                        case 2:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter3.Value = intShuttleValue;
                            else
                                txt_CameraShutter3.Value = 10;
                            break;
                    }
                }

                for (int i = 0; i < m_arrCameraGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraGain1.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 1:
                            txt_CameraGain2.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 2:
                            txt_CameraGain3.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                    }
                }

                for (int i = 0; i < m_arrImageGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (txt_CameraGain1.Value == 50)
                                txt_CameraGain1.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 1:
                            if (txt_CameraGain2.Value == 50)
                                txt_CameraGain2.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 2:
                            if (txt_CameraGain3.Value == 50)
                                txt_CameraGain3.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                    }
                }
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
                        objSeq.ref_imageNo = 0;
                        objSeq.ref_LightSourceNo = j;
                        switch (i)
                        {
                            case 0:  // first image view
                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Image1Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image1Light1.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[0] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light1.Value = m_arrLightSourcePrev[j].ref_arrValue[0];
                                        lbl_Image1Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Image1Light1.Tag = m_arrSeq.Count;
                                        break;
                                    case 2:
                                        lbl_Image1Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image1Light2.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[0] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light2.Value = m_arrLightSourcePrev[j].ref_arrValue[0];
                                        lbl_Image1Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image1Light2.Visible = true;
                                        txt_Image1Light2.Visible = true;
                                        lbl_Image1Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Image1Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image1Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image1Light3.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[0] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light3.Value = m_arrLightSourcePrev[j].ref_arrValue[0];
                                        lbl_Image1Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image1Light3.Visible = true;
                                        txt_Image1Light3.Visible = true;
                                        lbl_Image1Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Image1Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image1Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image1Light4.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[0] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light4.Value = m_arrLightSourcePrev[j].ref_arrValue[0];
                                        lbl_Image1Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image1Light4.Visible = true;
                                        txt_Image1Light4.Visible = true;
                                        lbl_Image1Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Image1Light4.Tag = m_arrSeq.Count;
                                        break;
                                }
                                break;
                            case 1:
                                int intLightSourceArrayNo1 = 0;
                                for (int x = 0; x < m_arrSeq.Count; x++)
                                {
                                    if (m_arrSeq[x].ref_LightSourceNo == j)   // check whether this light source is used in previous image
                                    {
                                        intLightSourceArrayNo1++;
                                    }
                                }

                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Image2Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light1.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] = intMaxLimit;
                                        txt_Image2Light1.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1];
                                        lbl_Image2Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo1;
                                        txt_Image2Light1.Tag = m_arrSeq.Count;
                                        break;
                                    case 2:
                                        lbl_Image2Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light2.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] = intMaxLimit;
                                        txt_Image2Light2.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1];
                                        lbl_Image2Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image2Light2.Visible = true;
                                        txt_Image2Light2.Visible = true;
                                        lbl_Image2Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo1;
                                        txt_Image2Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image2Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light3.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] = intMaxLimit;
                                        txt_Image2Light3.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1];
                                        lbl_Image2Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image2Light3.Visible = true;
                                        txt_Image2Light3.Visible = true;
                                        lbl_Image2Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo1;
                                        txt_Image2Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image2Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light4.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1] = intMaxLimit;
                                        txt_Image2Light4.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo1];
                                        lbl_Image2Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image2Light4.Visible = true;
                                        txt_Image2Light4.Visible = true;
                                        lbl_Image2Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo1;
                                        txt_Image2Light4.Tag = m_arrSeq.Count;
                                        break;
                                }
                                break;
                            case 2:
                                int intLightSourceArrayNo2 = 0;
                                for (int x = 0; x < m_arrSeq.Count; x++)
                                {
                                    if (m_arrSeq[x].ref_LightSourceNo == j)   // check whether this light source is used in previous image
                                    {
                                        intLightSourceArrayNo2++;
                                    }
                                }

                                switch (intLightSourceNo)  // light source sequence
                                {
                                    case 1:
                                        lbl_Image3Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light1.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] = intMaxLimit;
                                        txt_Image3Light1.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2];
                                        lbl_Image3Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo2;
                                        txt_Image3Light1.Tag = m_arrSeq.Count;
                                        break;
                                    case 2:
                                        lbl_Image3Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light2.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] = intMaxLimit;
                                        txt_Image3Light2.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2];
                                        lbl_Image3Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image3Light2.Visible = true;
                                        txt_Image3Light2.Visible = true;
                                        lbl_Image3Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo2;
                                        txt_Image3Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image3Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light3.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] = intMaxLimit;
                                        txt_Image3Light3.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2];
                                        lbl_Image3Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image3Light3.Visible = true;
                                        txt_Image3Light3.Visible = true;
                                        lbl_Image3Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo2;
                                        txt_Image3Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image3Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light4.Maximum = intMaxLimit;
                                        if (m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] > intMaxLimit)
                                            m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2] = intMaxLimit;
                                        txt_Image3Light4.Value = m_arrLightSourcePrev[j].ref_arrValue[intLightSourceArrayNo2];
                                        lbl_Image3Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image3Light4.Visible = true;
                                        txt_Image3Light4.Visible = true;
                                        lbl_Image3Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo2;
                                        txt_Image3Light4.Tag = m_arrSeq.Count;
                                        break;
                                }
                                break;
                        }
                        m_arrSeq.Add(objSeq);
                    }
                }
            }
            if (m_intGrabCount < 2)
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting2);

            if (m_intGrabCount < 3)
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting3);
        }


        private void txt_CameraGain_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UInt32 intValue = Convert.ToUInt32(((NumericUpDown)sender).Value);
            if (intValue <= 50)
            {
                m_smVisionInfo.g_arrImageGain[m_intSelectedImage] = 1;
                if (m_objIDSCamera != null)
                {
                    m_objIDSCamera.SetGain(Convert.ToInt32(((NumericUpDown)sender).Value));
                }
                else if (m_objAVTFireGrab != null)
                {

                    //intValue = ((uint)Math.Round((float)intValue * 6.8, 0, MidpointRounding.AwayFromZero));
                    double dValue = intValue;
                    if (!m_objAVTFireGrab.SetCameraParameter(2, dValue))
                        SRMMessageBox.Show("Fail to set camera's gain");
                  
                }
                else if (m_objTeliCamera != null)
                {

                    // intValue = ((uint)Math.Round((float)intValue * 6.8, 0, MidpointRounding.AwayFromZero));
                    float fValue = intValue;
                    if (!m_objTeliCamera.SetCameraParameter(2, fValue))
                        SRMMessageBox.Show("Fail to set camera's gain");

                }
                m_smVisionInfo.g_arrCameraGain[m_intSelectedImage] = intValue;
            }
            else
            {
                m_smVisionInfo.g_arrImageGain[m_intSelectedImage] = 1 + (intValue - 50) * 0.18f;
            }
        }


        

        private void txt_Image1Light1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((NumericUpDown)sender).Tag);

            LightSource objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intTag].ref_LightSourceNo];
            int intValue = Convert.ToInt32(((NumericUpDown)sender).Value);
            objLight.ref_arrValue[m_arrSeq[intTag].ref_LightSourceArray] = intValue;

            if (m_smVisionInfo.g_intLightControllerType == 1)
            {
                //if (m_smVisionInfo.g_arrImages.Count == 1)    // Prevent both side intensity during camera live (Warning: This condition should apply if intensity change during grab image)
                {
                    if (m_blnLEDi)
                        LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(intValue));
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
                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << m_intSelectedImage)) > 0)
                        {
                            switch (m_intSelectedImage)
                            {
                                case 0:  // first image view
                                    switch (j)  // light source
                                    {
                                        case 0:
                                            intValue1 = Convert.ToInt32(txt_Image1Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_Image1Light2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_Image1Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_Image1Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_Image1Light2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_Image1Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_Image1Light4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_Image1Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_Image1Light2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_Image1Light1.Value);
                                            break;
                                    }
                                    break;
                                case 1: 
                                    switch (j)  // light source
                                    {
                                        case 0:
                                            intValue1 = Convert.ToInt32(txt_Image2Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_Image2Light2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_Image2Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_Image2Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_Image2Light2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_Image2Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_Image2Light4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_Image2Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_Image2Light2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_Image2Light1.Value);
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (j)  // light source
                                    {
                                        case 0:
                                            intValue1 = Convert.ToInt32(txt_Image3Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_Image3Light2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_Image3Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_Image3Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_Image3Light2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_Image3Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_Image3Light4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_Image3Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_Image3Light2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_Image3Light1.Value);
                                            break;
                                    }
                                    break;
                            }
                        }
                    }

                    LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                    Thread.Sleep(10);
                    LEDi_Control.SetSeqIntensity(objLight.ref_intPortNo, 0, m_intSelectedImage, intValue1, intValue2, intValue3, intValue4);
                    Thread.Sleep(10);
                    LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                    Thread.Sleep(100);
                    LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                    m_smVisionInfo.AT_PR_PauseLiveImage = false;
                }
            }
        }
        
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
           // ReadSettings();
          
            this.Close();
            this.Dispose();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            SaveSettings();
            this.Close();
            this.Dispose();
        }
        
        private void CameraSettings_Load(object sender, EventArgs e)
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

        private void CameraSettings_FormClosing(object sender, FormClosingEventArgs e)
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
        private void tabCtrl_Camera_TabIndexChanged(object sender, EventArgs e)
        {
            switch (tabCtrl_Camera.SelectedTab.Name)
            {
                case "tp_GrabSetting1":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType_Ori;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabSetting2":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 1;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType_Ori;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabSetting3":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 2;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType_Ori;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
            }

            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_intSelectedImage]);

            m_blnRepaint = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_CameraShutter1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = Convert.ToSingle(((NumericUpDown)sender).Value);

            if (m_objIDSCamera != null)
            {
                //m_objIDSCamera.SetShuttle(Convert.ToSingle(((NumericUpDown)sender).Value));

                fValue = fValue * 0.1f;
            }
            else if (m_objAVTFireGrab != null)
            {
                fValue = fValue * 100f;

                if (!m_objAVTFireGrab.SetCameraParameter(1, (uint)fValue)) //m_intSelectedImage == 0 &&
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }
            else if (m_objTeliCamera != null)
            {
                fValue = fValue * 100f;     // 1% == Shuttle value 100. For Teli, ST 1 == 1 microsecond. So, 100% == 100% * 100 = ST 10000 = 10000 * 1 microsecond = 10ms

                if (!m_objTeliCamera.SetCameraParameter(1, fValue))
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }
            m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] = fValue;
        }
        
    }
}