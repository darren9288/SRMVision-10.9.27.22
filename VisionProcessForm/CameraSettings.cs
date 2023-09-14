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

namespace VisionProcessForm
{
    public partial class CameraSettings : Form
    {
        #region Member Variables
        
        private float m_fShutterPrev = 0;
        private int m_intUserGroup = 5;
        private int m_intSelectedImage = 0;
        private bool m_blnRepaint = false;
        private bool m_blnInitDone = false;
        private bool m_blnViewImagePHPrev = false;
        private bool m_blnViewImageEmptyPrev = false;
        private bool m_blnLEDi;
        private bool m_blnVT;
        private string m_strFilePath;

        private int m_intMergeType;
        private string m_strCameraFilePath;
        private string m_strSelectedRecipe = "";
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
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        
        //PH
        private uint m_uintPHCameraGainPrev;
        private float m_fPHImageGainPrev;
        private float m_fPHCameraShuttlePrev;
        private List<LightSource> m_arrPHLightSourcePrev = new List<LightSource>();

        //Empty
        private uint m_uintEmptyCameraGainPrev;
        private float m_fEmptyImageGainPrev;
        private float m_fEmptyCameraShuttlePrev;
        private List<LightSource> m_arrEmptyLightSourcePrev = new List<LightSource>();
        #endregion

        public class PHSequenceArray
        {
            public int ref_imageNo;
            public int ref_LightSourceNo;
            public int ref_LightSourceArray;
        }
        public class SequenceArray
        {
            public int ref_imageNo;
            public int ref_LightSourceNo;
            public int ref_LightSourceArray;
        }

        public CameraSettings(VisionInfo smVisionInfo, bool blnLEDi, bool blnVT,
            string strSelectedRecipe, AVTVimba objAVTFireGrab, int intUserGroup)
        {
            m_blnLEDi = blnLEDi;
            m_blnVT = blnVT;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                 "\\Camera.xml";

            m_objAVTFireGrab = objAVTFireGrab;
            m_smVisionInfo = smVisionInfo;

            InitializeComponent();

            m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;  // View image 1 first bcos selected tabpage is first image also.
            
            UpdateGUI();

            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);

            m_blnInitDone = true;
            m_blnRepaint = true;
        }

        public CameraSettings(VisionInfo smVisionInfo, bool blnLEDi, bool blnVT,
            string strSelectedRecipe, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera, int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            m_blnLEDi = blnLEDi;
            m_blnVT = blnVT;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                 "\\Camera.xml";
            // m_strPHPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\PHCamera.xml";
            m_strCameraFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe;
            m_objAVTFireGrab = objAVTFireGrab;
            m_objIDSCamera = objIDSCamera;
            m_objTeliCamera = objTeliCamera;
            m_smVisionInfo = smVisionInfo;
            m_intMergeType = m_smVisionInfo.g_intImageMergeType;
            m_blnViewImagePHPrev = m_smVisionInfo.g_blnViewPHImage;
            m_blnViewImageEmptyPrev = m_smVisionInfo.g_blnViewEmptyImage;
            InitializeComponent();

            m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;  // View image 1 first bcos selected tabpage is first image also.

            DisableField2();
            UpdateGUI();

            UpdatePHGUI();
            UpdateEmptyGUI();
            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                if (!tabCtrl_Camera.TabPages.Contains(tp_GrabPH))
                    tabCtrl_Camera.Controls.Add(tp_GrabPH);

                if (m_smVisionInfo.g_blnViewPHImage)
                {
                    tabCtrl_Camera.SelectedTab = tp_GrabPH;
                    m_blnViewImagePHPrev = true;
                }
            }
            else
            {
                if (tabCtrl_Camera.TabPages.Contains(tp_GrabPH))
                    tabCtrl_Camera.Controls.Remove(tp_GrabPH);
            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                if (!tabCtrl_Camera.TabPages.Contains(tp_GrabEmpty))
                    tabCtrl_Camera.Controls.Add(tp_GrabEmpty);

                if (m_smVisionInfo.g_blnViewEmptyImage)
                {
                    tabCtrl_Camera.SelectedTab = tp_GrabEmpty;
                    m_blnViewImageEmptyPrev = true;
                }
            }
            else
            {
                if (tabCtrl_Camera.TabPages.Contains(tp_GrabEmpty))
                    tabCtrl_Camera.Controls.Remove(tp_GrabEmpty);
            }

