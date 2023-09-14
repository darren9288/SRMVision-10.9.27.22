using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;

namespace SRMVision
{
    public partial class LightChannelRegistryForm : Form
    {
        
        public List<string> rowString = new List<string>();
        public List<string> rowCOM = new List<string>();
        public List<string> rowVisionNum = new List<string>();
        public List<string> rowVisionName = new List<string>();
        public List<string> removeCOM = new List<string>();
        public List<string> removeVisionNum = new List<string>();
        public List<string> removeVisionName = new List<string>();
        public List<string> removeNameNum = new List<string>();
        public List<string> TEMPremoveCOM = new List<string>();
        public List<string> TEMPremoveVisionNum = new List<string>();
        public List<string> TEMPremoveVisionName = new List<string>();
        public LightChannelRegistryForm()
        {
            InitializeComponent();

            //UpdateGUI();
            //GetAvailableVisions();
            //UpdateComboBox();
            AssignDataGridValue();
            UpdateGUI();

        }



    private void AssignDataGridValue()
    {
            
            dgd_VisionList.Rows.Clear();
        ((DataGridViewComboBoxColumn)dc_PortName).Items.Clear();
        ((DataGridViewComboBoxColumn)dc_VisionID).Items.Clear();
        ((DataGridViewComboBoxColumn)dc_Type).Items.Clear();
        ((DataGridViewComboBoxColumn)dc_LightChannel).Items.Clear();


           // GetAvailableVisions();
            UpdateComboBox();



    }
    private void UpdateGUI()
        {
            rowString.Clear();
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\LightControl");
            string[] strComPortList = subKey.GetSubKeyNames();
            string[] strLightSourceNameList;

            RegistryKey subKey1;
            int row = 0;
            for (int i = 0; i < subKey.SubKeyCount; i++)
            {

                
                RegistryKey subCOMKey = key.CreateSubKey("SVG\\LightControl\\"+ strComPortList[i]);//COMPORT8,9
                string[] strComPortVisionList = subCOMKey.GetSubKeyNames();//Vision1,2,3,4
                
                for (int j=0; j< subCOMKey.SubKeyCount; j++)
                {
                   
                  
                    subKey1 = subCOMKey.OpenSubKey(strComPortVisionList[j]);
                    strLightSourceNameList = subKey1.GetValueNames();

                    for (int k = 0; k < subKey1.ValueCount; k++)
                    {
                        string typeName = strLightSourceNameList[k].ToString();
                        string Clone = typeName;
                        string firstsection = Clone.Remove(Clone.Length-1, 1);
                        string typeNameNum = typeName.Remove(0, Clone.Length - 1);
                        //((DataGridViewComboBoxColumn)dc_PortName).Items.Add(strComPortList[i].ToString());
                        //((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(strComPortVisionList[j].ToString());
                        //((DataGridViewComboBoxColumn)dc_Type).Items.Add(strLightSourceNameList[k].ToString());
                        //((DataGridViewComboBoxColumn)dc_LightChannel).Items.Add(subKey1.GetValue(strLightSourceNameList[k]).ToString());
                        if (!((DataGridViewComboBoxColumn)dc_PortName).Items.Contains(strComPortList[i].ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_PortName).Items.Add(strComPortList[i].ToString());
                            
                        }

                        if (!((DataGridViewComboBoxColumn)dc_VisionID).Items.Contains(strComPortVisionList[j].ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(strComPortVisionList[j].ToString());
                        }

                        if (!((DataGridViewComboBoxColumn)dc_Type).Items.Contains(firstsection))//strLightSourceNameList[k].ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_Type).Items.Add(firstsection);// strLightSourceNameList[k].ToString());
                        }

                        //if (!((DataGridViewComboBoxColumn)dc_Type).Items.Contains(strLightSourceNameList[k].ToString()))
                        //{
                        //    ((DataGridViewComboBoxColumn)dc_Type).Items.Add( strLightSourceNameList[k].ToString());
                        //}

                        if (!((DataGridViewComboBoxColumn)dc_LightChannel).Items.Contains((Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k]))+1).ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_LightChannel).Items.Add((Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k]))+1).ToString());
                        }

                        dgd_VisionList.Rows.Add(new DataGridViewRow());


                        dgd_VisionList.Rows[row].Cells[0].Value = strComPortList[i].ToString();
                        rowCOM.Add(strComPortList[i].ToString());
                        dgd_VisionList.Rows[row].Cells[1].Value = strComPortVisionList[j].ToString();
                        rowVisionNum.Add(strComPortVisionList[j].ToString());
                        dgd_VisionList.Rows[row].Cells[2].Value = firstsection; // strLightSourceNameList[k].ToString();
                        rowVisionName.Add(firstsection);
                        dgd_VisionList.Rows[row].Cells[3].Value = (Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k]))+1).ToString();

                        dgd_VisionList.Rows[row].Cells[4].Value = typeNameNum;

                        rowString.Add( strComPortList[i].ToString() + strComPortVisionList[j].ToString() + firstsection + (Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k])) + 1).ToString());

                        row++;
                    }
                }
            }
            for (int i = 1; i < 10; i++)
            {

                if (((DataGridViewComboBoxColumn)dc_PortName).Items.Contains("*COM" + i)&& ((DataGridViewComboBoxColumn)dc_PortName).Items.Contains("COM" + i))
                    ((DataGridViewComboBoxColumn)dc_PortName).Items.Remove("*COM" + i);

            }
        }


        private void DeleteCOM()
        {
            
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\LightControl");
            string[] strComPortList = subKey.GetSubKeyNames();
            //for (int i = 0; i < subKey.SubKeyCount; i++)//2
            //{
            //    for (int x = 0; x < dgd_VisionList.Rows.Count; x++)
            //     {
            //        if (strComPortList[i] != dgd_VisionList.Rows[x].Cells[0].Value.ToString())
            //        {
            //            subKey.DeleteSubKey(strComPortList[i]);

            //        }
            //     }

            //}
            string[] strLightSourceNameList;

            RegistryKey subKey1;
            int row = 0;
            for (int i = 0; i < subKey.SubKeyCount; i++)//2
            {


                RegistryKey subCOMKey = key.CreateSubKey("SVG\\LightControl\\" + strComPortList[i]);//COMPORT8,9
                

                string[] strComPortVisionList = subCOMKey.GetSubKeyNames();//Vision1,2,3,4

                for (int j = 0; j < subCOMKey.SubKeyCount; j++) //4
                {


                    subKey1 = subCOMKey.OpenSubKey(strComPortVisionList[j]);
                    strLightSourceNameList = subKey1.GetValueNames();

                    for (int k = 0; k < subKey1.ValueCount; k++)//3
                    {
                        string typeName = strLightSourceNameList[k].ToString();
                        string firstsection = typeName.Remove(typeName.Length - 1, 1);
                        //((DataGridViewComboBoxColumn)dc_PortName).Items.Add(strComPortList[i].ToString());
                        //((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(strComPortVisionList[j].ToString());
                        //((DataGridViewComboBoxColumn)dc_Type).Items.Add(strLightSourceNameList[k].ToString());
                        //((DataGridViewComboBoxColumn)dc_LightChannel).Items.Add(subKey1.GetValue(strLightSourceNameList[k]).ToString());
                        if (!((DataGridViewComboBoxColumn)dc_PortName).Items.Contains(strComPortList[i].ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_PortName).Items.Add(strComPortList[i].ToString());

                        }

                        if (!((DataGridViewComboBoxColumn)dc_VisionID).Items.Contains(strComPortVisionList[j].ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(strComPortVisionList[j].ToString());
                        }

                        if (!((DataGridViewComboBoxColumn)dc_Type).Items.Contains(firstsection))//strLightSourceNameList[k].ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_Type).Items.Add(firstsection);// strLightSourceNameList[k].ToString());
                        }

                        //if (!((DataGridViewComboBoxColumn)dc_Type).Items.Contains(strLightSourceNameList[k].ToString()))
                        //{
                        //    ((DataGridViewComboBoxColumn)dc_Type).Items.Add( strLightSourceNameList[k].ToString());
                        //}

                        if (!((DataGridViewComboBoxColumn)dc_LightChannel).Items.Contains((Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k])) + 1).ToString()))
                        {
                            ((DataGridViewComboBoxColumn)dc_LightChannel).Items.Add((Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k])) + 1).ToString());
                        }

                        dgd_VisionList.Rows.Add(new DataGridViewRow());


                        dgd_VisionList.Rows[row].Cells[0].Value = strComPortList[i].ToString();

                        dgd_VisionList.Rows[row].Cells[1].Value = strComPortVisionList[j].ToString();

                        dgd_VisionList.Rows[row].Cells[2].Value = firstsection; // strLightSourceNameList[k].ToString();

                        dgd_VisionList.Rows[row].Cells[3].Value = (Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k])) + 1).ToString();

                        rowString.Add(strComPortList[i].ToString() + strComPortVisionList[j].ToString() + firstsection + (Convert.ToInt32(subKey1.GetValue(strLightSourceNameList[k])) + 1).ToString());

                        row++;
                    }
                }
            }
            
        }


        private void btn_Save_Click(object sender, EventArgs e)
        {
            //if (!CheckSetting())
            // return;

            if (!SaveToRegistry())
                return;
            // else
            // {
            this.DialogResult = DialogResult.OK;
            Close();
            Dispose();
           // }
        
        }
        /// <summary>
        /// Save all setups into CPU registry
        /// </summary>
        private bool SaveToRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey key1 = key.OpenSubKey("SVG\\LightControl", true);
            //if (key1!=null)
            //    key.DeleteSubKeyTree("SVG\\LightControl");

            RegistryKey Key2 = key.CreateSubKey("SVG\\LightControl");
            
            
            int intCount = dgd_VisionList.Rows.Count ;

            string[] strComPortList = Key2.GetSubKeyNames();


            

            for (int x = 0; x < intCount; x++)
            {
                if (dgd_VisionList.Rows[x].Cells[1].Value == null || dgd_VisionList.Rows[x].Cells[1].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose vision application ID for existing camera(s) first");
                    return false; //errorCount++;
                }

                if (dgd_VisionList.Rows[x].Cells[2].Value == null || dgd_VisionList.Rows[x].Cells[2].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose correct light source type");
                    return false; //errorCount++;
                }

                if (dgd_VisionList.Rows[x].Cells[3].Value == null || Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) < 0)
                {
                    SRMMessageBox.Show("Please assign valid value for light source channel");
                    return false; // errorCount++;
                }

                if (dgd_VisionList.Rows[x].Cells[0].Value == null || dgd_VisionList.Rows[x].Cells[0].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose correct serial port for this light source channel");
                    return false; //errorCount++;
                }



                if (rowString[x] != (dgd_VisionList.Rows[x].Cells[0].Value.ToString() + dgd_VisionList.Rows[x].Cells[1].Value.ToString() + dgd_VisionList.Rows[x].Cells[2].Value.ToString() + dgd_VisionList.Rows[x].Cells[3].Value.ToString()))
                {
                    int intRowCount = dgd_VisionList.Rows.Count;
                    for (int i = 0; i < intRowCount; i++)
                    {
                        if (x != i && dgd_VisionList.Rows[x].Cells[0].Value.ToString() == dgd_VisionList.Rows[i].Cells[0].Value.ToString()
                            && dgd_VisionList.Rows[x].Cells[1].Value.ToString() == dgd_VisionList.Rows[i].Cells[1].Value.ToString()
                            && dgd_VisionList.Rows[x].Cells[2].Value.ToString() == dgd_VisionList.Rows[i].Cells[2].Value.ToString()
                            )
                        {
                            SRMMessageBox.Show("Cannot have same type of lighting in " + dgd_VisionList.Rows[x].Cells[1].Value.ToString() + " in " + dgd_VisionList.Rows[x].Cells[0].Value.ToString(), "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // repeatNameCount++;
                            return false;
                        }

                        //if (x != i && dgd_VisionList.Rows[x].Cells[0].Value.ToString() == dgd_VisionList.Rows[i].Cells[0].Value.ToString()
                        //    && Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value).ToString() == Convert.ToInt32(dgd_VisionList.Rows[i].Cells[3].Value).ToString())
                        //{
                        //    SRMMessageBox.Show("This Light Channel ID is used by others already", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    // repeatChannelCount++;
                        //    return false;
                        //}

                       
                    }
                    bool Changed = false;
                    bool COMChanged = false;
                    bool VisionNumChanged = false;
                    bool VisionNameChanged = false;
                    bool COMNUMChanged = false;
                    bool COMNAMEChanged = false;
                    bool NUMNAMEChanged = false;
                    if (rowCOM[x] != dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                        rowVisionNum[x] != dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                        rowVisionName[x] != dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                        
                    {
                        removeCOM.Add(rowCOM[x]);
                        removeVisionNum.Add(rowVisionNum[x]);
                        removeVisionName.Add(rowVisionName[x] + dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        removeNameNum.Add(dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                       // Changed = true;
                    }

                    if (rowCOM[x] == dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                         rowVisionNum[x] == dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                         rowVisionName[x] != dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                    {
                        removeCOM.Add(rowCOM[x]);
                        removeVisionNum.Add(rowVisionNum[x]);
                        removeVisionName.Add(rowVisionName[x] + dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        removeNameNum.Add(dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        VisionNameChanged = true;
                    }

                     if (rowCOM[x] == dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                        rowVisionNum[x] != dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                        rowVisionName[x] == dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                    {
                       
                        VisionNumChanged = true;
                    }
                    if (rowCOM[x] != dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                      rowVisionNum[x] == dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                      rowVisionName[x] == dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                    {
                        removeCOM.Add(rowCOM[x]);
                        removeVisionNum.Add(rowVisionNum[x]);
                        removeVisionName.Add(rowVisionName[x] + dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        removeNameNum.Add(dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        COMChanged = true;
                    }
                    if (rowCOM[x] != dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                      rowVisionNum[x] != dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                      rowVisionName[x] == dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                    {
                        removeCOM.Add(rowCOM[x]);
                        removeVisionNum.Add(rowVisionNum[x]);
                        removeVisionName.Add(rowVisionName[x] + dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        removeNameNum.Add(dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        COMNUMChanged = true;
                    }
                    if (rowCOM[x] != dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                      rowVisionNum[x] == dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                      rowVisionName[x] != dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                    {
                        removeCOM.Add(rowCOM[x]);
                        removeVisionNum.Add(rowVisionNum[x]);
                        removeVisionName.Add(rowVisionName[x] + dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        removeNameNum.Add(dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        COMNAMEChanged = true;
                    }
                    if (rowCOM[x] == dgd_VisionList.Rows[x].Cells[0].Value.ToString() &&
                      rowVisionNum[x] != dgd_VisionList.Rows[x].Cells[1].Value.ToString() &&
                      rowVisionName[x] != dgd_VisionList.Rows[x].Cells[2].Value.ToString())
                    {
                        removeCOM.Add(rowCOM[x]);
                        removeVisionNum.Add(rowVisionNum[x]);
                        removeVisionName.Add(rowVisionName[x] + dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        removeNameNum.Add(dgd_VisionList.Rows[x].Cells[4].Value.ToString());
                        NUMNAMEChanged = true;
                    }
                    //Saving part
                    RegistryKey subkey2 = Key2.CreateSubKey(dgd_VisionList.Rows[x].Cells[0].Value.ToString()); // Save CommPort Value - Com1/Com2/Com3
                    RegistryKey subkey3 = subkey2.CreateSubKey(dgd_VisionList.Rows[x].Cells[1].Value.ToString());   // Save VisionName under CommPort
                                                                                                                    // if (dgd_VisionList.Rows[x].Cells[4].Value != null)
                                                                                                                    //{
                    if (VisionNameChanged && removeNameNum.Count > 0)
                    {

                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + removeNameNum,
                    Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                    else if (VisionNumChanged)
                    {
                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + dgd_VisionList.Rows[x].Cells[4].Value.ToString(),
                            Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                    else if (COMChanged)
                    {
                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + subkey3.ValueCount.ToString(),//dgd_VisionList.Rows[x].Cells[4].Value.ToString(),
                            Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                    else if (COMNUMChanged)
                    {
                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + subkey3.ValueCount.ToString(),//dgd_VisionList.Rows[x].Cells[4].Value.ToString(),
                            Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                    else if (COMNAMEChanged)
                    {
                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + subkey3.ValueCount.ToString(),//dgd_VisionList.Rows[x].Cells[4].Value.ToString(),
                            Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                    else if (NUMNAMEChanged)
                    {
                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + subkey3.ValueCount.ToString(),//dgd_VisionList.Rows[x].Cells[4].Value.ToString(),
                            Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                    else 
                    {
                        subkey3.SetValue(dgd_VisionList.Rows[x].Cells[2].Value.ToString() + subkey3.ValueCount.ToString(),//dgd_VisionList.Rows[x].Cells[4].Value.ToString(),
                            Convert.ToInt32(dgd_VisionList.Rows[x].Cells[3].Value) - 1); // Strobe Intensity for each Light Source
                    }
                }
                
            }
            // Need Delete removed row in registry
            if (removeCOM.Count > 0)
            {
                for (int a = 0; a < removeCOM.Count; a++)
                {
                    Key2 = key.OpenSubKey("SVG\\LightControl", true);
                    RegistryKey COMkey = Key2.OpenSubKey(removeCOM[a], true); // Save CommPort Value - Com1/Com2/Com3
                    RegistryKey VisionKey = COMkey.OpenSubKey(removeVisionNum[a], true);   // Save VisionName under CommPort 
                    string[] abc = VisionKey.GetValueNames() ;
                    if (abc.Length>0)
                    VisionKey.DeleteValue(removeVisionName[a], true);
                    
                }

            }

            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            subKey.SetValue("Registration", 1);

            return true;
        }

        private void dgd_VisionList_CellClick(object sender, EventArgs e)
        {
            //TEMPremoveCOM.Clear();
            //    TEMPremoveVisionNum.Clear();
            //    TEMPremoveVisionName.Clear();
            //int selectedIndex = dgd_VisionList.CurrentCell.RowIndex;
            //TEMPremoveCOM.Add(dgd_VisionList.Rows[selectedIndex].Cells[0].Value.ToString());
            //TEMPremoveVisionNum.Add(dgd_VisionList.Rows[selectedIndex].Cells[1].Value.ToString());
            //TEMPremoveVisionName.Add(dgd_VisionList.Rows[selectedIndex].Cells[2].Value.ToString() + dgd_VisionList.Rows[selectedIndex].Cells[4].Value.ToString());
        }
        private void dgd_VisionList_CellEndEdit(object sender, EventArgs e)
        {

        }
        private void dgd_VisionList_CellLeave(object sender, EventArgs e)
        {
            //int RowIndex = dgd_VisionList.CurrentCell.RowIndex;
            //int ColIndex = dgd_VisionList.CurrentCell.ColumnIndex;
            //if (TEMPremoveCOM[0] == dgd_VisionList.Rows[selectedIndex].Cells[0].Value.ToString()&&
            //    TEMPremoveVisionNum[0]== dgd_VisionList.Rows[selectedIndex].Cells[1].Value.ToString()&&
            //    TEMPremoveVisionName[0]== dgd_VisionList.Rows[selectedIndex].Cells[2].Value.ToString() + dgd_VisionList.Rows[selectedIndex].Cells[4].Value.ToString())
            //{
            //    TEMPremoveCOM.Add(dgd_VisionList.Rows[selectedIndex].Cells[0].Value.ToString());
            //    TEMPremoveVisionNum.Add(dgd_VisionList.Rows[selectedIndex].Cells[1].Value.ToString());
            //    TEMPremoveVisionName.Add(dgd_VisionList.Rows[selectedIndex].Cells[2].Value.ToString() + dgd_VisionList.Rows[selectedIndex].Cells[4].Value.ToString());
            //}
        }
        private void UpdateComboBox()
        {
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("Back Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("Coaxial Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("Square Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("Side Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("Ring Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("TopRing Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("CenterRing Light");
            ((DataGridViewComboBoxColumn)dc_Type).Items.Add("BottomRing Light");

            for (int i = 1; i < 9; i++)
            {
                ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add("Vision" + i.ToString());
            }

            for (int j = 1; j <= 8; j++)
            {
                ((DataGridViewComboBoxColumn)dc_LightChannel).Items.Add(j.ToString());
            }
            
            for (int i = 1; i < 10; i++)
            {
                bool blnFound = false;
                string[] arrName = SerialPort.GetPortNames();
                foreach (string strName in arrName)
                {
                    if (strName == "COM" + i)
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (blnFound)
                    ((DataGridViewComboBoxColumn)dc_PortName).Items.Add("COM" + i);
                else
                    ((DataGridViewComboBoxColumn)dc_PortName).Items.Add("*" + "COM" + i);
            }
        }


        
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }


        private void btn_Remove_Click(object sender, EventArgs e)
        {
            if (dgd_VisionList.Rows.Count > 0)
            {
                    int selectedIndex = dgd_VisionList.CurrentCell.RowIndex;
                if (selectedIndex > -1)
                {
                    if ( dgd_VisionList.Rows[selectedIndex].Cells[0].Value != null&& dgd_VisionList.Rows[selectedIndex].Cells[1].Value != null&& dgd_VisionList.Rows[selectedIndex].Cells[2].Value != null&& dgd_VisionList.Rows[selectedIndex].Cells[3].Value != null)
                    {
                        removeCOM.Add(dgd_VisionList.Rows[selectedIndex].Cells[0].Value.ToString());
                        removeVisionNum.Add(dgd_VisionList.Rows[selectedIndex].Cells[1].Value.ToString());
                        removeVisionName.Add(dgd_VisionList.Rows[selectedIndex].Cells[2].Value.ToString() + dgd_VisionList.Rows[selectedIndex].Cells[4].Value.ToString());
                    }
                    dgd_VisionList.Rows.RemoveAt(selectedIndex);
                    rowString.RemoveAt(selectedIndex);
                    rowVisionName.RemoveAt(selectedIndex);
                    rowVisionNum.RemoveAt(selectedIndex);
                    rowCOM.RemoveAt(selectedIndex);

                    //dgd_VisionList.Refresh(); 
                }
             
            }
            else
            {
                SRMMessageBox.Show("There is no row to be removed!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btn_Add_Click(object sender, EventArgs e)
        {
            int selectedIndex = dgd_VisionList.CurrentCell.RowIndex;
            if (selectedIndex > -1)
            {
                dgd_VisionList.Rows.Insert(selectedIndex+1, new DataGridViewRow());
                rowString.Insert(selectedIndex + 1,"");
                rowVisionName.Insert(selectedIndex + 1, "");
                rowVisionNum.Insert(selectedIndex + 1, "");
                rowCOM.Insert(selectedIndex + 1, "");
                if (selectedIndex > 1)
                    dgd_VisionList.FirstDisplayedScrollingRowIndex = selectedIndex - 1;
            }
        }

        /// <summary>
        /// Check whether 2 same Light Channel ID are selected
        /// For instance, if Channel 1 is used in first row, it shouldn't be appeared in next row.
        /// </summary>
        /// <param name="intRow">selected row in datagridview</param>
        private bool CheckLightChannelComboBox(int intRow)
        {

            int intRowCount = dgd_VisionList.Rows.Count - 1;
            for (int i = 0; i < intRowCount; i++)
            {
                if (i != intRow && Convert.ToInt32(dgd_VisionList.Rows[intRow].Cells[3].Value) == Convert.ToInt32(dgd_VisionList.Rows[i].Cells[3].Value) && dgd_VisionList.Rows[intRow].Cells[1].Value == dgd_VisionList.Rows[i].Cells[1].Value)
                {
                    SRMMessageBox.Show("This Light Channel ID is used by others already", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether all required important info is being filled in datagridview before continue
        /// For instance, vision name, application ID and at least 1 feature for it.
        /// </summary>
        /// <returns>false if info is not complete, true if everything is perfect</returns>
        private bool CheckSetting()
        {
            int intCount = dgd_VisionList.Rows.Count - 1;
            for (int i = 0; i < intCount; i++)
            {
                if (dgd_VisionList.Rows[i].Cells[1].Value == null || dgd_VisionList.Rows[i].Cells[1].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose vision application ID for existing camera(s) first");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[2].Value == null || dgd_VisionList.Rows[i].Cells[2].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose correct light source type");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[3].Value == null || Convert.ToInt32(dgd_VisionList.Rows[i].Cells[3].Value) < 0)
                {
                    SRMMessageBox.Show("Please assign valid value for light source channel");
                    return false;
                }

                if (dgd_VisionList.Rows[i].Cells[0].Value == null || dgd_VisionList.Rows[i].Cells[0].Value.Equals(""))
                {
                    SRMMessageBox.Show("Please choose correct serial port for this light source channel");
                    return false;
                }

                if (CheckLightChannelComboBox(i))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Get all registered vision and assign them to visionID comboBox
        /// </summary>
        private void GetAvailableVisions()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");

            string[] strSubkeyList = subKey.GetSubKeyNames();
            int intCount = strSubkeyList.Length;
            for (int i = 0; i < intCount; i++)
            {
                ((DataGridViewComboBoxColumn)dc_VisionID).Items.Add(strSubkeyList[i].ToString());
            }
        }



        /// <summary>
        /// Report error if there is any error on datagridview
        /// </summary>
        private void dgd_VisionList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            SRMMessageBox.Show("Error happened " + e.Context.ToString());

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
    }
}