using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Common
{
    public class CopyFiles
    {
        public bool CopyAllFiles(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);
              
                if (!dir.Exists)
                {
                    return false;
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(Destination, file.Name);
                    file.CopyTo(temppath, true);
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles(subdir.FullName, temppath);
                }

            }
            catch (Exception ex)
            {
               
            }
            return true;
        }
        public void CopyAllImageFiles(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);
               
                if (!dir.Exists)
                {
                    Directory.CreateDirectory(Source);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Extension == ".bmp" && (file.Name.Contains("OriTemplate") || file.Name.Contains("MarkTemplate") || file.Name.Contains("PocketTemplate")))
                    {
                        string temppath = Path.Combine(Destination, file.Name);
                        file.CopyTo(temppath, true);
                    }

                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles(subdir.FullName, temppath);
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}

