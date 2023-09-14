using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using System.Threading;

namespace AutoMode
{
    public partial class RecipeForm : Form
    {
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private int m_intUserGroup;
        private SRMWaitingFormThread m_thWaitingFormThread;
        private VisionInfo[] m_smVSInfo;
        private List<string> m_arrExistingRecipe = new List<string>();

        public RecipeForm(int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, VisionInfo[] smVSInfo)
        {
            InitializeComponent();

            m_smVSInfo = smVSInfo;
            m_intUserGroup = intUserGroup;
            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            UpdateInfo();
            UpdateRecipeAvailable();
        }

        private void UpdateInfo()
        {
            if (m_smCustomizeInfo.g_blnConfigShowNetwork)
            {
                StartWaiting("Loading Recipe Page...");
                if (m_smCustomizeInfo.g_blnWantNetwork && NetworkTransfer.IsConnectionPass(m_smCustomizeInfo.g_strHostIP))
                {
                    if (Directory.Exists(m_smCustomizeInfo.g_strDeviceUploadDir))
                        radioBtn_NetworkRecipe.Checked = true;
                    else
                        radioBtn_LocalRecipe.Checked = true;
                }
                else
                    radioBtn_LocalRecipe.Checked = true;

                btn_CopyLocal.BringToFront();
                StopWaiting();
            }
            else
            {
                radioBtn_LocalRecipe.Checked = true;
                radioBtn_LocalRecipe.Visible = false;
                radioBtn_NetworkRecipe.Visible = false;
                lbl_Source.Visible = false;
                btn_CopyServer.Visible = false;
                btn_CopyLocal.SendToBack();
            }
        }

        /// <summary>
        /// Copy directory and all files inside the directory from source path to destination path
        /// </summary>
        /// <param name="strSource">source path</param>
        /// <param name="strDestination">destination path</param>
        public void CopyDirectory(string strSource, string strDestination)
        {
            if (strDestination[strDestination.Length - 1] != Path.DirectorySeparatorChar)
                strDestination += Path.DirectorySeparatorChar;
            if (!Directory.Exists(strDestination))
                Directory.CreateDirectory(strDestination);

            String[] strFiles = Directory.GetFileSystemEntries(strSource);
            foreach (string strElement in strFiles)
            {
                // Sub directories
                if (Directory.Exists(strElement))
                    CopyDirectory(strElement, strDestination + Path.GetFileName(strElement));
                // Files in directory
                else
                    File.Copy(strElement, strDestination + Path.GetFileName(strElement), true);
            }
        }
       
        /// <summary>
        /// Display all the available recipe, select and display current recipe and customize GUI based on user access level
        /// </summary>
        private void UpdateRecipeAvailable()
        {
            string strTargetDirectory;
            if (radioBtn_LocalRecipe.Checked)
            {
                lbl_Source.Text = "Local";
                strTargetDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo";
            }
            else
            {
                lbl_Source.Text = "Server";
                strTargetDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo";
            }

            string[] strDirectoriesList = Directory.GetDirectories(strTargetDirectory);
            int intDirectoryNameLength = strTargetDirectory.Length + 1;

            lst_RecipeAvailable.Items.Clear();
            m_arrExistingRecipe = new List<string>();
            if (strDirectoriesList.Length != 0)
            {
                string strRecipeID = "";
                foreach (string strDirectoryName in strDirectoriesList)
                {
                    strRecipeID = strDirectoryName.Remove(0, intDirectoryNameLength);
                    lst_RecipeAvailable.Items.Add(strRecipeID);
                    m_arrExistingRecipe.Add(strRecipeID);
                }

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                string strSelectedRecipe = (string)subkey.GetValue("SelectedRecipeID", "Default");
                int n = lst_RecipeAvailable.FindStringExact(strSelectedRecipe);
                if (n != -1)
                {
                    lst_RecipeAvailable.SetSelected(n, true);
                    txt_RecipeSelected.Text = strSelectedRecipe;
                    lst_RecipeAvailable.SelectedIndex = n;
                }
                DisableField2(false);
            }
            else
                DisableField2(true);
        }

