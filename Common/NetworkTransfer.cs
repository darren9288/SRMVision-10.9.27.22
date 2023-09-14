using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Common
{
    public class NetworkTransfer
    {

        private static void CloneDirectory(string root, string dest)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                string dirName = Path.GetFileName(directory);
                if (!Directory.Exists(Path.Combine(dest, dirName)))
                {
                    Directory.CreateDirectory(Path.Combine(dest, dirName));
                }
                CloneDirectory(directory, Path.Combine(dest, dirName));
            }

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            foreach (var file in Directory.GetFiles(root))
            {
                string newFile = Path.Combine(dest, Path.GetFileName(file));
                FileInfo old = new FileInfo(file);
                old.CopyTo(newFile, true);
            }
        }

        public static bool TransferFile_MappedNetwork(string root, string dest)
        {
            try
            {
                STTrackLog.WriteLine("TransferFile_MappedNetwork from " + root + " to " + dest);

                CloneDirectory(root, dest);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("NetworkTransfer.cs -> TransferFile_MappedNetwork -> Ex = " + ex.ToString());
                SRMMessageBox.Show("NetworkTransfer.cs -> TransferFile_MappedNetwork -> Ex = " + ex.ToString());
            }

            return true;
        }

        public static bool IsConnectionPass(string strHostIp)
        {
            bool blnConnectPass = false;
            int intConnectCount = 0;
            while (true)
            {
                if (NetworkTransfer.IsNetworkConnectionON(strHostIp, false))
                {
                    blnConnectPass = true;
                    break;
                }
                else
                {
                    STTrackLog.WriteLine("Fail server try: " + intConnectCount.ToString());
                }

                if (intConnectCount > 3)
                {
                    break;
                }

                intConnectCount++;

                System.Threading.Thread.Sleep(10);
            }

            if (blnConnectPass)
                return true;
            else
                return false;
        }

        public static bool IsNetworkConnectionON(string ipAddress, bool blnDisplayAlarmMessageBox)
        {
            try
            {
                Ping x = new Ping();
                PingReply reply = x.Send(IPAddress.Parse(ipAddress));

                if (reply.Status == IPStatus.Success)
                    return true;
                else
                {
                    if (blnDisplayAlarmMessageBox)
                        SRMMessageBox.Show("Ping Host IP Address Fail", "Network Error");
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }
     
    }
}
