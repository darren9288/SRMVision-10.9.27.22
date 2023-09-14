using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;

namespace SRMVision
{
    static class Program
    {
        #region Member Variables

        private static Mutex m_mutex1;
        private static Mutex m_mutex2;

        #endregion


        /// <summary>
        /// The main entry point for the application.
        /// Make sure that only 1 SRM software is being triggered
        /// Make sure there are camera(s) connected to system at least 1
        /// Make sure all camera(s) had been registered.
        /// Make sure Option.xml file is attached inside project
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool bOut = false;
            m_mutex1 = new Mutex(true, "SRMVision", out bOut);
            if (!bOut)
            {
                SRMMessageBox.Show("You Already Have Another SRM Vision Program Running \n Only One Program Is Allowed To Run", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            m_mutex2 = new Mutex(true, "SVGRestoreBackUp", out bOut);
            if (!bOut)
            {
                MessageBox.Show("Please Close SVG Restore Backup Program To Proceed!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            bool blnRegister = false;
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            if (subKey.GetValue("Registration", 0).Equals(0))
            {
                CameraRegistryForm visionForm = new CameraRegistryForm();
                visionForm.TopMost = true;
                if (visionForm.ShowDialog() != DialogResult.OK)
                    return;

                CameraRegistrySettingForm visionForm2 = new CameraRegistrySettingForm();
                visionForm.TopMost = true;
                if (visionForm2.ShowDialog() != DialogResult.OK)
                    return;

                LightChannelRegistryForm lightForm = new LightChannelRegistryForm();
                lightForm.TopMost = true;
                if (lightForm.ShowDialog() != DialogResult.OK)
                    return;

                blnRegister = true;
            }

            string strMissingFile = CheckMissingFile();
            if (strMissingFile != "")
            {
                SRMMessageBox.Show("SRMVision program detected missing required file :\n" +
                    strMissingFile + "SRMVision program will exit now.",
                    "SRM", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            
            Application.Run(new MainForm(blnRegister));
        }


        /// <summary>
        /// Check whether all required folders and files are completed inside software
        /// </summary>
        /// <returns>If not, missing files' and folders' name will be returned</returns>
        private static string CheckMissingFile()
        {
            string strFileName = "";

            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Option.xml")))
                strFileName += "- Option.xml\n";
            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "boardslist.xml")))
                strFileName += "- boardslist.xml\n";
            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Access\\Setting.mdb")))
                strFileName += "- Access\\Setting.mdb\n";
            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\History.mdb")))
                strFileName += "- Data\\History.mdb\n";
            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Report\\LotlyReport.rpt")))
                strFileName += "-Report\\LotlyReport.rpt\n";
             if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Report\\GRRReport.rpt")))
                strFileName += "-Report\\GRRReport.rpt\n";
            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Report\\CPKReport.rpt")))
                strFileName += "-Report\\CPKReport.rpt\n";

            //if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Misc\\White.bmp")))
            //    strFileName += "- Misc\\White.bmp\n";
            //if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Misc\\Black.bmp")))
            //    strFileName += "- Misc\\Black.bmp\n";
            if (!(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Misc\\DontCare.png")))
                strFileName += "- Misc\\DontCare.png\n";

            //if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "SaveImage"))
            //    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "SaveImage");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "LotReport"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "LotReport");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\Default"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\Default");

        

            return strFileName;
        }
       

    }
}