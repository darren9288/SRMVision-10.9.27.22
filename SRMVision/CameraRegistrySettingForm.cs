using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using ImageAcquisition;
using Common;

namespace SRMVision
{
    public partial class CameraRegistrySettingForm : Form
    {
        #region Constant Variables

        private string[] m_strVisionModule = {  "Barcode",
                                                "Orient", 
                                                "BottomOrient",
                                                "Mark",
                                                "MarkOrient",
                                                "MarkPkg",   
                                                "MOPkg",
                                                "MOLiPkg",
                                                "MOLi",
                                                "Package",
                                                "BottomPosition",
                                                "BottomPositionOrient",
                                                "TapePocketPosition",
                                                "BottomOrientPad",
                                                "Pad",
                                                "PadPos",
                                                "PadPkg",
                                                "PadPkgPos",
                                                "Pad5S",
                                                "Pad5SPos",
                                                "Pad5SPkg",
                                                "Pad5SPkgPos",
                                                "InPocket",
                                                "InPocketPkg",
                                                "InPocketPkgPos",
                                                "IPMLi",
                                                "IPMLiPkg",
                                                "Seal"};
         
        private string[] m_strVisionModuleName =  { "Barcode",
                                                    "Orient",
                                                    "Mark",
                                                    "MO",
                                                    "MarkPkg",
                                                    "MOPkg",
                                                    "Package",
                                                    "Pos",
                                                    "PosOrient",
                                                    "PocketPos",
                                                    "BottomOrientPad",
                                                    "Pad",
                                                    "PadPos",
                                                    "PadPkg",
                                                    "PadPkgPos",   
                                                    "Pad5S",
                                                    "Pad5SPos",
                                                    "Pad5SPkg",
                                                    "Pad5SPkgPos",
                                                    "InPocket",
                                                    "InPocketPkg",
                                                    "InPocketPkgPos",
                                                    "IPMLi",
                                                    "IPMLiPkg",
                                                    "Seal"};

        private List<int> m_intSelectedModuleIndex = new List<int>();

        #endregion

        #region Member Variables

        private List<string> m_strSelectedVision = new List<string>();
        //private AVTVimba m_objAVTFireGrab = new AVTVimba();
        private List<int> m_arrCameraPortNoList = new List<int>();
        private List<string> m_arrCameraIDList = new List<string>();
        #endregion



        public CameraRegistrySettingForm()
        {
            InitializeComponent();
            AssignDataGridValue();
            UpdateGUI();     
        }

