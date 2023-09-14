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
    public partial class CameraRegistryForm : Form
    {
        #region Constant Variables

        private string[] m_strVisionModule = {  "Barcode",
                                                "Orient", 
                                                "BottomOrient",
                                                "Mark",
                                                "MarkPkg",
                                                "MarkOrient",   
                                                "MOPkg",
                                                "MOLi",
                                                "MOLiPkg",
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
                                                "Li3D",
                                                "Li3DPkg",
                                                "Seal"};
       
        private string[] m_strVisionModuleName =  { "Barcode",
                                                    "Orient",
                                                    "Mark",
                                                    "MarkPkg",
                                                    "MO",
                                                    "MOPkg",
                                                    "MOLi",
                                                    "MOLiPkg",
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
                                                    "Li3D",
                                                    "Li3DPkg",
                                                    "Seal"};

        private List<int> m_intSelectedModuleIndex = new List<int>();

        #endregion

        #region Member Variables

        private List<string> m_strSelectedVision = new List<string>();
        //private AVTVimba m_objAVTFireGrab = new AVTVimba();
        private List<int> m_arrCameraPortNoList = new List<int>();
        private List<string> m_arrCameraIDList = new List<string>();
        #endregion



        public CameraRegistryForm()
        {
            InitializeComponent();
            AssignDataGridValue();
            UpdateGUI();     
        }

        private void AssignDataGridValue()
        {
            dgd_VisionList.Rows.Clear();
            ((DataGridViewComboBoxColumn)dc_VisionID).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_CameraModel).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_camera).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_PortNo).Items.Clear();
            ((DataGridViewComboBoxColumn)dc_VisionName).Items.Clear();

            //Vision ID
            for (int m = 1; m <= 10; m++)
                ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(m.ToString());

            //Camera Model
            ((DataGridViewComboBoxColumn)dc_CameraModel).Items.Add("Teli");
            ((DataGridViewComboBoxColumn)dc_CameraModel).Items.Add("AVT");

            //Camera color
            ((DataGridViewComboBoxColumn)dc_camera).Items.Add("Color");
            ((DataGridViewComboBoxColumn)dc_camera).Items.Add("Mono");

            //Vision name
            for (int m = 0; m < m_strVisionModule.Length; m++)
                ((DataGridViewComboBoxColumn)dc_VisionName).Items.Add(m_strVisionModule[m]);
        }

        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            string[] strVisionFeatureList;
            RegistryKey subKey1;

            for (int i = 0; i < strVisionList.Length; i++)
            {
                int intVisionNo = Convert.ToInt32(strVisionList[i].Substring(6));
                dgd_VisionList.Rows.Add(new DataGridViewRow());
                subKey1 = subKey.OpenSubKey(strVisionList[i]);
                strVisionFeatureList = subKey1.GetValueNames();

                //AssignVisionID(i);
                dgd_VisionList.Rows[i].Cells[0].Value = intVisionNo.ToString();

                dgd_VisionList.Rows[i].Cells[1].Value = subKey1.GetValue("CameraModel", "AVT").ToString();
                dgd_VisionList.Rows[i].Cells[2].Value = "Mono";

                AssignPortNo(i);
                if (dgd_VisionList.Rows[i].Cells[1].Value.ToString() == "Teli")
                {
                    dgd_VisionList.Rows[i].Cells[3].Value = "-1";
                    dgd_VisionList.Rows[i].Cells[4].Value = subKey1.GetValue("SerialNo", "").ToString();
                }
                else
                {
                    dgd_VisionList.Rows[i].Cells[3].Value = subKey1.GetValue("PortNo", -1).ToString();
                    dgd_VisionList.Rows[i].Cells[4].Value = "-------";
                }

                dgd_VisionList.Rows[i].Cells[5].Value = subKey1.GetValue("VisionName", "Vision " + i).ToString();

                dgd_VisionList.Rows[i].Cells[6].Value = subKey1.GetValue("VisionDisplayName", "Vision " + i).ToString();
                AssignVisionName(i);
               
            }
            //AssignPortNo();
        }

        /// <summary>
        /// Assign port number to datagridview PortNo column's combo box
        /// </summary>
        private void AssignPortNo(int intRow)
        {
            DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            int intPortNo;

            if (dgd_VisionList.Rows[intRow].Cells[1].Value.ToString() == "AVT")
            {
                for (intPortNo = 0; intPortNo < 10; intPortNo++)
                {
                    dgvComboBox.Items.Add(intPortNo.ToString());
                }

                dgvComboBox.Value = dgvComboBox.Items[0];
                dgd_VisionList.Rows[intRow].Cells[3] = dgvComboBox;
                dgd_VisionList.Rows[intRow].Cells[4].Value = "-------";
            }
            else
            {
                dgvComboBox.Items.Add("-1");
                dgvComboBox.Value = dgvComboBox.Items[0];
                dgd_VisionList.Rows[intRow].Cells[3] = dgvComboBox;
                dgd_VisionList.Rows[intRow].Cells[4].Value = "0";
            }
        }

        private void AssignPortNo()
        {
            DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            DataGridViewComboBoxCell dgvComboBox2 = new DataGridViewComboBoxCell();
            dgvComboBox2.Items.Add("-1");

            int intPortNo;
            bool blnRemove;
            int intPreviosRowCount = dgd_VisionList.Rows.Count - 1;

            for (intPortNo = 0; intPortNo < 10; intPortNo++)
            {
                blnRemove = false;
                dgvComboBox.Items.Add(intPortNo.ToString());
                for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
                {
                    if (dgd_VisionList.Rows[i].Cells[1].Value.ToString() == "AVT")
                    {
                        if (Convert.ToInt32(dgd_VisionList.Rows[i].Cells[3].Value) == intPortNo)
                        {
                            blnRemove = true;
                            break;
                        }
                    }
                }
                if (blnRemove)
                    dgvComboBox.Items.Remove(intPortNo.ToString());
            }

            for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
            {
                DataGridViewComboBoxCell dgvComboBoxTemp = new DataGridViewComboBoxCell();
                if (dgd_VisionList.Rows[i].Cells[1].Value.ToString() == "AVT")
                {
                    if (dgd_VisionList.Rows[i].Cells[3].Value.ToString() != "-1")
                        dgvComboBoxTemp.Items.Add(dgd_VisionList.Rows[i].Cells[3].Value.ToString());

                    for (int j = 0; j < dgvComboBox.Items.Count; j++)
                    {
                        dgvComboBoxTemp.Items.Add(dgvComboBox.Items[j].ToString());
                    }

                    dgvComboBoxTemp.Value = dgvComboBoxTemp.Items[0];
                    dgd_VisionList.Rows[i].Cells[3] = dgvComboBoxTemp;
                    dgd_VisionList.Rows[i].Cells[4].Value = "-------";
                }
                else
                {
                    dgvComboBoxTemp.Items.Add(dgvComboBox2.Items[0]);
                    dgvComboBoxTemp.Value = dgvComboBoxTemp.Items[0];
                    dgd_VisionList.Rows[i].Cells[3] = dgvComboBoxTemp;
                }
            }
        }

        private void AssignVisionID(int intRow)
        {
            DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            int intVisionID;
            bool blnAdd = false;
            int intPreviosRowCount = dgd_VisionList.Rows.Count - 1;

            for (intVisionID = 1; intVisionID <= 10; intVisionID++)
            {
                blnAdd = true;
                for (int i = 0; i < intPreviosRowCount; i++)
                {
                    if (i == intRow)
                        continue;

                    if (Convert.ToInt32(dgd_VisionList.Rows[i].Cells[0].Value) == intVisionID)
                    {
                        blnAdd = false;
                        break;
                    }
                }
                if (blnAdd)
                    dgvComboBox.Items.Add(intVisionID.ToString());
            }

            dgvComboBox.Value = dgvComboBox.Items[0];
            dgd_VisionList.Rows[intRow].Cells[0] = dgvComboBox;
        }

        /// <summary>
        /// Assign name to each vision system
        /// Vision 1 - MarkLeadOrientPackage, ....
        /// </summary>
        /// <param name="intRow">selected row for datagridview</param>
        private void AssignVisionName(int intRow)
        {
            if (dgd_VisionList.Rows[intRow].Cells[5].Value == null)
            {
                SRMMessageBox.Show("Registration Error: Vision Name should not be blank");
                return;
            }

            DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            string strName = "";

            for (int i = 0; i < m_strVisionModule.Length; i++)
            {
                if (dgd_VisionList.Rows[intRow].Cells[5].Value.ToString() == m_strVisionModule[i])
                {
                    m_intSelectedModuleIndex.Add(i);
                    strName = m_strVisionModuleName[i];
                    m_strSelectedVision.Add(dgd_VisionList.Rows[intRow].Cells[5].Value.ToString());
                    dgvComboBox.Items.Add(strName);
                    dgvComboBox.Value = dgvComboBox.Items[0];
                  //  dgd_VisionList.Rows[intRow].Cells[6] = dgvComboBox;
                }
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
                    SRMMessageBox.Show("Please choose Camera Model for existing camera(s).");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[2].Value == null || dgd_VisionList.Rows[i].Cells[2].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose Camera Color Mode for existing camera(s).");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[1].Value.ToString() != "Teli")
                {
                    if (dgd_VisionList.Rows[i].Cells[3].Value == null)
                    {
                        SRMMessageBox.Show("Please select port no. for existing camera(s).");
                        return false;
                    }
                }

                if (dgd_VisionList.Rows[i].Cells[5].Value == null)
                {
                    SRMMessageBox.Show("Please assign Vision Name for existing camera(s).");
                    return false;
                }

                if (CheckVisionID(i))
                    return false;

                if (CheckPortNo(i))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// if user suddenly reselect the vision application ID, clear all information behind it
        /// </summary>
        /// <param name="intRow">selected row in datagridview</param>
        private void ClearVisionFeature(int intRow)
        {
            dgd_VisionList.Rows[intRow].Cells[1].Value = "";
            dgd_VisionList.Rows[intRow].Cells[2].Value = "";
            dgd_VisionList.Rows[intRow].Cells[3].Value = "";
            dgd_VisionList.Rows[intRow].Cells[4].Value = "";
            dgd_VisionList.Rows[intRow].Cells[5].Value = "";
            dgd_VisionList.Rows[intRow].Cells[6].Value = "";
        }
     
    
        /// <summary>
        /// Check whether 2 same vision ID are selected
        /// For instance, if vision 1 is used in first row, it shouldn't be appeared in next row.
        /// </summary>
        /// <param name="intRow">selected row in datagridview</param>
        private bool CheckVisionID(int intRow)
        {
            int intVisionID = Convert.ToInt32(dgd_VisionList[0, intRow].Value);

            DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            foreach (string strName in m_strVisionModule)
            {
                dgvComboBox.Items.Add(strName);
            }

            for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
            {
                if (i != intRow && Convert.ToInt32(dgd_VisionList[0, i].Value) == intVisionID)
                {
                    SRMMessageBox.Show("This Vision ID is used by others already", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }

            //DataGridViewComboBoxCell dgvComboBox = new DataGridViewComboBoxCell();
            //switch (intVisionID)
            //{
            //    case 1:
            //        foreach (string strName in m_strVisionModule)
            //        {
            //            dgvComboBox.Items.Add(strName);
            //        }
            //        break;
            //    case 2:
            //        foreach (string strName in m_strVision2)
            //        {
            //            dgvComboBox.Items.Add(strName);
            //        }
            //        break;
            //    case 3:
            //        foreach (string strName in m_strVision3)
            //        {
            //            dgvComboBox.Items.Add(strName);
            //        }
            //        break;
            //    case 4:
            //    case 5:
            //        foreach (string strName in m_strVision4)
            //        {
            //            dgvComboBox.Items.Add(strName);
            //        }
            //        break;
            //    case 6:
            //    case 7:
            //        foreach (string strName in m_strVision6)
            //        {
            //            dgvComboBox.Items.Add(strName);
            //        }
            //        break;
            //    default:
            //        return false;
            //}
            if ((dgd_VisionList.Rows[intRow].Cells[5].Value != null && !dgd_VisionList.Rows[intRow].Cells[5].Value.Equals("")) && dgvComboBox.Items.Contains(dgd_VisionList.Rows[intRow].Cells[5].Value))
            {
            }
            else
            {
                dgvComboBox.Value = dgvComboBox.Items[0];
                dgd_VisionList.Rows[intRow].Cells[5] = dgvComboBox;
            }
           
            return false;
        }

        private bool CheckPortNo(int intRow)
        {
            int intPortNo = Convert.ToInt32(dgd_VisionList[3, intRow].Value);

            for (int i = 0; i < dgd_VisionList.Rows.Count; i++)
            {
                if (i != intRow && Convert.ToInt32(dgd_VisionList[3, i].Value) == intPortNo && intPortNo != -1 && dgd_VisionList[3, i].Value != null)
                {
                    SRMMessageBox.Show("This Port No is used by others already", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }

            return false;
        }

        private void SaveAll()
        {
            if (dgd_VisionList.Rows.Count == 0)
            {
                SRMMessageBox.Show("Please create At least 1 vision ID.");
                return;
            }

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
                subKey1.SetValue("CameraModel", dgd_VisionList.Rows[i].Cells[1].Value.ToString());
                subKey1.SetValue("PortNo", dgd_VisionList.Rows[i].Cells[3].Value.ToString());
                subKey1.SetValue("SerialNo", dgd_VisionList.Rows[i].Cells[4].Value.ToString());
                subKey1.SetValue("VisionName", dgd_VisionList.Rows[i].Cells[5].Value.ToString());
                subKey1.SetValue("VisionDisplayName", dgd_VisionList.Rows[i].Cells[6].Value.ToString());

                if (dgd_VisionList.Rows[i].Cells[2].Value.Equals("Color"))
                     subKey1.SetValue("ColorCamera", "1");

                string strExecutionPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\Default\\";
                Directory.CreateDirectory(strExecutionPath);

                // ------- delete existing subkey ---------
                subKey1.DeleteValue("Barcode", false);
                subKey1.DeleteValue("Orient", false);
                subKey1.DeleteValue("Bottom", false);
                subKey1.DeleteValue("Mark", false);
                subKey1.DeleteValue("Package", false);
                subKey1.DeleteValue("Position", false);
                subKey1.DeleteValue("Pad", false);
                subKey1.DeleteValue("Pad5S", false);
                subKey1.DeleteValue("Lead3D", false);
                subKey1.DeleteValue("Seal", false);

                switch (m_strSelectedVision[i])
                {
                    case "Barcode":
                        subKey1.SetValue("Barcode", "1");
                        break;
                    case "Orient":
                        subKey1.SetValue("Orient", "1");
                        break;
                    case "BottomOrient":
                        subKey1.SetValue("Orient", "1");
                        subKey1.SetValue("Bottom", "1");
                        break;
                    case "Mark":
                        subKey1.SetValue("Mark", "1");
                        break;
                    case "MarkOrient":
                    case "MOLi":
                    case "InPocket":
                    case "IPMLi":
                        subKey1.SetValue("Mark", "1");
                        subKey1.SetValue("Orient", "1");
                        break;
                    case "MarkPkg":
                        subKey1.SetValue("Mark", "1");
                        subKey1.SetValue("Package", "1");
                        break;
                    case "MOPkg":
                    case "MOLiPkg":
                    case "InPocketPkg":
                    case "IPMLiPkg":
                        subKey1.SetValue("Mark", "1");
                        subKey1.SetValue("Orient", "1");
                        subKey1.SetValue("Package", "1");
                        break;
                    case "InPocketPkgPos":
                        subKey1.SetValue("Mark", "1");
                        subKey1.SetValue("Orient", "1");
                        subKey1.SetValue("Package", "1");
                        subKey1.SetValue("Position", "1");
                        break;
                    case "Package":
                        subKey1.SetValue("Package", "1");
                        break;
                    case "BottomPosition":
                    case "BottomPositionOrient":
                        subKey1.SetValue("Position", "1");
                        subKey1.SetValue("Bottom", "1");
                        break;
                    case "TapePocketPosition":
                        subKey1.SetValue("Position", "1");
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                        subKey1.SetValue("Pad", "1");
                        subKey1.SetValue("Orient", "1");
                        break;
                    case "Pad":
                        subKey1.SetValue("Pad", "1");
                        break;
                    case "PadPos":
                        subKey1.SetValue("Pad", "1");
                        subKey1.SetValue("Position", "1");
                        break;
                    case "PadPkg":
                        subKey1.SetValue("Pad", "1");
                        subKey1.SetValue("Package", "1");
                        break;
                    case "PadPkgPos":
                        subKey1.SetValue("Pad", "1");
                        subKey1.SetValue("Package", "1");
                        subKey1.SetValue("Position", "1");
                        break;
                    case "Pad5S":
                        subKey1.SetValue("Pad5S", "1");
                        break;
                    case "Pad5SPos":
                        subKey1.SetValue("Pad5S", "1");
                        subKey1.SetValue("Position", "1");
                        break;
                    case "Pad5SPkg":
                         subKey1.SetValue("Pad5S", "1");
                        subKey1.SetValue("Package", "1");
                        break;
                    case "Pad5SPkgPos":
                        subKey1.SetValue("Pad5S", "1");
                        subKey1.SetValue("Package", "1");
                        subKey1.SetValue("Position", "1");
                        break;
                    case "Li3D":
                        subKey1.SetValue("Lead3D", "1");
                        break;
                    case "Li3DPkg":
                        subKey1.SetValue("Lead3D", "1");
                        subKey1.SetValue("Package", "1");
                        break;
                    case "Seal":
                        subKey1.SetValue("Seal", "1");
                        break;
                    default:
                        SRMMessageBox.Show("SaveToRegistry() -> There is no such vision module name " + m_strSelectedVision[i] + " in this SRMVision software version.");
                        break;
                }                  
            }
        }
      
        /// <summary>
        /// Update VisionID combo box
        /// </summary>
        private void UpdateComboBox()
        {
            ((DataGridViewComboBoxColumn)dc_VisionID).Items.Clear();

            for (int m = 1; m <= 10; m++)
                ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(m.ToString());         
        }



       
        /// <summary>
        /// Name the vision after selection on feature done
        /// </summary>
        private void dgd_VisionList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {           
                case 0:
                    if(!CheckVisionID(e.RowIndex))
                        ClearVisionFeature(e.RowIndex);                    
                    break;
                case 1:
                    AssignPortNo(e.RowIndex);
                    break;
                case 5:
                    AssignVisionName(e.RowIndex);
                    break;
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
        /// <summary>
        /// Detecting camera connection again to continue on next set up
        /// </summary>
        private void btn_Next_Click(object sender, EventArgs e)
        {         
            if (!CheckSetting())
                return;
                     
            if (dgd_VisionList.Rows.Count < 10)
            {
                dgd_VisionList.Rows.Add(new DataGridViewRow());
            }
            else
            {
                SRMMessageBox.Show("Registration cameras finish. Please press OK to continue.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveAll();
            }
        }
        

        
        private void RegistryForm_Load(object sender, EventArgs e)
        {
            //dgd_VisionList.Rows.Clear();
            //UpdateComboBox();
        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            if(dgd_VisionList.Rows.Count > 0)
                dgd_VisionList.Rows.RemoveAt(dgd_VisionList.Rows.Count - 1);
        }
    }
}