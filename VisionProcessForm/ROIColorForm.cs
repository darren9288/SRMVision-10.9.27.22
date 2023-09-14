using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using Common;
using System.IO;

namespace VisionProcessForm
{
    public partial class ROIColorForm : Form
    {
        private int m_intUserGroup = 5;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private FileSorting m_objTimeComparer = new FileSorting();
        private CustomOption m_smCustomizeInfo;
        private bool m_blnInitDone = false;

        public ROIColorForm(ProductionInfo smProductionInfo, VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe)
        {
            InitializeComponent();


            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strSelectedRecipe = strSelectedRecipe;

            m_smCustomizeInfo = smCustomizeInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;

            UpdateGUI();

            m_blnInitDone = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROIColorSetting(strFolderPath + "ROIColor.xml");
            this.Close();
            this.Dispose();
        }
        private void LoadROIColorSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("MarkOrient");
            for (int i = 0; i < m_smVisionInfo.g_arrMarkOrientROIColor.Length; i++)
            {
                objFile.GetSecondSection("MarkOrient" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrMarkOrientROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrMarkOrientROIColor[i][j] = Color.FromName(objFile.GetValueAsString("MarkOrient" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrMarkOrientROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Package");
            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIColor.Length; i++)
            {
                objFile.GetSecondSection("Package" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrPackageROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Package" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPackageROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Seal");
            for (int i = 0; i < m_smVisionInfo.g_arrSealROIColor.Length; i++)
            {
                objFile.GetSecondSection("Seal" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrSealROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrSealROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Seal" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrSealROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Barcode");
            for (int i = 0; i < m_smVisionInfo.g_arrBarcodeROIColor.Length; i++)
            {
                objFile.GetSecondSection("Barcode" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrBarcodeROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrBarcodeROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Barcode" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrBarcodeROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Pad");
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIColor.Length; i++)
            {
                objFile.GetSecondSection("Pad" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrPadROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrPadROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Pad" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPadROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("PadPackage");
            for (int i = 0; i < m_smVisionInfo.g_arrPadPackageROIColor.Length; i++)
            {
                objFile.GetSecondSection("PadPackage" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrPadPackageROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrPadPackageROIColor[i][j] = Color.FromName(objFile.GetValueAsString("PadPackage" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPadPackageROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Lead3D");
            for (int i = 0; i < m_smVisionInfo.g_arrLead3DROIColor.Length; i++)
            {
                objFile.GetSecondSection("Lead3D" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrLead3DROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrLead3DROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Lead3D" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrLead3DROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Lead");
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIColor.Length; i++)
            {
                objFile.GetSecondSection("Lead" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrLeadROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Lead" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrLeadROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("Empty");
            for (int i = 0; i < m_smVisionInfo.g_arrEmptyROIColor.Length; i++)
            {
                objFile.GetSecondSection("Empty" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrEmptyROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrEmptyROIColor[i][j] = Color.FromName(objFile.GetValueAsString("Empty" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrEmptyROIColor[i][j].Name, 2));
                }
            }

            objFile.GetFirstSection("PocketPosition");
            for (int i = 0; i < m_smVisionInfo.g_arrPocketPositionROIColor.Length; i++)
            {
                objFile.GetSecondSection("PocketPosition" + i.ToString());
                for (int j = 0; j < m_smVisionInfo.g_arrPocketPositionROIColor[i].Length; j++)
                {
                    m_smVisionInfo.g_arrPocketPositionROIColor[i][j] = Color.FromName(objFile.GetValueAsString("PocketPosition" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPocketPositionROIColor[i][j].Name, 2));
                }
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            SaveROIColorSetting(strFolderPath + "ROIColor.xml");
            this.Close();
            this.Dispose();
        }

        private void SaveROIColorSetting(string strFilePath)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            objFile.WriteSectionElement("MarkOrient", true);
            for (int i = 0; i < m_smVisionInfo.g_arrMarkOrientROIColor.Length; i++)
            {
                objFile.WriteElement1Value("MarkOrient" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrMarkOrientROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("MarkOrient" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrMarkOrientROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Package", true);
            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Package" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Package" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPackageROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Seal", true);
            for (int i = 0; i < m_smVisionInfo.g_arrSealROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Seal" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrSealROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Seal" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrSealROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Barcode", true);
            for (int i = 0; i < m_smVisionInfo.g_arrBarcodeROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Barcode" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrBarcodeROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Barcode" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrBarcodeROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Pad", true);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Pad" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrPadROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Pad" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPadROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("PadPackage", true);
            for (int i = 0; i < m_smVisionInfo.g_arrPadPackageROIColor.Length; i++)
            {
                objFile.WriteElement1Value("PadPackage" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrPadPackageROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("PadPackage" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPadPackageROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Lead3D", true);
            for (int i = 0; i < m_smVisionInfo.g_arrLead3DROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Lead3D" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrLead3DROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Lead3D" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrLead3DROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Lead", true);
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Lead" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Lead" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrLeadROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("Empty", true);
            for (int i = 0; i < m_smVisionInfo.g_arrEmptyROIColor.Length; i++)
            {
                objFile.WriteElement1Value("Empty" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrEmptyROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("Empty" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrEmptyROIColor[i][j].Name);
                }
            }

            objFile.WriteSectionElement("PocketPosition", true);
            for (int i = 0; i < m_smVisionInfo.g_arrPocketPositionROIColor.Length; i++)
            {
                objFile.WriteElement1Value("PocketPosition" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrPocketPositionROIColor[i].Length; j++)
                {
                    objFile.WriteElement2Value("PocketPosition" + i.ToString() + j.ToString(), m_smVisionInfo.g_arrPocketPositionROIColor[i][j].Name);
                }
            }

            objFile.WriteEndElement();

        }
        private void ROIColorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateGUI()
        {
            //int intTabPageCount = tab_VisionControl.Controls.Count;
            //for (int i = 0; i < intTabPageCount; i++)
            //{
            //    tab_VisionControl.Controls.Remove(tab_VisionControl.Controls[0]);
            //}
            tab_VisionControl.Controls.Clear();
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                    if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        tab_VisionControl.Controls.Add(tp_MarkOrient);
                        pnl_CharROIFiltered_MO.Visible = false;
                        pnl_CharROIIgnored_MO.Visible = false;
                        pnl_CharROIInspect_MO.Visible = false;
                        pnl_CharROISelected_MO.Visible = false;
                        pnl_CharROISplit_MO.Visible = false;
                        pnl_DontCareROI_MO.Visible = false;
                        pnl_GaugeROI_MO.Visible = false;
                        pnl_Mark2DROI_MO.Visible = false;
                        pnl_MarkROI_MO.Visible = false;
                        pnl_Pin1ROI_MO.Visible = false;
                        pnl_RetestROI_MO.Visible = false;
                        UpdateMarkOrientSetting();
                    }
                    break;
                case "Mark":
                case "InPocket":
                case "IPMLi":
                    tab_VisionControl.Controls.Add(tp_MarkOrient);

                    pnl_OrientROI_MO.Visible = false;

                    if (m_smVisionInfo.g_strVisionName == "Mark")
                        pnl_UnitROI_MO.Visible = false;
                    else
                    {
                        tab_VisionControl.Controls.Add(tp_Empty);
                        UpdateEmptySetting();
                    }

                    if(m_smVisionInfo.g_intUnitsOnImage == 1)
                        pnl_RetestROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_Mark2DROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadSetting();
                    }

                    UpdateMarkOrientSetting();

                    break;
                case "MarkOrient":
                case "MOLi":
                    tab_VisionControl.Controls.Add(tp_MarkOrient);
                    pnl_UnitROI_MO.Visible = false;
                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        pnl_RetestROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_Mark2DROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadSetting();
                    }

                    UpdateMarkOrientSetting();

                    break;
                case "MarkPkg":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                    tab_VisionControl.Controls.Add(tp_MarkOrient);
                    tab_VisionControl.Controls.Add(tp_Package);

                    if (m_smVisionInfo.g_strVisionName == "MarkPkg")
                        pnl_UnitROI_MO.Visible = false;
                    else
                    {
                        tab_VisionControl.Controls.Add(tp_Empty);
                        UpdateEmptySetting();

                        tab_VisionControl.Controls.Add(tp_PocketPosition);
                        UpdatePocketPositionSetting();
                    }

                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        pnl_RetestROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_Mark2DROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadSetting();
                    }

                    UpdateMarkOrientSetting();

                    UpdatePackageSetting();

                    break;
                case "MOPkg":
                case "MOLiPkg":
                    tab_VisionControl.Controls.Add(tp_MarkOrient);
                    tab_VisionControl.Controls.Add(tp_Package);

                    pnl_UnitROI_MO.Visible = false;
                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        pnl_RetestROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_Mark2DROI_MO.Visible = false;

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadSetting();
                    }

                    UpdateMarkOrientSetting();

                    UpdatePackageSetting();

                    break;
                case "Package":
                    tab_VisionControl.Controls.Add(tp_Package);

                    tp_Package.Controls.Add(pnl_SearchROI_MO);
                    tp_Package.Controls.Add(pnl_RetestROI_MO);
                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        pnl_RetestROI_MO.Visible = false;

                    UpdateMarkOrientSetting();

                    UpdatePackageSetting();

                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_Pad2);
                    UpdatePadSetting();
                    break;
                case "Pad":
                case "PadPos":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_Pad2);
                    pnl_UnitROI_Pad.Visible = false;
                    pnl_OrientROI_Pad.Visible = false;
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        pnl_ColorROITolerance_Pad.Visible = false;
                        pnl_ColorDontCareROI_Pad.Visible = false;
                    }
                    UpdatePadSetting();
                    break;
                case "PadPkg":
                case "PadPkgPos":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_Pad2);
                    tab_VisionControl.Controls.Add(tp_PadPackage);
                    pnl_UnitROI_Pad.Visible = false;
                    pnl_OrientROI_Pad.Visible = false;
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        pnl_ColorROITolerance_Pad.Visible = false;
                        pnl_ColorDontCareROI_Pad.Visible = false;
                    }
                    UpdatePadSetting();
                    UpdatePadPackageSetting();
                    break;
                case "Pad5S":
                case "Pad5SPos":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_Pad2);
                    pnl_UnitROI_Pad.Visible = false;
                    pnl_OrientROI_Pad.Visible = false;
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        pnl_ColorROITolerance_Pad.Visible = false;
                        pnl_ColorDontCareROI_Pad.Visible = false;
                    }
                    UpdatePadSetting();
                    break;
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_Pad2);
                    tab_VisionControl.Controls.Add(tp_PadPackage);
                    pnl_UnitROI_Pad.Visible = false;
                    pnl_OrientROI_Pad.Visible = false;
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        pnl_ColorROITolerance_Pad.Visible = false;
                        pnl_ColorDontCareROI_Pad.Visible = false;
                    }
                    UpdatePadSetting();
                    UpdatePadPackageSetting();
                    break;
                case "Li3D":
                    tab_VisionControl.Controls.Add(tp_Lead3D);
                    UpdateLead3DSetting();
                    break;
                //case "Li3DPkg":
                //    tab_VisionControl.Controls.Add(tp_Lead3D);
                //    tab_VisionControl.Controls.Add(tp_Lead3D_2);
                //    tab_VisionControl.Controls.Add(tp_Lead3DPkg);

                //    if (m_smVisionInfo.g_blnWantPin1)
                //        tab_VisionControl.Controls.Add(tp_Pin1);

                //    UpdateLead3DFailMaskGUI();
                //    UpdateLead3DPkgFailMaskGUI();
                //    pnl_PageButton.Visible = false;
                //    break;
                case "Seal":
                    tab_VisionControl.Controls.Add(tp_Seal);
                    UpdateSealSetting();
                    break;
                case "Barcode":
                    tab_VisionControl.Controls.Add(tp_Barcode);
                    UpdateBarcodeSetting();
                    break;
                case "UnitPresent":
                    break;
                default:
                    SRMMessageBox.Show("btn_ROIColor_Click -> There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;
            }

        }
        private void UpdateMarkOrientSetting()
        {
            cbo_SearchROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[0][0].Name;
            pnl2_SearchROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[0][0];

            cbo_RetestROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[1][0].Name;
            pnl2_RetestROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[1][0];

            cbo_MarkROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[2][0].Name;
            pnl2_MarkROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[2][0];

            cbo_OrientROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[3][0].Name;
            pnl2_OrientROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[3][0];

            cbo_UnitROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[4][0].Name;
            pnl2_UnitROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[4][0];

            cbo_Pin1ROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[5][0].Name;
            pnl2_Pin1ROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[5][0];

            cbo_DontCareROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[6][0].Name;
            pnl2_DontCareROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[6][0];

            cbo_GaugeROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[7][0].Name;
            pnl2_GaugeROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[7][0];

            cbo_CharROIInspect_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][0].Name;
            pnl2_CharROIInspect_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[8][0];

            cbo_CharROISelected_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][1].Name;
            pnl2_CharROISelected_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[8][1];

            cbo_CharROIIgnored_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][2].Name;
            pnl2_CharROIIgnored_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[8][2];

            cbo_CharROIFiltered_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][3].Name;
            pnl2_CharROIFiltered_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[8][3];

            cbo_CharROISplit_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][4].Name;
            pnl2_CharROISplit_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[8][4];

            cbo_Mark2DROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[9][0].Name;
            pnl2_Mark2DROI_MO.BackColor = m_smVisionInfo.g_arrMarkOrientROIColor[9][0];

        }
        private void UpdatePackageSetting()
        {
            cbo_PackageROIBright.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[0][0].Name;
            pnl2_PackageROIBright.BackColor = m_smVisionInfo.g_arrPackageROIColor[0][0];

            cbo_PackageROIDark.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[0][1].Name;
            pnl2_PackageROIDark.BackColor = m_smVisionInfo.g_arrPackageROIColor[0][1];

            cbo_PackageROIDark2.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[0][2].Name;
            pnl2_PackageROIDark2.BackColor = m_smVisionInfo.g_arrPackageROIColor[0][2];

            cbo_CrackROI.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[1][0].Name;
            pnl2_CrackROI.BackColor = m_smVisionInfo.g_arrPackageROIColor[1][0];

            cbo_ChippedROIBright.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[2][0].Name;
            pnl2_ChippedROIBright.BackColor = m_smVisionInfo.g_arrPackageROIColor[2][0];

            cbo_ChippedROIDark.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[2][1].Name;
            pnl2_ChippedROIDark.BackColor = m_smVisionInfo.g_arrPackageROIColor[2][1];

            cbo_MoldROI.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[3][0].Name;
            pnl2_MoldROI.BackColor = m_smVisionInfo.g_arrPackageROIColor[3][0];

            cbo_PackageDontCareROI.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[4][0].Name;
            pnl2_PackageDontCareROI.BackColor = m_smVisionInfo.g_arrPackageROIColor[4][0];

            cbo_MoldDontCareROI.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[4][1].Name;
            pnl2_MoldDontCareROI.BackColor = m_smVisionInfo.g_arrPackageROIColor[4][1];

            cbo_SideLightGaugeROI.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[5][0].Name;
            pnl2_SideLightGaugeROI.BackColor = m_smVisionInfo.g_arrPackageROIColor[5][0];

            cbo_TopLightGaugeROI.SelectedItem = m_smVisionInfo.g_arrPackageROIColor[5][1].Name;
            pnl2_TopLightGaugeROI.BackColor = m_smVisionInfo.g_arrPackageROIColor[5][1];
            
        }
        private void UpdateSealSetting()
        {
            cbo_PositionSearchROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[0][0].Name;
            pnl2_PositionSearchROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[0][0];

            cbo_PositionPositionROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[0][1].Name;
            pnl2_PositionPositionROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[0][1];

            cbo_SealLineROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[1][0].Name;
            pnl2_SealLineROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[1][0];

            cbo_DistanceROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[2][0].Name;
            pnl2_DistanceROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[2][0];

            cbo_CircleGaugeROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[3][0].Name;
            pnl2_CircleGaugeROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[3][0];

            cbo_CircleGaugeResult_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[3][1].Name;
            pnl2_CircleGaugeResult_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[3][1];

            cbo_SprocketHoleDefectROIInward_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[3][2].Name;
            pnl2_SprocketHoleDefectROIInward_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[3][2];

            cbo_SprocketHoleBrokenROIOuter_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[3][3].Name;
            pnl2_SprocketHoleBrokenROIOuter_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[3][3];

            cbo_SprocketHoleBrokenROIInner_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[3][4].Name;
            pnl2_SprocketHoleBrokenROIInner_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[3][4];

            cbo_OverHeatROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[4][0].Name;
            pnl2_OverHeatROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[4][0];

            cbo_TestROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[5][0].Name;
            pnl2_TestROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[5][0];

            cbo_EmptyROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[6][0].Name;
            pnl2_EmptyROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[6][0];

            cbo_EmptyDontCareROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[6][1].Name;
            pnl2_EmptyDontCareROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[6][1];

            cbo_MarkROI_Seal.SelectedItem = m_smVisionInfo.g_arrSealROIColor[7][0].Name;
            pnl2_MarkROI_Seal.BackColor = m_smVisionInfo.g_arrSealROIColor[7][0];

        }
        private void UpdateBarcodeSetting()
        {
            cbo_SearchROI_Barcode.SelectedItem = m_smVisionInfo.g_arrBarcodeROIColor[0][0].Name;
            pnl2_SearchROI_Barcode.BackColor = m_smVisionInfo.g_arrBarcodeROIColor[0][0];

            cbo_PatternROI_Barcode.SelectedItem = m_smVisionInfo.g_arrBarcodeROIColor[1][0].Name;
            pnl2_PatternROI_Barcode.BackColor = m_smVisionInfo.g_arrBarcodeROIColor[1][0];

            cbo_BarcodeROI_Barcode.SelectedItem = m_smVisionInfo.g_arrBarcodeROIColor[2][0].Name;
            pnl2_BarcodeROI_Barcode.BackColor = m_smVisionInfo.g_arrBarcodeROIColor[2][0];

            cbo_BarcodeAreaTolerance_Barcode.SelectedItem = m_smVisionInfo.g_arrBarcodeROIColor[3][0].Name;
            pnl2_BarcodeAreaTolerance_Barcode.BackColor = m_smVisionInfo.g_arrBarcodeROIColor[3][0];

            cbo_PatternAreaTolerance_Barcode.SelectedItem = m_smVisionInfo.g_arrBarcodeROIColor[4][0].Name;
            pnl2_PatternAreaTolerance_Barcode.BackColor = m_smVisionInfo.g_arrBarcodeROIColor[4][0];
        }
        private void UpdatePadSetting()
        {
            cbo_SearchROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[0][0].Name;
            pnl2_SearchROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[0][0];

            cbo_PatternROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[1][0].Name;
            pnl2_PatternROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[1][0];

            cbo_GaugeROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[2][0].Name;
            pnl2_GaugeROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[2][0];

            cbo_PadROITolerance_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[3][0].Name;
            pnl2_PadROITolerance_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[3][0];

            cbo_IndividualPadInspect_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[4][0].Name;
            pnl2_IndividualPadInspect_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[4][0];

            cbo_IndividualPadDisabled_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[4][1].Name;
            pnl2_IndividualPadDisabled_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[4][1];

            cbo_IndividualPadSelected_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[4][2].Name;
            pnl2_IndividualPadSelected_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[4][2];

            cbo_IndividualPadLabel_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[4][3].Name;
            pnl2_IndividualPadLabel_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[4][3];

            cbo_IndividualPadInspectionAreaROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[5][0].Name;
            pnl2_IndividualPadInspectionAreaROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[5][0];

            cbo_Pin1_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[6][0].Name;
            pnl2_Pin1_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[6][0];

            cbo_PHROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[7][0].Name;
            pnl2_PHROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[7][0];

            cbo_ColorROITolerance_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[8][0].Name;
            pnl2_ColorROITolerance_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[8][0];

            cbo_PitchGapLink_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[9][0].Name;
            pnl2_PitchGapLink_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[9][0];

            cbo_PadDontCareROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[10][0].Name;
            pnl2_PadDontCareROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[10][0];

            cbo_ColorDontCareROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[10][1].Name;
            pnl2_ColorDontCareROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[10][1];

            cbo_UnitROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[11][0].Name;
            pnl2_UnitROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[11][0];

            cbo_OrientROI_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[12][0].Name;
            pnl2_OrientROI_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[12][0];

            cbo_EdgeLimitSetting_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[13][0].Name;
            pnl2_EdgeLimitSetting_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[13][0];

            cbo_StandOffLimitSetting_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[14][0].Name;
            pnl2_StandOffLimitSetting_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[14][0];

            cbo_PositionSetting_Pad.SelectedItem = m_smVisionInfo.g_arrPadROIColor[15][0].Name;
            pnl2_PositionSetting_Pad.BackColor = m_smVisionInfo.g_arrPadROIColor[15][0];

        }
        private void UpdatePadPackageSetting()
        {
            cbo_PackageROIBright_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[0][0].Name;
            pnl2_PackageROIBright_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[0][0];

            cbo_PackageROIDark_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[0][1].Name;
            pnl2_PackageROIDark_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[0][1];

            cbo_CrackROI_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[1][0].Name;
            pnl2_CrackROI_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[1][0];

            cbo_ChipROIBright_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[2][0].Name;
            pnl2_ChipROIBright_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[2][0];

            cbo_ChipROIDark_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[2][1].Name;
            pnl2_ChipROIDark_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[2][1];

            cbo_MoldROI_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[3][0].Name;
            pnl2_MoldROI_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[3][0];

            cbo_DontCareROI_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[4][0].Name;
            pnl2_DontCareROI_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[4][0];

            cbo_ForeignMaterialROI_PadPackage.SelectedItem = m_smVisionInfo.g_arrPadPackageROIColor[5][0].Name;
            pnl2_ForeignMaterialROI_PadPackage.BackColor = m_smVisionInfo.g_arrPadPackageROIColor[5][0];

        }
        private void UpdateLead3DSetting()
        {
            cbo_SearchROI_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[0][0].Name;
            pnl2_SearchROI_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[0][0];

            cbo_PackageToBaseROITolerance_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[1][0].Name;
            pnl2_PackageToBaseROITolerance_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[1][0];

            cbo_PitchGapLink_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[2][0].Name;
            pnl2_PitchGapLink_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[2][0];

            cbo_DontCareROI_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[3][0].Name;
            pnl2_DontCareROI_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[3][0];

            cbo_AGVROITolerance_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[4][0].Name;
            pnl2_AGVROITolerance_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[4][0];

            cbo_Pin1ROI_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[5][0].Name;
            pnl2_Pin1ROI_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[5][0];

            cbo_PHROI_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[6][0].Name;
            pnl2_PHROI_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[6][0];

            cbo_PackageROI_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[7][0].Name;
            pnl2_PackageROI_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[7][0];

            cbo_PositionSetting_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[8][0].Name;
            pnl2_PositionSetting_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[8][0];

            cbo_TipBuildAreaROITolerance_Lead3D.SelectedItem = m_smVisionInfo.g_arrLead3DROIColor[9][0].Name;
            pnl2_TipBuildAreaROITolerance_Lead3D.BackColor = m_smVisionInfo.g_arrLead3DROIColor[9][0];
        }
        private void UpdateLeadSetting()
        {
            cbo_SearchROI_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[0][0].Name;
            pnl2_SearchROI_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[0][0];

            cbo_LeadROI_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[1][0].Name;
            pnl2_LeadROI_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[1][0];

            cbo_PitchGapLink_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[2][0].Name;
            pnl2_PitchGapLink_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[2][0];

            cbo_AGVROITolerance_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[3][0].Name;
            pnl2_AGVROITolerance_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[3][0];

            cbo_GaugeROI_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[4][0].Name;
            pnl2_GaugeROI_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[4][0];

            cbo_PackageToBaseROITolerance_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[5][0].Name;
            pnl2_PackageToBaseROITolerance_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[5][0];

            cbo_ReferenceSearchROIAuto_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[6][0].Name;
            pnl2_ReferenceSearchROIAuto_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[6][0];

            cbo_ReferencePatternROIAuto_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[6][1].Name;
            pnl2_ReferencePatternROIAuto_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[6][1];

            cbo_SearchAreaROIAuto_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[6][2].Name;
            pnl2_SearchAreaROIAuto_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[6][2];

            cbo_ReferenceSearchROIManual_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[7][0].Name;
            pnl2_ReferenceSearchROIManual_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[7][0];

            cbo_ReferencePatternROIManual_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[7][1].Name;
            pnl2_ReferencePatternROIManual_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[7][1];

            cbo_DontCareAreaROIManual_Lead.SelectedItem = m_smVisionInfo.g_arrLeadROIColor[7][2].Name;
            pnl2_DontCareAreaROIManual_Lead.BackColor = m_smVisionInfo.g_arrLeadROIColor[7][2];
        }
        private void UpdateEmptySetting()
        {
            cbo_EmptyROI.SelectedItem = m_smVisionInfo.g_arrEmptyROIColor[0][0].Name;
            pnl2_EmptyROI.BackColor = m_smVisionInfo.g_arrEmptyROIColor[0][0];
        }
        private void UpdatePocketPositionSetting()
        {
            cbo_SearchROI_PocketPosition.SelectedItem = m_smVisionInfo.g_arrPocketPositionROIColor[0][0].Name;
            pnl2_SearchROI_PocketPosition.BackColor = m_smVisionInfo.g_arrPocketPositionROIColor[0][0];

            cbo_PatternROI_PocketPosition.SelectedItem = m_smVisionInfo.g_arrPocketPositionROIColor[1][0].Name;
            pnl2_PatternROI_PocketPosition.BackColor = m_smVisionInfo.g_arrPocketPositionROIColor[1][0];

            cbo_PocketGaugeROI_PocketPosition.SelectedItem = m_smVisionInfo.g_arrPocketPositionROIColor[2][0].Name;
            pnl2_PocketGaugeROI_PocketPosition.BackColor = m_smVisionInfo.g_arrPocketPositionROIColor[2][0];

            cbo_PlateGaugeROI_PocketPosition.SelectedItem = m_smVisionInfo.g_arrPocketPositionROIColor[3][0].Name;
            pnl2_PlateGaugeROI_PocketPosition.BackColor = m_smVisionInfo.g_arrPocketPositionROIColor[3][0];
        }
        private void cbo_MO_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_SearchROI_MO":
                        pnl2_SearchROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_RetestROI_MO":
                        pnl2_RetestROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_MarkROI_MO":
                        pnl2_MarkROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_OrientROI_MO":
                        pnl2_OrientROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_UnitROI_MO":
                        pnl2_UnitROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_Pin1ROI_MO":
                        pnl2_Pin1ROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_DontCareROI_MO":
                        pnl2_DontCareROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_GaugeROI_MO":
                        pnl2_GaugeROI_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CharROIInspect_MO":
                        pnl2_CharROIInspect_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CharROISelected_MO":
                        pnl2_CharROISelected_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CharROIIgnored_MO":
                        pnl2_CharROIIgnored_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CharROIFiltered_MO":
                        pnl2_CharROIFiltered_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CharROISplit_MO":
                        pnl2_CharROISplit_MO.BackColor = Color.FromName(text);
                        break;
                    case "cbo_Mark2DROI_MO":
                        pnl2_Mark2DROI_MO.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Package_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_PackageROIBright":
                        pnl2_PackageROIBright.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageROIDark":
                        pnl2_PackageROIDark.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageROIDark2":
                        pnl2_PackageROIDark2.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CrackROI":
                        pnl2_CrackROI.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ChippedROIBright":
                        pnl2_ChippedROIBright.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ChippedROIDark":
                        pnl2_ChippedROIDark.BackColor = Color.FromName(text);
                        break;
                    case "cbo_MoldROI":
                        pnl2_MoldROI.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageDontCareROI":
                        pnl2_PackageDontCareROI.BackColor = Color.FromName(text);
                        break;
                    case "cbo_MoldDontCareROI":
                        pnl2_MoldDontCareROI.BackColor = Color.FromName(text);
                        break;
                    case "cbo_SideLightGaugeROI":
                        pnl2_SideLightGaugeROI.BackColor = Color.FromName(text);
                        break;
                    case "cbo_TopLightGaugeROI":
                        pnl2_TopLightGaugeROI.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Seal_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_PositionSearchROI_Seal":
                        pnl2_PositionSearchROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PositionPositionROI_Seal":
                        pnl2_PositionPositionROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_SealLineROI_Seal":
                        pnl2_SealLineROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_DistanceROI_Seal":
                        pnl2_DistanceROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CircleGaugeROI_Seal":
                        pnl2_CircleGaugeROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CircleGaugeResult_Seal":
                        pnl2_CircleGaugeResult_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_SprocketHoleDefectROIInward_Seal":
                        pnl2_SprocketHoleDefectROIInward_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_SprocketHoleBrokenROIOuter_Seal":
                        pnl2_SprocketHoleBrokenROIOuter_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_SprocketHoleBrokenROIInner_Seal":
                        pnl2_SprocketHoleBrokenROIInner_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_OverHeatROI_Seal":
                        pnl2_OverHeatROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_TestROI_Seal":
                        pnl2_TestROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_EmptyROI_Seal":
                        pnl2_EmptyROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_EmptyDontCareROI_Seal":
                        pnl2_EmptyDontCareROI_Seal.BackColor = Color.FromName(text);
                        break;
                    case "cbo_MarkROI_Seal":
                        pnl2_MarkROI_Seal.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Barcode_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_SearchROI_Barcode":
                        pnl2_SearchROI_Barcode.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PatternROI_Barcode":
                        pnl2_PatternROI_Barcode.BackColor = Color.FromName(text);
                        break;
                    case "cbo_BarcodeROI_Barcode":
                        pnl2_BarcodeROI_Barcode.BackColor = Color.FromName(text);
                        break;
                    case "cbo_BarcodeAreaTolerance_Barcode":
                        pnl2_BarcodeAreaTolerance_Barcode.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PatternAreaTolerance_Barcode":
                        pnl2_PatternAreaTolerance_Barcode.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Pad_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_SearchROI_Pad":
                        pnl2_SearchROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PatternROI_Pad":
                        pnl2_PatternROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_GaugeROI_Pad":
                        pnl2_GaugeROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PadROITolerance_Pad":
                        pnl2_PadROITolerance_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_IndividualPadInspect_Pad":
                        pnl2_IndividualPadInspect_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_IndividualPadDisabled_Pad":
                        pnl2_IndividualPadDisabled_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_IndividualPadSelected_Pad":
                        pnl2_IndividualPadSelected_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_IndividualPadLabel_Pad":
                        pnl2_IndividualPadLabel_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_IndividualPadInspectionAreaROI_Pad":
                        pnl2_IndividualPadInspectionAreaROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_Pin1_Pad":
                        pnl2_Pin1_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PHROI_Pad":
                        pnl2_PHROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ColorROITolerance_Pad":
                        pnl2_ColorROITolerance_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PitchGapLink_Pad":
                        pnl2_PitchGapLink_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PadDontCareROI_Pad":
                        pnl2_PadDontCareROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ColorDontCareROI_Pad":
                        pnl2_ColorDontCareROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_UnitROI_Pad":
                        pnl2_UnitROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_OrientROI_Pad":
                        pnl2_OrientROI_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_EdgeLimitSetting_Pad":
                        pnl2_EdgeLimitSetting_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_StandOffLimitSetting_Pad":
                        pnl2_StandOffLimitSetting_Pad.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PositionSetting_Pad":
                        pnl2_PositionSetting_Pad.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_PadPackage_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_PackageROIBright_PadPackage":
                        pnl2_PackageROIBright_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageROIDark_PadPackage":
                        pnl2_PackageROIDark_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_CrackROI_PadPackage":
                        pnl2_CrackROI_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ChipROIBright_PadPackage":
                        pnl2_ChipROIBright_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ChipROIDark_PadPackage":
                        pnl2_ChipROIDark_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_MoldROI_PadPackage":
                        pnl2_MoldROI_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_DontCareROI_PadPackage":
                        pnl2_DontCareROI_PadPackage.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ForeignMaterialROI_PadPackage":
                        pnl2_ForeignMaterialROI_PadPackage.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Lead3D_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_SearchROI_Lead3D":
                        pnl2_SearchROI_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageToBaseROITolerance_Lead3D":
                        pnl2_PackageToBaseROITolerance_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PitchGapLink_Lead3D":
                        pnl2_PitchGapLink_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_DontCareROI_Lead3D":
                        pnl2_DontCareROI_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_AGVROITolerance_Lead3D":
                        pnl2_AGVROITolerance_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_Pin1ROI_Lead3D":
                        pnl2_Pin1ROI_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PHROI_Lead3D":
                        pnl2_PHROI_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageROI_Lead3D":
                        pnl2_PackageROI_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PositionSetting_Lead3D":
                        pnl2_PositionSetting_Lead3D.BackColor = Color.FromName(text);
                        break;
                    case "cbo_TipBuildAreaROITolerance_Lead3D":
                        pnl2_TipBuildAreaROITolerance_Lead3D.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Lead_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_SearchROI_Lead":
                        pnl2_SearchROI_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_LeadROI_Lead":
                        pnl2_LeadROI_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PitchGapLink_Lead":
                        pnl2_PitchGapLink_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_AGVROITolerance_Lead":
                        pnl2_AGVROITolerance_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_GaugeROI_Lead":
                        pnl2_GaugeROI_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PackageToBaseROITolerance_Lead":
                        pnl2_PackageToBaseROITolerance_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ReferenceSearchROIAuto_Lead":
                        pnl2_ReferenceSearchROIAuto_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ReferencePatternROIAuto_Lead":
                        pnl2_ReferencePatternROIAuto_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_SearchAreaROIAuto_Lead":
                        pnl2_SearchAreaROIAuto_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ReferenceSearchROIManual_Lead":
                        pnl2_ReferenceSearchROIManual_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_ReferencePatternROIManual_Lead":
                        pnl2_ReferencePatternROIManual_Lead.BackColor = Color.FromName(text);
                        break;
                    case "cbo_DontCareAreaROIManual_Lead":
                        pnl2_DontCareAreaROIManual_Lead.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_Empty_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_EmptyROI":
                        pnl2_EmptyROI.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_PocketPosition_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            string text = (sender as ComboBox).Items[e.Index].ToString();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                switch ((sender as ComboBox).Name)
                {
                    case "cbo_SearchROI_PocketPosition":
                        pnl2_SearchROI_PocketPosition.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PatternROI_PocketPosition":
                        pnl2_PatternROI_PocketPosition.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PocketGaugeROI_PocketPosition":
                        pnl2_PocketGaugeROI_PocketPosition.BackColor = Color.FromName(text);
                        break;
                    case "cbo_PlateGaugeROI_PocketPosition":
                        pnl2_PlateGaugeROI_PocketPosition.BackColor = Color.FromName(text);
                        break;
                }
            }
        }
        private void cbo_MO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_SearchROI_MO":
                    if (cbo_RetestROI_MO.Visible && (cbo_SearchROI_MO.SelectedItem.ToString() == cbo_RetestROI_MO.SelectedItem.ToString()))
                    {
                        cbo_SearchROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[0][0].Name;
                        SRMMessageBox.Show("Cannot select same color as ReTest ROI");
                        break;
                    }
                    m_smVisionInfo.g_arrMarkOrientROIColor[0][0] = Color.FromName(cbo_SearchROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_RetestROI_MO":
                    if (cbo_SearchROI_MO.SelectedItem.ToString() == cbo_RetestROI_MO.SelectedItem.ToString())
                    {
                        cbo_RetestROI_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[1][0].Name;
                        SRMMessageBox.Show("Cannot select same color as Search ROI");
                        break;
                    }
                    m_smVisionInfo.g_arrMarkOrientROIColor[1][0] = Color.FromName(cbo_RetestROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_MarkROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[2][0] = Color.FromName(cbo_MarkROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_OrientROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[3][0] = Color.FromName(cbo_OrientROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_UnitROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[4][0] = Color.FromName(cbo_UnitROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_Pin1ROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[5][0] = Color.FromName(cbo_Pin1ROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_DontCareROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[6][0] = Color.FromName(cbo_DontCareROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_GaugeROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[7][0] = Color.FromName(cbo_GaugeROI_MO.SelectedItem.ToString());
                    break;
                case "cbo_CharROIInspect_MO":
                    if (cbo_CharROIInspect_MO.SelectedItem.ToString() == cbo_CharROISelected_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIInspect_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][0].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Selected)");
                        break;
                    }
                    else if (cbo_CharROIInspect_MO.SelectedItem.ToString() == cbo_CharROIIgnored_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIInspect_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][0].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Ignored)");
                        break;
                    }
                    else if (cbo_CharROIInspect_MO.SelectedItem.ToString() == cbo_CharROIFiltered_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIInspect_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][0].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Filtered)");
                        break;
                    }
                    m_smVisionInfo.g_arrMarkOrientROIColor[8][0] = Color.FromName(cbo_CharROIInspect_MO.SelectedItem.ToString());
                    break;
                case "cbo_CharROISelected_MO":
                    if (cbo_CharROISelected_MO.SelectedItem.ToString() == cbo_CharROIInspect_MO.SelectedItem.ToString())
                    {
                        cbo_CharROISelected_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][1].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Inspect)");
                        break;
                    }
                    else if (cbo_CharROISelected_MO.SelectedItem.ToString() == cbo_CharROIIgnored_MO.SelectedItem.ToString())
                    {
                        cbo_CharROISelected_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][1].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Ignored)");
                        break;
                    }
                    else if (cbo_CharROISelected_MO.SelectedItem.ToString() == cbo_CharROIFiltered_MO.SelectedItem.ToString())
                    {
                        cbo_CharROISelected_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][1].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Filtered)");
                        break;
                    }
                    m_smVisionInfo.g_arrMarkOrientROIColor[8][1] = Color.FromName(cbo_CharROISelected_MO.SelectedItem.ToString());
                    break;
                case "cbo_CharROIIgnored_MO":
                    if (cbo_CharROIIgnored_MO.SelectedItem.ToString() == cbo_CharROIInspect_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIIgnored_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][2].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Inspect)");
                        break;
                    }
                    else if (cbo_CharROIIgnored_MO.SelectedItem.ToString() == cbo_CharROISelected_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIIgnored_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][2].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Selected)");
                        break;
                    }
                    else if (cbo_CharROIIgnored_MO.SelectedItem.ToString() == cbo_CharROIFiltered_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIIgnored_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][2].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Filtered)");
                        break;
                    }
                    m_smVisionInfo.g_arrMarkOrientROIColor[8][2] = Color.FromName(cbo_CharROIIgnored_MO.SelectedItem.ToString());
                    break;
                case "cbo_CharROIFiltered_MO":
                    if (cbo_CharROIFiltered_MO.SelectedItem.ToString() == cbo_CharROIInspect_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIFiltered_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][3].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Inspect)");
                        break;
                    }
                    else if (cbo_CharROIFiltered_MO.SelectedItem.ToString() == cbo_CharROISelected_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIFiltered_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][3].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Selected)");
                        break;
                    }
                    else if (cbo_CharROIFiltered_MO.SelectedItem.ToString() == cbo_CharROIIgnored_MO.SelectedItem.ToString())
                    {
                        cbo_CharROIFiltered_MO.SelectedItem = m_smVisionInfo.g_arrMarkOrientROIColor[8][3].Name;
                        SRMMessageBox.Show("Cannot select same color as Char ROI (Ignored)");
                        break;
                    }
                    m_smVisionInfo.g_arrMarkOrientROIColor[8][3] = Color.FromName(cbo_CharROIFiltered_MO.SelectedItem.ToString());
                    break;
                case "cbo_CharROISplit_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[8][4] = Color.FromName(cbo_CharROISplit_MO.SelectedItem.ToString());
                    break;
                case "cbo_Mark2DROI_MO":
                    m_smVisionInfo.g_arrMarkOrientROIColor[9][0] = Color.FromName(cbo_Mark2DROI_MO.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Package_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_PackageROIBright":
                    m_smVisionInfo.g_arrPackageROIColor[0][0] = Color.FromName(cbo_PackageROIBright.SelectedItem.ToString());
                    break;
                case "cbo_PackageROIDark":
                    m_smVisionInfo.g_arrPackageROIColor[0][1] = Color.FromName(cbo_PackageROIDark.SelectedItem.ToString());
                    break;
                case "cbo_PackageROIDark2":
                    m_smVisionInfo.g_arrPackageROIColor[0][2] = Color.FromName(cbo_PackageROIDark2.SelectedItem.ToString());
                    break;
                case "cbo_CrackROI":
                    m_smVisionInfo.g_arrPackageROIColor[1][0] = Color.FromName(cbo_CrackROI.SelectedItem.ToString());
                    break;
                case "cbo_ChippedROIBright":
                    m_smVisionInfo.g_arrPackageROIColor[2][0] = Color.FromName(cbo_ChippedROIBright.SelectedItem.ToString());
                    break;
                case "cbo_ChippedROIDark":
                    m_smVisionInfo.g_arrPackageROIColor[2][1] = Color.FromName(cbo_ChippedROIDark.SelectedItem.ToString());
                    break;
                case "cbo_MoldROI":
                    m_smVisionInfo.g_arrPackageROIColor[3][0] = Color.FromName(cbo_MoldROI.SelectedItem.ToString());
                    break;
                case "cbo_PackageDontCareROI":
                    m_smVisionInfo.g_arrPackageROIColor[4][0] = Color.FromName(cbo_PackageDontCareROI.SelectedItem.ToString());
                    break;
                case "cbo_MoldDontCareROI":
                    m_smVisionInfo.g_arrPackageROIColor[4][1] = Color.FromName(cbo_MoldDontCareROI.SelectedItem.ToString());
                    break;
                case "cbo_SideLightGaugeROI":
                    m_smVisionInfo.g_arrPackageROIColor[5][0] = Color.FromName(cbo_SideLightGaugeROI.SelectedItem.ToString());
                    break;
                case "cbo_TopLightGaugeROI":
                    m_smVisionInfo.g_arrPackageROIColor[5][1] = Color.FromName(cbo_TopLightGaugeROI.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Seal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_PositionSearchROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[0][0] = Color.FromName(cbo_PositionSearchROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_PositionPositionROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[0][1] = Color.FromName(cbo_PositionPositionROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_SealLineROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[1][0] = Color.FromName(cbo_SealLineROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_DistanceROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[2][0] = Color.FromName(cbo_DistanceROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_CircleGaugeROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[3][0] = Color.FromName(cbo_CircleGaugeROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_CircleGaugeResult_Seal":
                    m_smVisionInfo.g_arrSealROIColor[3][1] = Color.FromName(cbo_CircleGaugeResult_Seal.SelectedItem.ToString());
                    break;
                case "cbo_SprocketHoleDefectROIInward_Seal":
                    m_smVisionInfo.g_arrSealROIColor[3][2] = Color.FromName(cbo_SprocketHoleDefectROIInward_Seal.SelectedItem.ToString());
                    break;
                case "cbo_SprocketHoleBrokenROIOuter_Seal":
                    m_smVisionInfo.g_arrSealROIColor[3][3] = Color.FromName(cbo_SprocketHoleBrokenROIOuter_Seal.SelectedItem.ToString());
                    break;
                case "cbo_SprocketHoleBrokenROIInner_Seal":
                    m_smVisionInfo.g_arrSealROIColor[3][4] = Color.FromName(cbo_SprocketHoleBrokenROIInner_Seal.SelectedItem.ToString());
                    break;
                case "cbo_OverHeatROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[4][0] = Color.FromName(cbo_OverHeatROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_TestROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[5][0] = Color.FromName(cbo_TestROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_EmptyROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[6][0] = Color.FromName(cbo_EmptyROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_EmptyDontCareROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[6][1] = Color.FromName(cbo_EmptyDontCareROI_Seal.SelectedItem.ToString());
                    break;
                case "cbo_MarkROI_Seal":
                    m_smVisionInfo.g_arrSealROIColor[7][0] = Color.FromName(cbo_MarkROI_Seal.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Barcode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_SearchROI_Barcode":
                    m_smVisionInfo.g_arrBarcodeROIColor[0][0] = Color.FromName(cbo_SearchROI_Barcode.SelectedItem.ToString());
                    break;
                case "cbo_PatternROI_Barcode":
                    m_smVisionInfo.g_arrBarcodeROIColor[1][0] = Color.FromName(cbo_PatternROI_Barcode.SelectedItem.ToString());
                    break;
                case "cbo_BarcodeROI_Barcode":
                    m_smVisionInfo.g_arrBarcodeROIColor[2][0] = Color.FromName(cbo_BarcodeROI_Barcode.SelectedItem.ToString());
                    break;
                case "cbo_BarcodeAreaTolerance_Barcode":
                    m_smVisionInfo.g_arrBarcodeROIColor[3][0] = Color.FromName(cbo_BarcodeAreaTolerance_Barcode.SelectedItem.ToString());
                    break;
                case "cbo_PatternAreaTolerance_Barcode":
                    m_smVisionInfo.g_arrBarcodeROIColor[4][0] = Color.FromName(cbo_PatternAreaTolerance_Barcode.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Pad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_SearchROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[0][0] = Color.FromName(cbo_SearchROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_PatternROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[1][0] = Color.FromName(cbo_PatternROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_GaugeROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[2][0] = Color.FromName(cbo_GaugeROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_PadROITolerance_Pad":
                    m_smVisionInfo.g_arrPadROIColor[3][0] = Color.FromName(cbo_PadROITolerance_Pad.SelectedItem.ToString());
                    break;
                case "cbo_IndividualPadInspect_Pad":
                    m_smVisionInfo.g_arrPadROIColor[4][0] = Color.FromName(cbo_IndividualPadInspect_Pad.SelectedItem.ToString());
                    break;
                case "cbo_IndividualPadDisabled_Pad":
                    m_smVisionInfo.g_arrPadROIColor[4][1] = Color.FromName(cbo_IndividualPadDisabled_Pad.SelectedItem.ToString());
                    break;
                case "cbo_IndividualPadSelected_Pad":
                    m_smVisionInfo.g_arrPadROIColor[4][2] = Color.FromName(cbo_IndividualPadSelected_Pad.SelectedItem.ToString());
                    break;
                case "cbo_IndividualPadLabel_Pad":
                    m_smVisionInfo.g_arrPadROIColor[4][3] = Color.FromName(cbo_IndividualPadLabel_Pad.SelectedItem.ToString());
                    break;
                case "cbo_IndividualPadInspectionAreaROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[5][0] = Color.FromName(cbo_IndividualPadInspectionAreaROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_Pin1_Pad":
                    m_smVisionInfo.g_arrPadROIColor[6][0] = Color.FromName(cbo_Pin1_Pad.SelectedItem.ToString());
                    break;
                case "cbo_PHROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[7][0] = Color.FromName(cbo_PHROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_ColorROITolerance_Pad":
                    m_smVisionInfo.g_arrPadROIColor[8][0] = Color.FromName(cbo_ColorROITolerance_Pad.SelectedItem.ToString());
                    break;
                case "cbo_PitchGapLink_Pad":
                    m_smVisionInfo.g_arrPadROIColor[9][0] = Color.FromName(cbo_PitchGapLink_Pad.SelectedItem.ToString());
                    break;
                case "cbo_PadDontCareROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[10][0] = Color.FromName(cbo_PadDontCareROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_ColorDontCareROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[10][1] = Color.FromName(cbo_ColorDontCareROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_UnitROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[11][0] = Color.FromName(cbo_UnitROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_OrientROI_Pad":
                    m_smVisionInfo.g_arrPadROIColor[12][0] = Color.FromName(cbo_OrientROI_Pad.SelectedItem.ToString());
                    break;
                case "cbo_EdgeLimitSetting_Pad":
                    m_smVisionInfo.g_arrPadROIColor[13][0] = Color.FromName(cbo_EdgeLimitSetting_Pad.SelectedItem.ToString());
                    break;
                case "cbo_StandOffLimitSetting_Pad":
                    m_smVisionInfo.g_arrPadROIColor[14][0] = Color.FromName(cbo_StandOffLimitSetting_Pad.SelectedItem.ToString());
                    break;
                case "cbo_PositionSetting_Pad":
                    m_smVisionInfo.g_arrPadROIColor[15][0] = Color.FromName(cbo_PositionSetting_Pad.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_PadPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_PackageROIBright_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[0][0] = Color.FromName(cbo_PackageROIBright_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_PackageROIDark_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[0][1] = Color.FromName(cbo_PackageROIDark_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_CrackROI_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[1][0] = Color.FromName(cbo_CrackROI_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_ChipROIBright_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[2][0] = Color.FromName(cbo_ChipROIBright_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_ChipROIDark_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[2][1] = Color.FromName(cbo_ChipROIDark_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_MoldROI_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[3][0] = Color.FromName(cbo_MoldROI_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_DontCareROI_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[4][0] = Color.FromName(cbo_DontCareROI_PadPackage.SelectedItem.ToString());
                    break;
                case "cbo_ForeignMaterialROI_PadPackage":
                    m_smVisionInfo.g_arrPadPackageROIColor[5][0] = Color.FromName(cbo_ForeignMaterialROI_PadPackage.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Lead3D_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_SearchROI_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[0][0] = Color.FromName(cbo_SearchROI_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_PackageToBaseROITolerance_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[1][0] = Color.FromName(cbo_PackageToBaseROITolerance_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_PitchGapLink_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[2][0] = Color.FromName(cbo_PitchGapLink_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_DontCareROI_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[3][0] = Color.FromName(cbo_DontCareROI_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_AGVROITolerance_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[4][0] = Color.FromName(cbo_AGVROITolerance_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_Pin1ROI_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[5][0] = Color.FromName(cbo_Pin1ROI_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_PHROI_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[6][0] = Color.FromName(cbo_PHROI_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_PackageROI_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[7][0] = Color.FromName(cbo_PackageROI_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_PositionSetting_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[8][0] = Color.FromName(cbo_PositionSetting_Lead3D.SelectedItem.ToString());
                    break;
                case "cbo_TipBuildAreaROITolerance_Lead3D":
                    m_smVisionInfo.g_arrLead3DROIColor[9][0] = Color.FromName(cbo_TipBuildAreaROITolerance_Lead3D.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Lead_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_SearchROI_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[0][0] = Color.FromName(cbo_SearchROI_Lead.SelectedItem.ToString());
                    break;
                case "cbo_LeadROI_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[1][0] = Color.FromName(cbo_LeadROI_Lead.SelectedItem.ToString());
                    break;
                case "cbo_PitchGapLink_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[2][0] = Color.FromName(cbo_PitchGapLink_Lead.SelectedItem.ToString());
                    break;
                case "cbo_AGVROITolerance_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[3][0] = Color.FromName(cbo_AGVROITolerance_Lead.SelectedItem.ToString());
                    break;
                case "cbo_GaugeROI_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[4][0] = Color.FromName(cbo_GaugeROI_Lead.SelectedItem.ToString());
                    break;
                case "cbo_PackageToBaseROITolerance_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[5][0] = Color.FromName(cbo_PackageToBaseROITolerance_Lead.SelectedItem.ToString());
                    break;
                case "cbo_ReferenceSearchROIAuto_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[6][0] = Color.FromName(cbo_ReferenceSearchROIAuto_Lead.SelectedItem.ToString());
                    break;
                case "cbo_ReferencePatternROIAuto_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[6][1] = Color.FromName(cbo_ReferencePatternROIAuto_Lead.SelectedItem.ToString());
                    break;
                case "cbo_SearchAreaROIAuto_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[6][2] = Color.FromName(cbo_SearchAreaROIAuto_Lead.SelectedItem.ToString());
                    break;
                case "cbo_ReferenceSearchROIManual_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[7][0] = Color.FromName(cbo_ReferenceSearchROIManual_Lead.SelectedItem.ToString());
                    break;
                case "cbo_ReferencePatternROIManual_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[7][1] = Color.FromName(cbo_ReferencePatternROIManual_Lead.SelectedItem.ToString());
                    break;
                case "cbo_DontCareAreaROIManual_Lead":
                    m_smVisionInfo.g_arrLeadROIColor[7][2] = Color.FromName(cbo_DontCareAreaROIManual_Lead.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_Empty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_EmptyROI":
                    m_smVisionInfo.g_arrEmptyROIColor[0][0] = Color.FromName(cbo_EmptyROI.SelectedItem.ToString());
                    break;
            }
        }
        private void cbo_PocketPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch ((sender as ComboBox).Name)
            {
                case "cbo_SearchROI_PocketPosition":
                    m_smVisionInfo.g_arrPocketPositionROIColor[0][0] = Color.FromName(cbo_SearchROI_PocketPosition.SelectedItem.ToString());
                    break;
                case "cbo_PatternROI_PocketPosition":
                    m_smVisionInfo.g_arrPocketPositionROIColor[1][0] = Color.FromName(cbo_PatternROI_PocketPosition.SelectedItem.ToString());
                    break;
                case "cbo_PocketGaugeROI_PocketPosition":
                    m_smVisionInfo.g_arrPocketPositionROIColor[2][0] = Color.FromName(cbo_PocketGaugeROI_PocketPosition.SelectedItem.ToString());
                    break;
                case "cbo_PlateGaugeROI_PocketPosition":
                    m_smVisionInfo.g_arrPocketPositionROIColor[3][0] = Color.FromName(cbo_PlateGaugeROI_PocketPosition.SelectedItem.ToString());
                    break;
            }
        }
    }
}