            if (m_smVisionInfo.g_strVisionName.Contains("Pad5S") && m_intUserGroup == 1)
            {
                btn_ExtraGain1.Visible = true;
                btn_ExtraGain2.Visible = true;
                btn_ExtraGain3.Visible = true;
                btn_ExtraGain4.Visible = true;
                btn_ExtraGain5.Visible = true;
            }
            else
            {
                btn_ExtraGain1.Visible = false;
                btn_ExtraGain2.Visible = false;
                btn_ExtraGain3.Visible = false;
                btn_ExtraGain4.Visible = false;
                btn_ExtraGain5.Visible = false;
            }

            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);

            m_blnInitDone = true;
            m_blnRepaint = true;
        }

        public CameraSettings(VisionInfo smVisionInfo, bool blnLEDi, bool blnVT,
            string strSelectedRecipe, TeliCamera objTeliCamera, IDSuEyeCamera objIDSCamera, int intUserGroup)
        {
            m_blnLEDi = blnLEDi;
            m_blnVT = blnVT;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                 "\\Camera.xml";

            m_objTeliCamera = objTeliCamera;
            m_objIDSCamera = objIDSCamera;
            m_smVisionInfo = smVisionInfo;

            InitializeComponent();

            m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;  // View image 1 first bcos selected tabpage is first image also.

            UpdateGUI();

            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);

            m_blnInitDone = true;
            m_blnRepaint = true;
        }
        private int GetUserRightGroup_Child3(string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(Child2, Child3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(Child2, Child3);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(Child2, Child3);
                    break;
            }

            return 1;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Camera Page";
            string strChild2 = "";

            strChild2 = "Save Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            strChild2 = "Grab Delay";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_CameraGrabDelay.Enabled = false;
            }

            strChild2 = "Shutter";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_CameraShutter1.Enabled = false;
                txt_CameraShutter2.Enabled = false;
                txt_CameraShutter3.Enabled = false;
                txt_CameraShutter4.Enabled = false;
                txt_CameraShutter5.Enabled = false;
                txt_CameraShutterPH.Enabled = false;
                txt_CameraShutterEmpty.Enabled = false;
            }

            strChild2 = "Camera Gain";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_CameraGain.Enabled = false;
                txt_CameraGain2.Enabled = false;
                txt_CameraGain3.Enabled = false;
                txt_CameraGain4.Enabled = false;
                txt_CameraGain5.Enabled = false;
                txt_CameraGainPH.Enabled = false;
                txt_CameraGainEmpty.Enabled = false;
            }

            strChild2 = "Intensity";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_Image1Light1.Enabled = false;
                txt_Image1Light2.Enabled = false;
                txt_Image1Light3.Enabled = false;
                txt_Image1Light4.Enabled = false;
                txt_Image2Light1.Enabled = false;
                txt_Image2Light2.Enabled = false;
                txt_Image2Light3.Enabled = false;
                txt_Image2Light4.Enabled = false;
                txt_Image3Light1.Enabled = false;
                txt_Image3Light2.Enabled = false;
                txt_Image3Light3.Enabled = false;
                txt_Image3Light4.Enabled = false;
                txt_Image4Light1.Enabled = false;
                txt_Image4Light2.Enabled = false;
                txt_Image4Light3.Enabled = false;
                txt_Image4Light4.Enabled = false;
                txt_Image5Light1.Enabled = false;
                txt_Image5Light2.Enabled = false;
                txt_Image5Light3.Enabled = false;
                txt_Image5Light4.Enabled = false;
                txt_ImagePHLight1.Enabled = false;
                txt_ImagePHLight2.Enabled = false;
                txt_ImagePHLight3.Enabled = false;
                txt_ImagePHLight4.Enabled = false;
                txt_ImageEmptyLight1.Enabled = false;
                txt_ImageEmptyLight2.Enabled = false;
                txt_ImageEmptyLight3.Enabled = false;
                txt_ImageEmptyLight4.Enabled = false;
            }

            strChild2 = "Extra Gain Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_ExtraGain1.Enabled = false;
                btn_ExtraGain2.Enabled = false;
                btn_ExtraGain3.Enabled = false;
                btn_ExtraGain4.Enabled = false;
                btn_ExtraGain5.Enabled = false;
            }
        }
        private void ReadSettings()
        {
            TrackLog objTL = new TrackLog();
            if (m_objIDSCamera != null)
            {
                //if (!m_objIDSCamera.SetGain(Convert.ToInt32(m_arrCameraGainPrev[0])))
                //    SRMMessageBox.Show("Fail to set camera's gain");

                //if (!m_objIDSCamera.SetShuttle(m_fShutterPrev))
                //    SRMMessageBox.Show("Fail to set camera's shutter");
            }
            else
            {
                //if (!m_objAVTFireGrab.SetCameraParameter(2, Convert.ToUInt32(m_arrCameraGainPrev[0])))
                //    SRMMessageBox.Show("Fail to set camera's gain");
                //if (!m_objAVTFireGrab.SetCameraParameter(1, Convert.ToUInt32(m_arrCameraShuttlePrev[0])))
                //    SRMMessageBox.Show("Fail to set camera's shutter");
            }
            m_smVisionInfo.g_arrLightSource = m_arrLightSourcePrev;
            m_smVisionInfo.g_arrCameraGain = m_arrCameraGainPrev;
            m_smVisionInfo.g_arrImageGain = m_arrImageGainPrev;
            m_smVisionInfo.g_arrCameraShuttle = m_arrCameraShuttlePrev;
            m_smVisionInfo.g_fCameraShuttle = m_fShutterPrev;

            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                m_smVisionInfo.g_uintPHCameraGain = m_uintPHCameraGainPrev;
                m_smVisionInfo.g_fPHImageGain = m_fPHImageGainPrev;
                m_smVisionInfo.g_fPHCameraShuttle = m_fPHCameraShuttlePrev;
            }


            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                m_smVisionInfo.g_uintEmptyCameraGain = m_uintEmptyCameraGainPrev;
                m_smVisionInfo.g_fEmptyImageGain = m_fEmptyImageGainPrev;
                m_smVisionInfo.g_fEmptyCameraShuttle = m_fEmptyCameraShuttlePrev;
            }

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
                    else if (m_blnVT)
                    {
                        VT_Control.SetConfigMode(objLight.ref_intPortNo);
                        VT_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, objLight.ref_arrValue[0]);
                        VT_Control.SaveIntensity(objLight.ref_intPortNo, objLight.ref_intChannel);
                        VT_Control.SetRunMode(objLight.ref_intPortNo);
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
                if (m_blnVT)
                {
                    int intImageCount;
                    if (m_smVisionInfo.g_arrColorImages == null)
                        intImageCount = m_smVisionInfo.g_arrImages.Count;
                    else if (m_smVisionInfo.g_arrImages == null)
                        intImageCount = m_smVisionInfo.g_arrColorImages.Count;
                    else
                        intImageCount = Math.Max(m_smVisionInfo.g_arrColorImages.Count, m_smVisionInfo.g_arrImages.Count);

                    VT_Control.SetConfigMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);

                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                    {
                        int intCount = 0;
                        for (int i = 0; i < intImageCount; i++)
                        {
                            if (intImageCount > 0)
                            {
                                // if this image no is in array intCount
                                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                                {
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo.Count != intCount)
                                    {
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[intCount] == i)
                                        {
                                            VT_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, i, m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intCount]);
                                            intCount++;
                                        }
                                        else
                                        {
                                            VT_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, i, m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                                        }
                                    }
                                    else
                                    {
                                        VT_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, i, m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                                    }
                                }
                            }
                        }
                        VT_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, m_smVisionInfo.g_arrLightSource[j].ref_intChannel);
                    }

                    VT_Control.SetRunMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
                }
                else if (m_blnLEDi)
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

        private void SaveSettings()
        {
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);



            
            STDeviceEdit.CopySettingFile(m_strCameraFilePath, "\\Camera.xml");


            //objFile.WriteElement1Value("Shutter", txt_CameraShutter.Text);
            objFile.WriteElement1Value("Shutter", m_smVisionInfo.g_fCameraShuttle);
            objFile.WriteElement1Value("GrabDelay", txt_CameraGrabDelay.Text);

            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                objFile.WriteElement1Value("Shutter" + i.ToString(), m_smVisionInfo.g_arrCameraShuttle[i]);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
            {
                objFile.WriteElement1Value("Gain" + i.ToString(), m_smVisionInfo.g_arrCameraGain[i]);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                objFile.WriteElement1Value("ImageGain" + i.ToString(), m_smVisionInfo.g_arrImageGain[i]);
            }

            if (m_smVisionInfo.g_blnWantCheckPH)
            {

                objFile.WriteElement1Value("PHShutter", ((int)(txt_CameraShutterPH.Value) * 100).ToString());

                if (txt_CameraGainPH.Value <= 50)
                    objFile.WriteElement1Value("PHGain", ((uint)Math.Round((float)txt_CameraGainPH.Value, 0, MidpointRounding.AwayFromZero)).ToString());
                else
                    objFile.WriteElement1Value("PHGain", "50");

                objFile.WriteElement1Value("PHImageGain", m_smVisionInfo.g_fPHImageGain);

            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {

                objFile.WriteElement1Value("EmptyShutter", ((int)(txt_CameraShutterEmpty.Value) * 100).ToString());

                if (txt_CameraGainEmpty.Value <= 50)
                    objFile.WriteElement1Value("EmptyGain", ((uint)Math.Round((float)txt_CameraGainEmpty.Value, 0, MidpointRounding.AwayFromZero)).ToString());
                else
                    objFile.WriteElement1Value("EmptyGain", "50");

                objFile.WriteElement1Value("EmptyImageGain", m_smVisionInfo.g_fEmptyImageGain);

            }

            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                {
                    string strSearch = m_smVisionInfo.g_arrLightSource[i].ref_strType.Replace(" ", string.Empty);
                    switch (i)
                    {
                        case 0:
                            objFile.WriteElement1Value("PH" + strSearch, txt_ImagePHLight1.Value.ToString());
                            break;
                        case 1:
                            objFile.WriteElement1Value("PH" + strSearch, txt_ImagePHLight2.Value.ToString());
                            break;
                        case 2:
                            objFile.WriteElement1Value("PH" + strSearch, txt_ImagePHLight3.Value.ToString());
                            break;
                        case 3:
                            objFile.WriteElement1Value("PH" + strSearch, txt_ImagePHLight4.Value.ToString());
                            break;
                    }
                }
            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                {
                    string strSearch = m_smVisionInfo.g_arrLightSource[i].ref_strType.Replace(" ", string.Empty);
                    switch (i)
                    {
                        case 0:
                            objFile.WriteElement1Value("Empty" + strSearch, txt_ImageEmptyLight1.Value.ToString());
                            break;
                        case 1:
                            objFile.WriteElement1Value("Empty" + strSearch, txt_ImageEmptyLight2.Value.ToString());
                            break;
                        case 2:
                            objFile.WriteElement1Value("Empty" + strSearch, txt_ImageEmptyLight3.Value.ToString());
                            break;
                        case 3:
                            objFile.WriteElement1Value("Empty" + strSearch, txt_ImageEmptyLight4.Value.ToString());
                            break;
                    }
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                objFile.WriteElement1Value(m_smVisionInfo.g_arrLightSource[i].ref_strType, "");
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[i].ref_arrValue.Count; j++)
                {
                    objFile.WriteElement2Value("Seq" + j.ToString(), m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j]);
                }
            }

            if (m_blnVT)
            {
                VT_Control.SetConfigMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
                VT_Control.SaveAllSetting(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
                VT_Control.SetRunMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
            }



            m_smVisionInfo.g_intCameraGrabDelay = Convert.ToInt32(txt_CameraGrabDelay.Text);
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">", m_smProductionInfo.g_strLotID);
            
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

            // Update value to GUI
            txt_CameraGrabDelay.Text = objFile.GetValueAsString("GrabDelay", "5");

            if (m_objIDSCamera != null)
            {
                txt_CameraShutter.Text = Math.Round(m_fShutterPrev / 0.1f, 0, MidpointRounding.AwayFromZero).ToString();

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
                        case 3:
                            txt_CameraShutter4.Value = Math.Min(100, (int)(m_arrCameraShuttlePrev[i] * 10));
                            break;
                        case 4:
                            txt_CameraShutter5.Value = Math.Min(100, (int)(m_arrCameraShuttlePrev[i] * 10));
                            break;
                    }
                }

                for (int i = 0; i < m_arrCameraGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraGain.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 1:
                            txt_CameraGain2.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 2:
                            txt_CameraGain3.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 3:
                            txt_CameraGain4.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 4:
                            txt_CameraGain5.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                    }
                }

                for (int i = 0; i < m_arrImageGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (txt_CameraGain.Value == 50)
                                txt_CameraGain.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 1:
                            if (txt_CameraGain2.Value == 50)
                                txt_CameraGain2.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 2:
                            if (txt_CameraGain3.Value == 50)
                                txt_CameraGain3.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 3:
                            if (txt_CameraGain4.Value == 50)
                                txt_CameraGain4.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 4:
                            if (txt_CameraGain5.Value == 50)
                                txt_CameraGain5.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                    }
                }


            }
            else if (m_objTeliCamera != null)
            {
                txt_CameraShutter.Text = Math.Round(m_fShutterPrev / 100, 0, MidpointRounding.AwayFromZero).ToString();
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
                        case 3:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter4.Value = intShuttleValue;
                            else
                                txt_CameraShutter4.Value = 10;
                            break;
                        case 4:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 100f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter5.Value = intShuttleValue;
                            else
                                txt_CameraShutter5.Value = 10;
                            break;
                    }
                }

                for (int i = 0; i < m_arrCameraGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraGain.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 1:
                            txt_CameraGain2.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 2:
                            txt_CameraGain3.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 3:
                            txt_CameraGain4.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 4:
                            txt_CameraGain5.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                    }
                }

                for (int i = 0; i < m_arrImageGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (txt_CameraGain.Value == 50)
                                txt_CameraGain.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 1:
                            if (txt_CameraGain2.Value == 50)
                                txt_CameraGain2.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 2:
                            if (txt_CameraGain3.Value == 50)
                                txt_CameraGain3.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 3:
                            if (txt_CameraGain4.Value == 50)
                                txt_CameraGain4.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 4:
                            if (txt_CameraGain5.Value == 50)
                                txt_CameraGain5.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                    }
                }
            }
            else if (m_objAVTFireGrab != null)
            {
                txt_CameraShutter.Text = Math.Round(m_fShutterPrev / 5, 0, MidpointRounding.AwayFromZero).ToString();
                int intShuttleValue;

                for (int i = 0; i < m_arrCameraShuttlePrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 5f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter1.Value = intShuttleValue;
                            else
                                txt_CameraShutter1.Value = 10;
                            break;
                        case 1:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 5f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter2.Value = intShuttleValue;
                            else
                                txt_CameraShutter2.Value = 10;
                            break;
                        case 2:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 5f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter3.Value = intShuttleValue;
                            else
                                txt_CameraShutter3.Value = 10;
                            break;
                        case 3:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 5f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter4.Value = intShuttleValue;
                            else
                                txt_CameraShutter4.Value = 10;
                            break;
                        case 4:
                            intShuttleValue = (int)Math.Round(m_arrCameraShuttlePrev[i] / 5f, 0, MidpointRounding.AwayFromZero);
                            if (intShuttleValue > 0 && intShuttleValue <= 100)
                                txt_CameraShutter5.Value = intShuttleValue;
                            else
                                txt_CameraShutter5.Value = 10;
                            break;
                    }
                }

                for (int i = 0; i < m_arrCameraGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            txt_CameraGain.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 1:
                            txt_CameraGain2.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 2:
                            txt_CameraGain3.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 3:
                            txt_CameraGain4.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                        case 4:
                            txt_CameraGain5.Value = Math.Min(50, m_arrCameraGainPrev[i]);
                            break;
                    }
                }

                for (int i = 0; i < m_arrImageGainPrev.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (txt_CameraGain.Value == 50)
                                txt_CameraGain.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 1:
                            if (txt_CameraGain2.Value == 50)
                                txt_CameraGain2.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 2:
                            if (txt_CameraGain3.Value == 50)
                                txt_CameraGain3.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 3:
                            if (txt_CameraGain4.Value == 50)
                                txt_CameraGain4.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                        case 4:
                            if (txt_CameraGain5.Value == 50)
                                txt_CameraGain5.Value += Convert.ToUInt32((m_arrImageGainPrev[i] - 1) / 0.18f);
                            break;
                    }
                }
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

            int intImageCount;
            if (m_smVisionInfo.g_arrColorImages == null)
                intImageCount = m_smVisionInfo.g_arrImages.Count;
            else if (m_smVisionInfo.g_arrImages == null)
                intImageCount = m_smVisionInfo.g_arrColorImages.Count;
            else
                intImageCount = Math.Max(m_smVisionInfo.g_arrColorImages.Count, m_smVisionInfo.g_arrImages.Count);


            if (intImageCount < 2)
            {
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting2);
                chk_SeparateGrab.Visible = false;
            }
            if (intImageCount < 3)
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting3);

            if (intImageCount < 4)
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting4);

            if (intImageCount < 5)
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting5);

            for (int i = 0; i < intImageCount; i++)
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
                                        lbl_Image1Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image1Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
                                        lbl_Image1Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        objSeq.ref_LightSourceArray = 0;
                                        txt_Image1Light1.Tag = m_arrSeq.Count;
                                        break;
                                    case 2:
                                        lbl_Image1Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image1Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
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
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
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
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0] = intMaxLimit;
                                        txt_Image1Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0];
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
                                        lbl_Image2Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image2Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image2Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        txt_Image2Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Image2Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image2Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image2Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image2Light2.Visible = true;
                                        txt_Image2Light2.Visible = true;
                                        lbl_Image2Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image2Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image2Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image2Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image2Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image2Light3.Visible = true;
                                        txt_Image2Light3.Visible = true;
                                        lbl_Image2Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image2Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image2Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image2Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image2Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image2Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image2Light4.Visible = true;
                                        txt_Image2Light4.Visible = true;
                                        lbl_Image2Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image2Light4.Tag = m_arrSeq.Count;
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
                                        lbl_Image3Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image3Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image3Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        txt_Image3Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Image3Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image3Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image3Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image3Light2.Visible = true;
                                        txt_Image3Light2.Visible = true;
                                        lbl_Image3Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image3Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image3Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image3Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image3Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image3Light3.Visible = true;
                                        txt_Image3Light3.Visible = true;
                                        lbl_Image3Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image3Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image3Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image3Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image3Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image3Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image3Light4.Visible = true;
                                        txt_Image3Light4.Visible = true;
                                        lbl_Image3Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image3Light4.Tag = m_arrSeq.Count;
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
                                        lbl_Image4Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image4Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image4Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image4Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        txt_Image4Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Image4Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image4Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image4Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image4Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image4Light2.Visible = true;
                                        txt_Image4Light2.Visible = true;
                                        lbl_Image4Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image4Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image4Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image4Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image4Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image4Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image4Light3.Visible = true;
                                        txt_Image4Light3.Visible = true;
                                        lbl_Image4Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image4Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image4Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image4Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image4Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image4Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image4Light4.Visible = true;
                                        txt_Image4Light4.Visible = true;
                                        lbl_Image4Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image4Light4.Tag = m_arrSeq.Count;
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
                                        lbl_Image5Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image5Light1.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image5Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image5Label1.Text = "(1-" + intMaxLimit.ToString() + ")";
                                        txt_Image5Light1.Tag = m_arrSeq.Count;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        break;
                                    case 2:
                                        lbl_Image5Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image5Light2.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image5Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image5Label2.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image5Light2.Visible = true;
                                        txt_Image5Light2.Visible = true;
                                        lbl_Image5Label2.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image5Light2.Tag = m_arrSeq.Count;
                                        break;
                                    case 3:
                                        lbl_Image5Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image5Light3.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image5Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image5Label3.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image5Light3.Visible = true;
                                        txt_Image5Light3.Visible = true;
                                        lbl_Image5Label3.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image5Light3.Tag = m_arrSeq.Count;
                                        break;
                                    case 4:
                                        lbl_Image5Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                                        txt_Image5Light4.Maximum = intMaxLimit;
                                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] > intMaxLimit)
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo] = intMaxLimit;
                                        txt_Image5Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intLightSourceArrayNo];
                                        lbl_Image5Label4.Text = "(1-" + intMaxLimit.ToString() + ")";

                                        lbl_Image5Light4.Visible = true;
                                        txt_Image5Light4.Visible = true;
                                        lbl_Image5Label4.Visible = true;
                                        objSeq.ref_LightSourceArray = intLightSourceArrayNo;
                                        txt_Image5Light4.Tag = m_arrSeq.Count;
                                        break;
                                }
                                break;
                        }

                        m_arrSeq.Add(objSeq);
                    }
                }
            }

            if (intImageCount < 2)
                tabCtrl_Camera.TabPages.Remove(tp_GrabSetting2);

            UserRight objUserRight = new UserRight();
            string strChild1 = "Learn Page";
            string strChild2 = "";

            strChild2 = "Advance Setting";
            if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
            {
                btn_AdvanceSettings.Visible = false;
            }

            if (!m_smVisionInfo.g_blnViewColorImage)
                btn_AdvanceSettings.Visible = false;

            if (m_intUserGroup == 1)    // SRM User
            {
                chk_CalculateGrayPixel.Visible = true;
                srmLabel10.Visible = true;
            }
        }




        private void txt_CameraShutter_TextChanged(object sender, EventArgs e)
        {
            /*
             * Shuttle 1 == 20 nano second.
             */
            if (!m_blnInitDone)
                return;

            float fValue = Convert.ToSingle(txt_CameraShutter.Value);

            if (m_objIDSCamera != null)
            {
                // IDS camera: 1% == 0.1ms
                fValue = fValue * 0.1f;

                if (!m_objIDSCamera.SetShuttle(fValue))
                    SRMMessageBox.Show("Fail to set camera's shutter");
            }
            else
            {
                //  AVT Camera: 1% == 5 == 5x20nano second = 0.1ms. (Recommend value is 10%)
                fValue = fValue * 5;

                if (!m_objAVTFireGrab.SetCameraParameter(1, Convert.ToUInt32(fValue)))
                    SRMMessageBox.Show("Fail to set camera's shutter");
            }

            m_smVisionInfo.g_fCameraShuttle = fValue;
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

                    //double dValue = (double)Math.Round((float)intValue * 0.46f, 0, MidpointRounding.AwayFromZero);
                    double dValue = intValue;

                    if (m_intSelectedImage == 0 && !m_objAVTFireGrab.SetCameraParameter(2, dValue))
                        SRMMessageBox.Show("Fail to set camera's gain");
                }
                else if (m_objTeliCamera != null)
                {
                    //TrackLog objTL = new TrackLog();
                    //objTL.WriteLine("User Set value =" + intValue.ToString());
                    //float fValue = (float)Math.Round((float)intValue * 0.46f, 0, MidpointRounding.AwayFromZero);
                    //objTL.WriteLine("User Set value After convert =" + intValue.ToString());

                    float fValue = intValue;

                    if (m_intSelectedImage == 0 && !m_objTeliCamera.SetCameraParameter(2, fValue))
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
                    {
                        LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(intValue));
                        //Thread.Sleep(15);
                    }
                    else if (m_blnVT)
                    {
                        m_smVisionInfo.AT_PR_PauseLiveImage = true;

                        while (m_smVisionInfo.g_blnGrabbing)
                        {
                            Thread.Sleep(1);
                        }
                        VT_Control.SetConfigMode(objLight.ref_intPortNo);
                        VT_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, intValue);
                        VT_Control.SetRunMode(objLight.ref_intPortNo);
                        m_smVisionInfo.AT_PR_PauseLiveImage = false;
                    }
                    else
                        TCOSIO_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, intValue);
                }
            }
            else
            {
                if (m_blnVT)
                {
                    m_smVisionInfo.AT_PR_PauseLiveImage = true;

                    while (m_smVisionInfo.g_blnGrabbing)
                    {
                        Thread.Sleep(1);
                    }

                    VT_Control.SetConfigMode(objLight.ref_intPortNo);
                    VT_Control.SetSeqIntensity(objLight.ref_intPortNo, m_intSelectedImage, objLight.ref_intChannel, intValue);
                    VT_Control.SetRunMode(objLight.ref_intPortNo);
                    m_smVisionInfo.AT_PR_PauseLiveImage = false;
                }
                else if (m_blnLEDi)
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
                                            intLightSourceUsed++;
                                            break;
                                    }
                                    break;
                                case 1:
                                    switch (j)
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
                                            intLightSourceUsed++;
                                            break;
                                    }
                                    break;
                                case 2:
                                    switch (j)  // light source sequence
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
                                            intLightSourceUsed++;
                                            break;
                                    }
                                    break;
                                case 3:
                                    switch (j)  // light source sequence
                                    {
                                        case 0:
                                            intValue1 = Convert.ToInt32(txt_Image4Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_Image4Light2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_Image4Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_Image4Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_Image4Light2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_Image4Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_Image4Light4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_Image4Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_Image4Light2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_Image4Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                    }
                                    break;
                                case 4:
                                    switch (j)  // light source sequence
                                    {
                                        case 0:
                                            intValue1 = Convert.ToInt32(txt_Image5Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_Image5Light2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_Image5Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_Image5Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_Image5Light2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_Image5Light1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_Image5Light4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_Image5Light3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_Image5Light2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_Image5Light1.Value);
                                            intLightSourceUsed++;
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




        private void btn_AdvanceSettings_Click(object sender, EventArgs e)
        {
            AdjustCameraForm objForm = new AdjustCameraForm(m_objAVTFireGrab, m_strSelectedRecipe, m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);
            objForm.Location = new Point(670, 200);
            objForm.ShowDialog();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnPixelGrayMaxRangeON = false;
            ReadSettings();
            if (m_smVisionInfo.g_blnWantCheckPH && m_blnViewImagePHPrev)
            {
                ReadPHSettings();
            }
            if (m_smVisionInfo.g_blnWantCheckEmpty && m_blnViewImageEmptyPrev)
            {
                ReadEmptySettings();
            }
            Close();
            Dispose();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveSettings();

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            Close();
            Dispose();
        }




        private void tabCtrl_Camera_TabIndexChanged(object sender, EventArgs e)
        {
            switch (tabCtrl_Camera.SelectedTab.Name)
            {
                case "tp_GrabSetting1":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabSetting2":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 1;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabSetting3":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 2;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabSetting4":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 3;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabSetting5":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 4;
                    m_smVisionInfo.g_intImageMergeType = m_intMergeType;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabPH":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_intImageMergeType = 0;
                    m_smVisionInfo.g_blnViewPHImage = true;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    break;
                case "tp_GrabEmpty":
                    m_intSelectedImage = m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_intImageMergeType = 0;
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = true;
                    break;
            }

            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_intSelectedImage]);

            m_blnRepaint = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }




        private void CameraSettings_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_blncboImageView = false;
            m_smVisionInfo.g_blnViewNormalImage = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void CameraSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Camera Setting Page Closed", "Exit Camera Setting Page", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewNormalImage = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_blnSeparateGrab = false;
            m_smVisionInfo.g_intImageMergeType = m_intMergeType;

            m_smVisionInfo.g_blnViewPHImage = m_blnViewImagePHPrev;
            m_smVisionInfo.g_blnViewEmptyImage = m_blnViewImageEmptyPrev;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }




        private void ClientTimer_Tick(object sender, EventArgs e)
        {
            ClientTimer.Enabled = false;

            if (m_blnRepaint || m_smVisionInfo.AT_VM_UpdateHistogram || m_smVisionInfo.AT_PR_StartLiveImage)
            {
                pic_Histogram.Refresh();
                ImageDrawing.DrawHistogram(m_Graphic, m_smVisionInfo.g_objCameraROI);

                m_blnRepaint = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }

            ClientTimer.Enabled = true;

            if (m_smVisionInfo.g_blnPixelGrayMaxRangeON)
            {
                int intValue;
                if (int.TryParse(srmLabel10.Text, out intValue))
                {
                    if (intValue != m_smVisionInfo.g_fPixelGrayMaxRange)
                    {
                        srmLabel10.Text = m_smVisionInfo.g_fPixelGrayMaxRange.ToString();
                    }
                }
            }
            else
            {
                if (srmLabel10.Text != "0")
                {
                    srmLabel10.Text = "0";
                }
            }
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnRepaint = true;
        }

        private void chk_SeparateGrab_CheckedChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSeparateGrab = chk_SeparateGrab.Checked;
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
                fValue = fValue * 100f;   // 1% == Shuttle value 100. For AVTVimba, ST 1 == 100 microsecond. So, 100% == 100% * 100 = ST 10000 = 10ms

                if (m_intSelectedImage == 0 && !m_objAVTFireGrab.SetCameraParameter(1, (uint)fValue))
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }
            else if (m_objTeliCamera != null)
            {
                fValue = fValue * 100f;     // 1% == Shuttle value 100. For Teli, ST 1 == 1 microsecond. So, 100% == 100% * 100 = ST 10000 = 10000 * 1 microsecond = 10ms

                if (m_intSelectedImage == 0 && !m_objTeliCamera.SetCameraParameter(1, fValue))
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }

            m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] = fValue;

            //if (tabCtrl_Camera.SelectedTab == tp_GrabSetting1)
            //{
            //    lbl_ShuttleTime1.Text = "(" + (m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] / 10f).ToString() + " ms )";
            //}
            //else if (tabCtrl_Camera.SelectedTab == tp_GrabSetting2)
            //{
            //    lbl_ShuttleTime1.Text = "(" + (m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] / 10f).ToString() + " ms )";
            //}
            //else if (tabCtrl_Camera.SelectedTab == tp_GrabSetting3)
            //{
            //    lbl_ShuttleTime1.Text = "(" + (m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] / 10f).ToString() + " ms )";
            //}
            //else if (tabCtrl_Camera.SelectedTab == tp_GrabSetting4)
            //{
            //    lbl_ShuttleTime1.Text = "(" + (m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] / 10f).ToString() + " ms )";
            //}
            //else if (tabCtrl_Camera.SelectedTab == tp_GrabPH)
            //{
            //    lbl_ShuttleTime1.Text = "(" + (m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] / 10f).ToString() + " ms )";
            //}
            //else if (tabCtrl_Camera.SelectedTab == tp_GrabEmpty)
            //{
            //    lbl_ShuttleTime1.Text = "(" + (m_smVisionInfo.g_arrCameraShuttle[m_intSelectedImage] / 10f).ToString() + " ms )";
            //}



        }

        private void chk_CalculateGrayPixel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnPixelGrayMaxRangeON = chk_CalculateGrayPixel.Checked;

            if (!chk_CalculateGrayPixel.Checked)
            {
                m_smVisionInfo.g_fPixelGrayMaxRange = -1;
            }
        }

        private void txt_CameraGrabDelay_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //m_objAVTFireGrab.ref_intNextGrabDelay = Convert.ToInt32(txt_CameraGrabDelay.Text);
        }

        private void UpdatePHGUI()
        {

            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

            // Get camera grab delay, shuttle and gain setting from camera file to keep as previous setting
            //m_smVisionInfo.g_objPositioning.LoadPHCamera(m_strPHPath, m_smVisionInfo.g_strVisionFolderName);


            //m_fPHCameraShuttlePrev = m_smVisionInfo.g_objPositioning.ref_fPHCameraShutter;

            //m_uintPHCameraGainPrev = m_smVisionInfo.g_objPositioning.ref_uintPHCameraGain;
            objFile = new XmlParser(m_strFilePath);
            objFile.GetFirstSection(m_smVisionInfo.g_strVisionFolderName);

            m_fPHCameraShuttlePrev = objFile.GetValueAsFloat("PHShutter", 200f);

            m_uintPHCameraGainPrev = objFile.GetValueAsUInt("PHGain", 20);

            m_fPHImageGainPrev = objFile.GetValueAsFloat("PHImageGain", 1.0f);

            if (m_objTeliCamera != null)
            {
                int intShuttleValue;

                intShuttleValue = (int)Math.Round(m_fPHCameraShuttlePrev / 100, 0, MidpointRounding.AwayFromZero);
                if (intShuttleValue > 0 && intShuttleValue <= 100)
                    txt_CameraShutterPH.Value = intShuttleValue;
                else
                    txt_CameraShutterPH.Value = 10;

                txt_CameraGainPH.Value = Math.Min(50, (int)Math.Round((float)m_uintPHCameraGainPrev, 0, MidpointRounding.AwayFromZero)); ;// Math.Min(50, (int)Math.Round((float)m_fPHImageGainPrev, 0, MidpointRounding.AwayFromZero));

                if (txt_CameraGainPH.Value == 50)
                    if (m_fPHImageGainPrev >= 1)
                        txt_CameraGainPH.Value += Convert.ToUInt32((m_fPHImageGainPrev - 1) / 0.18f);
            }


            int intLightSourceNo = 0;

            // add image
            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is image i using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 1)) > 0)
                {
                    intLightSourceNo++;

                    SequenceArray objSeq = new SequenceArray();
                    objSeq.ref_imageNo = 0;
                    objSeq.ref_LightSourceNo = j;

                    switch (intLightSourceNo)  // light source sequence
                    {
                        case 1:
                            lbl_ImagePHLight1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImagePHLight1.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_ImagePHLight1.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            lbl_ImagePHLabel1.Text = "(1-" + intMaxLimit.ToString() + ")";
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImagePHLight1.Tag = m_arrSeq.Count;
                            break;
                        case 2:
                            lbl_ImagePHLight2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImagePHLight2.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_ImagePHLight2.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            lbl_ImagePHLabel2.Text = "(1-" + intMaxLimit.ToString() + ")";

                            lbl_ImagePHLight2.Visible = true;
                            txt_ImagePHLight2.Visible = true;
                            lbl_ImagePHLabel2.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImagePHLight2.Tag = m_arrSeq.Count;
                            break;
                        case 3:
                            lbl_ImagePHLight3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImagePHLight3.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_ImagePHLight3.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            lbl_ImagePHLabel3.Text = "(1-" + intMaxLimit.ToString() + ")";

                            lbl_ImagePHLight3.Visible = true;
                            txt_ImagePHLight3.Visible = true;
                            lbl_ImagePHLabel3.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImagePHLight3.Tag = m_arrSeq.Count;
                            break;
                        case 4:
                            lbl_ImagePHLight4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImagePHLight4.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_ImagePHLight4.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            lbl_ImagePHLabel4.Text = "(1-" + intMaxLimit.ToString() + ")";

                            lbl_ImagePHLight4.Visible = true;
                            txt_ImagePHLight4.Visible = true;
                            lbl_ImagePHLabel4.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImagePHLight4.Tag = m_arrSeq.Count;
                            break;
                    }
                    m_arrSeq.Add(objSeq);
                }
            }
        }

        private void SavePHSettings()
        {
            if (txt_CameraGainPH.Value.ToString() == null || txt_CameraGainPH.Value.ToString() == "" ||
                txt_CameraShutterPH.Value.ToString() == null || txt_CameraShutterPH.Value.ToString() == "" ||
                txt_ImagePHLight1.Value.ToString() == null || txt_ImagePHLight1.Value.ToString() == "" ||
                txt_ImagePHLight2.Value.ToString() == null || txt_ImagePHLight2.Value.ToString() == "" ||
                txt_ImagePHLight3.Value.ToString() == null || txt_ImagePHLight3.Value.ToString() == "" ||
                txt_ImagePHLight4.Value.ToString() == null || txt_ImagePHLight4.Value.ToString() == "")
            {
                SRMMessageBox.Show("Value for Grab PH setting cannot be empty!");
                return;
            }


            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);


            objFile.WriteElement1Value("PHShutter", ((int)(txt_CameraShutterPH.Value) * 100).ToString());

            if (txt_CameraGainPH.Value <= 50)
                objFile.WriteElement1Value("PHGain", ((uint)Math.Round((float)txt_CameraGainPH.Value, 0, MidpointRounding.AwayFromZero)).ToString());
            else
                objFile.WriteElement1Value("PHGain","50");

            objFile.WriteElement1Value("PHImageGain", m_smVisionInfo.g_fPHImageGain);

            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                objFile.WriteElement1Value(m_smVisionInfo.g_arrLightSource[i].ref_strType, "");

                switch (i)
                {
                    case 0:
                        objFile.WriteElement2Value("PH", txt_ImagePHLight1.Value.ToString());
                        break;
                    case 1:
                        objFile.WriteElement2Value("PH", txt_ImagePHLight2.Value.ToString());
                        break;
                    case 2:
                        objFile.WriteElement2Value("PH", txt_ImagePHLight3.Value.ToString());
                        break;
                    case 3:
                        objFile.WriteElement2Value("PH", txt_ImagePHLight4.Value.ToString());
                        break;
                }
            }

            objFile.WriteEndElement();
        }

        private void txt_CameraShutterPH_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = Convert.ToSingle(((NumericUpDown)sender).Value);

            if (m_objTeliCamera != null)
            {
                fValue = fValue * 100f;     // 1% == Shuttle value 100. For Teli, ST 1 == 1 microsecond. So, 100% == 100% * 100 = ST 10000 = 10000 * 1 microsecond = 10ms

                if (!m_objTeliCamera.SetCameraParameter(1, fValue))
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }
            m_smVisionInfo.g_fPHCameraShuttle = fValue;
        }

        private void txt_CameraGainPH_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UInt32 intValue = Convert.ToUInt32(((NumericUpDown)sender).Value);
            if (intValue <= 50)
            {
                m_smVisionInfo.g_fPHImageGain = 1;
                if (m_objTeliCamera != null)
                {

                    // intValue = ((uint)Math.Round((float)intValue * 6.8, 0, MidpointRounding.AwayFromZero));
                    float fValue = intValue;
                    if (!m_objTeliCamera.SetCameraParameter(2, fValue))
                        SRMMessageBox.Show("Fail to set camera's gain");

                }
                m_smVisionInfo.g_uintPHCameraGain = intValue;
            }
            else
            {
                m_smVisionInfo.g_fPHImageGain = 1 + (intValue - 50) * 0.18f;
            }
        }

        private void txt_ImagePHLight_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((NumericUpDown)sender).Tag);

            LightSource objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intTag].ref_LightSourceNo];
            int intValue = Convert.ToInt32(((NumericUpDown)sender).Value);
            objLight.ref_PHValue = intValue;

            if (m_smVisionInfo.g_intLightControllerType == 2)
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
                                            intValue1 = Convert.ToInt32(txt_ImagePHLight1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_ImagePHLight2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_ImagePHLight1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_ImagePHLight3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_ImagePHLight2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_ImagePHLight1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_ImagePHLight4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_ImagePHLight3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_ImagePHLight2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_ImagePHLight1.Value);
                                            intLightSourceUsed++;
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

        private void ReadPHSettings()
        {
            if (m_smVisionInfo.g_intLightControllerType == 2)
            {
                m_smVisionInfo.g_uintPHCameraGain = m_uintPHCameraGainPrev;
                m_smVisionInfo.g_fPHImageGain = m_fPHImageGainPrev;
                m_smVisionInfo.g_fPHCameraShuttle = m_fPHCameraShuttlePrev;

                if (m_blnLEDi)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        int intValue1 = 0;
                        int intValue2 = 0;
                        int intValue3 = 0;
                        int intValue4 = 0;

                        for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                        {
                            // if this image no is in array k
                            if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                            {
                                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[0] == i)
                                {
                                    switch (j)
                                    {
                                        case 0:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue1 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                                            }
                                            break;
                                        case 1:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue2 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                                            }
                                            break;
                                        case 2:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue3 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                                            }
                                            break;
                                        case 3:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                                            }
                                            break;
                                    }

                                    break;
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
                    ////Set to run mode
                    //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    //Thread.Sleep(10);
                }
            }
        }

        private void UpdateEmptyGUI()
        {

            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

            // Get camera grab delay, shuttle and gain setting from camera file to keep as previous setting
            //m_smVisionInfo.g_objPositioning.LoadEmptyCamera(m_strEmptyPath, m_smVisionInfo.g_strVisionFolderName);


            //m_fEmptyCameraShuttlePrev = m_smVisionInfo.g_objPositioning.ref_fEmptyCameraShutter;

            //m_uintEmptyCameraGainPrev = m_smVisionInfo.g_objPositioning.ref_uintEmptyCameraGain;
            objFile = new XmlParser(m_strFilePath);
            objFile.GetFirstSection(m_smVisionInfo.g_strVisionFolderName);

            m_fEmptyCameraShuttlePrev = objFile.GetValueAsFloat("EmptyShutter", 200f);

            m_uintEmptyCameraGainPrev = objFile.GetValueAsUInt("EmptyGain", 20);

            m_fEmptyImageGainPrev = objFile.GetValueAsFloat("EmptyImageGain", 1.0f);

            if (m_objTeliCamera != null)
            {
                int intShuttleValue;

                intShuttleValue = (int)Math.Round(m_fEmptyCameraShuttlePrev / 100, 0, MidpointRounding.AwayFromZero);
                if (intShuttleValue > 0 && intShuttleValue <= 100)
                    txt_CameraShutterEmpty.Value = intShuttleValue;
                else
                    txt_CameraShutterEmpty.Value = 10;

                
                txt_CameraGainEmpty.Value = Math.Min(50, (int)Math.Round((float)m_uintEmptyCameraGainPrev, 0, MidpointRounding.AwayFromZero));// Math.Min(50, (int)Math.Round((float)m_fEmptyImageGainPrev, 0, MidpointRounding.AwayFromZero));

                if (txt_CameraGainEmpty.Value == 50)
                    if (m_fEmptyImageGainPrev >= 1)
                        txt_CameraGainEmpty.Value += Convert.ToUInt32((m_fEmptyImageGainPrev - 1) / 0.18f);
            }


            int intLightSourceNo = 0;

            // add image
            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is image i using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 1)) > 0)
                {
                    intLightSourceNo++;

                    SequenceArray objSeq = new SequenceArray();
                    objSeq.ref_imageNo = 0;
                    objSeq.ref_LightSourceNo = j;

                    switch (intLightSourceNo)  // light source sequence
                    {
                        case 1:
                            lbl_ImageEmptyLight1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImageEmptyLight1.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_ImageEmptyLight1.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            lbl_ImageEmptyLabel1.Text = "(1-" + intMaxLimit.ToString() + ")";
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImageEmptyLight1.Tag = m_arrSeq.Count;
                            break;
                        case 2:
                            lbl_ImageEmptyLight2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImageEmptyLight2.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_ImageEmptyLight2.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            lbl_ImageEmptyLabel2.Text = "(1-" + intMaxLimit.ToString() + ")";

                            lbl_ImageEmptyLight2.Visible = true;
                            txt_ImageEmptyLight2.Visible = true;
                            lbl_ImageEmptyLabel2.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImageEmptyLight2.Tag = m_arrSeq.Count;
                            break;
                        case 3:
                            lbl_ImageEmptyLight3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImageEmptyLight3.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_ImageEmptyLight3.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            lbl_ImageEmptyLabel3.Text = "(1-" + intMaxLimit.ToString() + ")";

                            lbl_ImageEmptyLight3.Visible = true;
                            txt_ImageEmptyLight3.Visible = true;
                            lbl_ImageEmptyLabel3.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImageEmptyLight3.Tag = m_arrSeq.Count;
                            break;
                        case 4:
                            lbl_ImageEmptyLight4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_ImageEmptyLight4.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_ImageEmptyLight4.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            lbl_ImageEmptyLabel4.Text = "(1-" + intMaxLimit.ToString() + ")";

                            lbl_ImageEmptyLight4.Visible = true;
                            txt_ImageEmptyLight4.Visible = true;
                            lbl_ImageEmptyLabel4.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_ImageEmptyLight4.Tag = m_arrSeq.Count;
                            break;
                    }
                    m_arrSeq.Add(objSeq);
                }
            }
        }

        private void txt_CameraShutterEmpty_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = Convert.ToSingle(((NumericUpDown)sender).Value);

            if (m_objTeliCamera != null)
            {
                fValue = fValue * 100f;     // 1% == Shuttle value 100. For Teli, ST 1 == 1 microsecond. So, 100% == 100% * 100 = ST 10000 = 10000 * 1 microsecond = 10ms

                if (!m_objTeliCamera.SetCameraParameter(1, fValue))
                    SRMMessageBox.Show("Fail to set camera's shuttle");
            }
            m_smVisionInfo.g_fEmptyCameraShuttle = fValue;
        }

        private void txt_CameraGainEmpty_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UInt32 intValue = Convert.ToUInt32(((NumericUpDown)sender).Value);
            if (intValue <= 50)
            {
                m_smVisionInfo.g_fEmptyImageGain = 1;
                if (m_objTeliCamera != null)
                {

                    // intValue = ((uint)Math.Round((float)intValue * 6.8, 0, MidpointRounding.AwayFromZero));
                    float fValue = intValue;
                    if (!m_objTeliCamera.SetCameraParameter(2, fValue))
                        SRMMessageBox.Show("Fail to set camera's gain");

                }
                m_smVisionInfo.g_uintEmptyCameraGain = intValue;
            }
            else
            {
                m_smVisionInfo.g_fEmptyImageGain = 1 + (intValue - 50) * 0.18f;
            }
        }

        private void txt_ImageEmptyLight_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTag = Convert.ToInt32(((NumericUpDown)sender).Tag);

            LightSource objLight = m_smVisionInfo.g_arrLightSource[m_arrSeq[intTag].ref_LightSourceNo];
            int intValue = Convert.ToInt32(((NumericUpDown)sender).Value);
            objLight.ref_EmptyValue = intValue;

            if (m_smVisionInfo.g_intLightControllerType == 2)
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
                                            intValue1 = Convert.ToInt32(txt_ImageEmptyLight1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 1:
                                            if (intLightSourceUsed > 0)
                                                intValue2 = Convert.ToInt32(txt_ImageEmptyLight2.Value);
                                            else
                                                intValue2 = Convert.ToInt32(txt_ImageEmptyLight1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 2:
                                            if (intLightSourceUsed > 1)
                                                intValue3 = Convert.ToInt32(txt_ImageEmptyLight3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue3 = Convert.ToInt32(txt_ImageEmptyLight2.Value);
                                            else
                                                intValue3 = Convert.ToInt32(txt_ImageEmptyLight1.Value);
                                            intLightSourceUsed++;
                                            break;
                                        case 3:
                                            if (intLightSourceUsed > 2)
                                                intValue4 = Convert.ToInt32(txt_ImageEmptyLight4.Value);
                                            else if (intLightSourceUsed > 1)
                                                intValue4 = Convert.ToInt32(txt_ImageEmptyLight3.Value);
                                            else if (intLightSourceUsed > 0)
                                                intValue4 = Convert.ToInt32(txt_ImageEmptyLight2.Value);
                                            else
                                                intValue4 = Convert.ToInt32(txt_ImageEmptyLight1.Value);
                                            intLightSourceUsed++;
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

        private void ReadEmptySettings()
        {
            if (m_smVisionInfo.g_intLightControllerType == 2)
            {
                m_smVisionInfo.g_uintEmptyCameraGain = m_uintEmptyCameraGainPrev;
                m_smVisionInfo.g_fEmptyImageGain = m_fEmptyImageGainPrev;
                m_smVisionInfo.g_fEmptyCameraShuttle = m_fEmptyCameraShuttlePrev;

                if (m_blnLEDi)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        int intValue1 = 0;
                        int intValue2 = 0;
                        int intValue3 = 0;
                        int intValue4 = 0;

                        for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                        {
                            // if this image no is in array k
                            if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                            {
                                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[0] == i)
                                {
                                    switch (j)
                                    {
                                        case 0:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue1 = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                                            }
                                            break;
                                        case 1:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue2 = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                                            }
                                            break;
                                        case 2:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue3 = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                                            }
                                            break;
                                        case 3:
                                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                            {
                                                intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                                            }
                                            break;
                                    }

                                    break;
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
                    ////Set to run mode
                    //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    //Thread.Sleep(10);
                }
            }
        }

        private void btn_ExtraGain_Click(object sender, EventArgs e)
        {
            ExtraGainSettingForm objExtraGainSettingForm = new ExtraGainSettingForm(m_smVisionInfo, m_strSelectedRecipe, m_smProductionInfo.g_intUserGroup, m_smProductionInfo, m_smCustomizeInfo, tabCtrl_Camera.SelectedIndex);
            objExtraGainSettingForm.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField2();
                }
            }
            else if (m_intUserGroup == 5 && m_intUserGroup != m_smProductionInfo.g_intUserGroup)
            {
                m_smProductionInfo.g_intUserGroup = 5;
                m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                DisableField2();

            }
        }
    }
}