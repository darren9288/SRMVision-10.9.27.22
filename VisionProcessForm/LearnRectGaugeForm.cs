using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;
using System.Windows.Forms;
using System.Reflection;

namespace VisionProcessForm
{

    public partial class LearnRectGaugeForm : Form
    {
        #region Member Variable

        //create local shared memory to hold common vision, the object is pass in by caller

        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomOption = new CustomOption();

        //create local shared memory to hold rectangle gauge setting, it is current working object
        //assign by caller from the array list          
        private RectGauge m_objGauge;
        private ArrayList m_arrVisionModule = new ArrayList();

        //local recipe name
        private string m_strSelectedRecipe;

        //timer
        private HiPerfTimer m_tmrExec = new HiPerfTimer();
        private UserRight m_objUserRight = new UserRight();

        //parent form will display it value
        public double m_dDuration;
        private ProductionInfo m_smProductionInfo;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smVisionInfo">shared memory of vision common info</param>
        /// <param name="strVisionName">vision module name</param>
        /// <param name="strFolder">folder to load and save rect gauge setting</param>
        public LearnRectGaugeForm(VisionInfo smVisionInfo, CustomOption smCustomOption,
            string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            //assign local variable
            m_smCustomOption = smCustomOption;
            m_smVisionInfo = smVisionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;

            FillComboBox();
            DisableField();
            UpdateGUI();
            // Make sure original image is displaying
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewPackageImage = true;
            // Make sure all roi is attached to original image
            //m_smVisionInfo.AT_PR_AttachImagetoROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            int intImageIndex = 0;
            RectGauge objRectGauge = new RectGauge(m_smVisionInfo.g_WorldShape);
            switch (m_arrVisionModule[0].ToString())
            {
                case "Orient":
                case "MarkOrient":
                    objRectGauge = m_smVisionInfo.g_arrOrientGauge[0];
                    //if (m_arrVisionModule.Count == 1)    // Mean Mark Orient only
                    //{
                    //    gb_GainSetting.Visible = false;
                    //}
                    break;
                case "Mark":
                    objRectGauge = m_smVisionInfo.g_arrMarkGauge[0];
                    //gb_GainSetting.Visible = false;
                    break;
                case "Package":
                    objRectGauge = m_smVisionInfo.g_arrPackageGauge[0];
                    intImageIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);    // why index 0?
                    break;
                case "Package1":
                    objRectGauge = m_smVisionInfo.g_arrPackageGauge[0];
                    intImageIndex = 1;
                    break;
                case "Package2":
                    objRectGauge = m_smVisionInfo.g_arrPackageGauge2[0];
                    intImageIndex = 2;
                    break;
            }

            trackBar_Gain.Value = (int)objRectGauge.ref_fGainValue;

            if (!m_smVisionInfo.g_blnDisableMOGauge)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs.Count; i++)
                {
                    m_smVisionInfo.g_arrOrientROIs[i][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                    m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                }
            }