        private void AssignDataGridValue()
        {
            dgd_VisionList.Rows.Clear();
            ((DataGridViewComboBoxColumn)dc_VisionID).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_ImageUnits).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_TriggerMode).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_LightController).Items.Clear();

            //Vision ID
            for (int m = 1; m <= 10; m++)
                ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(m.ToString());

            //Image Units
            for (int m = 1; m <=2; m++)
                ((DataGridViewComboBoxColumn)dc_ImageUnits).Items.Add(m.ToString());

            //Trigger Mode
            ((DataGridViewComboBoxColumn)dc_TriggerMode).Items.Add("Integration Enable Inverted Yes");
            ((DataGridViewComboBoxColumn)dc_TriggerMode).Items.Add("Integration Enable Inverted No");

            //Resolution
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("640x480");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("480x480");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("780x582");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1080x810");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1000x1000");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1280x1024");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1336x1002");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1450x1088");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1500x1500");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("1600x1200");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("2000x1500");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("2000x2000");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("2448x2010");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("3000x3000");
            ((DataGridViewComboBoxColumn)dc_Resolution).Items.Add("4000x3000");

            //Light Source Controller type
            ((DataGridViewComboBoxColumn)dc_LightController).Items.Add("Normal");
            ((DataGridViewComboBoxColumn)dc_LightController).Items.Add("Sequential");
        }

        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            for (int i = 0; i < strVisionList.Length; i++)
            {
                int intVisionNo = Convert.ToInt32(strVisionList[i].Substring(6));
                dgd_VisionList.Rows.Add(new DataGridViewRow());
                subKey1 = subKey.OpenSubKey(strVisionList[i]);

                dgd_VisionList.Rows[i].Cells[0].Value = intVisionNo.ToString();

                dgd_VisionList.Rows[i].Cells[1].Value = subKey1.GetValue("ImageUnits", "1").ToString();

                string strTriggerMode;
                if (subKey1.GetValue("TriggerMode", "2").ToString() == "1")
                    strTriggerMode = "Integration Enable Inverted Yes";
                else if (subKey1.GetValue("TriggerMode", "2").ToString() == "2")
                    strTriggerMode = "Integration Enable Inverted No";
                else
                    strTriggerMode = "Trigger Off";

                dgd_VisionList.Rows[i].Cells[2].Value = strTriggerMode;

                dgd_VisionList.Rows[i].Cells[3].Value = subKey1.GetValue("Resolution", "640x480").ToString();

                string strLightControllerType;
                if (subKey1.GetValue("LightControllerType", "1").ToString() == "2")
                    strLightControllerType = "Sequential";
                else
                    strLightControllerType = "Normal";

                dgd_VisionList.Rows[i].Cells[4].Value = strLightControllerType;
            }
        }


        /// <summary>
        /// Check whether all required important info is being filled in datagridview before continue
        /// For instance, vision name, application ID and at least 1 feature for it.
        /// </summary>
        /// <returns>false if info is not complete, true if everything is perfect</returns>
        private bool CheckSetting()
        {
            for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
            {
                if (dgd_VisionList.Rows[i].Cells[0].Value == null || dgd_VisionList.Rows[i].Cells[0].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose vision application ID for existing camera(s).");
                    return false;
                }
                if (dgd_VisionList.Rows[i].Cells[1].Value == null || dgd_VisionList.Rows[i].Cells[1].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose Image Unit(s) for respective vision.");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[2].Value == null || dgd_VisionList.Rows[i].Cells[2].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose Trigger Mode for respective vision.");
                    return false;
                }
                if (dgd_VisionList.Rows[i].Cells[3].Value == null || dgd_VisionList.Rows[i].Cells[3].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please select resolution for existing camera(s).");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[4].Value == null || dgd_VisionList.Rows[i].Cells[4].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose light controller type for respective vision.");
                    return false;
                }
            }

            return true;
        }

        private void SaveAll()
        {
            try
            {
                string a;
                for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
                {
                    for (int j = 0; j < dgd_VisionList.ColumnCount; j++)
                        a = dgd_VisionList.Rows[i].Cells[j].Value.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Please make sure all columns have their own value");
                return;
            }

            //m_objAVTFireGrab.OFFCamera();
            SaveToRegistry();
            DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }
        /// <summary>
        /// Save all setups into CPU registry
        /// </summary>
        private void SaveToRegistry()
        {          
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey key1 = key.OpenSubKey("SVG\\Visions", true);
            //if (key1!=null)
            //      key.DeleteSubKeyTree("SVG\\Visions");
            
            RegistryKey Key = key.CreateSubKey("SVG\\Visions");
            RegistryKey subKey1;
            for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
            {
                subKey1 = Key.CreateSubKey("Vision" + dgd_VisionList.Rows[i].Cells[0].Value.ToString());
                subKey1.SetValue("ImageUnits", dgd_VisionList.Rows[i].Cells[1].Value.ToString());

                int intTriggerModdeIndex;
                switch (dgd_VisionList.Rows[i].Cells[2].Value.ToString())
                {
                    default:
                    case "Trigger Off":
                        intTriggerModdeIndex = 0;
                        break;
                    case "Integration Enable Inverted Yes":
                        intTriggerModdeIndex = 1;
                        break;
                    case "Integration Enable Inverted No":
                        intTriggerModdeIndex = 2;
                        break;
                }
                subKey1.SetValue("TriggerMode", intTriggerModdeIndex);
                subKey1.SetValue("Resolution", dgd_VisionList.Rows[i].Cells[3].Value.ToString());

                int intLightControllerType;
                switch (dgd_VisionList.Rows[i].Cells[4].Value.ToString())
                {
                    default:
                    case "Normal":
                        intLightControllerType = 1;
                        break;
                    case "Sequential":
                        intLightControllerType = 2;
                        break;
                }
                subKey1.SetValue("LightControllerType", intLightControllerType);
            }
        }
 
        /// <summary>
        /// Report error if there is any error on datagridview
        /// </summary>
        private void dgd_VisionList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            SRMMessageBox.Show("Datagridview Error  " + e.Exception.Message);

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                SRMMessageBox.Show("Commit error");
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                SRMMessageBox.Show("Cell change");
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                SRMMessageBox.Show("parsing error");
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                SRMMessageBox.Show("leave control error");
            }

            if ((e.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }

        }
        /// <summary>
        /// Close the application without saving
        /// </summary>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            //m_objAVTFireGrab.OFFCamera();
            Close();
            Dispose();
        }
        /// <summary>
        /// Press this button to finish camera registration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Finish_Click(object sender, EventArgs e)
        {
            if (!CheckSetting())
                return;
            SaveAll();
        }                      

    }
}