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
    public partial class CameraSettingsImageView : Form
    {
        #region Member Variables
        private int m_intTotalLightSourceNo = 0;

        private float m_fShutterPrev = 0;
        private int m_intUserGroup = 5;
        //private int m_intSelectedImage = 0;

        private bool m_blnInitDone = false;
        private double m_dRedRatioPrev = 2.309997559;
        private double m_dBlueRatioPrev = 2.539978027;
        private bool m_blnViewImagePHPrev = false;
        private bool m_blnViewImageEmptyPrev = false;
        private bool m_blnLEDi;
        private bool m_blnVT;
        private string m_strFilePath;
        private string m_strFolderPath;

        private int m_intMergeType;
        private string m_strCameraFilePath;
        private string m_strSelectedRecipe = "";
        private AVTVimba m_objAVTFireGrab;
        private IDSuEyeCamera m_objIDSCamera;
        private TeliCamera m_objTeliCamera;
        
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
        public class SequenceArray
        {
            public int ref_imageNo;
            public int ref_LightSourceNo;
            public int ref_LightSourceArray;
        }
        public CameraSettingsImageView(VisionInfo smVisionInfo, bool blnLEDi, bool blnVT,
            string strSelectedRecipe, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera,
            int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;

            m_blnLEDi = blnLEDi;
            m_blnVT = blnVT;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\";
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\GlobalCamera.xml";
            else
                m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                 "\\Camera.xml";
            // m_strPHPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\PHCamera.xml";
            m_strCameraFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe;
            m_objAVTFireGrab = objAVTFireGrab;
            m_objIDSCamera = objIDSCamera;
            m_objTeliCamera = objTeliCamera;
            m_intMergeType = m_smVisionInfo.g_intImageMergeType;
            m_blnViewImagePHPrev = m_smVisionInfo.g_blnViewPHImage;
            m_blnViewImageEmptyPrev = m_smVisionInfo.g_blnViewEmptyImage;
            InitializeComponent();

            m_smVisionInfo.g_intSelectedImage = 0;  // View image 1 first bcos selected tabpage is first image also.


                pnl_Lighting1.Visible = false;

                pnl_Lighting2.Visible = false;

                pnl_Lighting3.Visible = false;

                pnl_Lighting4.Visible = false;
       
                pnl_Lighting5.Visible = false;

                pnl_Lighting6.Visible = false;
       
                pnl_Lighting7.Visible = false;
    
                pnl_Lighting8.Visible = false;
         
                pnl_Lighting9.Visible = false;

            DisableField2();
            UpdateGUI();

            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                UpdatePHGUI();

            }


            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {
                UpdateEmptyGUI();
            }

            if (tre_ImageView.SelectedNode == null)
            {
                grpBox_Camera.Enabled = false;
                grpBox_Lighting.Enabled = false;
            }

            if (m_smVisionInfo.g_strVisionName.Contains("Pad5S") && m_intUserGroup == 1)
            {
                btn_ExtraGain.Visible = true;
            }
            else
            {
                btn_ExtraGain.Visible = false;
            }

            m_blnInitDone = true;

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
                txt_Shutter.Enabled = false;
                trackBar_Shutter.Enabled = false;
            }

            strChild2 = "Camera Gain";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_Gain.Enabled = false;
                trackBar_Gain.Enabled = false;
            }

            strChild2 = "Intensity";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_Lighting1.Enabled = false;
                pnl_Lighting2.Enabled = false;
                pnl_Lighting3.Enabled = false;
                pnl_Lighting4.Enabled = false;
                pnl_Lighting5.Enabled = false;
                pnl_Lighting6.Enabled = false;
                pnl_Lighting7.Enabled = false;
                pnl_Lighting8.Enabled = false;
                pnl_Lighting9.Enabled = false;
            }

            strChild2 = "Extra Gain Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_ExtraGain.Enabled = false;
            }

            strChild2 = "Enhance Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_EnhanceImage.Enabled = false;
            }

            strChild2 = "White Balance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_RedRatio.Enabled = false;
                txt_BlueRatio.Enabled = false;
                btn_AutoWhiteBalance.Enabled = false;
            }

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

            m_smVisionInfo.g_dRedRatio = m_dRedRatioPrev;
            m_smVisionInfo.g_dBlueRatio = m_dBlueRatioPrev;

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
                            // g_arrLightSource index represent quantity of lighting channel. (if have 3 lighting, mean g_arrLightSource.count have 3 also)
                            // g_arrLightSource[j].ref_arrValue index represent quantity of grab image. (If have 4 grabs, mean arrValue.count is 4 also)

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

                            // 2021 04 20 - CCENG: Reupdate intensity according to image display mode
                            LEDi_Control.UpdateIntensityValueAccordingToImageDisplayMode(m_smVisionInfo.g_intImageDisplayMode, i,
                                                                                        ref intValue1, ref intValue2, ref intValue3, ref intValue4,
                                                                                        ref intValue5, ref intValue6, ref intValue7, ref intValue8);

                            //Set all light source for sequence light controller for each grab
                            //LEDi_Control.SetSeqIntensity(arrCOMList[c], 0, i, intValue1, intValue2, intValue3, intValue4);

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
        private void UpdateGUI()
        {
            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);
            if (intMaxLimit == 31 && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml"))
            {
                if (SRMMessageBox.Show("Lighting limit has been set to 31 due to corruption of Option file. Do you want to recover from backup file?", "SRM Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Option_backup.xml", AppDomain.CurrentDomain.BaseDirectory + "Option.xml", true);
                    objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
                    objFile.GetFirstSection("LightSource");
                    intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);
                }
            }
            else if (intMaxLimit == 31)
                SRMMessageBox.Show("Option File corrupted!! Please check all option settings!!");
            grpBox_Lighting.Text = "Lighting (1-" + intMaxLimit.ToString() + ")";
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
            
            m_dRedRatioPrev = m_smVisionInfo.g_dRedRatio;
            m_dBlueRatioPrev = m_smVisionInfo.g_dBlueRatio;

            if (m_smVisionInfo.g_blnViewColorImage)
            {
                double dRedRatio = 0, dBlueRatio = 0;
                m_objTeliCamera.GetWhiteBalance(ref dRedRatio, ref dBlueRatio);

                txt_RedRatio.Value = Math.Max(1, Convert.ToDecimal(dRedRatio));
                txt_BlueRatio.Value = Math.Max(1, Convert.ToDecimal(dBlueRatio));

                //txt_RedRatio.DecimalPlaces = m_objTeliCamera.GetRedRatioMax().ToString().Length - m_objTeliCamera.GetRedRatioMax().ToString().IndexOf('.');
                txt_RedRatio.Minimum = Convert.ToDecimal(m_objTeliCamera.GetRedRatioMin());
                txt_RedRatio.Maximum = Convert.ToDecimal(m_objTeliCamera.GetRedRatioMax());

                //txt_BlueRatio.DecimalPlaces = m_objTeliCamera.GetBlueRatioMax().ToString().Length - m_objTeliCamera.GetBlueRatioMax().ToString().IndexOf('.');
                txt_BlueRatio.Minimum = Convert.ToDecimal(m_objTeliCamera.GetBlueRatioMin());
                txt_BlueRatio.Maximum = Convert.ToDecimal(m_objTeliCamera.GetBlueRatioMax());

                lbl_RedRatio.Visible = m_smProductionInfo.g_blnWantShowWhiteBalance;
                txt_RedRatio.Visible = m_smProductionInfo.g_blnWantShowWhiteBalance;
                lbl_BlueRatio.Visible = m_smProductionInfo.g_blnWantShowWhiteBalance;
                txt_BlueRatio.Visible = m_smProductionInfo.g_blnWantShowWhiteBalance;
                btn_AutoWhiteBalance.Visible = m_smProductionInfo.g_blnWantShowWhiteBalance;
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

            if (m_smVisionInfo.g_intImageMergeType != 0)
            {
                intImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);
            }

            for (int i = 0; i < intImageCount; i++)
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
                            //if (i == 0)
                            //{
                            //    if (m_smVisionInfo.g_blnWantCheckPH)
                            //    {
                            //        strImageViewChildName = "View PH";
                            //        ImageViewChild = new TreeNode(strImageViewChildName);
                            //        ImageView.Nodes.Add(ImageViewChild);
                            //    }

                            //    if (m_smVisionInfo.g_blnWantCheckEmpty)
                            //    {
                            //        strImageViewChildName = "View Empty";
                            //        ImageViewChild = new TreeNode(strImageViewChildName);
                            //        ImageView.Nodes.Add(ImageViewChild);
                            //    }
                            //}
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

                                //if (m_smVisionInfo.g_blnWantCheckPH)
                                //{
                                //    strImageViewChildName = "View PH";
                                //    ImageViewChild = new TreeNode(strImageViewChildName);
                                //    ImageView.Nodes.Add(ImageViewChild);
                                //}

                                //if (m_smVisionInfo.g_blnWantCheckEmpty)
                                //{
                                //    strImageViewChildName = "View Empty";
                                //    ImageViewChild = new TreeNode(strImageViewChildName);
                                //    ImageView.Nodes.Add(ImageViewChild);
                                //}
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

                                //if (m_smVisionInfo.g_blnWantCheckPH)
                                //{
                                //    strImageViewChildName = "View PH";
                                //    ImageViewChild = new TreeNode(strImageViewChildName);
                                //    ImageView.Nodes.Add(ImageViewChild);
                                //}

                                //if (m_smVisionInfo.g_blnWantCheckEmpty)
                                //{
                                //    strImageViewChildName = "View Empty";
                                //    ImageViewChild = new TreeNode(strImageViewChildName);
                                //    ImageView.Nodes.Add(ImageViewChild);
                                //}
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

                                //if (m_smVisionInfo.g_blnWantCheckPH)
                                //{
                                //    strImageViewChildName = "View PH";
                                //    ImageViewChild = new TreeNode(strImageViewChildName);
                                //    ImageView.Nodes.Add(ImageViewChild);
                                //}

                                //if (m_smVisionInfo.g_blnWantCheckEmpty)
                                //{
                                //    strImageViewChildName = "View Empty";
                                //    ImageViewChild = new TreeNode(strImageViewChildName);
                                //    ImageView.Nodes.Add(ImageViewChild);
                                //}
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

                if (i == intImageCount - 1)
                {
                    if (m_smVisionInfo.g_blnWantCheckPH)
                    {
                        ImageView = new TreeNode("View PH");
                        tre_ImageView.Nodes.Add(ImageView);
                        strImageViewChildName = "View PH";
                        TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                        ImageView.Nodes.Add(ImageViewChild);
                    }

                    if (m_smVisionInfo.g_blnWantCheckEmpty)
                    {
                        ImageView = new TreeNode("View Empty");
                        tre_ImageView.Nodes.Add(ImageView);
                        strImageViewChildName = "View Empty";
                        TreeNode ImageViewChild = new TreeNode(strImageViewChildName);
                        ImageView.Nodes.Add(ImageViewChild);
                    }
                }
            }

            tre_ImageView.ExpandAll();
            
            if (m_smVisionInfo.g_arrColorImages == null)
                intImageCount = m_smVisionInfo.g_arrImages.Count;
            else if (m_smVisionInfo.g_arrImages == null)
                intImageCount = m_smVisionInfo.g_arrColorImages.Count;
            else
                intImageCount = Math.Max(m_smVisionInfo.g_arrColorImages.Count, m_smVisionInfo.g_arrImages.Count);

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
                            case 5:
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
                        if (intLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4)) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                        {
                            if (i == 0)
                                m_intTotalLightSourceNo = intLightSourceNo;
                        }
                        m_arrSeq.Add(objSeq);
                    }
                }
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4)) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                    UpdateDisplayMode(i);
            }
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
                    txt_Shutter.Value = intShuttleValue;
                else
                    txt_Shutter.Value = 10;

                trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);

                txt_Gain.Value = Math.Min(50, (int)Math.Round((float)m_uintPHCameraGainPrev, 0, MidpointRounding.AwayFromZero)); ;// Math.Min(50, (int)Math.Round((float)m_fPHImageGainPrev, 0, MidpointRounding.AwayFromZero));

                if (txt_Gain.Value == 50)
                    if (m_fPHImageGainPrev >= 1)
                        txt_Gain.Value += Convert.ToUInt32((m_fPHImageGainPrev - 1) / 0.18f);

                trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
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
                            lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light1.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light1.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                            pnl_Lighting1.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light1.Tag = m_arrSeq.Count;
                            trackBar_Light1.Tag = m_arrSeq.Count;
                            break;
                        case 2:
                            lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light2.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light2.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);
                            pnl_Lighting2.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light2.Tag = m_arrSeq.Count;
                            trackBar_Light2.Tag = m_arrSeq.Count;
                            break;
                        case 3:
                            lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light3.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light3.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);
                            pnl_Lighting3.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light3.Tag = m_arrSeq.Count;
                            trackBar_Light3.Tag = m_arrSeq.Count;
                            break;
                        case 4:
                            lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light4.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light4.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);
                
                            pnl_Lighting4.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light4.Tag = m_arrSeq.Count;
                            trackBar_Light4.Tag = m_arrSeq.Count;
                            break;
                        case 5:
                            lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light5.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light5.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                            pnl_Lighting5.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light5.Tag = m_arrSeq.Count;
                            trackBar_Light5.Tag = m_arrSeq.Count;
                            break;
                        case 6:
                            lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light6.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light6.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                            pnl_Lighting6.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light6.Tag = m_arrSeq.Count;
                            trackBar_Light6.Tag = m_arrSeq.Count;
                            break;
                        case 7:
                            lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light7.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light7.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                            pnl_Lighting7.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light7.Tag = m_arrSeq.Count;
                            trackBar_Light7.Tag = m_arrSeq.Count;
                            break;
                        case 8:
                            lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light8.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_PHValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_PHValue = intMaxLimit;
                            txt_Light8.Value = m_arrLightSourcePrev[j].ref_PHValue;
                            trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                            pnl_Lighting8.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light8.Tag = m_arrSeq.Count;
                            trackBar_Light8.Tag = m_arrSeq.Count;
                            break;
                    }
                    m_arrSeq.Add(objSeq);
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
                    txt_Shutter.Value = intShuttleValue;
                else
                    txt_Shutter.Value = 10;

                trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);

                txt_Gain.Value = Math.Min(50, (int)Math.Round((float)m_uintEmptyCameraGainPrev, 0, MidpointRounding.AwayFromZero));// Math.Min(50, (int)Math.Round((float)m_fEmptyImageGainPrev, 0, MidpointRounding.AwayFromZero));

                if (txt_Gain.Value == 50)
                    if (m_fEmptyImageGainPrev >= 1)
                        txt_Gain.Value += Convert.ToUInt32((m_fEmptyImageGainPrev - 1) / 0.18f);

                trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
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
                            lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light1.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light1.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                            pnl_Lighting1.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light1.Tag = m_arrSeq.Count;
                            trackBar_Light1.Tag = m_arrSeq.Count;
                            break;
                        case 2:
                            lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light2.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light2.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                            pnl_Lighting2.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light2.Tag = m_arrSeq.Count;
                            trackBar_Light2.Tag = m_arrSeq.Count;
                            break;
                        case 3:
                            lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light3.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light3.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                            pnl_Lighting3.Visible = true;
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light3.Tag = m_arrSeq.Count;
                            trackBar_Light3.Tag = m_arrSeq.Count;
                            break;
                        case 4:
                            lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light4.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light4.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                            pnl_Lighting4.Visible = true;
                    
                            objSeq.ref_LightSourceArray = 0;
                            txt_Light4.Tag = m_arrSeq.Count;
                            trackBar_Light4.Tag = m_arrSeq.Count;
                            break;
                        case 5:
                            lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light5.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light5.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                            pnl_Lighting5.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light5.Tag = m_arrSeq.Count;
                            trackBar_Light5.Tag = m_arrSeq.Count;
                            break;
                        case 6:
                            lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light6.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light6.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                            pnl_Lighting6.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light6.Tag = m_arrSeq.Count;
                            trackBar_Light6.Tag = m_arrSeq.Count;
                            break;
                        case 7:
                            lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light7.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light7.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                            pnl_Lighting7.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light7.Tag = m_arrSeq.Count;
                            trackBar_Light7.Tag = m_arrSeq.Count;
                            break;
                        case 8:
                            lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light8.Maximum = intMaxLimit;
                            if (m_arrLightSourcePrev[j].ref_EmptyValue > intMaxLimit)
                                m_arrLightSourcePrev[j].ref_EmptyValue = intMaxLimit;
                            txt_Light8.Value = m_arrLightSourcePrev[j].ref_EmptyValue;
                            trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                            pnl_Lighting8.Visible = true;

                            objSeq.ref_LightSourceArray = 0;
                            txt_Light8.Tag = m_arrSeq.Count;
                            trackBar_Light8.Tag = m_arrSeq.Count;
                            break;
                    }
                    m_arrSeq.Add(objSeq);
                }
            }
        }
        private void UpdateSetting(int intImageIndex)
        {
            if ((m_smVisionInfo.g_strVisionName.Contains("Pad5S") || m_smVisionInfo.g_strVisionName.Contains("Li3D")) && m_intUserGroup == 1)
            {
                if (intImageIndex < 2 && m_smVisionInfo.g_intImageMergeType == 1)
                    btn_ExtraGain.Visible = true;
                else if (intImageIndex < 3 && m_smVisionInfo.g_intImageMergeType == 2)
                    btn_ExtraGain.Visible = true;
                else if (intImageIndex < 4 && m_smVisionInfo.g_intImageMergeType == 3)
                    btn_ExtraGain.Visible = true;
                else if (intImageIndex < 5 && m_smVisionInfo.g_intImageMergeType == 4)
                    btn_ExtraGain.Visible = true;
                else
                    btn_ExtraGain.Visible = false;
            }
            else
            {
                btn_ExtraGain.Visible = false;
            }

            if (intImageIndex == 0 && (m_smVisionInfo.g_strVisionName.Contains("Mark") || m_smVisionInfo.g_strVisionName.Contains("MO") ||
               m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")))
            {
                btn_EnhanceImage.Visible = true;
                grpBox_Camera.Controls.Add(this.btn_EnhanceImage);
                btn_EnhanceImage.Location = btn_ExtraGain.Location;
            }
            else
            {
                btn_EnhanceImage.Visible = false;
            }

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
            if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4)) // 2020-10-10 ZJYEOH : Only 8 Channels lighting will need to change display mode
                UpdateDisplayMode(intImageIndex);
            m_blnInitDone = true;
        }
        private void UpdatePHSetting()
        {
            btn_ExtraGain.Visible = false;

            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

            if (m_objTeliCamera != null)
            {
                int intShuttleValue;

                intShuttleValue = (int)Math.Round(m_smVisionInfo.g_fPHCameraShuttle / 100, 0, MidpointRounding.AwayFromZero);
                if (intShuttleValue > 0 && intShuttleValue <= 100)
                    txt_Shutter.Value = intShuttleValue;
                else
                    txt_Shutter.Value = 10;

                trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);

                txt_Gain.Value = Math.Min(50, (int)Math.Round((float)m_smVisionInfo.g_uintPHCameraGain, 0, MidpointRounding.AwayFromZero)); ;// Math.Min(50, (int)Math.Round((float)m_fPHImageGainPrev, 0, MidpointRounding.AwayFromZero));

                if (txt_Gain.Value == 50)
                    if (m_smVisionInfo.g_fPHImageGain >= 1)
                        txt_Gain.Value += Convert.ToUInt32((m_smVisionInfo.g_fPHImageGain - 1) / 0.18f);

                trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
            }


            int intLightSourceNo = 0;

            // add image
            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is image i using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 1)) > 0)
                {
                    intLightSourceNo++;

                    switch (intLightSourceNo)  // light source sequence
                    {
                        case 1:
                            lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light1.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                            pnl_Lighting1.Visible = true;
                            
                            txt_Light1.Tag = intLightSourceNo - 1;
                            trackBar_Light1.Tag = intLightSourceNo - 1;
                            break;
                        case 2:
                            lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light2.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);
                            pnl_Lighting2.Visible = true;
                            
                            txt_Light2.Tag = intLightSourceNo - 1;
                            trackBar_Light2.Tag = intLightSourceNo - 1;
                            break;
                        case 3:
                            lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light3.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);
                            pnl_Lighting3.Visible = true;
                            
                            txt_Light3.Tag = intLightSourceNo - 1;
                            trackBar_Light3.Tag = intLightSourceNo - 1;
                            break;
                        case 4:
                            lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light4.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                            pnl_Lighting4.Visible = true;
                            
                            txt_Light4.Tag = intLightSourceNo - 1;
                            trackBar_Light4.Tag = intLightSourceNo - 1;
                            break;
                        case 5:
                            lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light5.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                            pnl_Lighting5.Visible = true;


                            txt_Light5.Tag = intLightSourceNo - 1;
                            trackBar_Light5.Tag = intLightSourceNo - 1;
                            break;
                        case 6:
                            lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light6.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                            pnl_Lighting6.Visible = true;


                            txt_Light6.Tag = intLightSourceNo - 1;
                            trackBar_Light6.Tag = intLightSourceNo - 1;
                            break;
                        case 7:
                            lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light7.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                            pnl_Lighting7.Visible = true;


                            txt_Light7.Tag = intLightSourceNo - 1;
                            trackBar_Light7.Tag = intLightSourceNo - 1;
                            break;
                        case 8:
                            lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light8.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_PHValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_PHValue = intMaxLimit;
                            txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                            pnl_Lighting8.Visible = true;


                            txt_Light8.Tag = intLightSourceNo - 1;
                            trackBar_Light8.Tag = intLightSourceNo - 1;
                            break;
                    }
                    
                }
            }
            m_blnInitDone = true;
        }
        private void UpdateEmptySetting()
        {
            btn_ExtraGain.Visible = false;

            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

            if (m_objTeliCamera != null)
            {
                int intShuttleValue;

                intShuttleValue = (int)Math.Round(m_smVisionInfo.g_fEmptyCameraShuttle / 100, 0, MidpointRounding.AwayFromZero);
                if (intShuttleValue > 0 && intShuttleValue <= 100)
                    txt_Shutter.Value = intShuttleValue;
                else
                    txt_Shutter.Value = 10;

                trackBar_Shutter.Value = Convert.ToInt32(txt_Shutter.Value);

                txt_Gain.Value = Math.Min(50, (int)Math.Round((float)m_smVisionInfo.g_uintEmptyCameraGain, 0, MidpointRounding.AwayFromZero));// Math.Min(50, (int)Math.Round((float)m_fEmptyImageGainPrev, 0, MidpointRounding.AwayFromZero));

                if (txt_Gain.Value == 50)
                    if (m_smVisionInfo.g_fEmptyImageGain >= 1)
                        txt_Gain.Value += Convert.ToUInt32((m_smVisionInfo.g_fEmptyImageGain - 1) / 0.18f);

                trackBar_Gain.Value = Convert.ToInt32(txt_Gain.Value);
            }


            int intLightSourceNo = 0;

            // add image
            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                // Check is image i using the light source
                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 1)) > 0)
                {
                    intLightSourceNo++;
                    
                    switch (intLightSourceNo)  // light source sequence
                    {
                        case 1:
                            lbl_Light1.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light1.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light1.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light1.Value = Convert.ToInt32(txt_Light1.Value);
                            pnl_Lighting1.Visible = true;
                            
                            txt_Light1.Tag = intLightSourceNo - 1;
                            trackBar_Light1.Tag = intLightSourceNo - 1;
                            break;
                        case 2:
                            lbl_Light2.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light2.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light2.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light2.Value = Convert.ToInt32(txt_Light2.Value);

                            pnl_Lighting2.Visible = true;
                            
                            txt_Light2.Tag = intLightSourceNo - 1;
                            trackBar_Light2.Tag = intLightSourceNo - 1;
                            break;
                        case 3:
                            lbl_Light3.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light3.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light3.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light3.Value = Convert.ToInt32(txt_Light3.Value);

                            pnl_Lighting3.Visible = true;
                            
                            txt_Light3.Tag = intLightSourceNo - 1;
                            trackBar_Light3.Tag = intLightSourceNo - 1;
                            break;
                        case 4:
                            lbl_Light4.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light4.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light4.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light4.Value = Convert.ToInt32(txt_Light4.Value);

                            pnl_Lighting4.Visible = true;

                            
                            txt_Light4.Tag = intLightSourceNo - 1;
                            trackBar_Light4.Tag = intLightSourceNo - 1;
                            break;
                        case 5:
                            lbl_Light5.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light5.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light5.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light5.Value = Convert.ToInt32(txt_Light5.Value);

                            pnl_Lighting5.Visible = true;


                            txt_Light5.Tag = intLightSourceNo - 1;
                            trackBar_Light5.Tag = intLightSourceNo - 1;
                            break;
                        case 6:
                            lbl_Light6.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light6.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light6.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light6.Value = Convert.ToInt32(txt_Light6.Value);

                            pnl_Lighting6.Visible = true;


                            txt_Light6.Tag = intLightSourceNo - 1;
                            trackBar_Light6.Tag = intLightSourceNo - 1;
                            break;
                        case 7:
                            lbl_Light7.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light7.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light7.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light7.Value = Convert.ToInt32(txt_Light7.Value);

                            pnl_Lighting7.Visible = true;


                            txt_Light7.Tag = intLightSourceNo - 1;
                            trackBar_Light7.Tag = intLightSourceNo - 1;
                            break;
                        case 8:
                            lbl_Light8.Text = m_smVisionInfo.g_arrLightSource[j].ref_strType;
                            txt_Light8.Maximum = intMaxLimit;
                            if (m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue > intMaxLimit)
                                m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue = intMaxLimit;
                            txt_Light8.Value = m_smVisionInfo.g_arrLightSource[j].ref_EmptyValue;
                            trackBar_Light8.Value = Convert.ToInt32(txt_Light8.Value);

                            pnl_Lighting8.Visible = true;


                            txt_Light8.Tag = intLightSourceNo - 1;
                            trackBar_Light8.Tag = intLightSourceNo - 1;
                            break;
                    }
          
                }
            }
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
                                return intUserSelectImageIndex + 2;
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

        private void CameraSettingsImageView_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_blncboImageView = false;
            m_smVisionInfo.g_blnViewNormalImage = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void CameraSettingsImageView_FormClosing(object sender, FormClosingEventArgs e)
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

            if (!btn_AutoWhiteBalance.Enabled && !m_smVisionInfo.g_blnWhiteBalanceAuto)
            {
                if (m_smVisionInfo.g_dRedRatio != (double)Convert.ToDecimal(txt_RedRatio.Value))
                {
                    txt_RedRatio.Value = Convert.ToDecimal(m_smVisionInfo.g_dRedRatio);
                }

                if (m_smVisionInfo.g_dBlueRatio != (double)Convert.ToDecimal(txt_BlueRatio.Value))
                {
                    txt_BlueRatio.Value = Convert.ToDecimal(m_smVisionInfo.g_dBlueRatio);
                }

                btn_AutoWhiteBalance.Enabled = true;

            }

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
                    if (selectedNode.Text.ToString() == "View PH")
                    {
                        m_smVisionInfo.g_intSelectedImage = 0;
                        UpdatePHSetting();
                        m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_blnViewPHImage = true;
                        m_smVisionInfo.g_blnViewEmptyImage = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                    else if (selectedNode.Text.ToString() == "View Empty")
                    {
                        m_smVisionInfo.g_intSelectedImage = 0;
                        UpdateEmptySetting();
                        m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_blnViewPHImage = false;
                        m_smVisionInfo.g_blnViewEmptyImage = true;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                    else
                    {
                        int intImageViewNo = Convert.ToInt32(ParentNode.Text.ToString().Substring(ParentNode.Text.ToString().Length - 1, 1));
                        m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(intImageViewNo - 1);
                        UpdateSetting(GetGrabImageIndex(intImageViewNo, intselectedNodeIndex));
                        m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_blnViewPHImage = false;
                        m_smVisionInfo.g_blnViewEmptyImage = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
                else
                {
                    if (selectedNode.Text.ToString() == "View PH")
                    {
                        m_smVisionInfo.g_intSelectedImage = 0;
                        UpdatePHSetting();
                        m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_blnViewPHImage = true;
                        m_smVisionInfo.g_blnViewEmptyImage = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                    else if (selectedNode.Text.ToString() == "View Empty")
                    {
                        m_smVisionInfo.g_intSelectedImage = 0;
                        UpdateEmptySetting();
                        m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_blnViewPHImage = false;
                        m_smVisionInfo.g_blnViewEmptyImage = true;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                    else
                    {
                        int intImageViewNo = Convert.ToInt32(ParentNode.Text.ToString().Substring(ParentNode.Text.ToString().Length - 1, 1));
                        m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(intImageViewNo - 1);
                        UpdateSetting(GetGrabImageIndex(intImageViewNo, intselectedNodeIndex));
                        m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_blnViewPHImage = false;
                        m_smVisionInfo.g_blnViewEmptyImage = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }
            else
            {
                m_blnInitDone = false;

                grpBox_Camera.Enabled = false;
                grpBox_Lighting.Enabled = false;

                if (selectedNode.Text.ToString() == "View PH")
                {
                    m_smVisionInfo.g_intSelectedImage = 0;
                    UpdatePHSetting();
                    m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    m_smVisionInfo.g_blnViewPHImage = true;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
                else if (selectedNode.Text.ToString() == "View Empty")
                {
                    m_smVisionInfo.g_intSelectedImage = 0;
                    UpdateEmptySetting();
                    m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = true;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
                else
                {
                    int intImageViewNo = Convert.ToInt32(selectedNode.Text.ToString().Substring(selectedNode.Text.ToString().Length - 1, 1));
                    m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(intImageViewNo - 1);
                    UpdateSetting(GetGrabImageIndex(intImageViewNo, intselectedNodeIndex));
                    m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    m_smVisionInfo.g_blnViewPHImage = false;
                    m_smVisionInfo.g_blnViewEmptyImage = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
            }
        }

        private void btn_ExtraGain_Click(object sender, EventArgs e)
        {
            if (tre_ImageView.SelectedNode == null)
                return;
            int intSelectedImage = 0;
            if (tre_ImageView.SelectedNode.Text.ToString() != "View PH" && tre_ImageView.SelectedNode.Text.ToString() != "View Empty")
            {
                int intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));

                intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);
            }
            ExtraGainSettingForm objExtraGainSettingForm = new ExtraGainSettingForm(m_smVisionInfo, m_strSelectedRecipe, m_smProductionInfo.g_intUserGroup, m_smProductionInfo, m_smCustomizeInfo, intSelectedImage);
            objExtraGainSettingForm.ShowDialog();
        }

        private void txt_Shutter_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (tre_ImageView.SelectedNode == null)
                return;
            int intSelectedImage = 0;
            if (tre_ImageView.SelectedNode.Text.ToString() != "View PH" && tre_ImageView.SelectedNode.Text.ToString() != "View Empty")
            {
                int intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));

                intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);
            }

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

            if (tre_ImageView.SelectedNode.Text.ToString() == "View PH")
                m_smVisionInfo.g_fPHCameraShuttle = fValue;
            else if (tre_ImageView.SelectedNode.Text.ToString() == "View Empty")
                m_smVisionInfo.g_fEmptyCameraShuttle = fValue;
            else
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
            if (tre_ImageView.SelectedNode.Text.ToString() != "View PH" && tre_ImageView.SelectedNode.Text.ToString() != "View Empty")
            {
                int intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));

                intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);
            }

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

                if (tre_ImageView.SelectedNode.Text.ToString() == "View PH")
                    m_smVisionInfo.g_uintPHCameraGain = intValue;
                else if (tre_ImageView.SelectedNode.Text.ToString() == "View Empty")
                    m_smVisionInfo.g_uintEmptyCameraGain = intValue;
                else
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

                if (tre_ImageView.SelectedNode.Text.ToString() == "View PH")
                    m_smVisionInfo.g_uintPHCameraGain = 50;
                else if (tre_ImageView.SelectedNode.Text.ToString() == "View Empty")
                    m_smVisionInfo.g_uintEmptyCameraGain = 50;
                else
                    m_smVisionInfo.g_arrCameraGain[intSelectedImage] = 50;

                if (tre_ImageView.SelectedNode.Text.ToString() == "View PH")
                    m_smVisionInfo.g_fPHImageGain = 1 + (intValue - 50) * 0.18f;
                else if (tre_ImageView.SelectedNode.Text.ToString() == "View Empty")
                    m_smVisionInfo.g_fEmptyImageGain = 1 + (intValue - 50) * 0.18f;
                else
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
            if (tre_ImageView.SelectedNode.Text.ToString() != "View PH" && tre_ImageView.SelectedNode.Text.ToString() != "View Empty")
            {
                intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));
                STTrackLog.WriteLine("intImageViewNo = " + intImageViewNo);
                intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);
                STTrackLog.WriteLine("intSelectedImage = " + intSelectedImage);
            }

            int intTag = Convert.ToInt32(((NumericUpDown)sender).Tag);
            STTrackLog.WriteLine("Sendor = " + ((NumericUpDown)sender).Name);
            STTrackLog.WriteLine("Tag = " + ((NumericUpDown)sender).Tag.ToString());

            int intSeq = 0;
            for (int i = 0; i < m_arrSeq.Count;i++)
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
                for (int c = 0; c < arrCOMList.Count;c++)
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
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4))
                {
                    switch (intSelectedImage)
                    {
                        case 1: // Top Left
                            break;
                        case 2: // Bottom Right
                            if (m_smVisionInfo.g_intImageDisplayMode == 2) // Lead
                            {
                                txt_Light3.Value = intValue;
                            }
                            break;
                        case 0: // Center
                        default: // Other
                            if (m_smVisionInfo.g_intImageDisplayMode == 1) // Pad
                            {
                                txt_Light2.Value = intValue;
                            }
                            else if (m_smVisionInfo.g_intImageDisplayMode == 2) // Lead
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
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4))
                {
                    switch (intSelectedImage)
                    {
                        case 1: // Top Left
                            if (m_smVisionInfo.g_intImageDisplayMode == 2) // Lead
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
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4) && (intSelectedImage == 0 || intSelectedImage > 2))
                {
                    if (m_smVisionInfo.g_intImageDisplayMode == 1) // Pad
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
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4) && (intSelectedImage == 0 || intSelectedImage > 2))
                {
                    if (m_smVisionInfo.g_intImageDisplayMode == 1) // Pad
                    {
                        txt_Light6.Value = intValue;
                    }
                    else if (m_smVisionInfo.g_intImageDisplayMode == 2) // Lead
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
                if (m_intTotalLightSourceNo > 4 && (m_smVisionInfo.g_intImageMergeType == 2 || m_smVisionInfo.g_intImageMergeType == 4) && (intSelectedImage == 0 || intSelectedImage > 2))
                {
                    if (m_smVisionInfo.g_intImageDisplayMode == 1) // Pad
                    {
                        txt_Light8.Value = intValue;
                    }
                    else if (m_smVisionInfo.g_intImageDisplayMode == 2) // Lead
                    {
                        txt_Light8.Value = intValue;
                    }
                }
            }
            else if (Convert.ToInt32(trackBar_Light8.Tag) == intTag)
                trackBar_Light8.Value = intValue;
            else if (Convert.ToInt32(trackBar_Light9.Tag) == intTag)
                trackBar_Light9.Value = intValue;

            if (tre_ImageView.SelectedNode.Text.ToString() == "View PH")
                objLight.ref_PHValue = intValue;
            else if (tre_ImageView.SelectedNode.Text.ToString() == "View Empty")
                objLight.ref_EmptyValue = intValue;
            else
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
                    VT_Control.SetSeqIntensity(objLight.ref_intPortNo, intSelectedImage, objLight.ref_intChannel, intValue);
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

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveSettings();

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            Close();
            Dispose();
        }

        private void SaveSettings()
        {
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);



            
            STDeviceEdit.CopySettingFile(m_strCameraFilePath, "\\Camera.xml");


            //objFile.WriteElement1Value("Shutter", txt_CameraShutter.Text);
            objFile.WriteElement1Value("Shutter", m_smVisionInfo.g_fCameraShuttle);
            objFile.WriteElement1Value("GrabDelay", txt_CameraGrabDelay.Text);

            objFile.WriteElement1Value("RedRatio", m_smVisionInfo.g_dRedRatio);
            objFile.WriteElement1Value("BlueRatio", m_smVisionInfo.g_dBlueRatio);

            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                objFile.WriteElement1Value("Shutter" + i.ToString(), m_smVisionInfo.g_arrCameraShuttle[i], "Camera Setting-Image " + (i + 1).ToString(), true);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
            {
                objFile.WriteElement1Value("Gain" + i.ToString(), m_smVisionInfo.g_arrCameraGain[i], "Camera Setting-Image " + (i + 1).ToString(), true);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                objFile.WriteElement1Value("ImageGain" + i.ToString(), m_smVisionInfo.g_arrImageGain[i], "Camera Setting-Image " + (i + 1).ToString(), true);
            }

            if (m_smVisionInfo.g_blnWantCheckPH)
            {

                objFile.WriteElement1Value("PHShutter", m_smVisionInfo.g_fPHCameraShuttle.ToString(), "Camera Setting-Image PH", true);

                if (m_smVisionInfo.g_uintPHCameraGain <= 50)
                    objFile.WriteElement1Value("PHGain", ((uint)Math.Round((float)m_smVisionInfo.g_uintPHCameraGain, 0, MidpointRounding.AwayFromZero)).ToString(), "Camera Setting-Image PH", true);
                else
                    objFile.WriteElement1Value("PHGain", "50", "Camera Setting-Image PH", true);

                objFile.WriteElement1Value("PHImageGain", m_smVisionInfo.g_fPHImageGain, "Camera Setting-Image PH", true);

            }

            if (m_smVisionInfo.g_blnWantCheckEmpty)
            {

                objFile.WriteElement1Value("EmptyShutter", m_smVisionInfo.g_fEmptyCameraShuttle.ToString(), "Camera Setting-Image Empty", true);

                if (m_smVisionInfo.g_uintEmptyCameraGain <= 50)
                    objFile.WriteElement1Value("EmptyGain", ((uint)Math.Round((float)m_smVisionInfo.g_uintEmptyCameraGain, 0, MidpointRounding.AwayFromZero)).ToString(), "Camera Setting-Image Empty", true);
                else
                    objFile.WriteElement1Value("EmptyGain", "50", "Camera Setting-Image Empty", true);

                objFile.WriteElement1Value("EmptyImageGain", m_smVisionInfo.g_fEmptyImageGain, "Camera Setting-Image Empty", true);

            }

            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                {
                    string strSearch = m_smVisionInfo.g_arrLightSource[i].ref_strType.Replace(" ", string.Empty);
                    switch (i)
                    {
                        case 0:
                            objFile.WriteElement1Value("PH" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_PHValue.ToString(), "Camera Setting-Image PH", true);
                            break;
                        case 1:
                            objFile.WriteElement1Value("PH" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_PHValue.ToString(), "Camera Setting-Image PH", true);
                            break;
                        case 2:
                            objFile.WriteElement1Value("PH" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_PHValue.ToString(), "Camera Setting-Image PH", true);
                            break;
                        case 3:
                            objFile.WriteElement1Value("PH" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_PHValue.ToString(), "Camera Setting-Image PH", true);
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
                            objFile.WriteElement1Value("Empty" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_EmptyValue.ToString(), "Camera Setting-Image Empty", true);
                            break;
                        case 1:
                            objFile.WriteElement1Value("Empty" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_EmptyValue.ToString(), "Camera Setting-Image Empty", true);
                            break;
                        case 2:
                            objFile.WriteElement1Value("Empty" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_EmptyValue.ToString(), "Camera Setting-Image Empty", true);
                            break;
                        case 3:
                            objFile.WriteElement1Value("Empty" + strSearch, m_smVisionInfo.g_arrLightSource[i].ref_EmptyValue.ToString(), "Camera Setting-Image Empty", true);
                            break;
                    }
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                objFile.WriteElement1Value(m_smVisionInfo.g_arrLightSource[i].ref_strType, "", "Camera Setting-Image " + (i + 1).ToString(), true);
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[i].ref_arrValue.Count; j++)
                {
                    objFile.WriteElement2Value("Seq" + j.ToString(), m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j], "Camera Setting-Image " + (m_smVisionInfo.g_arrLightSource[i].ref_arrImageNo[j] + 1).ToString() + "-" + m_smVisionInfo.g_arrLightSource[i].ref_strType, true);
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