            if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                    m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            }


            txt_GainValue.Text = (Convert.ToSingle(trackBar_Gain.Value) / 1000).ToString();

            m_smVisionInfo.g_arrImages[intImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(txt_GainValue.Text));

            UpdateGUI(objRectGauge);

            chk_WantGauge.Checked = m_smVisionInfo.g_blnWantGauge;
            if (chk_WantGauge.Checked)
            {
                // Enable all gauge parameter GUI
                cbo_RectGList.Enabled = true;
                tbCtrl_Gauge.Enabled = true;
                gb_GainSetting.Enabled = true;
            }
            else
            {
                // Disable all gauge parameter GUI
                //cbo_RectGList.Enabled = false;    // 2019 05 30 - CCENG: Not need disabled bcos need to go to Package gauge.
                tbCtrl_Gauge.Enabled = false;
                gb_GainSetting.Enabled = false;
            }
        }

        /// <summary>
        /// Add Gauge to Search ROI
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="arrGauge">rectangle gauge</param>
        private void AddGauge(ROI objSearchROI, List<RectGauge> arrGauge)
        {
            XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Gauge.xml");
            objFile.GetFirstSection("RectG");

            //create new gauge and attach to selected ROI
            m_objGauge = new RectGauge(m_smVisionInfo.g_WorldShape);

            //attach searcher parent
            m_objGauge.SetRectGaugePlacement(objSearchROI, objFile.GetValueAsFloat("Tolerance", 25),
                objFile.GetValueAsInt("SizeTolerance", 10));

            //set gauge measurement
            m_objGauge.ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
            m_objGauge.ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);

            //set gauge setting
            m_objGauge.SetRectGaugeSetting(objFile.GetValueAsInt("Thickness", 13), objFile.GetValueAsInt("Filter", 1),
                objFile.GetValueAsInt("Threshold", 2), objFile.GetValueAsInt("MinAmp", 10), objFile.GetValueAsInt("MinArea", 0));

            //set gauge fitting sampling step
            m_objGauge.SetRectGaugeFitting(objFile.GetValueAsInt("SamplingStep", 5));


            m_objGauge.ref_GaugeFilteringPasses = objFile.GetValueAsInt("FilteringPass", 0);
            m_objGauge.ref_GaugeFilterThreshold = objFile.GetValueAsFloat("FilteringThreshold", 3.0f);

            arrGauge.Add(m_objGauge);
        }

        /// <summary>
        /// Add search ROI 
        /// </summary>
        /// <param name="intUnitNo">unit no</param>
        /// <param name="arrROIs">ROI</param>
        /// <param name="arrGauge">rectangle gauge</param>
        private void AddSearchROI(int intUnitNo, List<List<ROI>> arrROIs, List<RectGauge> arrGauge)
        {
            bool blnFound = false;
            ROI objROI = new ROI();

            for (int i = arrROIs.Count; i <= intUnitNo; i++)
            {
                arrROIs.Add(new List<ROI>());
            }

            if (arrROIs[intUnitNo].Count > 0)
                blnFound = true;

            if (!blnFound)
            {
                if (intUnitNo == 0)
                    objROI = new ROI("Test ROI", 1);
                else
                    objROI = new ROI("ReTest ROI", 1);

                if (intUnitNo == 0)
                {
                    int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 100;
                    int intPositionY = (480 / 2) - 100;
                    objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                }
                else if (intUnitNo != 0)
                {
                    int intPositionX = (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - (((ROI)arrROIs[0][0]).ref_ROIWidth / 2);
                    objROI.LoadROISetting(intPositionX, ((ROI)arrROIs[0][0]).ref_ROIPositionY,
                        ((ROI)arrROIs[0][0]).ref_ROIWidth,
                        ((ROI)arrROIs[0][0]).ref_ROIHeight);
                }

                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                AddGauge(objROI, arrGauge);
                arrROIs[intUnitNo].Add(objROI);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void AddSearchROI(int intUnitNo, List<List<ROI>> arrROIs, List<RectGauge> arrGauge, List<RectGauge> arrGauge2)
        {
            bool blnFound = false;
            ROI objROI = new ROI();

            for (int i = arrROIs.Count; i <= intUnitNo; i++)
            {
                arrROIs.Add(new List<ROI>());
            }

            if (arrROIs[intUnitNo].Count > 0)
                blnFound = true;

            //if (!blnFound)
            {
                if (intUnitNo == 0)
                    objROI = new ROI("Test ROI", 1);
                else
                    objROI = new ROI("ReTest ROI", 1);

                if (intUnitNo == 0)
                {
                    int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 100;
                    int intPositionY = (480 / 2) - 100;
                    objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                }
                else if (intUnitNo != 0)
                {
                    int intPositionX = (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - (((ROI)arrROIs[0][0]).ref_ROIWidth / 2);
                    objROI.LoadROISetting(intPositionX, ((ROI)arrROIs[0][0]).ref_ROIPositionY,
                        ((ROI)arrROIs[0][0]).ref_ROIWidth,
                        ((ROI)arrROIs[0][0]).ref_ROIHeight);
                }

                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                if (arrGauge.Count < arrROIs.Count)
                    AddGauge(objROI, arrGauge);
                if (arrGauge2.Count < arrROIs.Count)
                    AddGauge(objROI, arrGauge2);

                if (!blnFound)
                    arrROIs[intUnitNo].Add(objROI);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        /// <summary>
        /// Copy orient gauge and ROI information to package 
        /// </summary>
        private void CopyOrientInfoToPackage()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                ROI objROI = new ROI();
                RectGauge objGauge = new RectGauge(m_smVisionInfo.g_WorldShape);

                ((ROI)m_smVisionInfo.g_arrOrientROIs[i][0]).CopyTo(ref objROI);
                m_smVisionInfo.g_arrOrientGauge[i].CopyTo(ref objGauge);

                if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                    m_smVisionInfo.g_arrPackageROIs.Add(new List<ROI>());

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 0)
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objROI);
                else
                    m_smVisionInfo.g_arrPackageROIs[i][0] = objROI;

                if (m_smVisionInfo.g_arrPackageGauge.Count <= i)
                    m_smVisionInfo.g_arrPackageGauge.Add(objGauge);
                else
                    m_smVisionInfo.g_arrPackageGauge[i] = objGauge;
            }
        }

        private void CopyROIGaugeInfoToOtherModule(List<List<ROI>> arrSourceROI, List<RectGauge> arrSourceGauge,
            ref List<List<ROI>> arrDestinationROI, ref List<RectGauge> arrDestinationGauge)
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                //ROI objROI = new ROI();
                RectGauge objGauge = new RectGauge(m_smVisionInfo.g_WorldShape);

                //arrSourceROI[i][0].CopyTo(ref objROI);
                arrSourceGauge[i].CopyTo(ref objGauge);

                //if (i >= arrDestinationROI.Count)
                //    arrDestinationROI.Add(new List<ROI>());

                //if (arrDestinationROI[i].Count == 0)
                //    arrDestinationROI[i].Add(objROI);
                //else
                //    arrDestinationROI[i][0] = objROI;

                if (arrDestinationGauge.Count <= i)
                    arrDestinationGauge.Add(objGauge);
                else
                    arrDestinationGauge[i] = objGauge;
            }
        }

        private void CopyInfoToMark()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                ROI objROI = new ROI();
                RectGauge objGauge = new RectGauge(m_smVisionInfo.g_WorldShape);

                ((ROI)m_smVisionInfo.g_arrOrientROIs[i][0]).CopyTo(ref objROI);
                objGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[i];

                if (i >= m_smVisionInfo.g_arrMarkROIs.Count)
                    m_smVisionInfo.g_arrMarkROIs.Add(new List<ROI>());

                if (m_smVisionInfo.g_arrMarkROIs[i].Count == 0)
                    m_smVisionInfo.g_arrMarkROIs[i].Add(objROI);
                else
                    m_smVisionInfo.g_arrMarkROIs[i][0] = objROI;
                if (m_smVisionInfo.g_arrMarkGauge.Count <= i)
                    m_smVisionInfo.g_arrMarkGauge.Add(objGauge);
                else
                    m_smVisionInfo.g_arrMarkGauge[i] = objGauge;
            }
        }

        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Gauge Page";
            string strChild2 = "";

            strChild2 = "Position Config";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_PosTolerance.Enabled = false;
                txt_Size.Enabled = false;
            }

            strChild2 = "Measurement Config";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                tbCtrl_Gauge.TabPages.Remove(tp_Measurement);
            }

            strChild2 = "Fitting Config";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_FittingSamplingStep.Enabled = false;
                txt_FilteringPass.Enabled = false;
                txt_FilteringThreshold.Enabled = false;
            }
        }

        /// <summary>
        /// Fill in combox box selection values
        /// </summary>
        private void FillComboBox()
        {
            int intVisionPos = Convert.ToInt32(m_smVisionInfo.g_strVisionFolderName.Substring(6)) - 1;

            if (!m_smVisionInfo.g_blnDisableMOGauge)
            {
                if ((m_smCustomOption.g_intWantOrient & (1 << intVisionPos)) > 0 && (m_smCustomOption.g_intWantMark & (1 << intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrOrientGauge.Count == 0 || m_smVisionInfo.g_arrMarkGauge.Count == 0)
                    {
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            AddSearchROI(u, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGauge);

                        CopyInfoToMark();
                    }
                    m_arrVisionModule.Add("MarkOrient");
                }
                else if ((m_smCustomOption.g_intWantOrient & (1 << intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrOrientGauge.Count == 0)
                    {
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            AddSearchROI(u, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGauge);
                    }
                    m_arrVisionModule.Add("Orient");
                }
                else if ((m_smCustomOption.g_intWantMark & (1 << intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrMarkGauge.Count == 0)
                    {
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            AddSearchROI(u, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkGauge);
                    }
                    m_arrVisionModule.Add("Mark");
                }
            }

            if (m_smVisionInfo.g_blnDisableMOGauge)
            {
                if ((m_smCustomOption.g_intWantPackage & (1 << intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_blnDisableMOGauge)
                    {
                        //if (m_smVisionInfo.g_arrOrientGauge.Count == 0 || m_smVisionInfo.g_arrMarkGauge.Count == 0)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                AddSearchROI(u, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGauge);

                            CopyInfoToMark();
                        }

                        // Add ROI and gauge
                        //if (m_smVisionInfo.g_arrPackageGauge.Count == 0)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                AddSearchROI(u, m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrPackageGauge, m_smVisionInfo.g_arrPackageGauge2);
                        }

                        m_arrVisionModule.Add("Package1");
                        m_arrVisionModule.Add("Package2");

                        for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                            cbo_ImagesList.Items.Add("Image " + (i + 1).ToString());
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0) < cbo_ImagesList.Items.Count)
                            cbo_ImagesList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);
                        chk_WantGauge.Checked = true;
                        chk_WantGauge.Visible = false;
                    }
                    else
                    {
                        // Add ROI and gauge
                        if (m_smVisionInfo.g_arrPackageGauge.Count == 0)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                AddSearchROI(u, m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrPackageGauge);
                        }

                        m_arrVisionModule.Add("Package1");

                        for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                            cbo_ImagesList.Items.Add("Image " + (i + 1).ToString());
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0) < cbo_ImagesList.Items.Count)
                            cbo_ImagesList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);
                    }
                }
            }
            else
            {

                if ((m_smCustomOption.g_intWantPackage & (1 << intVisionPos)) > 0)
                {
                    //if (m_smVisionInfo.g_arrPackageGauge.Count == 0)
                    //{
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        AddSearchROI(u, m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrPackageGauge, m_smVisionInfo.g_arrPackageGauge2);
                    //}

                    m_arrVisionModule.Add("Package1");
                    m_arrVisionModule.Add("Package2");

                    chk_UseMarkOrientGauge.Checked = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intUseOtherGaugeMeasurePackage & 0x0F) > 0;

                    for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                        cbo_ImagesList.Items.Add("Image " + (i + 1).ToString());
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0) < cbo_ImagesList.Items.Count)
                        cbo_ImagesList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);
                }
            }

            for (int i = 0; i < m_arrVisionModule.Count; i++)
            {
                cbo_RectGList.Items.Add(m_arrVisionModule[i]);
            }
            if (cbo_RectGList.Items.Count > 0)
                cbo_RectGList.SelectedIndex = 0;

            if (cbo_RectGList.SelectedItem.ToString() != "Package")
            {
                chk_UseMarkOrientGauge.Visible = false;

                if (cbo_RectGList.SelectedItem.ToString() != "Package")
                    srmLabel3.Visible = cbo_ImagesList.Visible = false;
            }
        }

        /// <summary>
        /// Load gauge settings from xml
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="arrGauge">rectangle gauge</param>
        /// <param name="arrROIs">ROI</param>
        private void LoadGaugeSetting(string strPath, List<RectGauge> arrGauge, string strSectionName)
        {
            arrGauge.Clear();

            XmlParser objFile = new XmlParser(strPath);
            RectGauge objRectGauge;
            int intParentCount = objFile.GetFirstSectionCount();

            for (int j = 0; j < m_smVisionInfo.g_intUnitsOnImage; j++)
            {
                if (j >= intParentCount)
                    continue;

                //create new ROI base on file read out
                objRectGauge = new RectGauge(m_smVisionInfo.g_WorldShape);
                //objRectGauge.LoadGauge(strPath, "RectG" + j);
                objRectGauge.LoadGauge(strPath, strSectionName + j);
                arrGauge.Add(objRectGauge);
            }

            objRectGauge = null;
        }

        /// <summary>
        /// Load ROI settings
        /// </summary>
        private void LoadROISetting()
        {
            for (int i = 0; i < m_arrVisionModule.Count; i++)
            {
                string strSelectedModule = m_arrVisionModule[i].ToString();
                if (strSelectedModule == "MarkOrient")
                    strSelectedModule = "Orient";

                //get the correct file location to save
                XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\" + strSelectedModule + "\\" + "ROI.xml");

                int intChildCount = 0;
                ROI objROI;

                for (int j = 0; j < m_smVisionInfo.g_intUnitsOnImage; j++)
                {
                    objFile.GetFirstSection("Unit" + j);
                    intChildCount = objFile.GetSecondSectionCount();

                    objROI = new ROI();
                    objFile.GetSecondSection("ROI0");
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                    switch (m_arrVisionModule[i].ToString())
                    {
                        case "Orient":
                            if (m_smVisionInfo.g_arrOrientROIs[j].Count > 0)
                                m_smVisionInfo.g_arrOrientROIs[j][0] = objROI;
                            break;
                        case "Mark":
                            if (m_smVisionInfo.g_arrMarkROIs[j].Count > 0)
                                m_smVisionInfo.g_arrMarkROIs[j][0] = objROI;
                            break;
                        case "MarkOrient":
                            if (m_smVisionInfo.g_arrOrientROIs[j].Count > 0)
                            {
                                m_smVisionInfo.g_arrOrientROIs[j][0] = objROI;
                                objROI = new ROI();
                                ((ROI)m_smVisionInfo.g_arrOrientROIs[j][0]).CopyTo(ref objROI);
                                m_smVisionInfo.g_arrMarkROIs[j][0] = objROI;
                            }
                            break;
                    }

                    objROI = null;
                }
            }
        }

        /// <summary>
        /// Load ROI settings from selected path into ROI list
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="arrROIList">ROI</param>
        private void LoadROISetting(string strPath, List<List<ROI>> arrROIList)
        {
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                arrROIList.Add(new List<ROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    if (objROI.ref_intType > 1)
                    {
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    }

                    arrROIList[i].Add(objROI);
                }
            }
        }

        /// <summary>
        /// Load package settings from selected path 
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadPackageSettings(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("Settings");
            int intPackageThreshold = objFile.GetValueAsInt("PackageViewThreshold", 50);
            int intMarkHighThreshold = objFile.GetValueAsInt("MarkViewHighThreshold", 200);
            int intMarkLowThreshold = objFile.GetValueAsInt("MarkViewLowThreshold", 50);
            int intMarkMinArea = objFile.GetValueAsInt("MarkViewMinArea", 20);
            int intPkgMinArea = objFile.GetValueAsInt("PkgViewMinArea", 20);

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].SetBlobSettings(intPackageThreshold, intPkgMinArea, intMarkHighThreshold,
                    intMarkLowThreshold, intMarkMinArea);
            }
        }

        /// <summary>
        /// Read mark settings from xml
        /// </summary>
        private void ReadMarkSettings()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Mark\\";

            LoadROISetting(strPath + "ROI.xml", m_smVisionInfo.g_arrMarkROIs);
            LoadGaugeSetting(strPath + "Gauge.xml", m_smVisionInfo.g_arrMarkGauge, "RectG");
            for (int i = 0; i < m_smVisionInfo.g_arrMarkROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrMarkROIs[i].Count; j++)
                {
                    if (j == 0)
                        ((ROI)m_smVisionInfo.g_arrMarkROIs[i][j]).AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else
                        ((ROI)m_smVisionInfo.g_arrMarkROIs[i][j]).AttachImage((ROI)m_smVisionInfo.g_arrMarkROIs[i][0]);
                }
        }

        /// <summary>
        /// Read orient settings from xml
        /// </summary>
        private void ReadOrientSettings()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
              m_smVisionInfo.g_strVisionFolderName + "\\Orient\\";

            LoadROISetting(strPath + "ROI.xml", m_smVisionInfo.g_arrOrientROIs);
            LoadGaugeSetting(strPath + "Gauge.xml", m_smVisionInfo.g_arrOrientGauge, "RectG");
            for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[i].Count; j++)
                {
                    if (j == 0)
                        ((ROI)m_smVisionInfo.g_arrOrientROIs[i][j]).AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else
                        ((ROI)m_smVisionInfo.g_arrOrientROIs[i][j]).AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[i][0]);
                }
        }

        private void ReadPackageSettings()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
              m_smVisionInfo.g_strVisionFolderName + "\\Package\\";

            LoadROISetting(strPath + "ROI.xml", m_smVisionInfo.g_arrPackageROIs);
            LoadGaugeSetting(strPath + "Gauge.xml", m_smVisionInfo.g_arrPackageGauge, "RectG");
            if (m_smVisionInfo.g_arrPackageGauge2.Count > 0)
                LoadGaugeSetting(strPath + "Gauge2.xml", m_smVisionInfo.g_arrPackageGauge2, "RectG");

            LoadPackageSettings(strPath + "Settings.xml");

            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIs[i].Count; j++)
                {
                    if (j == 0)
                        m_smVisionInfo.g_arrPackageROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else
                        m_smVisionInfo.g_arrPackageROIs[i][j].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][0]);
                }
        }

        /// <summary>
        /// Save gauge settings into xml
        /// </summary>
        private void SaveGaugeSetting()
        {
            // Save gauge advance setting
            string strAdvancePath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\General.xml";
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                   m_smVisionInfo.g_strVisionFolderName + "\\";

            
            STDeviceEdit.CopySettingFile(strPath, "\\General.xml");

            XmlParser objFileHandle = new XmlParser(strAdvancePath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantGauge", m_smVisionInfo.g_blnWantGauge);
            objFileHandle.WriteEndElement();

            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Gauge", m_smProductionInfo.g_strLotID);

            // Save gauge parameters setting


            string strSelectedModule = "";
            for (int i = 0; i < m_arrVisionModule.Count; i++)
            {
                strSelectedModule = m_arrVisionModule[i].ToString();
                if (strSelectedModule == "MarkOrient")
                    strSelectedModule = "Orient";

                List<RectGauge> arrRectGauges = new List<RectGauge>();
                switch (m_arrVisionModule[i].ToString())
                {
                    case "Orient":
                        STDeviceEdit.CopySettingFile(strPath, "Orient\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrOrientGauge, strPath + "Orient\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);
                        break;
                    case "MarkOrient":
                        STDeviceEdit.CopySettingFile(strPath, "Orient\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrOrientGauge, strPath + "Orient\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);

                        STDeviceEdit.CopySettingFile(strPath, "Mark\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrOrientGauge, strPath + "Mark\\Gauge.xml", "RectG");   // Mark Gauge has same setting with orient gauge
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);
                        break;
                    case "Mark":
                        STDeviceEdit.CopySettingFile(strPath, "Mark\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrMarkGauge, strPath + "Mark\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);
                        break;
                    case "Package":
                        if (chk_UseMarkOrientGauge.Checked)
                            CopyOrientInfoToPackage();
                        STDeviceEdit.CopySettingFile(strPath, "Package\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrPackageGauge, strPath + "Package\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);
                        break;
                    case "Package1":
                        STDeviceEdit.CopySettingFile(strPath, "Package\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrPackageGauge, strPath + "Package\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);
                        if (m_smVisionInfo.g_blnDisableMOGauge)
                        {
                            CopyROIGaugeInfoToOtherModule(m_smVisionInfo.g_arrPackageROIs,
                                m_smVisionInfo.g_arrPackageGauge, ref m_smVisionInfo.g_arrOrientROIs,
                                ref m_smVisionInfo.g_arrOrientGauge);

                            CopyROIGaugeInfoToOtherModule(m_smVisionInfo.g_arrPackageROIs,
                                m_smVisionInfo.g_arrPackageGauge, ref m_smVisionInfo.g_arrMarkROIs,
                                ref m_smVisionInfo.g_arrMarkGauge);
                        }
                        STDeviceEdit.CopySettingFile(strPath, "Orient\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrOrientGauge, strPath + "Orient\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);

                        STDeviceEdit.CopySettingFile(strPath, "Mark\\Gauge.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrMarkGauge, strPath + "Mark\\Gauge.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);

                        break;
                    case "Package2":
                        STDeviceEdit.CopySettingFile(strPath, "Package\\Gauge2.xml");
                        WriteGaugeFile(m_smVisionInfo.g_arrPackageGauge2, strPath + "Package\\Gauge2.xml", "RectG");
                        STDeviceEdit.XMLChangesTracing(m_arrVisionModule[i].ToString(), m_smProductionInfo.g_strLotID);
                        break;
                }

            }
            
        }

        /// <summary>
        /// Save ROI settings into xml
        /// </summary>
        private void SaveROISetting()
        {
            ROI objROI = new ROI();
            int intSubVisionModule;
            for (int i = 0; i < m_arrVisionModule.Count; i++)
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    string strSelectedModule;
                    if (m_arrVisionModule[i].ToString() == "MarkOrient")       // If module is markOrient, save the settings in both mark and orient recipe
                        intSubVisionModule = 2;
                    else
                        intSubVisionModule = 1;

                    for (int j = 0; j < intSubVisionModule; j++)
                    {
                        if ((m_arrVisionModule[i].ToString() == "MarkOrient") && j == 0)
                            strSelectedModule = "Orient";
                        else if ((m_arrVisionModule[i].ToString() == "MarkOrient") && j == 1)
                        {
                            ROI objMarkROI = new ROI();
                            ((ROI)m_smVisionInfo.g_arrOrientROIs[u][0]).CopyTo(ref objMarkROI);
                            m_smVisionInfo.g_arrMarkROIs[u][0] = objMarkROI;

                            for (int m = 0; m < m_smVisionInfo.g_arrMarkROIs.Count; m++)
                                for (int n = 0; n < m_smVisionInfo.g_arrMarkROIs[m].Count; n++)
                                {
                                    if (n == 0)
                                        ((ROI)m_smVisionInfo.g_arrMarkROIs[m][n]).AttachImage(m_smVisionInfo.g_arrImages[0]);
                                    else
                                        ((ROI)m_smVisionInfo.g_arrMarkROIs[m][n]).AttachImage((ROI)m_smVisionInfo.g_arrMarkROIs[m][0]);
                                }

                            strSelectedModule = "Mark";
                        }
                        else if ((m_arrVisionModule[i].ToString() == "Package1") || (m_arrVisionModule[i].ToString() == "Package2"))
                        {
                            strSelectedModule = "Package";
                        }
                        else
                            strSelectedModule = m_arrVisionModule[i].ToString();

                        //get the correct file location to save
                        string m_strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                       m_smVisionInfo.g_strVisionFolderName + "\\" + strSelectedModule + "\\";
                        
                        STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");

                        XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\" + strSelectedModule + "\\" + "ROI.xml");

                        switch (strSelectedModule)
                        {
                            case "Orient":
                                objROI = (ROI)m_smVisionInfo.g_arrOrientROIs[u][0];
                                break;
                            case "Mark":
                                objROI = (ROI)m_smVisionInfo.g_arrMarkROIs[u][0];
                                break;
                            case "Package":
                            case "Package1":
                            case "Package2":
                                objROI = (ROI)m_smVisionInfo.g_arrPackageROIs[u][0];
                                break;
                        }

                        objFile.WriteSectionElement("Unit" + u);

                        objFile.WriteElement1Value("ROI0", "");
                        objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                        objFile.WriteElement2Value("Type", objROI.ref_intType);
                        objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                        objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                        objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                        objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);

                        float fPixelAverage = objROI.GetROIAreaPixel();
                        objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                        objROI.SetROIPixelAverage(fPixelAverage);

                        objFile.WriteEndElement();
                        STDeviceEdit.XMLChangesTracing(strSelectedModule, m_smProductionInfo.g_strLotID);
                        
                    }
                }
            }
        }

        /// <summary>
        /// Save ROI settings into xml
        /// </summary>
        private void SaveROISetting_All()
        {
            // Pre Checking
            if (m_smVisionInfo.g_blnDisableMOGauge)
            {
                // Copy package ROI setting to mark and orient ROI setting
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    ROI objOrientROI = new ROI();
                    m_smVisionInfo.g_arrPackageROIs[i][0].CopyTo(ref objOrientROI);

                    if (i >= m_smVisionInfo.g_arrOrientROIs.Count)
                        m_smVisionInfo.g_arrOrientROIs.Add(new List<ROI>());

                    if (m_smVisionInfo.g_arrOrientROIs[i].Count == 0)
                        m_smVisionInfo.g_arrOrientROIs[i].Add(objOrientROI);
                    else
                        m_smVisionInfo.g_arrOrientROIs[i][0] = objOrientROI;

                    ROI objMarkROI = new ROI();
                    m_smVisionInfo.g_arrPackageROIs[i][0].CopyTo(ref objMarkROI);

                    if (i >= m_smVisionInfo.g_arrMarkROIs.Count)
                        m_smVisionInfo.g_arrMarkROIs.Add(new List<ROI>());

                    if (m_smVisionInfo.g_arrMarkROIs[i].Count == 0)
                        m_smVisionInfo.g_arrMarkROIs[i].Add(objMarkROI);
                    else
                        m_smVisionInfo.g_arrMarkROIs[i][0] = objMarkROI;

                }
            }
            else
            {
                for (int i = 0; i < m_arrVisionModule.Count; i++)
                {
                    if (m_arrVisionModule[i].ToString() == "MarkOrient")
                    {
                        // Copy Orient ROI setting to Mark ROI if MarkOrient exist
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        {
                            ROI objMarkROI = new ROI();
                            ((ROI)m_smVisionInfo.g_arrOrientROIs[u][0]).CopyTo(ref objMarkROI);
                            m_smVisionInfo.g_arrMarkROIs[u][0] = objMarkROI;
                        }
                        for (int m = 0; m < m_smVisionInfo.g_arrMarkROIs.Count; m++)
                            for (int n = 0; n < m_smVisionInfo.g_arrMarkROIs[m].Count; n++)
                            {
                                if (n == 0)
                                    ((ROI)m_smVisionInfo.g_arrMarkROIs[m][n]).AttachImage(m_smVisionInfo.g_arrImages[0]);
                                else
                                    ((ROI)m_smVisionInfo.g_arrMarkROIs[m][n]).AttachImage((ROI)m_smVisionInfo.g_arrMarkROIs[m][0]);
                            }
                    }
                }
            }



            // Save ROI
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                for (int i = 0; i < 3; i++) // 3 type of ROI: Orient, Mark, Package
                {
                    ROI objROI;
                    string strSelectedModule;
                    switch (i)
                    {
                        case 0:
                            objROI = m_smVisionInfo.g_arrOrientROIs[u][0];
                            strSelectedModule = "Orient";
                            break;
                        case 1:
                            objROI = m_smVisionInfo.g_arrMarkROIs[u][0];
                            strSelectedModule = "Mark";
                            break;
                        case 2:
                            objROI = m_smVisionInfo.g_arrPackageROIs[u][0];
                            strSelectedModule = "Package";
                            break;
                        default:
                            objROI = new ROI();
                            strSelectedModule = "";
                            break;
                    }

                    string m_strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\" + strSelectedModule + "\\";
                    
                    STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
                    //get the correct file location to save
                    XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\" + strSelectedModule + "\\" + "ROI.xml");

                    objFile.WriteSectionElement("Unit" + u);

                    objFile.WriteElement1Value("ROI0", "");
                    objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                    objFile.WriteElement2Value("Type", objROI.ref_intType);
                    objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                    objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);

                    float fPixelAverage = objROI.GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    objROI.SetROIPixelAverage(fPixelAverage);

                    objFile.WriteEndElement();
                    STDeviceEdit.XMLChangesTracing(strSelectedModule, m_smProductionInfo.g_strLotID);
                    
                }
            }
        }

        private void CopySelectedGaugeInfoToOther()
        {
            RectGauge objRectGauge = new RectGauge(m_smVisionInfo.g_WorldShape);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Orient":
                case "MarkOrient":
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objPackageROI = new ROI();
                        m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objPackageROI);

                        if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                            m_smVisionInfo.g_arrPackageROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrPackageROIs[i].Count == 0)
                            m_smVisionInfo.g_arrPackageROIs[i].Add(objPackageROI);
                        else
                            m_smVisionInfo.g_arrPackageROIs[i][0] = objPackageROI;
                    }

                    if (m_smVisionInfo.g_arrPackageGauge.Count > 0) // 2018 12 31 - CCENG: For Vision without package, the g_arrPackageGauge count is 0.
                    {
                        objRectGauge = (RectGauge)m_smVisionInfo.g_arrPackageGauge[0];
                        objRectGauge.ModifyGauge(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    }

                    if (m_smVisionInfo.g_arrPackageGauge2.Count > 0) // 2018 12 31 - CCENG: For Vision without package, the g_arrPackageGauge count is 0.
                    {
                        objRectGauge = (RectGauge)m_smVisionInfo.g_arrPackageGauge2[0];
                        objRectGauge.ModifyGauge(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    }

                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objMarkROI = new ROI();
                        m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objMarkROI);

                        if (i >= m_smVisionInfo.g_arrMarkROIs.Count)
                            m_smVisionInfo.g_arrMarkROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrMarkROIs[i].Count == 0)
                            m_smVisionInfo.g_arrMarkROIs[i].Add(objMarkROI);
                        else
                            m_smVisionInfo.g_arrMarkROIs[i][0] = objMarkROI;
                    }

                    if (m_smVisionInfo.g_arrMarkGauge.Count > 0)
                    {
                        objRectGauge = (RectGauge)m_smVisionInfo.g_arrMarkGauge[0];
                        objRectGauge.ModifyGauge(m_smVisionInfo.g_arrMarkROIs[0][0]);
                    }
                    break;
                case "Package1":
                case "Package2":
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objOrientROI = new ROI();
                        m_smVisionInfo.g_arrPackageROIs[i][0].CopyTo(ref objOrientROI);

                        if (i >= m_smVisionInfo.g_arrOrientROIs.Count)
                            m_smVisionInfo.g_arrOrientROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrOrientROIs[i].Count == 0)
                            m_smVisionInfo.g_arrOrientROIs[i].Add(objOrientROI);
                        else
                            m_smVisionInfo.g_arrOrientROIs[i][0] = objOrientROI;
                    }

                    objRectGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[0];
                    objRectGauge.ModifyGauge(m_smVisionInfo.g_arrOrientROIs[0][0]);
                    break;
            }
        }

        /// <summary>
        /// Customize GUI
        /// </summary>
        private void UpdateGUI(RectGauge objGauge)
        {
            //update current gauge position param
            txt_PosTolerance.Text = objGauge.ref_GaugeTolerance.ToString();
            txt_Size.Text = (objGauge.ref_GaugeSizeTolerance * 100).ToString();

            //update current gauge measurement param
            txt_MeasFilter.Text = objGauge.ref_GaugeFilter.ToString();
            txt_MeasThickness.Text = objGauge.ref_GaugeThickness.ToString();
            txt_MeasThreshold.Text = objGauge.ref_GaugeThreshold.ToString();
            txt_MeasMinAmp.Text = objGauge.ref_GaugeMinAmplitude.ToString();
            txt_MeasMinArea.Text = objGauge.ref_GaugeMinArea.ToString();

            cbo_TransType.SelectedIndex = objGauge.ref_GaugeTransType;
            cbo_TransChoice.SelectedIndex = objGauge.ref_GaugeTransChoice;

            //update current gauge fitting param
            txt_FittingSamplingStep.Text = objGauge.ref_GaugeSamplingStep.ToString();
            txt_FilteringPass.Text = objGauge.ref_GaugeFilteringPasses.ToString();
            txt_FilteringThreshold.Text = objGauge.ref_GaugeFilterThreshold.ToString();
        }

        private void WriteGaugeFile(List<RectGauge> arrGauge, string strFileName, string strSectionName)
        {
            for (int j = 0; j < arrGauge.Count; j++)
            {
                //set working rect gauge from array one by one and save
                RectGauge objRectGauge = (RectGauge)arrGauge[j];

                //objRectGauge.SaveGauge(strFileName, false, "RectG" + j, true);
                objRectGauge.SaveGauge(strFileName, false, strSectionName + j, true);

                objRectGauge.SetRectGaugeTemplate(objRectGauge.ref_ObjectCenterX, objRectGauge.ref_ObjectCenterY,
                         objRectGauge.ref_ObjectWidth, objRectGauge.ref_ObjectHeight);
            }
        }

        private void SavePackageSetting()
        {
            if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";
                
                STDeviceEdit.CopySettingFile(strFolderPath, "Settings.xml");
                for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
                {
                    if (chk_UseMarkOrientGauge.Checked)
                        m_smVisionInfo.g_arrPackage[i].ref_intUseOtherGaugeMeasurePackage = 1;  // Use Orient Gauge to measure package
                    else
                        m_smVisionInfo.g_arrPackage[i].ref_intUseOtherGaugeMeasurePackage = 0;  // Use own Package Gauge to measure package

                    if (i == 0)
                        m_smVisionInfo.g_arrPackage[i].SavePackage(strFolderPath + "Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    else
                        m_smVisionInfo.g_arrPackage[i].SavePackage(strFolderPath + "Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    
                }
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package Settings", m_smProductionInfo.g_strLotID);
                
            }
        }



        private void btn_Save_Click(object sender, EventArgs e)
        {
            //Copy selected rect gauge to other gauge
            CopySelectedGaugeInfoToOther();
            //save all setting from working memory to file
            SaveGaugeSetting();
            if (m_smVisionInfo.g_blnDisableMOGauge)
                SaveROISetting_All();
            else
                SaveROISetting();
            SavePackageSetting();

            if (m_smCustomOption.g_blnConfigShowNetwork && m_smCustomOption.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strAdvancePath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\General.xml";
            XmlParser objFile = new XmlParser(strAdvancePath);
            objFile.GetFirstSection("Advanced");
            m_smVisionInfo.g_blnWantGauge = objFile.GetValueAsBoolean("WantGauge", true, 1);

            if ((m_smCustomOption.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                ReadOrientSettings();
            if ((m_smCustomOption.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                ReadMarkSettings();
            if ((m_smCustomOption.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                ReadPackageSettings();

            this.Close();
            this.Dispose();
        }



        private void txt_FittingSamplingStep_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intSamplingStep = Convert.ToInt32(txt_FittingSamplingStep.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeSamplingStep = intSamplingStep;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeSamplingStep = intSamplingStep;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeSamplingStep = intSamplingStep;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeSamplingStep = intSamplingStep;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringPass_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intFilteringPass = Convert.ToInt32(txt_FilteringPass.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeFilteringPasses = intFilteringPass;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeFilteringPasses = intFilteringPass;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeFilteringPasses = intFilteringPass;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeFilteringPasses = intFilteringPass;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            float fFilteringThreshold = Convert.ToSingle(txt_FilteringThreshold.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeFilterThreshold = fFilteringThreshold;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeFilterThreshold = fFilteringThreshold;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeFilterThreshold = fFilteringThreshold;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeFilterThreshold = fFilteringThreshold;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PosTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_PosTolerance.Text == "")
                return;

            int i = 0;
            float fTolerance = float.Parse(txt_PosTolerance.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].SetRectGaugeTolerance(
                            m_smVisionInfo.g_arrPackageROIs[i][0], fTolerance);
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].SetRectGaugeTolerance(
                            m_smVisionInfo.g_arrPackageROIs[i][0], fTolerance);
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].SetRectGaugeTolerance(
                            m_smVisionInfo.g_arrMarkROIs[i][0], fTolerance);
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].SetRectGaugeTolerance(
                            m_smVisionInfo.g_arrOrientROIs[i][0], fTolerance);
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_MeasThreshold.Text == "")
                return;

            int i = 0;
            int intThreshold = Convert.ToInt32(txt_MeasThreshold.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeThreshold = intThreshold;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeThreshold = intThreshold;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeThreshold = intThreshold;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeThreshold = intThreshold;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intThickness = Convert.ToInt32(txt_MeasThickness.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeThickness = intThickness;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeThickness = intThickness;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeThickness = intThickness;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeThickness = intThickness;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinAmp_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intMinAmp = Convert.ToInt32(txt_MeasMinAmp.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeMinAmplitude = intMinAmp;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeMinAmplitude = intMinAmp;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeMinAmplitude = intMinAmp;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeMinAmplitude = intMinAmp;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intFilter = Convert.ToInt32(txt_MeasFilter.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeFilter = intFilter;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeFilter = intFilter;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeFilter = intFilter;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeFilter = intFilter;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_GaugeMinArea = intMinArea;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_GaugeMinArea = intMinArea;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_GaugeMinArea = intMinArea;
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_GaugeMinArea = intMinArea;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Size_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int i = 0;
            int intSize = Convert.ToInt32(txt_Size.Text);

            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].SetRectGaugeSize(m_smVisionInfo.g_arrPackageROIs[i][0], intSize / 100.0f);
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].SetRectGaugeSize(m_smVisionInfo.g_arrPackageROIs[i][0], intSize / 100.0f);
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrMarkGauge[i]).SetRectGaugeSize(m_smVisionInfo.g_arrMarkROIs[i][0], (intSize / 100.0f));
                    break;
                default:
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrOrientGauge[i]).SetRectGaugeSize(m_smVisionInfo.g_arrOrientROIs[i][0], (intSize / 100.0f));
                    break;
            }



            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void cbo_RectGList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_RectGList.SelectedIndex == -1)
                return;

            RectGauge objRectGauge = new RectGauge(m_smVisionInfo.g_WorldShape);
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Orient":
                case "MarkOrient":
                    // Copy package ROI setting to orient ROI setting
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objOrientROI = new ROI();
                        m_smVisionInfo.g_arrPackageROIs[i][0].CopyTo(ref objOrientROI);

                        if (i >= m_smVisionInfo.g_arrOrientROIs.Count)
                            m_smVisionInfo.g_arrOrientROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrOrientROIs[i].Count == 0)
                            m_smVisionInfo.g_arrOrientROIs[i].Add(objOrientROI);
                        else
                            m_smVisionInfo.g_arrOrientROIs[i][0] = objOrientROI;
                    }
                    objRectGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[0];
                    m_smVisionInfo.g_intSelectedImage = 0;  // Orient will use image index 0
                    objRectGauge.ModifyGauge(m_smVisionInfo.g_arrOrientROIs[0][0]);
                    chk_UseMarkOrientGauge.Visible = false; // Hide it bcos it is for package use only.
                    srmLabel3.Visible = cbo_ImagesList.Visible = false;
                    m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString();  // For drawing
                    chk_WantGauge.Visible = true;
                    chk_WantGauge.Checked = m_smVisionInfo.g_blnWantGauge;
                    if (chk_WantGauge.Checked)
                    {
                        tbCtrl_Gauge.Enabled = true;
                        gb_GainSetting.Enabled = true;
                    }
                    else
                    {
                        tbCtrl_Gauge.Enabled = false;
                        gb_GainSetting.Enabled = false;
                    }
                    break;
                case "Mark":
                    // Copy package ROI setting to orient ROI setting
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objMarkROI = new ROI();
                        m_smVisionInfo.g_arrPackageROIs[i][0].CopyTo(ref objMarkROI);

                        if (i >= m_smVisionInfo.g_arrMarkROIs.Count)
                            m_smVisionInfo.g_arrMarkROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrMarkROIs[i].Count == 0)
                            m_smVisionInfo.g_arrMarkROIs[i].Add(objMarkROI);
                        else
                            m_smVisionInfo.g_arrMarkROIs[i][0] = objMarkROI;
                    }
                    objRectGauge = (RectGauge)m_smVisionInfo.g_arrMarkGauge[0];
                    m_smVisionInfo.g_intSelectedImage = 0;  // Mark will use image index 0
                    objRectGauge.ModifyGauge(m_smVisionInfo.g_arrMarkROIs[0][0]);
                    chk_UseMarkOrientGauge.Visible = false; // Hide it bcos it is for package use only.
                    srmLabel3.Visible = cbo_ImagesList.Visible = false;
                    m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString(); // For drawing
                    tbCtrl_Gauge.Enabled = true;
                    gb_GainSetting.Enabled = true;
                    chk_WantGauge.Visible = true;
                    break;
                case "Package":

                    chk_UseMarkOrientGauge.Visible = true;

                    srmLabel3.Visible = cbo_ImagesList.Visible = true;
                    if (chk_UseMarkOrientGauge.Checked)
                    {
                        tbCtrl_Gauge.Enabled = false;
                        gb_GainSetting.Enabled = false;
                        cbo_ImagesList.Enabled = false;
                        objRectGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[0];
                    }
                    else
                    {
                        tbCtrl_Gauge.Enabled = true;
                        gb_GainSetting.Enabled = true;
                        cbo_ImagesList.Enabled = true;

                        objRectGauge = m_smVisionInfo.g_arrPackageGauge[0];

                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);

                        m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString();

                    }
                    break;
                case "Package1":
                    // Copy orient ROI setting to package ROI setting
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objPackageROI = new ROI();
                        m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objPackageROI);

                        if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                            m_smVisionInfo.g_arrPackageROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrPackageROIs[i].Count == 0)
                            m_smVisionInfo.g_arrPackageROIs[i].Add(objPackageROI);
                        else
                            m_smVisionInfo.g_arrPackageROIs[i][0] = objPackageROI;
                    }

                    objRectGauge = (RectGauge)m_smVisionInfo.g_arrPackageGauge[0];
                    m_smVisionInfo.g_intSelectedImage = 1;  // Package 1 will use image index 1
                    objRectGauge.ModifyGauge(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);    // Re-modify gauge to make sure the gauge always inside the Package ROI
                    chk_UseMarkOrientGauge.Visible = false; // Hide it bcos it is for package use only.
                    srmLabel3.Visible = cbo_ImagesList.Visible = false;
                    m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString();  // For drawing
                    tbCtrl_Gauge.Enabled = true;
                    gb_GainSetting.Enabled = true;
                    chk_WantGauge.Checked = true;
                    chk_WantGauge.Visible = false;
                    break;
                case "Package2":
                    // Copy orient ROI setting to package ROI setting
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        ROI objPackageROI = new ROI();
                        m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objPackageROI);

                        if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                            m_smVisionInfo.g_arrPackageROIs.Add(new List<ROI>());

                        if (m_smVisionInfo.g_arrPackageROIs[i].Count == 0)
                            m_smVisionInfo.g_arrPackageROIs[i].Add(objPackageROI);
                        else
                            m_smVisionInfo.g_arrPackageROIs[i][0] = objPackageROI;
                    }

                    objRectGauge = (RectGauge)m_smVisionInfo.g_arrPackageGauge2[0];
                    m_smVisionInfo.g_intSelectedImage = 2;  // Package 1 will use image index 1
                    objRectGauge.ModifyGauge(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);    // Re-modify gauge to make sure the gauge always inside the Package ROI
                    chk_UseMarkOrientGauge.Visible = false; // Hide it bcos it is for package use only.
                    srmLabel3.Visible = cbo_ImagesList.Visible = false;
                    m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString();  // For drawing
                    tbCtrl_Gauge.Enabled = true;
                    gb_GainSetting.Enabled = true;
                    chk_WantGauge.Checked = true;
                    chk_WantGauge.Visible = false;
                    break;
            }

            // Update package image with correct gain value
            trackBar_Gain.Value = (int)objRectGauge.ref_fGainValue;
            txt_GainValue.Text = (Convert.ToSingle(trackBar_Gain.Value) / 1000).ToString();
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, objRectGauge.ref_fGainValue / 1000);

            UpdateGUI(objRectGauge);

            //call redraw on pic box
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_TransType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            int intTransType = cbo_TransType.SelectedIndex;

            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Orient":
                case "MarkOrient":
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrOrientGauge[i]).ref_GaugeTransType = intTransType;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrMarkGauge[i]).ref_GaugeTransType = intTransType;
                    break;
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrPackageGauge[i]).ref_GaugeTransType = intTransType;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrPackageGauge2[i]).ref_GaugeTransType = intTransType;
                    break;
            }


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_TransChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            int intTransChoice = cbo_TransChoice.SelectedIndex;

            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Orient":
                case "MarkOrient":
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrOrientGauge[i]).ref_GaugeTransChoice = intTransChoice;
                    break;
                case "Mark":
                    for (i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrMarkGauge[i]).ref_GaugeTransChoice = intTransChoice;
                    break;
                case "Package":
                case "Package1":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrPackageGauge[i]).ref_GaugeTransChoice = intTransChoice;
                    break;
                case "Package2":
                    for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        ((RectGauge)m_smVisionInfo.g_arrPackageGauge2[i]).ref_GaugeTransChoice = intTransChoice;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void LearnRectGaugeForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;

            if (cbo_RectGList.Items.Count > 0)
            {
                cbo_RectGList.SelectedIndex = 0;
                m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString();
            }
            m_smVisionInfo.g_blncboImageView = false;
            m_smVisionInfo.g_blnViewGauge = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
        }

        private void LearnRectGaugeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewPackageImage = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intImageIndex = 0;
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Orient":
                case "MarkOrient":
                    for (int i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
                    break;
                case "Mark":
                    for (int i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
                    break;
                case "Package":
                case "Package1":
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);

                    intImageIndex = 1; // m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);
                    break;
                case "Package2":
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);

                    intImageIndex = 2;// m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
                    break;
            }
            float fGain = Convert.ToSingle(trackBar_Gain.Value) / 1000;
            txt_GainValue.Text = fGain.ToString();
            m_smVisionInfo.g_arrImages[intImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, fGain);
            if (!m_smVisionInfo.g_blnDisableMOGauge)
                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_UseMarkOrientGauge_Click(object sender, EventArgs e)
        {
            RectGauge objRectGauge;
            if (chk_UseMarkOrientGauge.Checked)
            {
                tbCtrl_Gauge.Enabled = false;
                gb_GainSetting.Enabled = false;
                cbo_ImagesList.Enabled = false;

                objRectGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[0];
                m_smVisionInfo.g_intSelectedImage = 0;

                for (int i = 0; i < cbo_RectGList.Items.Count; i++)
                {
                    if ((string)cbo_RectGList.Items[i] == "MarkOrient" ||
                        (string)cbo_RectGList.Items[i] == "Orient" ||
                        (string)cbo_RectGList.Items[i] == "Mark")
                    {
                        m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.Items[i].ToString();
                        break;
                    }
                }
            }
            else
            {
                tbCtrl_Gauge.Enabled = true;
                gb_GainSetting.Enabled = true;
                cbo_ImagesList.Enabled = true;

                m_smVisionInfo.g_blnViewPackageImage = true;
                objRectGauge = m_smVisionInfo.g_arrPackageGauge[0];

                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);

                m_smVisionInfo.g_strSelectedRectGauge = cbo_RectGList.SelectedItem.ToString();

            }

            UpdateGUI(objRectGauge);

            //call redraw on pic box
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = cbo_ImagesList.SelectedIndex;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(0, m_smVisionInfo.g_intSelectedImage);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, (Convert.ToSingle(trackBar_Gain.Value) / 1000));

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GainValue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            trackBar_Gain.Value = (int)Math.Round(Convert.ToSingle(txt_GainValue.Text) * 1000, 0, MidpointRounding.AwayFromZero);

            int intImageIndex = 0;
            switch (cbo_RectGList.SelectedItem.ToString())
            {
                case "Orient":
                case "MarkOrient":
                    for (int i = 0; i < m_smVisionInfo.g_arrOrientGauge.Count; i++)
                        m_smVisionInfo.g_arrOrientGauge[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
                    break;
                case "Mark":
                    for (int i = 0; i < m_smVisionInfo.g_arrMarkGauge.Count; i++)
                        m_smVisionInfo.g_arrMarkGauge[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
                    break;
                case "Package":
                case "Package1":
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);

                    intImageIndex = 1; // m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);
                    break;
                case "Package2":
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2.Count; i++)
                        m_smVisionInfo.g_arrPackageGauge2[i].ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);

                    intImageIndex = 2;// m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
                    break;
            }
            m_smVisionInfo.g_arrImages[intImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(txt_GainValue.Text));
            if (!m_smVisionInfo.g_blnDisableMOGauge)
                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_WantGauge_Click(object sender, EventArgs e)
        {
            if (chk_WantGauge.Checked)
            {
                // Enable all gauge parameter GUI
                cbo_RectGList.Enabled = true;
                tbCtrl_Gauge.Enabled = true;
                gb_GainSetting.Enabled = true;
            }
            else
            {
                // Disable all gauge parameter GUI
                // cbo_RectGList.Enabled = false;   // 2019 05 30 - CCENG: Not need disabled bcos need to go to Package gauge.
                tbCtrl_Gauge.Enabled = false;
                gb_GainSetting.Enabled = false;

                // Give warning Extra Pad and Text Shifted Checking are disabled when gauge is turning off. 
                for (int m = 0; m < m_smVisionInfo.g_arrMarks.Count; m++)
                {
                    if (m_smVisionInfo.g_arrMarks[m].IsExtraPadOrTextShiftedFailMaskON())
                    {
                        //SRMMessageBox.Show("To OFF the gauge feature, the Extra Pad (center area, side area and group) and Text Shifted Fail Mask will be DISABLED as well.");
                        SRMMessageBox.Show("To OFF the gauge feature, the Extra Pad (side area) and Text Shifted Fail Mask will be DISABLED as well.");
                        break;
                    }
                }
            }

            m_smVisionInfo.g_blnWantGauge = chk_WantGauge.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_WantGauge_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}