using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Common
{
    public class NewUserRight
    {
        #region Members Variables

        //private DataSet m_DataSetChild1_TopMenu = new DataSet();

        private DBCall m_dbCall = new DBCall(@"access\setting.mdb");

        private DataSet m_dsChild_Orientation;
        private DataSet m_dsChild_MarkOrient;
        private DataSet m_dsChild_Pad;
        private DataSet m_dsChild_Lead3D;
        private DataSet m_dsChild_InPocket;
        private DataSet m_dsChild_InPocket2;
        private DataSet m_dsChild_Seal;
        private DataSet m_dsChild_Seal2;
        private DataSet m_dsChild_Barcode;
        private DataSet m_dsParent;
        private OleDbDataAdapter m_childDataAdapter_Orientation;
        private OleDbDataAdapter m_childDataAdapter_MarkOrient;
        private OleDbDataAdapter m_childDataAdapter_Pad;
        private OleDbDataAdapter m_childDataAdapter_Lead3D;
        private OleDbDataAdapter m_childDataAdapter_InPocket;
        private OleDbDataAdapter m_childDataAdapter_InPocket2;
        private OleDbDataAdapter m_childDataAdapter_Seal;
        private OleDbDataAdapter m_childDataAdapter_Seal2;
        private OleDbDataAdapter m_childDataAdapter_Barcode;
        private OleDbDataAdapter m_ParentDataAdapter;
        private OleDbParameter m_workParam;

        private DataSet DataSetChild1_BottomMenu;
        private DataSet DataSetChild1_Orientation;
        private DataSet DataSetChild1_MarkOrient;
        private DataSet DataSetChild1_Pad;
        private DataSet DataSetChild1_Lead3D;
        private DataSet DataSetChild1_InPocket;
        private DataSet DataSetChild1_InPocket2;
        private DataSet DataSetChild1_Seal;
        private DataSet DataSetChild1_Seal2;
        private DataSet DataSetChild1_Barcode;

        private DataSet DataSetChild2_BottomMenu;
        private DataSet DataSetChild2_TopMenu;
        private DataSet DataSetChild2_Orientation;
        private DataSet DataSetChild2_MarkOrient;
        private DataSet DataSetChild2_Pad;
        private DataSet DataSetChild2_Lead3D;
        private DataSet DataSetChild2_InPocket;
        private DataSet DataSetChild2_InPocket2;
        private DataSet DataSetChild2_Seal;
        private DataSet DataSetChild2_Seal2;
        private DataSet DataSetChild2_Barcode;

        private DataSet DataSetChild3_Orientation;
        private DataSet DataSetChild3_MarkOrient;
        private DataSet DataSetChild3_Pad;
        private DataSet DataSetChild3_Lead3D;
        private DataSet DataSetChild3_InPocket;
        private DataSet DataSetChild3_InPocket2;
        private DataSet DataSetChild3_Seal;
        private DataSet DataSetChild3_Seal2;
        private DataSet DataSetChild3_Barcode;

        #endregion

        public NewUserRight(bool blnInit)
        {
            if (blnInit)
            {
                GetDataSet();
                UpdateAccessFile();
                InitTable();
            }
            //m_dbCall.Select("SELECT * FROM Child1_TopMenu", m_DataSetChild1_TopMenu);
        }

        public void InitTable()
        {
            DataSetChild1_BottomMenu = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_BottomMenu ORDER BY Number", DataSetChild1_BottomMenu);

            DataSetChild1_Orientation = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_Orientation ORDER BY Number", DataSetChild1_Orientation);

            DataSetChild1_MarkOrient = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_MarkOrient ORDER BY Number", DataSetChild1_MarkOrient);

            DataSetChild1_Pad = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_Pad ORDER BY Number", DataSetChild1_Pad);

            DataSetChild1_Lead3D = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_Lead3D ORDER BY Number", DataSetChild1_Lead3D);

            DataSetChild1_InPocket = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_InPocket ORDER BY Number", DataSetChild1_InPocket);

            DataSetChild1_InPocket2 = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_InPocket2 ORDER BY Number", DataSetChild1_InPocket2);

            DataSetChild1_Seal = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_Seal ORDER BY Number", DataSetChild1_Seal);

            DataSetChild1_Seal2 = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_Seal2 ORDER BY Number", DataSetChild1_Seal2);

            DataSetChild1_Barcode = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_Barcode ORDER BY Number", DataSetChild1_Barcode);

            DataSetChild2_BottomMenu = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_BottomMenu", DataSetChild2_BottomMenu);

            DataSetChild2_TopMenu = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_TopMenu", DataSetChild2_TopMenu);

            DataSetChild2_Orientation = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_Orientation", DataSetChild2_Orientation);

            DataSetChild2_MarkOrient = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_MarkOrient", DataSetChild2_MarkOrient);

            DataSetChild2_Pad = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_Pad", DataSetChild2_Pad);

            DataSetChild2_Lead3D = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_Lead3D", DataSetChild2_Lead3D);

            DataSetChild2_InPocket = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_InPocket", DataSetChild2_InPocket);

            DataSetChild2_InPocket2 = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_InPocket2", DataSetChild2_InPocket2);

            DataSetChild2_Seal = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_Seal", DataSetChild2_Seal);

            DataSetChild2_Seal2 = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_Seal2", DataSetChild2_Seal2);

            DataSetChild2_Barcode = new DataSet();
            m_dbCall.Select("SELECT * FROM Child2_Barcode", DataSetChild2_Barcode);

            DataSetChild3_Orientation = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_Orientation", DataSetChild3_Orientation);

            DataSetChild3_MarkOrient = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_MarkOrient", DataSetChild3_MarkOrient);

            DataSetChild3_Pad = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_Pad", DataSetChild3_Pad);

            DataSetChild3_Lead3D = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_Lead3D", DataSetChild3_Lead3D);

            DataSetChild3_InPocket = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_InPocket", DataSetChild3_InPocket);

            DataSetChild3_InPocket2 = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_InPocket2", DataSetChild3_InPocket2);

            DataSetChild3_Seal = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_Seal", DataSetChild3_Seal);

            DataSetChild3_Seal2 = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_Seal2", DataSetChild3_Seal2);

            DataSetChild3_Barcode = new DataSet();
            m_dbCall.Select("SELECT * FROM Child3_Barcode", DataSetChild3_Barcode);
        }

        private void UpdateAccessFile()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            //string[] strVisionFeatureList;
            RegistryKey subKey1;

            for (int i = 0; i < strVisionList.Length; i++)//6
            {
                string filter = "";
                DataRow[] ParentList;
                DataRow Parent;
                
                filter = "[Number] = '" + (i + 3).ToString() + "'";
                ParentList = m_dsParent.Tables["Parent"].Select(filter, "Number");
                Parent = ParentList[0];
                Parent["Name"] = " ";// "Vision " + (i + 1).ToString();
                Parent["Chi Name"] = " ";
                Parent["Number"] = i + 3;
                Parent["Child1"] = " ";

                try
                {
                    m_ParentDataAdapter.Update(m_dsParent, "Parent");
                    //m_childDataAdapter_Orientation.Update(m_dsChild_Orientation, "Child1_Orientation");
                    //m_childDataAdapter_MarkOrient.Update(m_dsChild_MarkOrient, "Child1_MarkOrient");
                    //m_childDataAdapter_Pad.Update(m_dsChild_Pad, "Child1_Pad");
                    //m_childDataAdapter_InPocket.Update(m_dsChild_InPocket, "Child1_InPocket");
                    //m_childDataAdapter_Seal.Update(m_dsChild_Seal, "Child1_Seal");

                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


                //List<string> arrVisionName = new List<string>();
            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i]);
                //int intVisionNo = Convert.ToInt32(strVisionList[i].Substring(6)) - 1;

                //arrVisionName.Add(subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString());

                string filter = "";
                DataRow[] ParentList;
                DataRow Parent;

                DataRow[] ChildList;
                DataRow Child;
                
                filter = "[Number] = '" + (i + 3).ToString() + "'";
                ParentList = m_dsParent.Tables["Parent"].Select(filter, "Number");
                Parent = ParentList[0];
                string strVisionName = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString() + subKey1.GetValue("VisionNameNo", "").ToString();
                Parent["Name"] = strVisionName;
                Parent["Chi Name"] = strVisionName;
                Parent["ChiT Name"] = strVisionName;
                Parent["Number"] = i + 3;
                Parent["Child1"] = " ";

                switch (strVisionName)
                {
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                    case "Orient":
                    case "BottomOrient":
                        Parent["Child1"] = "Child1_Orientation";
                        for (int j = 0; j < m_dsChild_Orientation.Tables["Child1_Orientation"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_Orientation.Tables["Child1_Orientation"].Select(filter);
                            Child = ChildList[0];
                            //Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            //Child["Parent Number"] = i + 3;
                            //Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            string strName = Child["Name"].ToString();
                            if (strName == "System" || strName == "Tolerance" || strName == "Option" || strName == "Test")
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Orient" && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Orient") && !subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pad"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if ((strName == "OPad" || strName == "Tol.Pad") && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Orient") && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pad"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Package" && (subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Package") || subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pkg") || subKey1.GetValue("Package", 0).ToString() == "1"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else
                            {
                                Child["Parent"] = "Vision " + i.ToString();
                                Child["Parent Number"] = "-1";
                                Child["Chi Parent"] = "Vision " + i.ToString();
                            }
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_Orientation.Update(m_dsChild_Orientation, "Child1_Orientation");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                       
                        break;
                    case "Mark":
                    case "MarkOrient":
                    case "MOLi":
                    case "Package":
                    case "MarkPkg":
                    case "MOPkg":
                    case "MOLiPkg":
                        Parent["Child1"] = "Child1_MarkOrient";
                        for (int j = 0; j < m_dsChild_MarkOrient.Tables["Child1_MarkOrient"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_MarkOrient.Tables["Child1_MarkOrient"].Select(filter);
                            Child = ChildList[0];
                            string strName = Child["Name"].ToString();
                            if (strName == "System" || strName == "Mark" || strName == "Tolerance" || strName == "Option" || strName == "Test")
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            //else if (strName == "Orient" && (subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Orient") || subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Ort")))
                            //{
                            //    Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            //    Child["Parent Number"] = i + 3;
                            //}
                            else if (strName == "Package" && (subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Package") || subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pkg")))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Lead" && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Li"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else
                            {
                                Child["Parent"] = "Vision " + i.ToString();
                                Child["Parent Number"] = "-1";
                                Child["Chi Parent"] = "Vision " + i.ToString();
                            }
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_MarkOrient.Update(m_dsChild_MarkOrient, "Child1_MarkOrient");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        
                        break;
                    case "IPMLi":
                    case "IPMLiPkg":
                    case "InPocket":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                        Parent["Child1"] = "Child1_InPocket";
                        for (int j = 0; j < m_dsChild_InPocket.Tables["Child1_InPocket"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_InPocket.Tables["Child1_InPocket"].Select(filter);
                            Child = ChildList[0];
                            string strName = Child["Name"].ToString();
                            if (strName == "System" || strName == "Mark" || strName == "General" || strName == "Pocket Position" || strName == "Tolerance" || strName == "Option" || strName == "Test" || strName == "ByPass")
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Package" && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pkg"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Lead" && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Li"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else
                            {
                                Child["Parent"] = "Vision " + i.ToString();
                                Child["Parent Number"] = "-1";
                                Child["Chi Parent"] = "Vision " + i.ToString();
                            }
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_InPocket.Update(m_dsChild_InPocket, "Child1_InPocket");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                       
                        break;
                    case "IPMLi2":
                    case "IPMLiPkg2":
                    case "InPocket2":
                    case "InPocketPkg2":
                    case "InPocketPkgPos2":
                        Parent["Child1"] = "Child1_InPocket2";
                        for (int j = 0; j < m_dsChild_InPocket2.Tables["Child1_InPocket2"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_InPocket2.Tables["Child1_InPocket2"].Select(filter);
                            Child = ChildList[0];
                            string strName = Child["Name"].ToString();
                            if (strName == "System" || strName == "Mark" || strName == "General" || strName == "Pocket Position" || strName == "Tolerance" || strName == "Option" || strName == "Test" || strName == "ByPass")
                            {
                                Child["Parent"] = strVisionName;
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = strVisionName;
                            }
                            else if (strName == "Package" && strVisionName.Contains("Pkg"))
                            {
                                Child["Parent"] = strVisionName;
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = strVisionName;
                            }
                            else if (strName == "Lead" && strVisionName.Contains("Li"))
                            {
                                Child["Parent"] = strVisionName;
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = strVisionName;
                            }
                            else
                            {
                                Child["Parent"] = "Vision " + i.ToString();
                                Child["Parent Number"] = "-1";
                                Child["Chi Parent"] = "Vision " + i.ToString();
                            }
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_InPocket2.Update(m_dsChild_InPocket2, "Child1_InPocket2");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        break;
                    //case "BottomPosition":
                    //case "BottomPositionOrient":
                    //    break;
                    //case "TapePocketPosition":
                    //    break; 
                    //case "BottomOrientPad":
                    case "Pad":
                    case "PadPos":
                    case "PadPkg":
                    case "PadPkgPos":
                    case "Pad5S":
                    case "Pad5SPos":
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        Parent["Child1"] = "Child1_Pad";
                        for (int j = 0; j < m_dsChild_Pad.Tables["Child1_Pad"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_Pad.Tables["Child1_Pad"].Select(filter);
                            Child = ChildList[0];
                            string strName = Child["Name"].ToString();
                            if (strName == "System" || strName == "Pad" || strName == "Tolerance" || strName == "Option" || strName == "Test" || strName == "Tol.Pad")
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Package" && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pkg"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else
                            {
                                Child["Parent"] = "Vision " + i.ToString();
                                Child["Parent Number"] = "-1";
                                Child["Chi Parent"] = "Vision " + i.ToString();
                            }
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_Pad.Update(m_dsChild_Pad, "Child1_Pad");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                       
                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        Parent["Child1"] = "Child1_Lead3D";
                        for (int j = 0; j < m_dsChild_Lead3D.Tables["Child1_Lead3D"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_Lead3D.Tables["Child1_Lead3D"].Select(filter);
                            Child = ChildList[0];
                            string strName = Child["Name"].ToString();
                            if (strName == "System" || strName == "Lead3D" || strName == "Tolerance" || strName == "Option" || strName == "Test" || strName == "Tol.Lead3D")
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else if (strName == "Package" && subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString().Contains("Pkg"))
                            {
                                Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                                Child["Parent Number"] = i + 3;
                                Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            }
                            else
                            {
                                Child["Parent"] = "Vision " + i.ToString();
                                Child["Parent Number"] = "-1";
                                Child["Chi Parent"] = "Vision " + i.ToString();
                            }
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_Lead3D.Update(m_dsChild_Lead3D, "Child1_Lead3D");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        break;
                    case "Seal":
                        Parent["Child1"] = "Child1_Seal";
                        for (int j = 0; j < m_dsChild_Seal.Tables["Child1_Seal"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_Seal.Tables["Child1_Seal"].Select(filter);
                            Child = ChildList[0];
                            Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            Child["Parent Number"] = i + 3;
                            Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_Seal.Update(m_dsChild_Seal, "Child1_Seal");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                       
                        break;
                    case "Seal2":
                        Parent["Child1"] = "Child1_Seal2";
                        for (int j = 0; j < m_dsChild_Seal2.Tables["Child1_Seal2"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_Seal2.Tables["Child1_Seal2"].Select(filter);
                            Child = ChildList[0];
                            Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            Child["Parent Number"] = i + 3;
                            Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_Seal2.Update(m_dsChild_Seal2, "Child1_Seal2");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        break;
                    case "Barcode":
                        Parent["Child1"] = "Child1_Barcode";
                        for (int j = 0; j < m_dsChild_Barcode.Tables["Child1_Barcode"].Rows.Count; j++)
                        {
                            filter = "[Number] = '" + (j + 1).ToString() + "'";
                            ChildList = m_dsChild_Barcode.Tables["Child1_Barcode"].Select(filter);
                            Child = ChildList[0];
                            Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            Child["Parent Number"] = i + 3;
                            Child["Chi Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                            Child["Number"] = j + 1;
                            try
                            {
                                m_childDataAdapter_Barcode.Update(m_dsChild_Barcode, "Child1_Barcode");
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        break;
                    default:
                        //filter = "[Child2] = ''";
                        //ChildList = m_dsChild.Tables["Child1"].Select(filter);
                        //Child = ChildList[0];
                        //Child["Parent"] = subKey1.GetValue("VisionName", "Vision " + (i + 1)).ToString();
                        //Child["Parent Number"] = i + 2;
                        //Child["Child2"] = "";
                        break;
                }

                try
                {
                    m_ParentDataAdapter.Update(m_dsParent, "Parent");
                    //m_childDataAdapter_Orientation.Update(m_dsChild_Orientation, "Child1_Orientation");
                    //m_childDataAdapter_MarkOrient.Update(m_dsChild_MarkOrient, "Child1_MarkOrient");
                    //m_childDataAdapter_Pad.Update(m_dsChild_Pad, "Child1_Pad");
                    //m_childDataAdapter_InPocket.Update(m_dsChild_InPocket, "Child1_InPocket");
                    //m_childDataAdapter_Seal.Update(m_dsChild_Seal, "Child1_Seal");

                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
           

        }

        private void GetDataSet()
        {
            string sqlUpdate = "";
            OleDbCommand accessCommand;
            OleDbConnection accessConn;

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + AppDomain.CurrentDomain.BaseDirectory + "access\\setting.mdb";
            accessConn.Open();

            m_dsParent = new DataSet();
            m_dsParent.Tables.Add("Parent");
            m_dsChild_Orientation = new DataSet();
            m_dsChild_Orientation.Tables.Add("Child1_Orientation");
            m_dsChild_MarkOrient = new DataSet();
            m_dsChild_MarkOrient.Tables.Add("Child1_MarkOrient");
            m_dsChild_Pad = new DataSet();
            m_dsChild_Pad.Tables.Add("Child1_Pad");
            m_dsChild_Lead3D = new DataSet();
            m_dsChild_Lead3D.Tables.Add("Child1_Lead3D");
            m_dsChild_InPocket = new DataSet();
            m_dsChild_InPocket.Tables.Add("Child1_InPocket");
            m_dsChild_InPocket2 = new DataSet();
            m_dsChild_InPocket2.Tables.Add("Child1_InPocket2");
            m_dsChild_Seal = new DataSet();
            m_dsChild_Seal.Tables.Add("Child1_Seal");
            m_dsChild_Seal2 = new DataSet();
            m_dsChild_Seal2.Tables.Add("Child1_Seal2");
            m_dsChild_Barcode = new DataSet();
            m_dsChild_Barcode.Tables.Add("Child1_Barcode");

            accessCommand = new OleDbCommand("SELECT * FROM Parent", accessConn);
            m_ParentDataAdapter = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Orientation", accessConn);
            m_childDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_MarkOrient", accessConn);
            m_childDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Pad", accessConn);
            m_childDataAdapter_Pad = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Lead3D", accessConn);
            m_childDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket", accessConn);
            m_childDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket2", accessConn);
            m_childDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal", accessConn);
            m_childDataAdapter_Seal = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal2", accessConn);
            m_childDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Barcode", accessConn);
            m_childDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE [Parent] SET [Name] = @Name , [Chi Name] = @ChiName , [Child1] = @Child1 WHERE [Number] = @Number";
            m_ParentDataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;


            sqlUpdate = "UPDATE [Child1_Orientation] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
      
            sqlUpdate = "UPDATE [Child1_MarkOrient] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Pad] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Lead3D] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_InPocket] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_InPocket2] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Seal] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Seal2] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Barcode] SET [Parent] = @Parent , [Parent Number] = @ParentNumber , [Chi Parent] = @ChiParent WHERE [Number] = @Number";
            m_childDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ParentNumber", OleDbType.Integer));
            m_workParam.SourceColumn = "Parent Number";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Number", OleDbType.Integer));
            m_workParam.SourceColumn = "Number";
            m_workParam.SourceVersion = DataRowVersion.Current;

            try
            {
                m_ParentDataAdapter.Fill(m_dsParent, "Parent");
                m_childDataAdapter_Orientation.Fill(m_dsChild_Orientation, "Child1_Orientation");
                m_childDataAdapter_MarkOrient.Fill(m_dsChild_MarkOrient, "Child1_MarkOrient");
                m_childDataAdapter_Pad.Fill(m_dsChild_Pad, "Child1_Pad");
                m_childDataAdapter_Lead3D.Fill(m_dsChild_Lead3D, "Child1_Lead3D");
                m_childDataAdapter_InPocket.Fill(m_dsChild_InPocket, "Child1_InPocket");
                m_childDataAdapter_InPocket2.Fill(m_dsChild_InPocket2, "Child1_InPocket2");
                m_childDataAdapter_Seal.Fill(m_dsChild_Seal, "Child1_Seal");
                m_childDataAdapter_Seal2.Fill(m_dsChild_Seal2, "Child1_Seal2");
                m_childDataAdapter_Barcode.Fill(m_dsChild_Seal, "Child1_Barcode");
            }
            finally
            {
                accessConn.Close();
            }
            
        }

        public int[] GetTopMenuChild1GroupList()
        {
            int[] groupList = new int[1];
            DataSet DataSetChild1_TopMenu = new DataSet();
            m_dbCall.Select("SELECT * FROM Child1_TopMenu ORDER BY Number", DataSetChild1_TopMenu);
            int rowCount = DataSetChild1_TopMenu.Tables[0].Rows.Count;
            if (rowCount > 0)
            {
                groupList = new int[rowCount + 1];
                for (int i = 1; i < rowCount + 1; i++)
                    groupList[i] = Convert.ToInt32(DataSetChild1_TopMenu.Tables[0].Rows[i - 1]["Group"]);
            }

            return groupList;
        }

        public int GetBottomMenuChild1Group(string parent, string child1)
        {
            int group = 1;
            //DataSet DataSetChild1_BottomMenu = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_BottomMenu ORDER BY Number", DataSetChild1_BottomMenu);
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = DataSetChild1_BottomMenu.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetOrientationChild1Group(string parent, string child1)
        {
            int group = 1;
            //DataSet DataSetChild1_Orientation = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_Orientation ORDER BY Number", DataSetChild1_Orientation);
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = DataSetChild1_Orientation.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetMarkOrientChild1Group(string parent, string child1)
        {
            int group = 1;
            //DataSet DataSetChild1_MarkOrient = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_MarkOrient ORDER BY Number", DataSetChild1_MarkOrient);
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = DataSetChild1_MarkOrient.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetPadChild1Group(string parent, string child1)
        {
            int group = 1;
            //DataSet DataSetChild1_Pad = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_Pad ORDER BY Number", DataSetChild1_Pad);
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = DataSetChild1_Pad.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetLead3DChild1Group(string parent, string child1)
        {
            int group = 1;
            
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = DataSetChild1_Lead3D.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetInPocketChild1Group(string parent, string child1, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild1_InPocket = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_InPocket ORDER BY Number", DataSetChild1_InPocket);
            try
            {
                if (intVisionNameNo == 2)
                    parent += intVisionNameNo.ToString();

                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild1_InPocket2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild1_InPocket.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetSealChild1Group(string parent, string child1, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild1_Seal = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_Seal ORDER BY Number", DataSetChild1_Seal);
            try
            {
                if (intVisionNameNo == 2)
                    parent += intVisionNameNo.ToString();

                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild1_Seal2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild1_Seal.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
        public int GetBarcodeChild1Group(string parent, string child1)
        {
            int group = 1;
            //DataSet DataSetChild1_Barcode = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child1_Barcode ORDER BY Number", DataSetChild1_Barcode);
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = DataSetChild1_Barcode.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
        public int GetBottomMenuChild2Group(string child1, string child2)
        {
            int group = 1;
            //DataSet DataSetChild2_BottomMenu = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_BottomMenu", DataSetChild2_BottomMenu);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_BottomMenu.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetTopMenuChild2Group(string child1, string child2)
        {
            int group = 1;
            //DataSet DataSetChild2_TopMenu = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_TopMenu", DataSetChild2_TopMenu);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_TopMenu.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetOrientationChild2Group(string child1, string child2)
        {
            int group = 1;
            //DataSet DataSetChild2_Orientation = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_Orientation", DataSetChild2_Orientation);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_Orientation.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetMarkOrientChild2Group(string child1, string child2)
        {
            int group = 1;
            //DataSet DataSetChild2_MarkOrient = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_MarkOrient", DataSetChild2_MarkOrient);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_MarkOrient.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetPadChild2Group(string child1, string child2)
        {
            int group = 1;
            //DataSet DataSetChild2_Pad = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_Pad", DataSetChild2_Pad);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_Pad.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetLead3DChild2Group(string child1, string child2)
        {
            int group = 1;
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_Lead3D.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetInPocketChild2Group(string child1, string child2, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild2_InPocket = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_InPocket", DataSetChild2_InPocket);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild2_InPocket2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild2_InPocket.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetSealChild2Group(string child1, string child2, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild2_Seal = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_Seal", DataSetChild2_Seal);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild2_Seal2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild2_Seal.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
        public int GetBarcodeChild2Group(string child1, string child2)
        {
            int group = 1;
            //DataSet DataSetChild2_Barcode = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child2_Barcode", DataSetChild2_Barcode);
            try
            {
                string filter = "[Child1] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = DataSetChild2_Barcode.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
        public int GetOrientationChild3Group(string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_Orientation = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Orientation", DataSetChild3_Orientation);
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Orientation.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetMarkOrientChild3Group(string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_MarkOrient = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_MarkOrient", DataSetChild3_MarkOrient);
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_MarkOrient.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetPadChild3Group(string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_Pad = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Pad", DataSetChild3_Pad);
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Pad.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetLead3DChild3Group(string child2, string child3)
        {
            int group = 1;
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Lead3D.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetInPocketChild3Group(string child2, string child3, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild3_InPocket = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_InPocket", DataSetChild3_InPocket);
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild3_InPocket2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild3_InPocket.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetSealChild3Group(string child2, string child3, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild3_Seal = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Seal", DataSetChild3_Seal);
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild3_Seal2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild3_Seal.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetBarcodeChild3Group(string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_Barcode = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Barcode", DataSetChild3_Barcode);
            try
            {
                string filter = "[Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Barcode.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetOrientationChild3Group(string child1, string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_Orientation = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Orientation", DataSetChild3_Orientation);
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Orientation.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetMarkOrientChild3Group(string child1, string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_MarkOrient = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_MarkOrient", DataSetChild3_MarkOrient);
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_MarkOrient.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetPadChild3Group(string child1, string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_Pad = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Pad", DataSetChild3_Pad);
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Pad.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetLead3DChild3Group(string child1, string child2, string child3)
        {
            int group = 1;
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Lead3D.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetInPocketChild3Group(string child1, string child2, string child3, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild3_InPocket = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_InPocket", DataSetChild3_InPocket);
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild3_InPocket2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild3_InPocket.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetSealChild3Group(string child1, string child2, string child3, int intVisionNameNo)
        {
            int group = 1;
            //DataSet DataSetChild3_Seal = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Seal", DataSetChild3_Seal);
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows;
                if (intVisionNameNo == 2)
                    rightRows = DataSetChild3_Seal2.Tables[0].Select(filter);
                else
                    rightRows = DataSetChild3_Seal.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
        public int GetBarcodeChild3Group(string child1, string child2, string child3)
        {
            int group = 1;
            //DataSet DataSetChild3_Barcode = new DataSet();
            //m_dbCall.Select("SELECT * FROM Child3_Barcode", DataSetChild3_Barcode);
            try
            {
                string filter = "[Child1] = '" + child1 + "' And [Child2] = '" + child2 + "' AND Name = '" + child3 + "'";
                DataRow[] rightRows = DataSetChild3_Barcode.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
    }
}
