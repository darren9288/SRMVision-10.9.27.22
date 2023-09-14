using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common
{
    public class FileSorting : IComparer
    {
        /// <summary>
        /// compare 1 file creation time
        /// </summary>
        /// <param name="x">1st file in directoryInfo format</param>
        /// <param name="y">2nd file in directoryInfo format</param>
        /// <returns>an indication of their relative value</returns>
        public int Compare(object x, object y)
        {
            return -DateTime.Compare(((DirectoryInfo)x).CreationTime, ((DirectoryInfo)y).CreationTime);
        }
        /// <summary>
        /// Compare 2 file creation time and sort in descending 
        /// </summary>
        /// <param name="x">1st file in string format - file path + file name</param>
        /// <param name="y">2nd file in string format - file path + file name</param>
        /// <returns>an indication of their relative value</returns>
        public int CompareCreateDescending(string x, string y)
        {
            DirectoryInfo File1 = new DirectoryInfo(x);
            DirectoryInfo File2 = new DirectoryInfo(y);

            return -DateTime.Compare(File1.CreationTime, File2.CreationTime);
        }
        /// <summary>
        ///  Compare 2 file creation time and sort in ascending
        /// </summary>
        /// <param name="x">1st file</param>
        /// <param name="y">2nd file</param>
        /// <returns>an indication of their relative value</returns>
        public int CompareCreateAscending(string x, string y)
        {
            DirectoryInfo File1 = new DirectoryInfo(x);
            DirectoryInfo File2 = new DirectoryInfo(y);

            return DateTime.Compare(File1.CreationTime, File2.CreationTime);
        }
    }
}
