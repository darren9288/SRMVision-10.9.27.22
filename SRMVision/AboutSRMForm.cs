using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SRMVision
{
    public partial class AboutSRMForm : Form
    {
        public AboutSRMForm()
        {
            InitializeComponent();

            UpdateGUI();
        }

        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG");
            label1.Text = subKey.GetValue("CompanyName", "SRM Integration (M) Sdn Bhd").ToString();
        }

        /// <summary>
        /// Fill all available vision station and features in it on list box
        /// </summary>
        private void FillTree()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            RegistryKey subKey1;
       
            string[] strVisionList = subKey.GetSubKeyNames();
            string[] strVisionFeatureList;

            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i]);

                TreeNode tre_ChildNode = new TreeNode(subKey1.GetValue("VisionName", "").ToString());
                tre_VisionsList.Nodes.Add(tre_ChildNode);

                strVisionFeatureList = subKey1.GetValueNames();                

                for (int j = 0; j < strVisionFeatureList.Length; j++)
                {
                    if (strVisionFeatureList[j] != "VisionName" && strVisionFeatureList[j] != "CameraID" && strVisionFeatureList[j] != "ImageUnits")
                    {
                        TreeNode tre_Node = new TreeNode(strVisionFeatureList[j]);
                        tre_Node.ImageIndex = 1;
                        tre_Node.SelectedImageIndex = 1;
                        tre_ChildNode.Nodes.Add(tre_Node);
                    }
                }
            }

            tre_VisionsList.ExpandAll();
        }



        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }



        private void AboutSRMForm_Load(object sender, EventArgs e)
        {
            lbl_SoftwareVersion.Text = Application.ProductVersion;

            FillTree();
        }

        
    }
}