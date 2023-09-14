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
namespace VisionProcessForm
{
    public partial class YieldSettingForm : Form
    {
        private int m_intUserGroup = 5;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private int m_intPassUnit = 1000;
        private int m_intFailUnit = 100;
        private CustomOption m_smCustomizeInfo;
        

        public YieldSettingForm(ProductionInfo smProductionInfo , VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;
            
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                  m_smVisionInfo.g_strVisionFolderName + "\\General.xml";
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("Yield");
            chk_StopLowYield.Checked = objFile.GetValueAsBoolean("StopLowYield", false);
            chk_StopContinuousPass.Checked = objFile.GetValueAsBoolean("StopContinuousPass", false);
            chk_StopContinuousFail.Checked = objFile.GetValueAsBoolean("StopContinuousFail", false);
            txt_LowYield.Text = objFile.GetValueAsFloat("LowYield", 95f).ToString();
            txt_MinUnitCheck.Text = objFile.GetValueAsInt("MinUnitCheck", 1000).ToString();
            txt_MinPassUnit.Text = objFile.GetValueAsInt("MinPassUnit", 1000).ToString();
            m_intPassUnit = Convert.ToInt32(txt_MinPassUnit.Text);
            txt_MinFailUnit.Text = objFile.GetValueAsInt("MinFailUnit", 100).ToString();
            m_intFailUnit = Convert.ToInt32(txt_MinFailUnit.Text);

            objFile.GetFirstSection("VisionSystem");
            txt_DelayCheckIO.Text = objFile.GetValueAsInt("DelayCheckIO", 0).ToString();

            DisableField2();
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Yield Page";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_OK.Enabled = false;
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
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                  m_smVisionInfo.g_strVisionFolderName + "\\General.xml";
            
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Yield");

            objFileHandle.WriteElement1Value("StopLowYield", chk_StopLowYield.Checked);
            objFileHandle.WriteElement1Value("StopContinuousPass", chk_StopContinuousPass.Checked);
            objFileHandle.WriteElement1Value("StopContinuousFail", chk_StopContinuousFail.Checked);
            objFileHandle.WriteElement1Value("LowYield", txt_LowYield.Text);
            objFileHandle.WriteElement1Value("MinUnitCheck", txt_MinUnitCheck.Text);
            objFileHandle.WriteElement1Value("MinPassUnit", txt_MinPassUnit.Text);
            objFileHandle.WriteElement1Value("MinFailUnit", txt_MinFailUnit.Text);

            objFileHandle.WriteSectionElement("VisionSystem");
            objFileHandle.WriteElement1Value("DelayCheckIO", txt_DelayCheckIO.Text);

            objFileHandle.WriteEndElement();

            m_smVisionInfo.g_blnStopLowYield = chk_StopLowYield.Checked;
            m_smVisionInfo.g_blnStopContinuousPass = chk_StopContinuousPass.Checked;
            m_smVisionInfo.g_blnStopContinuousFail = chk_StopContinuousFail.Checked;
            m_smVisionInfo.g_fLowYield = (float)Convert.ToDouble(txt_LowYield.Text);
            m_smVisionInfo.g_intMinUnitCheck = Convert.ToInt32(txt_MinUnitCheck.Text);
            m_smVisionInfo.g_intMinPassUnit = Convert.ToInt32(txt_MinPassUnit.Text);
            m_smVisionInfo.g_intMinFailUnit = Convert.ToInt32(txt_MinFailUnit.Text);
            m_smVisionInfo.g_intDelayCheckIO = Convert.ToInt32(txt_DelayCheckIO.Text);
            this.Close();
            this.Dispose();

        }

        private void txt_MinPassUnit_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txt_MinPassUnit.Text) < 1 || Convert.ToInt32(txt_MinPassUnit.Text) > 15000)
            {
                SRMMessageBox.Show("Please enter value within 1 to 15000");
                txt_MinPassUnit.Text = m_intPassUnit.ToString();
            }
            else
            {
                m_intPassUnit = Convert.ToInt32(txt_MinPassUnit.Text);
            }
        }

        private void txt_MinFailUnit_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txt_MinFailUnit.Text) < 1 || Convert.ToInt32(txt_MinFailUnit.Text) > 100)
            {
                SRMMessageBox.Show("Please enter value within 1 to 100");
                txt_MinFailUnit.Text = m_intFailUnit.ToString();
            }
            else
            {
                m_intFailUnit = Convert.ToInt32(txt_MinFailUnit.Text);
            }
        }

        private void YieldSettingForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
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
        }

        private void YieldSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Yield Setting Form Closed", "Exit Yield Form", "", "", m_smProductionInfo.g_strLotID);
            
        }
    }
}