        /// <summary>
        /// Customize user access level
        /// </summary>
        /// <param name="blnDisable">true = disable select, delete, copy, rename button, enable select, delete, copy, rename button</param>
        private void DisableField(bool blnDisable)
        {
            if (blnDisable)
            {
                btn_Select.Enabled = false;
                btn_Delete.Enabled = false;
                btn_Copy.Enabled = false;
                btn_CopyLocal.Enabled = false;
                btn_CopyServer.Enabled = false;
                btn_Rename.Enabled = false;               
            }
            else
            {
                UserRight objUserRight = new UserRight();
                string strChild1 = "DeviceNo";
                string strChild2 = "";

                strChild2 = "New Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_New.Enabled = false;
                }
                strChild2 = "Select Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_Select.Enabled = false;
                }
                strChild2 = "Delete Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_Delete.Enabled = false;
                }
                strChild2 = "Copy Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_Copy.Enabled = false;
                    btn_CopyLocal.Enabled = false;
                    btn_CopyServer.Enabled = false;
                }
                strChild2 = "Rename Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_Rename.Enabled = false;
                }
                strChild2 = "Import Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_Import.Enabled = false;
                }
                strChild2 = "Export Device No";
                if (m_intUserGroup > objUserRight.GetGroupLevel2(strChild1, strChild2))
                {
                    btn_Export.Enabled = false;
                }

            }
        }

        private void DisableField2(bool blnDisable)
        {
            if (blnDisable)
            {
                btn_Select.Enabled = false;
                btn_Delete.Enabled = false;
                btn_Copy.Enabled = false;
                btn_CopyToExisting.Enabled = false;
                btn_CopyLocal.Enabled = false;
                btn_CopyServer.Enabled = false;
                btn_Rename.Enabled = false;
            }
            else
            {
                //NewUserRight objUserRight = new NewUserRight(false);
                string strChild1 = "DeviceNo";
                string strChild2 = "";

                strChild2 = "New Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_New.Enabled = false;
                }

                strChild2 = "Select Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_Select.Enabled = false;
                }

                strChild2 = "Delete Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_Delete.Enabled = false;
                }

                strChild2 = "Copy Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_Copy.Enabled = false;
                    btn_CopyLocal.Enabled = false;
                }

                strChild2 = "Copy To Existing Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_CopyToExisting.Enabled = false;
                }

                strChild2 = "Copy Device No to Server";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_CopyServer.Enabled = false;
                }

                strChild2 = "Rename Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_Rename.Enabled = false;
                }

                strChild2 = "Import Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_Import.Enabled = false;
                }

                strChild2 = "Export Device No";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetTopMenuChild2Group(strChild1, strChild2))
                {
                    btn_Export.Enabled = false;
                }
            }
        }

        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            this.Enabled = false;
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Enabled = true;
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            //
            //XmlParser FileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            //FileHandle.WriteSectionElement("Network");
            //if (radioBtn_LocalRecipe.Checked)
            //    FileHandle.WriteElement1Value("WantNetwork", false);
            //else
            //    FileHandle.WriteElement1Value("WantNetwork", true);
            //FileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing("Network", AppDomain.CurrentDomain.BaseDirectory, "Option.xml");
            //STDeviceEdit.CopySettingFile(AppDomain.CurrentDomain.BaseDirectory, "Option.xml");

            Close();
            Dispose();
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            if (lst_RecipeAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Device No. To Copy", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                RecipeCopyForm objCopyForm = new RecipeCopyForm(lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                if (objCopyForm.ShowDialog() == DialogResult.OK)
                {
                    string strDirectory;
                    if (radioBtn_LocalRecipe.Checked)
                        strDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
                    else
                        strDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\";

                    StartWaiting("Copying Recipe to local...");
                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo);

                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                    {
                        string strPath = "D:\\PreTest Image\\Recipe\\";
                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text))
                        {
                            CopyDirectory(strPath + lst_RecipeAvailable.Text, strPath + objCopyForm.ref_strRecipeCopyTo);
                        }
                    }

                    //only add to the list if currently selected is local
                    if (radioBtn_LocalRecipe.Checked)
                    {
                        lst_RecipeAvailable.Items.Add(objCopyForm.ref_strRecipeCopyTo);
                        m_arrExistingRecipe.Add(objCopyForm.ref_strRecipeCopyTo);
                    }
                    

                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Recipe", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                    
                    StopWaiting();
                }
                objCopyForm.Dispose();
            }
        }

        private void btn_CopyServer_Click(object sender, EventArgs e)
        {
            if (lst_RecipeAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Device No. To Copy", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                RecipeCopyForm objCopyForm = new RecipeCopyForm(lst_RecipeAvailable.Text, m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\");
                if (objCopyForm.ShowDialog() == DialogResult.OK)
                {
                    string strDirectory;
                    if (radioBtn_LocalRecipe.Checked)
                        strDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
                    else
                        strDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\";

                    StartWaiting("Copying Recipe to server...");
                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text,
                        m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo);

                    //only add to the list if currently selected is network
                    if (radioBtn_NetworkRecipe.Checked)
                    {
                        lst_RecipeAvailable.Items.Add(objCopyForm.ref_strRecipeCopyTo);
                        m_arrExistingRecipe.Add(objCopyForm.ref_strRecipeCopyTo);
                    }
                    

                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Recipe", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                    
                    StopWaiting();
                }
                objCopyForm.Dispose();
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (lst_RecipeAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Device No To Delete", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (lst_RecipeAvailable.SelectedItem.Equals(txt_RecipeSelected.Text))
            {
                 SRMMessageBox.Show("Cannot Modify Current Selected Recipe", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            if (SRMMessageBox.Show("Are you sure you want to delete this recipe?", "Recipe Form", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string strDirectory;
                if (radioBtn_LocalRecipe.Checked)
                    strDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
                else
                    strDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\";

                try
                {
                    try
                    {
                        if (Directory.Exists(strDirectory + lst_RecipeAvailable.Text))
                        {
                            Directory.Delete(strDirectory + lst_RecipeAvailable.Text, true);

                            
                           
                         
                                STDeviceEdit.SaveDeviceEditLog("Recipe", "Delete Recipe", lst_RecipeAvailable.Text, "", m_smProductionInfo.g_strLotID);
                            
                        }

                        //2021-08-15 ZJYEOH : Delete the folder for PreTest Image
                        if (m_smProductionInfo.g_blnWantRecipeVerification)
                        {
                            string strPath = "D:\\PreTest Image\\Recipe\\";
                            if (Directory.Exists(strPath + lst_RecipeAvailable.Text))
                            {
                                Directory.Delete(strPath + lst_RecipeAvailable.Text, true);
                            }
                        }

                        lst_RecipeAvailable.Items.Remove(lst_RecipeAvailable.Text);
                        m_arrExistingRecipe.Remove(lst_RecipeAvailable.Text);
                    }
                    catch
                    {
                        Directory.Move(strDirectory + lst_RecipeAvailable.Text,
                              strDirectory + lst_RecipeAvailable.Text + "1");
                        Directory.Delete(strDirectory + lst_RecipeAvailable.Text + "1", true);
                        
                        //2021-08-15 ZJYEOH : Delete the folder for PreTest Image
                        if (m_smProductionInfo.g_blnWantRecipeVerification)
                        {
                            string strPath = "D:\\PreTest Image\\Recipe\\";
                            if (Directory.Exists(strPath + lst_RecipeAvailable.Text))
                            {
                                Directory.Move(strPath + lst_RecipeAvailable.Text,
                                 strPath + lst_RecipeAvailable.Text + "1");
                                Directory.Delete(strPath + lst_RecipeAvailable.Text + "1", true);
                            }
                        }
                        
                        STDeviceEdit.SaveDeviceEditLog("Recipe", "Delete Recipe", lst_RecipeAvailable.Text + "1", "", m_smProductionInfo.g_strLotID);
                        
                        lst_RecipeAvailable.Items.Remove(lst_RecipeAvailable.Text);
                        m_arrExistingRecipe.Remove(lst_RecipeAvailable.Text);


                    }
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Delete Recipe " + lst_RecipeAvailable.Text + " is fail because file may be being openned. Please try again later.");
                }

            }
        }
        
        private void btn_New_Click(object sender, EventArgs e)
        {
            string strNewRecipe;

            strNewRecipe = txt_NewRecipe.Text;
            if (strNewRecipe == "")
            {
                SRMMessageBox.Show("Please key in new recipe name!", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                if (lst_RecipeAvailable.FindStringExact(strNewRecipe) != -1)
                {
                    SRMMessageBox.Show("This Recipe already existed!", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (strNewRecipe.Contains("\\"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [\\]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains("/"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [/]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains(":"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [:]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains("*"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [*]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains("?"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [?]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains("\""))
            {
                SRMMessageBox.Show("Invalid Recipe containing [\"]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains("<"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [<]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains(">"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [>]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.Contains("|"))
            {
                SRMMessageBox.Show("Invalid Recipe containing [|]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string strDirectory;
            if (radioBtn_LocalRecipe.Checked)
                strDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
            else
                strDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\";

            string strDefaultFolder = strDirectory + "Default";
            string strNewFolder = strDirectory + strNewRecipe;

            CopyDirectory(strDefaultFolder, strNewFolder);

            lst_RecipeAvailable.Items.Add(strNewRecipe);
            m_arrExistingRecipe.Add(strNewRecipe);
            txt_NewRecipe.Text = "";


            
           

            
                STDeviceEdit.SaveDeviceEditLog("Recipe", "New Recipe", "", strNewRecipe, m_smProductionInfo.g_strLotID);
            
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            if (lst_RecipeAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Recipe ", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                if (txt_RecipeSelected.Text != lst_RecipeAvailable.Text)
                {
                    

                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Select Recipe", txt_RecipeSelected.Text, lst_RecipeAvailable.Text, m_smProductionInfo.g_strLotID);
                    

                    txt_RecipeSelected.Text = lst_RecipeAvailable.Text;

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                    subkey.SetValue("SelectedRecipeID", lst_RecipeAvailable.Text);
                    for (int i = 0; i < 10; i++)
                    {
                        subkey.SetValue("SingleRecipeID" + i.ToString(), lst_RecipeAvailable.Text);
                    }

                }
            }
        }

        private void btn_Rename_Click(object sender, EventArgs e)
        {
            if (lst_RecipeAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Device No. To Copy", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if(lst_RecipeAvailable.SelectedItem.Equals(txt_RecipeSelected.Text))
            {
                 SRMMessageBox.Show("Cannot Modify Current Selected Recipe", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                string strDirectory;
                if (radioBtn_LocalRecipe.Checked)
                    strDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
                else
                    strDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\";

                RecipeRenameForm objRenameFrom = new RecipeRenameForm(lst_RecipeAvailable.Text, strDirectory);
                if (objRenameFrom.ShowDialog() == DialogResult.OK)
                {
                    Directory.Move(strDirectory + lst_RecipeAvailable.Text,
                          strDirectory + objRenameFrom.ref_strRecipeRenameTo);

                    //2021-08-15 ZJYEOH : Rename the folder for PreTest Image
                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                    {
                        string strPath = "D:\\PreTest Image\\Recipe\\";
                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text))
                        {
                            Directory.Move(strPath + lst_RecipeAvailable.Text,
                          strPath + objRenameFrom.ref_strRecipeRenameTo);
                        }
                    }

                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Rename Recipe", lst_RecipeAvailable.Text, objRenameFrom.ref_strRecipeRenameTo, m_smProductionInfo.g_strLotID);

                    lst_RecipeAvailable.Items.Add(objRenameFrom.ref_strRecipeRenameTo);
                    lst_RecipeAvailable.Items.Remove(lst_RecipeAvailable.Text);
                    m_arrExistingRecipe.Add(objRenameFrom.ref_strRecipeRenameTo);
                    m_arrExistingRecipe.Remove(lst_RecipeAvailable.Text);

                }
                objRenameFrom.Dispose();
            }
        }

        private void radioBtn_Click(object sender, EventArgs e)
        {
            if (radioBtn_NetworkRecipe.Checked)
            {
                StartWaiting("Loading Recipe from Server...");
                if (NetworkTransfer.IsConnectionPass(m_smCustomizeInfo.g_strHostIP))
                {
                    if (!Directory.Exists(m_smCustomizeInfo.g_strDeviceUploadDir))
                    {
                        StopWaiting();
                        radioBtn_LocalRecipe.Checked = true;
                        SRMMessageBox.Show("Server recipe path does not exist. Fail to load recipe from server.",
                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    StopWaiting();
                    radioBtn_LocalRecipe.Checked = true;
                    SRMMessageBox.Show("Fail to connect to server. Please make sure network connection is available and Host IP address is correct.",
                    "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                StopWaiting();
            }

            UpdateRecipeAvailable();
        }

        private void btn_SaveAs_Click(object sender, EventArgs e)
        {
            RecipeExportForm objForm = new RecipeExportForm(m_smProductionInfo, "", lst_RecipeAvailable.SelectedItem.ToString());
            objForm.ShowDialog();
        }

        private void btn_LoadFrom_Click(object sender, EventArgs e)
        {
            RecipeImportForm objForm = new RecipeImportForm(m_smProductionInfo, "", lst_RecipeAvailable.SelectedItem.ToString(), m_smVSInfo, txt_RecipeSelected.Text);
            objForm.ShowDialog();
        }

        private void btn_CopyToExisting_Click(object sender, EventArgs e)
        {
            if (lst_RecipeAvailable.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please Select A Device No. To Copy", "Device No Setting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                RecipeCopyToExistingForm objCopyForm = new RecipeCopyToExistingForm(lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\", m_arrExistingRecipe.ToArray(), m_smVSInfo);
                if (objCopyForm.ShowDialog() == DialogResult.OK)
                {
                    string strDirectory;
                    if (radioBtn_LocalRecipe.Checked)
                        strDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";
                    else
                        strDirectory = m_smCustomizeInfo.g_strDeviceUploadDir + "\\DeviceNo\\";

                    StartWaiting("Copying Recipe to Local...");

                    if (objCopyForm.ref_strRecipeCopyTo == "All Existing Recipe")
                    {
                        foreach (string recipe in m_arrExistingRecipe)
                        {
                            if (recipe != lst_RecipeAvailable.Text)
                            {
                                if (objCopyForm.ref_blnAllVision)
                                {
                                    if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                    {
                                        CopyAdvanceSettingToExistingRecipe_AllVision(strDirectory + lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe);
                                        STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe All Vision", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                    }
                                    else
                                    {
                                        if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe))
                                        {
                                            string[] strDir = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe);
                                            foreach (string dir in strDir)
                                            {
                                                Directory.Delete(dir, true);
                                            }
                                        }

                                        CopyDirectory(strDirectory + lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe);
                                        STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe All Vision", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                        //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                        if (m_smProductionInfo.g_blnWantRecipeVerification)
                                        {
                                            string strPath = "D:\\PreTest Image\\Recipe\\";
                                            if (Directory.Exists(strPath + lst_RecipeAvailable.Text))
                                            {
                                                if (Directory.Exists(strPath + recipe))
                                                {
                                                    string[] strDir = Directory.GetDirectories(strPath + recipe);
                                                    foreach (string dir in strDir)
                                                    {
                                                        Directory.Delete(dir, true);
                                                    }
                                                }

                                                CopyDirectory(strPath + lst_RecipeAvailable.Text, strPath + recipe);
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    if (objCopyForm.ref_blnVision1)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision1\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision1\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 1", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision1\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision1\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision1\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision1\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 1", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision1\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision1\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision1\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision1\\", strPath + recipe + "\\Vision1\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision2)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision2\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision2\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 2", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision2\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision2\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision2\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision2\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 2", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision2\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision2\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision2\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision2\\", strPath + recipe + "\\Vision2\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision3)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision3\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision3\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 3", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision3\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision3\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision3\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision3\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 3", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision3\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision3\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision3\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision3\\", strPath + recipe + "\\Vision3\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision4)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision4\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision4\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 4", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision4\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision4\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision4\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision4\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 4", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision4\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision4\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision4\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision4\\", strPath + recipe + "\\Vision4\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision5)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision5\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision5\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 5", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision5\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision5\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision5\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision5\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 5", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision5\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision5\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision5\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision5\\", strPath + recipe + "\\Vision5\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision6)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision6\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision6\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 6", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision6\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision6\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision6\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision6\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 6", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision6\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision6\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision6\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision6\\", strPath + recipe + "\\Vision6\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision7)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision7\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision7\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 7", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision7\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision7\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision7\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision7\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 7", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision7\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision7\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision7\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision7\\", strPath + recipe + "\\Vision7\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision8)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision8\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision8\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 8", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision8\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision8\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision8\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision8\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 8", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision8\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision8\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision8\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision8\\", strPath + recipe + "\\Vision8\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision9)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision9\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision9\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 9", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision9\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision9\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision9\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision9\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 9", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision9\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision9\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision9\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision9\\", strPath + recipe + "\\Vision9\\");
                                                }
                                            }

                                        }
                                    }

                                    if (objCopyForm.ref_blnVision10)
                                    {
                                        if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                        {
                                            CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision10\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision10\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 10", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);
                                        }
                                        else
                                        {
                                            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision10\\"))
                                                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision10\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision10\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + recipe + "\\Vision10\\");
                                            STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 10", lst_RecipeAvailable.Text, recipe, m_smProductionInfo.g_strLotID);

                                            //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                            if (m_smProductionInfo.g_blnWantRecipeVerification)
                                            {
                                                string strPath = "D:\\PreTest Image\\Recipe\\";
                                                if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision10\\"))
                                                {
                                                    if (Directory.Exists(strPath + recipe + "\\Vision10\\"))
                                                        Directory.Delete(strPath + recipe + "\\Vision10\\", true);

                                                    CopyDirectory(strPath + lst_RecipeAvailable.Text + "\\Vision10\\", strPath + recipe + "\\Vision10\\");
                                                }
                                            }

                                        }
                                    }

                                }

                            }
                        }
                    }
                    else
                    {
                        if (objCopyForm.ref_blnAllVision)
                        {
                            if (objCopyForm.ref_CopyAdvanceSettingOnly)
                            {
                                CopyAdvanceSettingToExistingRecipe_AllVision(strDirectory + lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo);
                                STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe All Vision", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                            }
                            else
                            {
                                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo))
                                {
                                    string[] strDir = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo);
                                    foreach (string dir in strDir)
                                    {
                                        Directory.Delete(dir, true);
                                    }
                                }

                                CopyDirectory(strDirectory + lst_RecipeAvailable.Text, AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo);
                                STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe All Vision", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                if (m_smProductionInfo.g_blnWantRecipeVerification)
                                {
                                    string strPath = "D:\\PreTest Image\\Recipe\\";
                                    if (Directory.Exists(strPath + lst_RecipeAvailable.Text))
                                    {
                                        if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo))
                                        {
                                            string[] strDir = Directory.GetDirectories(strPath + objCopyForm.ref_strRecipeCopyTo);
                                            foreach (string dir in strDir)
                                            {
                                                Directory.Delete(dir, true);
                                            }
                                        }

                                        CopyDirectory(strPath + lst_RecipeAvailable.Text, strPath + objCopyForm.ref_strRecipeCopyTo);
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (objCopyForm.ref_blnVision1)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision1\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 1", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision1\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 1", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision1\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision1\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision1\\");
                                        }
                                    }

                                }
                            }

                            if (objCopyForm.ref_blnVision2)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision2\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 2", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision2\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 2", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision2\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision2\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision2\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision3)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision3\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 3", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision3\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 3", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision3\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision3\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision3\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision4)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision4\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 4", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision4\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 4", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision4\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision4\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision4\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision5)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision5\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 5", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision5\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 5", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision5\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision5\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision5\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision6)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision6\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 6", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision6\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 6", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision6\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision6\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision6\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision7)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision7\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 7", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision7\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 7", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision7\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision7\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision7\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision8)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision8\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 8", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision8\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 8", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision8\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision8\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision8\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision9)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision9\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 9", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision9\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 9", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision9\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision9\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision9\\");
                                        }
                                    }
                                }
                            }

                            if (objCopyForm.ref_blnVision10)
                            {
                                if (objCopyForm.ref_CopyAdvanceSettingOnly)
                                {
                                    CopyAdvanceSettingToExistingRecipe(strDirectory + lst_RecipeAvailable.Text + "\\Vision10\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy Advance Setting To Existing Recipe Vision 10", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);
                                }
                                else
                                {
                                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\"))
                                        Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\", true);

                                    CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision10\\", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\");
                                    STDeviceEdit.SaveDeviceEditLog("Recipe", "Copy To Existing Recipe Vision 10", lst_RecipeAvailable.Text, objCopyForm.ref_strRecipeCopyTo, m_smProductionInfo.g_strLotID);

                                    //2021-08-15 ZJYEOH : Copy the folder for PreTest Image
                                    if (m_smProductionInfo.g_blnWantRecipeVerification)
                                    {
                                        string strPath = "D:\\PreTest Image\\Recipe\\";
                                        if (Directory.Exists(strPath + lst_RecipeAvailable.Text + "\\Vision10\\"))
                                        {
                                            if (Directory.Exists(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\"))
                                                Directory.Delete(strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\", true);

                                            CopyDirectory(strDirectory + lst_RecipeAvailable.Text + "\\Vision10\\", strPath + objCopyForm.ref_strRecipeCopyTo + "\\Vision10\\");
                                        }
                                    }
                                }
                            }

                        }

                    }

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                    string strSelectedRecipe = (string)subkey.GetValue("SelectedRecipeID", "Default");
                    if ((objCopyForm.ref_strRecipeCopyTo == "All Existing Recipe" && lst_RecipeAvailable.Text != strSelectedRecipe) ||
                        (objCopyForm.ref_strRecipeCopyTo == strSelectedRecipe))
                    {
                        //Reload Recipe if selected recipe being modified
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] != null)
                            {
                                m_smVSInfo[i].AT_VM_ReloadRecipe = false;
                                m_smVSInfo[i].VM_AT_ReloadRecipe = true;
                            }
                        }
                    }
                    StopWaiting();
                }
                objCopyForm.Dispose();
                
            }
        }
        private void SaveBarcodeAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "Barcode\\Settings.xml") || !File.Exists(strDestinationPath + "Barcode\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                   m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\Settings.xml";

            string strSource = strSourcePath + "Barcode\\Settings.xml";
            string strDestination = strDestinationPath + "Barcode\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");
            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");
            objDestinationFile.WriteElement1Value("WantUseAngleRange", objSourceFile.GetValueAsBoolean("WantUseAngleRange", false));
            objDestinationFile.WriteElement1Value("WantUseGainRange", objSourceFile.GetValueAsBoolean("WantUseGainRange", false));
            objDestinationFile.WriteElement1Value("WantUseReferenceImage", objSourceFile.GetValueAsBoolean("WantUseReferenceImage", false));
            
            objSourceFile.GetFirstSection("Setting");

            objDestinationFile.WriteSectionElement("Settings", true);
            objDestinationFile.WriteElement1Value("TemplateCode", objSourceFile.GetValueAsString("TemplateCode", ""));
            objDestinationFile.WriteElement1Value("GainRangeTolerance", objSourceFile.GetValueAsFloat("GainRangeTolerance", 0));
            objDestinationFile.WriteElement1Value("BarcodeAngleRangeTolerance", objSourceFile.GetValueAsFloat("BarcodeAngleRangeTolerance", 0));
            objDestinationFile.WriteElement1Value("PatternAngleRangeTolerance", objSourceFile.GetValueAsInt("PatternAngleRangeTolerance", 50));
            objDestinationFile.WriteElement1Value("MinMatchingScore", objSourceFile.GetValueAsInt("MinMatchingScore", 50));
            objDestinationFile.WriteElement1Value("PatternReferenceOffsetX", objSourceFile.GetValueAsFloat("PatternReferenceOffsetX", 0));
            objDestinationFile.WriteElement1Value("PatternReferenceOffsetY", objSourceFile.GetValueAsFloat("PatternReferenceOffsetY", 0));
            objDestinationFile.WriteElement1Value("TemplateBarcodeWidth", objSourceFile.GetValueAsFloat("TemplateBarcodeWidth", 0));
            objDestinationFile.WriteElement1Value("TemplateBarcodeHeight", objSourceFile.GetValueAsFloat("TemplateBarcodeHeight", 0));
            objDestinationFile.WriteElement1Value("TemplateBarcodeAngle", objSourceFile.GetValueAsFloat("TemplateBarcodeAngle", 0));
            objDestinationFile.WriteElement1Value("TemplateBarcodeCenterX", objSourceFile.GetValueAsFloat("TemplateBarcodeCenterX", 0));
            objDestinationFile.WriteElement1Value("TemplateBarcodeCenterY", objSourceFile.GetValueAsFloat("TemplateBarcodeCenterY", 0));
            objDestinationFile.WriteElement1Value("BarcodeDetectionAreaTolerance", objSourceFile.GetValueAsInt("BarcodeDetectionAreaTolerance", 250));
            objDestinationFile.WriteElement1Value("DelayTimeAfterPass", objSourceFile.GetValueAsInt("DelayTimeAfterPass", 1000));
            objDestinationFile.WriteElement1Value("RetestCount", objSourceFile.GetValueAsInt("RetestCount", 0));
            objDestinationFile.WriteElement1Value("PatternDetectionAreaTolerance", objSourceFile.GetValueAsInt("PatternDetectionAreaTolerance", 20));
            objDestinationFile.WriteElement1Value("ImageGain", objSourceFile.GetValueAsFloat("ImageGain", 0.5f));
            objDestinationFile.WriteElement1Value("UniformizeGain", objSourceFile.GetValueAsInt("UniformizeGain", 1));
            objDestinationFile.WriteEndElement();

        }
        private void SaveGeneralAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "General.xml") || !File.Exists(strDestinationPath + "General.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                        m_smVisionInfo.g_strVisionFolderName + "\\General.xml";

            string strSource = strSourcePath + "General.xml";

            string strDestination = strDestinationPath + "General.xml";


            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");

            objDestinationFile.WriteElement1Value("WantCheckEmpty", objSourceFile.GetValueAsBoolean("WantCheckEmpty", false, 1));
            objDestinationFile.WriteElement1Value("WantUseEmptyPattern", objSourceFile.GetValueAsBoolean("WantUseEmptyPattern", false, 1));
            objDestinationFile.WriteElement1Value("WantUseEmptyThreshold", objSourceFile.GetValueAsBoolean("WantUseEmptyThreshold", true, 1));
            
            objDestinationFile.WriteElement1Value("WantUnitPRFindGauge", objSourceFile.GetValueAsBoolean("WantUnitPRFindGauge", false));

            objDestinationFile.WriteElement1Value("WantCheckUnitSitProper", objSourceFile.GetValueAsBoolean("WantCheckUnitSitProper", false, 1));

            objDestinationFile.WriteElement1Value("UseAutoRepalceCounter", objSourceFile.GetValueAsBoolean("UseAutoRepalceCounter", false, 1));

            objDestinationFile.WriteEndElement();

        }
        private void SaveLead3DAdvSetting(string strSourcePath, string strDestinationPath)
        {
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                  m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Settings.xml";

            string strSource = strSourcePath + "Lead3D\\Settings.xml";

            string strDestination = strDestinationPath + "Lead3D\\Settings.xml";

            XmlParser objSourceFile, objDestinationFile;
            if (File.Exists(strSourcePath + "Lead3D\\Settings.xml") && File.Exists(strDestinationPath + "General.xml"))
            {
                objSourceFile = new XmlParser(strSource);
                objSourceFile.GetFirstSection("Advanced");

                objDestinationFile = new XmlParser(strDestination);
                objDestinationFile.WriteSectionElement("Advanced");
                objDestinationFile.WriteElement1Value("WantShowGRR", objSourceFile.GetValueAsBoolean("WantShowGRR", false, 1));
                objDestinationFile.WriteElement1Value("WantDontCareAreaLead3D", objSourceFile.GetValueAsBoolean("WantDontCareAreaLead3D", false));
                objDestinationFile.WriteElement1Value("WantPin1", objSourceFile.GetValueAsBoolean("WantPin1", false));
                objDestinationFile.WriteElement1Value("WantCheckPH", objSourceFile.GetValueAsBoolean("WantCheckPH", false));
                objDestinationFile.WriteEndElement();
            }

            if (!File.Exists(strSourcePath + "Lead3D\\Template\\Template.xml") || !File.Exists(strDestinationPath + "Lead3D\\Template\\Template.xml"))
            {
                return;
            }

            strSource = strSourcePath + "Lead3D\\Template\\Template.xml";
            strDestination = strDestinationPath + "Lead3D\\Template\\Template.xml";
            objSourceFile = new XmlParser(strSource);
            objDestinationFile = new XmlParser(strDestination);
            for (int i = 0; i < objSourceFile.GetFirstSectionCount(); i++)
            {
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";
                else
                    break;

                objSourceFile.GetFirstSection(strSectionName);
                objSourceFile.GetSecondSection("LeadSetting");
                objDestinationFile.WriteSectionElement(strSectionName, false);

                // Save LeadSetting
                objDestinationFile.WriteElement1Value("LeadSetting", "", "", false);
                objDestinationFile.WriteElement2Value("WantUsePkgToBaseTolerance", objSourceFile.GetValueAsBoolean("WantUsePkgToBaseTolerance", false, 2));
                objDestinationFile.WriteElement2Value("WantUseAGVMasking", objSourceFile.GetValueAsBoolean("WantUseAGVMasking", false, 2));
                objDestinationFile.WriteElement2Value("WantUseAverageGrayValueMethod", objSourceFile.GetValueAsBoolean("WantUseAverageGrayValueMethod", false, 2));
                objDestinationFile.WriteElement2Value("LeadWidthDisplayOption", objSourceFile.GetValueAsInt("LeadWidthDisplayOption", 0, 2));
                objDestinationFile.WriteElement2Value("LeadLengthVarianceMethod", objSourceFile.GetValueAsInt("LeadLengthVarianceMethod", 0, 2));
                objDestinationFile.WriteElement2Value("LeadSpanMethod", objSourceFile.GetValueAsInt("LeadSpanMethod", 0, 2));
                objDestinationFile.WriteElement2Value("LeadContaminationRegion", objSourceFile.GetValueAsInt("LeadContaminationRegion", 0, 2));
                objDestinationFile.WriteElement2Value("LeadStandOffMethod", objSourceFile.GetValueAsInt("LeadStandOffMethod", 0, 2));
                objDestinationFile.WriteElement2Value("LeadWidthRangeSelection", objSourceFile.GetValueAsInt("LeadWidthRangeSelection", 0, 2));
                objDestinationFile.WriteElement2Value("LeadWidthRange", objSourceFile.GetValueAsInt("LeadWidthRange", 10, 2));
                
            }
            objDestinationFile.WriteEndElement();
        }
        private void SaveLead3DPackageAdvSetting(string strSourcePath, string strDestinationPath)
        {
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                    m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Settings.xml";

            string strSource = strSourcePath + "Lead3D\\Settings.xml";

            string strDestination = strDestinationPath + "Lead3D\\Settings.xml";
            XmlParser objSourceFile, objDestinationFile;

            if (File.Exists(strSourcePath + "Lead3D\\Settings.xml") && File.Exists(strDestinationPath + "Lead3D\\Settings.xml"))
            {
                objSourceFile = new XmlParser(strSource);
                objSourceFile.GetFirstSection("Advanced");

                objDestinationFile = new XmlParser(strDestination);
                objDestinationFile.WriteSectionElement("Advanced");
                objDestinationFile.WriteElement1Value("WantUseDetailThresholdLeadPackage", objSourceFile.GetValueAsBoolean("WantUseDetailThresholdLeadPackage", false, 1));
                objDestinationFile.WriteElement1Value("SeperateCrackDefectSetting", objSourceFile.GetValueAsBoolean("SeperateCrackDefectSetting", false, 1));
                objDestinationFile.WriteElement1Value("SeperateChippedOffDefectSetting", objSourceFile.GetValueAsBoolean("SeperateChippedOffDefectSetting", false, 1));
                objDestinationFile.WriteElement1Value("SeperateMoldFlashDefectSetting", objSourceFile.GetValueAsBoolean("SeperateMoldFlashDefectSetting", false, 1));
                objDestinationFile.WriteEndElement();
            }

            if (!File.Exists(strSourcePath + "Lead3D\\Template\\Template.xml") || !File.Exists(strDestinationPath + "Lead3D\\Template\\Template.xml"))
            {
                return;
            }

            strSource = strSourcePath + "Lead3D\\Template\\Template.xml";
            strDestination = strDestinationPath + "Lead3D\\Template\\Template.xml";
            objSourceFile = new XmlParser(strSource);
            objDestinationFile = new XmlParser(strDestination);
            for (int i = 0; i < objSourceFile.GetFirstSectionCount(); i++)
            {
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";
                else
                    break;

                objSourceFile.GetFirstSection(strSectionName);
                objSourceFile.GetSecondSection("LeadSetting");
                objDestinationFile.WriteSectionElement(strSectionName, false);

                objDestinationFile.WriteElement1Value("LeadSetting", "", "", false);
                objDestinationFile.WriteElement2Value("MeasureCenterPkgSizeUsingCorner", objSourceFile.GetValueAsBoolean("MeasureCenterPkgSizeUsingCorner", false, 2));
                
                // Save Package Setting
                objSourceFile.GetSecondSection("PackageSetting");
                objDestinationFile.WriteElement1Value("PackageSetting", "", "", false);
                objDestinationFile.WriteElement2Value("GrabImageIndexCount", objSourceFile.GetValueAsInt("GrabImageIndexCount", 0, 2));
                for (int j = 0; j < objSourceFile.GetValueAsInt("GrabImageIndexCount", 0, 2); j++)
                    objDestinationFile.WriteElement2Value("GrabImageIndex" + j.ToString(), objSourceFile.GetValueAsInt("GrabImageIndexCount" + j.ToString(), 0, 2));

                
            }
            objDestinationFile.WriteEndElement();
        }
        private void SaveLeadAdvSetting(string strSourcePath, string strDestinationPath)
        {
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                   m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Settings.xml";

            string strSource = strSourcePath + "Lead\\Settings.xml";

            string strDestination = strDestinationPath + "Lead\\Settings.xml";
            XmlParser objSourceFile, objDestinationFile;

            if (File.Exists(strSourcePath + "Lead\\Settings.xml") && File.Exists(strDestinationPath + "Lead\\Settings.xml"))
            {
                objSourceFile = new XmlParser(strSource);
                objSourceFile.GetFirstSection("Advanced");

                objDestinationFile = new XmlParser(strDestination);
                objDestinationFile.WriteSectionElement("Advanced");
                objDestinationFile.WriteElement1Value("WantCheckLead", objSourceFile.GetValueAsBoolean("WantCheckLead", false, 1));
                objDestinationFile.WriteElement1Value("WantPocketDontCareAreaFix_Lead", objSourceFile.GetValueAsBoolean("WantPocketDontCareAreaFix_Lead", false, 1));
                objDestinationFile.WriteElement1Value("WantPocketDontCareAreaManual_Lead", objSourceFile.GetValueAsBoolean("WantPocketDontCareAreaManual_Lead", false, 1));
                objDestinationFile.WriteElement1Value("WantPocketDontCareAreaAuto_Lead", objSourceFile.GetValueAsBoolean("WantPocketDontCareAreaAuto_Lead", false, 1));
                objDestinationFile.WriteEndElement();
            }

            if (!File.Exists(strSourcePath + "Lead\\Template\\Template.xml") || !File.Exists(strDestinationPath + "Lead\\Template\\Template.xml"))
            {
                return;
            }

            strSource = strSourcePath + "Lead\\Template\\Template.xml";
            strDestination = strDestinationPath + "Lead\\Template\\Template.xml";
            objSourceFile = new XmlParser(strSource);
            objDestinationFile = new XmlParser(strDestination);
            for (int i = 0; i < objSourceFile.GetFirstSectionCount(); i++)
            {
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "SearchROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";
                else
                    break;

                objSourceFile.GetFirstSection(strSectionName);
                objSourceFile.GetSecondSection("LeadSetting");
                objDestinationFile.WriteSectionElement(strSectionName, false);
                objDestinationFile.WriteElement1Value("LeadSetting", "", "", false);

                objDestinationFile.WriteElement2Value("WantInspectBaseLead", objSourceFile.GetValueAsBoolean("WantInspectBaseLead", false, 2));
                objDestinationFile.WriteElement2Value("ImageViewNo", objSourceFile.GetValueAsInt("ImageViewNo", 0, 2));
                objDestinationFile.WriteElement2Value("BaseLeadImageViewNo", objSourceFile.GetValueAsInt("BaseLeadImageViewNo", 0, 2));
                objDestinationFile.WriteElement2Value("RotationMethod", objSourceFile.GetValueAsInt("RotationMethod", 0, 2));
                objDestinationFile.WriteElement2Value("WantUseAGVMasking", objSourceFile.GetValueAsBoolean("WantUseAGVMasking", true, 2));
                objDestinationFile.WriteElement2Value("WantUsePkgToBaseTolerance", objSourceFile.GetValueAsBoolean("WantUsePkgToBaseTolerance", false, 2));
                objDestinationFile.WriteElement2Value("WantUseAverageGrayValueMethod", objSourceFile.GetValueAsBoolean("WantUseAverageGrayValueMethod", false, 2));
                objDestinationFile.WriteElement2Value("WantPocketDontCareAreaFix_Lead", objSourceFile.GetValueAsBoolean("WantPocketDontCareAreaFix_Lead", false, 2));
                objDestinationFile.WriteElement2Value("WantPocketDontCareAreaManual_Lead", objSourceFile.GetValueAsBoolean("WantPocketDontCareAreaManual_Lead", false, 2));
                objDestinationFile.WriteElement2Value("WantPocketDontCareAreaAuto_Lead", objSourceFile.GetValueAsBoolean("WantPocketDontCareAreaAuto_Lead", false, 2));
                objDestinationFile.WriteElement2Value("PocketDontCareMethod", objSourceFile.GetValueAsInt("PocketDontCareMethod", 0, 2));
                objDestinationFile.WriteElement2Value("WantUseGaugeMeasureLeadDimension", objSourceFile.GetValueAsBoolean("WantUseGaugeMeasureLeadDimension", false, 2));
            }
            objDestinationFile.WriteEndElement();
        }
        private void SaveMarkAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "Mark\\Settings.xml") || !File.Exists(strDestinationPath + "Mark\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //    m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";

            string strSource = strSourcePath + "Mark\\Settings.xml";

            string strDestination = strDestinationPath + "Mark\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");
            objDestinationFile.WriteElement1Value("WhiteOnBlack", objSourceFile.GetValueAsBoolean("WhiteOnBlack", true));
            objDestinationFile.WriteElement1Value("WantMultiGroups", objSourceFile.GetValueAsBoolean("WantMultiGroups", false));
            objDestinationFile.WriteElement1Value("WantBuildTexts", objSourceFile.GetValueAsBoolean("WantBuildTexts", false));
            objDestinationFile.WriteElement1Value("WantMultiTemplates", objSourceFile.GetValueAsBoolean("WantMultiTemplates", true));
            objDestinationFile.WriteElement1Value("WantSetTemplateBasedOnBinInfo_PurposelyRename", objSourceFile.GetValueAsBoolean("WantSetTemplateBasedOnBinInfo_PurposelyRename", false));
            objDestinationFile.WriteElement1Value("WantSet1ToAll", objSourceFile.GetValueAsBoolean("WantSet1ToAll", false));
            objDestinationFile.WriteElement1Value("WantSkipMark", objSourceFile.GetValueAsBoolean("WantSkipMark", false));
            objDestinationFile.WriteElement1Value("WantDontCareIgnoredMarkWholeArea", objSourceFile.GetValueAsBoolean("WantDontCareIgnoredMarkWholeArea", false));
            objDestinationFile.WriteElement1Value("WantPin1", objSourceFile.GetValueAsBoolean("WantPin1", false));
            objDestinationFile.WriteElement1Value("WantGaugeMeasureMarkDimension", objSourceFile.GetValueAsBoolean("WantGaugeMeasureMarkDimension", false));
            objDestinationFile.WriteElement1Value("WantClearMarkTemplateWhenNewLot", objSourceFile.GetValueAsBoolean("WantClearMarkTemplateWhenNewLot", false));
            objDestinationFile.WriteElement1Value("WantCheckNoMark", objSourceFile.GetValueAsBoolean("WantCheckNoMark", false));
            objDestinationFile.WriteElement1Value("WantCheckContourOnMark", objSourceFile.GetValueAsBoolean("WantCheckContourOnMark", false));
            objDestinationFile.WriteElement1Value("WantMark2DCode", objSourceFile.GetValueAsBoolean("WantMark2DCode", false));
            objDestinationFile.WriteElement1Value("WantDontCareAreaMark", objSourceFile.GetValueAsBoolean("WantDontCareAreaMark", false));
            objDestinationFile.WriteElement1Value("WantSampleAreaScore", objSourceFile.GetValueAsBoolean("WantSampleAreaScore", false));
            objDestinationFile.WriteElement1Value("WantRotateMarkImageUsingPkgAngle", objSourceFile.GetValueAsBoolean("WantRotateMarkImageUsingPkgAngle", false));
            objDestinationFile.WriteElement1Value("WantCheckMarkAngle", objSourceFile.GetValueAsBoolean("WantCheckMarkAngle", false));
            objDestinationFile.WriteElement1Value("UseDefaultMarkScoreAfterNewLotClearTemplate", objSourceFile.GetValueAsBoolean("UseDefaultMarkScoreAfterNewLotClearTemplate", false));
            objDestinationFile.WriteElement1Value("SeparateExtraMarkThreshold", objSourceFile.GetValueAsBoolean("SeparateExtraMarkThreshold", false));
            objDestinationFile.WriteElement1Value("WantExcessMarkThresholdFollowExtraMarkThreshold", objSourceFile.GetValueAsBoolean("WantExcessMarkThresholdFollowExtraMarkThreshold", false));
            objDestinationFile.WriteElement1Value("WantUseLeadPointOffsetMarkROI", objSourceFile.GetValueAsBoolean("WantUseLeadPointOffsetMarkROI", false));
            objDestinationFile.WriteElement1Value("WantRemoveBorderWhenLearnMark", objSourceFile.GetValueAsBoolean("WantRemoveBorderWhenLearnMark", false));
            objDestinationFile.WriteElement1Value("WantCheckBarPin1", objSourceFile.GetValueAsBoolean("WantCheckBarPin1", false));
            objDestinationFile.WriteElement1Value("WantCheckBrokenMark", objSourceFile.GetValueAsBoolean("WantCheckBrokenMark", false));
            objDestinationFile.WriteElement1Value("WantCheckTotalExcessMark", objSourceFile.GetValueAsBoolean("WantCheckTotalExcessMark", false));
            objDestinationFile.WriteElement1Value("WantCheckMarkAverageGrayValue", objSourceFile.GetValueAsBoolean("WantCheckMarkAverageGrayValue", false));
            objDestinationFile.WriteElement1Value("MarkCharROIOffsetX", objSourceFile.GetValueAsFloat("MarkCharROIOffsetX", 5));
            objDestinationFile.WriteElement1Value("MarkCharROIOffsetY", objSourceFile.GetValueAsFloat("MarkCharROIOffsetY", 5));
            objDestinationFile.WriteElement1Value("WantUseMarkTypeInspectionSetting", objSourceFile.GetValueAsBoolean("WantUseMarkTypeInspectionSetting", false));
            objDestinationFile.WriteElement1Value("ExtraExcessMarkInspectionAreaCutMode", objSourceFile.GetValueAsInt("ExtraExcessMarkInspectionAreaCutMode", 0));
            objDestinationFile.WriteElement1Value("CompensateMarkDiffSizeMode", objSourceFile.GetValueAsInt("CompensateMarkDiffSizeMode", 0));
            objDestinationFile.WriteElement1Value("CodeType", objSourceFile.GetValueAsInt("CodeType", 0));
            objDestinationFile.WriteElement1Value("MissingMarkInspectionMethod", objSourceFile.GetValueAsInt("MissingMarkInspectionMethod", 0));
            objDestinationFile.WriteElement1Value("DefaultMarkScore", objSourceFile.GetValueAsInt("DefaultMarkScore", 50));
            objDestinationFile.WriteElement1Value("MarkScoreOffset", objSourceFile.GetValueAsInt("MarkScoreOffset", 0));
            objDestinationFile.WriteElement1Value("MarkOriPositionScore", objSourceFile.GetValueAsInt("MarkOriPositionScore", 70));
            objDestinationFile.WriteElement1Value("MinMarkScore", objSourceFile.GetValueAsInt("MinMarkScore", 30));
            objDestinationFile.WriteElement1Value("MaxMarkTemplate", objSourceFile.GetValueAsInt("MaxMarkTemplate", 4));
            objDestinationFile.WriteElement1Value("NoMarkMaximumBlob", objSourceFile.GetValueAsFloat("NoMarkMaximumBlob", 200));
            objDestinationFile.WriteElement1Value("MarkDefectInspectionMethod", objSourceFile.GetValueAsInt("MarkDefectInspectionMethod", 0));
            objDestinationFile.WriteElement1Value("MarkTextShiftMethod", objSourceFile.GetValueAsInt("MarkTextShiftMethod", 0));
            objDestinationFile.WriteElement1Value("FinalReduction_Direction", objSourceFile.GetValueAsInt("FinalReduction_Direction", 2));
            objDestinationFile.WriteElement1Value("FinalReduction_MarkDeg", objSourceFile.GetValueAsInt("FinalReduction_MarkDeg", 0));
            objDestinationFile.WriteElement1Value("WantCheckCharMissingMark", objSourceFile.GetValueAsBoolean("WantCheckCharMissingMark", true));
            objDestinationFile.WriteElement1Value("WantCheckCharBrokenMark", objSourceFile.GetValueAsBoolean("WantCheckCharBrokenMark", true));
            objDestinationFile.WriteElement1Value("WantCheckCharExcessMark", objSourceFile.GetValueAsBoolean("WantCheckCharExcessMark", true));
            objDestinationFile.WriteElement1Value("WantCheckLogoExcessMark", objSourceFile.GetValueAsBoolean("WantCheckLogoExcessMark", true));
            objDestinationFile.WriteElement1Value("WantCheckLogoMissingMark", objSourceFile.GetValueAsBoolean("WantCheckLogoMissingMark", true));
            objDestinationFile.WriteElement1Value("WantCheckLogoBrokenMark", objSourceFile.GetValueAsBoolean("WantCheckLogoBrokenMark", true));
            objDestinationFile.WriteElement1Value("WantCheckSymbol1ExcessMark", objSourceFile.GetValueAsBoolean("WantCheckSymbol1ExcessMark", true));
            objDestinationFile.WriteElement1Value("WantCheckSymbol1MissingMark", objSourceFile.GetValueAsBoolean("WantCheckSymbol1MissingMark", true));
            objDestinationFile.WriteElement1Value("WantCheckSymbol1BrokenMark", objSourceFile.GetValueAsBoolean("WantCheckSymbol1BrokenMark", true));
            objDestinationFile.WriteElement1Value("WantCheckSymbol2ExcessMark", objSourceFile.GetValueAsBoolean("WantCheckSymbol2ExcessMark", true));
            objDestinationFile.WriteElement1Value("WantCheckSymbol2MissingMark", objSourceFile.GetValueAsBoolean("WantCheckSymbol2MissingMark", true));
            objDestinationFile.WriteElement1Value("WantCheckSymbol2BrokenMark", objSourceFile.GetValueAsBoolean("WantCheckSymbol2BrokenMark", true));
            objDestinationFile.WriteElement1Value("RotationInterpolation_Mark", objSourceFile.GetValueAsInt("RotationInterpolation_Mark", 4));
            objDestinationFile.WriteElement1Value("RotationInterpolation_PkgBright", objSourceFile.GetValueAsInt("RotationInterpolation_PkgBright", 4));
            objDestinationFile.WriteElement1Value("RotationInterpolation_PkgDark", objSourceFile.GetValueAsInt("RotationInterpolation_PkgDark", 4));
            objDestinationFile.WriteEndElement();
            
        }
        private void SaveOrientAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "Orient\\Settings.xml") || !File.Exists(strDestinationPath + "Orient\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //            m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Settings.xml";

            string strSource = strSourcePath + "Orient\\Settings.xml";

            string strDestination = strDestinationPath + "Orient\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");

            objDestinationFile.WriteElement1Value("Direction", objSourceFile.GetValueAsInt("Direction", 4));
            objDestinationFile.WriteElement1Value("WantSubROI", objSourceFile.GetValueAsBoolean("WantSubROI", false));
            objDestinationFile.WriteElement1Value("WantUsePositionCheckOrientation", objSourceFile.GetValueAsBoolean("WantUsePositionCheckOrientation", false));
            objDestinationFile.WriteElement1Value("CheckPositionOrientationWhenBelowDifferentScore", objSourceFile.GetValueAsFloat("CheckPositionOrientationWhenBelowDifferentScore", 0.1f));
            
            objDestinationFile.WriteEndElement();
            
        }
        private void SavePackageAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "Package\\Settings.xml") || !File.Exists(strDestinationPath + "Package\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //    m_smVisionInfo.g_strVisionFolderName + "\\Package";

            string strSource = strSourcePath + "Package\\Settings.xml";

            string strDestination = strDestinationPath + "Package\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");
            objDestinationFile.WriteElement1Value("WantShowGRR", objSourceFile.GetValueAsBoolean("WantShowGRR", false, 1));
            objDestinationFile.WriteElement1Value("WantUseSideLightGauge", objSourceFile.GetValueAsBoolean("WantUseSideLightGauge", false));
            objDestinationFile.WriteElement1Value("WantCheckVoidOnMark", objSourceFile.GetValueAsBoolean("WantCheckVoidOnMark", false));
            objDestinationFile.WriteElement1Value("WantUseDetailThresholdPackage", objSourceFile.GetValueAsBoolean("WantUseDetailThresholdPackage", false));
            objDestinationFile.WriteElement1Value("WantDontCareAreaPackage", objSourceFile.GetValueAsBoolean("WantDontCareAreaPackage", false));
            objDestinationFile.WriteElement1Value("WantCheckPackageAngle", objSourceFile.GetValueAsBoolean("WantCheckPackageAngle", false));
            objDestinationFile.WriteElement1Value("SquareUnit", objSourceFile.GetValueAsBoolean("SquareUnit", false));
            objDestinationFile.WriteElement1Value("PackageDefectInspectionMethod", objSourceFile.GetValueAsInt("PackageDefectInspectionMethod", 0));
            
            objSourceFile.GetFirstSection("Settings");
            objDestinationFile.WriteSectionElement("Settings", false);
            objDestinationFile.WriteElement1Value("SeperateBrightDarkROITolerance", objSourceFile.GetValueAsBoolean("SeperateBrightDarkROITolerance", false));
            objDestinationFile.WriteElement1Value("SeperateDarkField2DefectSetting", objSourceFile.GetValueAsBoolean("SeperateDarkField2DefectSetting", false));
            objDestinationFile.WriteElement1Value("SeperateCrackDefectSetting", objSourceFile.GetValueAsBoolean("SeperateCrackDefectSetting", false));
            objDestinationFile.WriteElement1Value("WantLinkBrightDefect", objSourceFile.GetValueAsBoolean("WantLinkBrightDefect", false));
            objDestinationFile.WriteElement1Value("WantLinkDarkDefect", objSourceFile.GetValueAsBoolean("WantLinkDarkDefect", false));
            objDestinationFile.WriteElement1Value("WantLinkDark2Defect", objSourceFile.GetValueAsBoolean("WantLinkDark2Defect", false));
            objDestinationFile.WriteElement1Value("WantLinkCrackDefect", objSourceFile.GetValueAsBoolean("WantLinkCrackDefect", false));
            objDestinationFile.WriteElement1Value("WantLinkMoldFlashDefect", objSourceFile.GetValueAsBoolean("WantLinkMoldFlashDefect", false));
            objDestinationFile.WriteElement1Value("BrightDefectLinkTolerance", objSourceFile.GetValueAsInt("BrightDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("DarkDefectLinkTolerance", objSourceFile.GetValueAsInt("DarkDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("Dark2DefectLinkTolerance", objSourceFile.GetValueAsInt("Dark2DefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("CrackDefectLinkTolerance", objSourceFile.GetValueAsInt("CrackDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("MoldFlashDefectLinkTolerance", objSourceFile.GetValueAsInt("MoldFlashDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("SeperateChippedOffDefectSetting", objSourceFile.GetValueAsBoolean("SeperateChippedOffDefectSetting", false));
            objDestinationFile.WriteElement1Value("SeperateVoidDefectSetting", objSourceFile.GetValueAsBoolean("SeperateVoidDefectSetting", false));
            objDestinationFile.WriteElement1Value("SeperateMoldFlashDefectSetting", objSourceFile.GetValueAsBoolean("SeperateMoldFlashDefectSetting", false));
            
            // Grab image index
            objDestinationFile.WriteElement1Value("GrabImageIndexCount", objSourceFile.GetValueAsInt("GrabImageIndexCount", 0));
            for (int j = 0; j < objSourceFile.GetValueAsInt("GrabImageIndexCount", 0); j++)
                objDestinationFile.WriteElement1Value("GrabImageIndex" + j.ToString(), objSourceFile.GetValueAsInt("GrabImageIndex" + j.ToString(), 0));

            objDestinationFile.WriteEndElement();

        }
        private void SavePadAdvSetting(string strSourcePath, string strDestinationPath)
        {
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            string strSource = strSourcePath + "Pad\\Settings.xml";

            string strDestination = strDestinationPath + "Pad\\Settings.xml";
            XmlParser objSourceFile, objDestinationFile;

            if (File.Exists(strSourcePath + "Pad\\Settings.xml") && File.Exists(strDestinationPath + "Pad\\Settings.xml"))
            {
                objSourceFile = new XmlParser(strSource);
                objSourceFile.GetFirstSection("Advanced");

                objDestinationFile = new XmlParser(strDestination);
                objDestinationFile.WriteSectionElement("Advanced");
                objDestinationFile.WriteElement1Value("WantCheckPad", objSourceFile.GetValueAsBoolean("WantCheckPad", true, 1));
                objDestinationFile.WriteElement1Value("WantCheckPadColor", objSourceFile.GetValueAsBoolean("WantCheckPadColor", true, 1));
                objDestinationFile.WriteElement1Value("WantCheck4Sides", objSourceFile.GetValueAsBoolean("WantCheck4Sides", false, 1));
                objDestinationFile.WriteElement1Value("WantShowGRR", objSourceFile.GetValueAsBoolean("WantShowGRR", false, 1));
                objDestinationFile.WriteElement1Value("WantCheckCPK", objSourceFile.GetValueAsBoolean("WantCheckCPK", false, 1));
                objDestinationFile.WriteElement1Value("CheckAllPadCPK", objSourceFile.GetValueAsBoolean("CheckAllPadCPK", false, 1));
                objDestinationFile.WriteElement1Value("PadCPKCount", objSourceFile.GetValueAsInt("PadCPKCount", 100));
                objDestinationFile.WriteElement1Value("WantPin1", objSourceFile.GetValueAsBoolean("WantPin1", false));
                objDestinationFile.WriteElement1Value("WantCheckPH", objSourceFile.GetValueAsBoolean("WantCheckPH", false));
                objDestinationFile.WriteElement1Value("WantDontCareAreaPad", objSourceFile.GetValueAsBoolean("WantDontCareAreaPad", false));
                objDestinationFile.WriteElement1Value("WantEdgeLimitPad", objSourceFile.GetValueAsBoolean("WantEdgeLimitPad", false));
                objDestinationFile.WriteElement1Value("WantStandOffPad", objSourceFile.GetValueAsBoolean("WantStandOffPad", false));
                objDestinationFile.WriteElement1Value("SavePadTemplateImageMethod", objSourceFile.GetValueAsInt("SavePadTemplateImageMethod", 0));
                objDestinationFile.WriteElement1Value("PadOffsetReferencePoint", objSourceFile.GetValueAsInt("PadOffsetReferencePoint", 0));
                objDestinationFile.WriteElement1Value("PadSubtractMethod_Center", objSourceFile.GetValueAsInt("PadSubtractMethod_Center", 0));
                objDestinationFile.WriteElement1Value("PadSubtractMethod_Side", objSourceFile.GetValueAsInt("PadSubtractMethod_Side", 0));
                objDestinationFile.WriteElement1Value("OrientDirection", objSourceFile.GetValueAsInt("OrientDirection", 4));
                objDestinationFile.WriteEndElement();
            }

            if (!File.Exists(strSourcePath + "Pad\\Template\\Template.xml") || !File.Exists(strDestinationPath + "Pad\\Template\\Template.xml"))
            {
                return;
            }

            strSource = strSourcePath + "Pad\\Template\\Template.xml";
            strDestination = strDestinationPath + "Pad\\Template\\Template.xml";
            objSourceFile = new XmlParser(strSource);
            objDestinationFile = new XmlParser(strDestination);

            for (int i = 0; i < objSourceFile.GetFirstSectionCount(); i++)
            {
                string strSectionName = "";

                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";
                else
                    break;

                objSourceFile.GetFirstSection(strSectionName);
                objSourceFile.GetSecondSection("PadSetting");
                objDestinationFile.WriteSectionElement(strSectionName, false);
                objDestinationFile.WriteElement1Value("PadSetting", "", "", false);
                
                objDestinationFile.WriteElement2Value("OrientDirections", objSourceFile.GetValueAsInt("OrientDirections", 4, 2));
                objDestinationFile.WriteElement2Value("WhiteOnBlack", objSourceFile.GetValueAsBoolean("WhiteOnBlack", true, 2));
                objDestinationFile.WriteElement2Value("DefaultPixelTolerance", objSourceFile.GetValueAsFloat("DefaultPixelTolerance", 3, 2));
                objDestinationFile.WriteElement2Value("PadROISizeToleranceADV", objSourceFile.GetValueAsInt("PadROISizeToleranceADV", 10, 2));
                objDestinationFile.WriteElement2Value("WantTightSetting", objSourceFile.GetValueAsBoolean("WantTightSetting", false, 2));
                objDestinationFile.WriteElement2Value("TightSettingTolerance", objSourceFile.GetValueAsFloat("TightSettingTolerance", 0.01f, 2));
                objDestinationFile.WriteElement2Value("TightSettingThresholdTolerance", objSourceFile.GetValueAsInt("TightSettingThresholdTolerance", 25, 2));
                objDestinationFile.WriteElement2Value("WantAutoGauge", objSourceFile.GetValueAsBoolean("WantAutoGauge", false, 2));
                objDestinationFile.WriteElement2Value("WantRotateSidePadImage", objSourceFile.GetValueAsBoolean("WantRotateSidePadImage", true, 2));
                objDestinationFile.WriteElement2Value("WantLinkDifferentGroupPitchGap", objSourceFile.GetValueAsBoolean("WantLinkDifferentGroupPitchGap", false, 2));
                objDestinationFile.WriteElement2Value("WantSeparateBrokenPadThresholdSetting", objSourceFile.GetValueAsBoolean("WantSeparateBrokenPadThresholdSetting", false, 2));
                objDestinationFile.WriteElement2Value("WantConsiderPadImage2", objSourceFile.GetValueAsBoolean("WantConsiderPadImage2", false, 2));
                objDestinationFile.WriteElement2Value("WantUseGaugeMeasureDimension", objSourceFile.GetValueAsBoolean("WantUseGaugeMeasureDimension", false, 2));
                objDestinationFile.WriteElement2Value("BrokenPadImageViewNo", objSourceFile.GetValueAsInt("BrokenPadImageViewNo", 0, 2));
                objDestinationFile.WriteElement2Value("SensitivityOnPadMethod", objSourceFile.GetValueAsInt("SensitivityOnPadMethod", 0, 2));
                objDestinationFile.WriteElement2Value("SensitivityOnPadValue", objSourceFile.GetValueAsInt("SensitivityOnPadValue", 0, 2));
                objDestinationFile.WriteElement2Value("InspectPadMode", objSourceFile.GetValueAsInt("InspectPadMode", 0, 2));
                objDestinationFile.WriteElement2Value("WantUseBorderLimitAsOffset", objSourceFile.GetValueAsBoolean("WantUseBorderLimitAsOffset", false, 2));
                objDestinationFile.WriteElement2Value("MeasureCenterPkgSizeUsingSidePkg", objSourceFile.GetValueAsBoolean("MeasureCenterPkgSizeUsingSidePkg", false, 2));
                objDestinationFile.WriteElement2Value("WantPRUnitLocationBeforeGauge", objSourceFile.GetValueAsBoolean("WantPRUnitLocationBeforeGauge", false, 2));
                objDestinationFile.WriteElement2Value("WantUseClosestSizeDefineTolerance", objSourceFile.GetValueAsBoolean("WantUseClosestSizeDefineTolerance", false, 2));
            }
            objDestinationFile.WriteEndElement();
        }
        private void SavePadPackageAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "Pad\\Settings.xml") || !File.Exists(strDestinationPath + "Pad\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            string strSource = strSourcePath + "Pad\\Settings.xml";

            string strDestination = strDestinationPath + "Pad\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");
            objDestinationFile.WriteElement1Value("WantCheckPackage", objSourceFile.GetValueAsBoolean("WantCheckPackage", false, 1));
            objDestinationFile.WriteElement1Value("WantUseDetailThresholdPadPackage", objSourceFile.GetValueAsBoolean("WantUseDetailThresholdPadPackage", false));
            objDestinationFile.WriteElement1Value("SeperateCrackDefectSetting", objSourceFile.GetValueAsBoolean("SeperateCrackDefectSetting", false));
            objDestinationFile.WriteElement1Value("SeperateForeignMaterialDefectSetting", objSourceFile.GetValueAsBoolean("SeperateForeignMaterialDefectSetting", false));
            objDestinationFile.WriteElement1Value("SeperateChippedOffDefectSetting", objSourceFile.GetValueAsBoolean("SeperateChippedOffDefectSetting", false));
            objDestinationFile.WriteElement1Value("SeperateMoldFlashDefectSetting", objSourceFile.GetValueAsBoolean("SeperateMoldFlashDefectSetting", false));
            objDestinationFile.WriteElement1Value("PadPkgSizeImageViewNo_Center", objSourceFile.GetValueAsInt("PadPkgSizeImageViewNo_Center", 0));
            objDestinationFile.WriteElement1Value("PadPkgBrightFieldImageViewNo_Center", objSourceFile.GetValueAsInt("PadPkgBrightFieldImageViewNo_Center", 0));
            objDestinationFile.WriteElement1Value("PadPkgDarkFieldImageNo_Center", objSourceFile.GetValueAsInt("PadPkgDarkFieldImageNo_Center", 1));
            objDestinationFile.WriteElement1Value("PadPkgMoldFlashImageViewNo_Center", objSourceFile.GetValueAsInt("PadPkgMoldFlashImageViewNo_Center", 0));
            objDestinationFile.WriteElement1Value("PadPkgSizeImageViewNo_Side", objSourceFile.GetValueAsInt("PadPkgSizeImageViewNo_Side", 0));
            objDestinationFile.WriteElement1Value("PadPkgBrightFieldImageViewNo_Side", objSourceFile.GetValueAsInt("PadPkgBrightFieldImageViewNo_Side", 0));
            objDestinationFile.WriteElement1Value("PadPkgDarkFieldImageNo_Side", objSourceFile.GetValueAsInt("PadPkgDarkFieldImageNo_Side", 1));
            objDestinationFile.WriteElement1Value("WantDontCareAreaPadPackage", objSourceFile.GetValueAsBoolean("WantDontCareAreaPadPackage", false));
            objDestinationFile.WriteElement1Value("PadPkgMoldFlashImageViewNo_Side", objSourceFile.GetValueAsInt("PadPkgMoldFlashImageViewNo_Side", 0));
            objDestinationFile.WriteElement1Value("SeperateBrightDarkROITolerancePadPackage", objSourceFile.GetValueAsBoolean("SeperateBrightDarkROITolerancePadPackage", false));

            objDestinationFile.WriteElement1Value("WantLinkBrightDefect", objSourceFile.GetValueAsBoolean("WantLinkBrightDefect", false));
            objDestinationFile.WriteElement1Value("WantLinkDarkDefect", objSourceFile.GetValueAsBoolean("WantLinkDarkDefect", false));
            objDestinationFile.WriteElement1Value("WantLinkCrackDefect", objSourceFile.GetValueAsBoolean("WantLinkCrackDefect", false));
            objDestinationFile.WriteElement1Value("WantLinkMoldFlashDefect", objSourceFile.GetValueAsBoolean("WantLinkMoldFlashDefect", false));

            objDestinationFile.WriteElement1Value("BrightDefectLinkTolerance", objSourceFile.GetValueAsInt("BrightDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("DarkDefectLinkTolerance", objSourceFile.GetValueAsInt("DarkDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("CrackDefectLinkTolerance", objSourceFile.GetValueAsInt("CrackDefectLinkTolerance", 10));
            objDestinationFile.WriteElement1Value("MoldFlashDefectLinkTolerance", objSourceFile.GetValueAsInt("MoldFlashDefectLinkTolerance", 10));

            objDestinationFile.WriteEndElement();
        }
        private void SavePocketPositionAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "PocketPosition\\Settings.xml") || !File.Exists(strDestinationPath + "PocketPosition\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                   m_smVisionInfo.g_strVisionFolderName + "\\PocketPosition\\Settings.xml";

            string strSource = strSourcePath + "PocketPosition\\Settings.xml";

            string strDestination = strDestinationPath + "PocketPosition\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");
            objDestinationFile.WriteElement1Value("WantCheckPocketPosition", objSourceFile.GetValueAsBoolean("WantCheckPocketPosition", false, 1));
            objDestinationFile.WriteElement1Value("WantUsePocketPattern", objSourceFile.GetValueAsBoolean("WantUsePocketPattern", false, 1));
            objDestinationFile.WriteElement1Value("WantUsePocketGauge", objSourceFile.GetValueAsBoolean("WantUsePocketGauge", false, 1));
            
            objSourceFile.GetFirstSection("Settings");
            objDestinationFile.WriteSectionElement("Settings", false);

            // Grab image index
            objDestinationFile.WriteElement1Value("GrabImageIndexCount", objSourceFile.GetValueAsInt("GrabImageIndexCount", 0));
            for (int j = 0; j < objSourceFile.GetValueAsInt("GrabImageIndexCount", 0); j++)
                objDestinationFile.WriteElement1Value("GrabImageIndex" + j.ToString(), objSourceFile.GetValueAsInt("GrabImageIndex" + j.ToString(), 0));

            objDestinationFile.WriteEndElement();
        }
        private void SaveSealAdvSetting(string strSourcePath, string strDestinationPath)
        {
            if (!File.Exists(strSourcePath + "Seal\\Settings.xml") || !File.Exists(strDestinationPath + "Seal\\Settings.xml"))
                return;
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //            m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Settings.xml";

            string strSource = strSourcePath + "Seal\\Settings.xml";

            string strDestination = strDestinationPath + "Seal\\Settings.xml";

            XmlParser objSourceFile = new XmlParser(strSource);
            objSourceFile.GetFirstSection("Advanced");

            XmlParser objDestinationFile = new XmlParser(strDestination);
            objDestinationFile.WriteSectionElement("Advanced");

            objDestinationFile.WriteElement1Value("SealWhiteOnBlack", objSourceFile.GetValueAsBoolean("SealWhiteOnBlack", true));
            objDestinationFile.WriteElement1Value("Direction", objSourceFile.GetValueAsInt("Direction", 4));
            objDestinationFile.WriteElement1Value("PocketPitch", objSourceFile.GetValueAsInt("PocketPitch", 0));
            objDestinationFile.WriteElement1Value("WantSkipOrient", objSourceFile.GetValueAsBoolean("WantSkipOrient", false));
            objDestinationFile.WriteElement1Value("WantSkipSprocketHole", objSourceFile.GetValueAsBoolean("WantSkipSprocketHole", true));
            objDestinationFile.WriteElement1Value("WantSkipSprocketHoleDiameterAndDefect", objSourceFile.GetValueAsBoolean("WantSkipSprocketHoleDiameterAndDefect", true));
            objDestinationFile.WriteElement1Value("WantSkipSprocketHoleBrokenAndRoundness", objSourceFile.GetValueAsBoolean("WantSkipSprocketHoleBrokenAndRoundness", true));
            objDestinationFile.WriteElement1Value("WantUsePatternCheckUnitPresent", objSourceFile.GetValueAsBoolean("WantUsePatternCheckUnitPresent", true));
            objDestinationFile.WriteElement1Value("WantUsePixelCheckUnitPresent", objSourceFile.GetValueAsBoolean("WantUsePixelCheckUnitPresent", true));
            objDestinationFile.WriteElement1Value("CheckMarkMethod", objSourceFile.GetValueAsInt("CheckMarkMethod", 0));
            objDestinationFile.WriteElement1Value("WantDontCareAreaSeal", objSourceFile.GetValueAsBoolean("WantDontCareAreaSeal", false));
            objDestinationFile.WriteElement1Value("MarkAreaBelowPercent", objSourceFile.GetValueAsFloat("MarkAreaBelowPercent", 3f));
            objDestinationFile.WriteElement1Value("PatternAngleTolerance", objSourceFile.GetValueAsInt("PatternAngleTolerance", 10));
            objDestinationFile.WriteElement1Value("ClearSealTemplateWhenNewLot", objSourceFile.GetValueAsBoolean("ClearSealTemplateWhenNewLot", false));

            objSourceFile.GetFirstSection("Settings");
            objDestinationFile.WriteSectionElement("Settings", false);

            // Grab image index
            objDestinationFile.WriteElement1Value("GrabImageIndexCount", objSourceFile.GetValueAsInt("GrabImageIndexCount", 0));
            for (int j = 0; j < objSourceFile.GetValueAsInt("GrabImageIndexCount", 0); j++)
                objDestinationFile.WriteElement1Value("GrabImageIndex" + j.ToString(), objSourceFile.GetValueAsInt("GrabImageIndex" + j.ToString(), 0));

            objDestinationFile.WriteEndElement();

        }

        private void CopyAdvanceSettingToExistingRecipe(string strSourcePath, string strDestinationPath)
        {
            SaveBarcodeAdvSetting(strSourcePath, strDestinationPath);
            SaveGeneralAdvSetting(strSourcePath, strDestinationPath);
            SaveLead3DAdvSetting(strSourcePath, strDestinationPath);
            SaveLead3DPackageAdvSetting(strSourcePath, strDestinationPath);
            SaveLeadAdvSetting(strSourcePath, strDestinationPath);
            SaveMarkAdvSetting(strSourcePath, strDestinationPath);
            SaveOrientAdvSetting(strSourcePath, strDestinationPath);
            SavePackageAdvSetting(strSourcePath, strDestinationPath);
            SavePadAdvSetting(strSourcePath, strDestinationPath);
            SavePadPackageAdvSetting(strSourcePath, strDestinationPath);
            SavePocketPositionAdvSetting(strSourcePath, strDestinationPath);
            SaveSealAdvSetting(strSourcePath, strDestinationPath);
        }
        private void CopyAdvanceSettingToExistingRecipe_AllVision(string strSourcePath, string strDestinationPath)
        {
            DirectoryInfo dir = new DirectoryInfo(strSourcePath);
            DirectoryInfo[] arrDirLength = dir.GetDirectories();
            for (int i = 0; i < arrDirLength.Length; i++)
            {
                if (!arrDirLength[i].Name.Contains("Vision"))
                    continue;

                string strSourcePath_new = strSourcePath + "\\Vision" + (i + 1).ToString() + "\\";
                string strDestinationPath_new = strDestinationPath + "\\Vision" + (i + 1).ToString() + "\\";

                SaveBarcodeAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveGeneralAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveLead3DAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveLead3DPackageAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveLeadAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveMarkAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveOrientAdvSetting(strSourcePath_new, strDestinationPath_new);
                SavePackageAdvSetting(strSourcePath_new, strDestinationPath_new);
                SavePadAdvSetting(strSourcePath_new, strDestinationPath_new);
                SavePadPackageAdvSetting(strSourcePath_new, strDestinationPath_new);
                SavePocketPositionAdvSetting(strSourcePath_new, strDestinationPath_new);
                SaveSealAdvSetting(strSourcePath_new, strDestinationPath_new);
            }
        }
    }
}