#if (RELEASE || Release_2_12 || RTXRelease)
                //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
                if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                {
                    File.Copy(m_strFolderPath + "GlobalCamera.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strFolderPath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\"))
                        {
                            File.Copy(m_strFolderPath + "GlobalCamera.xml", arrDirectory[j] + "GlobalCamera.xml", true);
                        }
                    }
                }
#elif (DEBUG || Debug_2_12 || RTXDebug)
            //2021-08-19 ZJYEOH : Overwrite global xml
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
            {
                File.Copy(m_strFolderPath + "GlobalCamera.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", true);
            }
#endif
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
            switch (m_smVisionInfo.g_intImageDisplayMode)
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

        private void btn_EnhanceImage_Click(object sender, EventArgs e)
        {
            if (tre_ImageView.SelectedNode == null)
                return;
            int intSelectedImage = 0;
            if (tre_ImageView.SelectedNode.Text.ToString() != "View PH" && tre_ImageView.SelectedNode.Text.ToString() != "View Empty")
            {
                int intImageViewNo = Convert.ToInt32(tre_ImageView.SelectedNode.Parent.Text.ToString().Substring(tre_ImageView.SelectedNode.Parent.Text.ToString().Length - 1, 1));

                intSelectedImage = GetGrabImageIndex(intImageViewNo, tre_ImageView.SelectedNode.Index);
            }
            EnchanceImageSettingForm objEnhanceImageSettingForm = new EnchanceImageSettingForm(m_smVisionInfo, m_strSelectedRecipe, m_smProductionInfo.g_intUserGroup, m_smProductionInfo, m_smCustomizeInfo, intSelectedImage);
            objEnhanceImageSettingForm.ShowDialog();
        }

        private void txt_RedRatio_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_dRedRatio = Convert.ToDouble(txt_RedRatio.Value);
        }

        private void txt_BlueRatio_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_dBlueRatio = Convert.ToDouble(txt_BlueRatio.Value);

        }

        private void btn_AutoWhiteBalance_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnWhiteBalanceAuto = true;
            btn_AutoWhiteBalance.Enabled = false;
            //m_objTeliCamera.SetWhiteBalanceAuto();

            //if (!m_smVisionInfo.AT_PR_StartLiveImage || m_smVisionInfo.AT_PR_PauseLiveImage)
            //{
            //    m_smVisionInfo.AT_PR_GrabImage = true;
            //    Thread.Sleep(5);
            //    m_smVisionInfo.AT_PR_GrabImage = false;

            //    double dRedRatio = 0, dBlueRatio = 0;
            //    m_objTeliCamera.GetWhiteBalance(ref dRedRatio, ref dBlueRatio);

            //    txt_RedRatio.Value = Convert.ToDecimal(dRedRatio);
            //    txt_BlueRatio.Value = Convert.ToDecimal(dBlueRatio);
            //}
            //else
            //{

            //    double dRedRatio = 0, dBlueRatio = 0;
            //    m_objTeliCamera.GetWhiteBalance(ref dRedRatio, ref dBlueRatio);

            //    txt_RedRatio.Value = Convert.ToDecimal(dRedRatio);
            //    txt_BlueRatio.Value = Convert.ToDecimal(dBlueRatio);
            //}
        }
    }
}
