using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common
{
    public class ProcessTh
    {
        public static List<string> arrDifferentThreadName = new List<string>();
        public static List<int> arrThreadHexCore = new List<int>();

        public static List<string> GetThreadsName(string strProcessName)
        {
            List<string> arrThreadName = new List<string>();
            Process[] runningProcess = Process.GetProcesses();
            foreach (Process proc in runningProcess)
            {
                if (proc.ProcessName.IndexOf(strProcessName) >= 0)
                {
                    int intThreadCount = proc.Threads.Count;
                    for (int i = 0; i < intThreadCount; i++)
                    {
                        arrThreadName.Add(proc.Threads[i].Id.ToString());
                    }
                    break;
                }
            }

            return arrThreadName;
        }

        public static void GetDifferentThreadsName(List<string> arrThreadNameSource, List<string> arrThreadNameFilter, string strCode, int intHexCore)
        {
            List<string> arrThreadName = new List<string>();
            for (int a = 0; a < arrThreadNameSource.Count; a++)
            {
                bool blnFound = false;
                for (int b = 0; b < arrThreadNameFilter.Count; b++)
                {
                    if (arrThreadNameSource[a] == arrThreadNameFilter[b])
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (!blnFound)
                {
                    arrDifferentThreadName.Add(arrThreadNameSource[a]);
                    arrThreadHexCore.Add(intHexCore);
                }
            }
        }

    }
}
