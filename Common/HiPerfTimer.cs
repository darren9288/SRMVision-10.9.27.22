using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;


namespace Common
{
    public class HiPerfTimer
    {
        #region DllImport

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        #endregion

        #region Member Variables

        private long m_longStartTime;
        private long m_longStopTime;
        private long m_longFreq;
        private float m_fDuration;

        #endregion


        public HiPerfTimer()
        {
            m_longStartTime = 0;
            m_longStopTime = 0;
            m_longFreq = 0;

            if (QueryPerformanceFrequency(out m_longFreq) == false)
            {
                throw new Win32Exception("Timer not supported"); // timer not supported
            }
        }



        /// <summary>
        /// Frequency of timer (no counts in one second on this machine)
        /// </summary>
        ///<returns>long - Frequency</returns>
        public long Frequency
        {
            get
            {
                QueryPerformanceFrequency(out m_longFreq);
                return m_longFreq;
            }
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Start()
        {
            QueryPerformanceCounter(out m_longStartTime);
            return m_longStartTime;
        }

        /// <summary>
        /// Stop timer 
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Stop()
        {
            QueryPerformanceCounter(out m_longStopTime);
            m_fDuration = (float)(m_longStopTime - m_longStartTime) / (float)m_longFreq * 1000;
            return m_longStopTime;
        }



        /// <summary>
        /// Return the duration of the timer (in seconds)
        /// </summary>
        /// <returns>double - duration</returns>
        public float Duration
        {
            get
            {
                return m_fDuration;
            }
        }

        public void Reset()
        {
            m_fDuration = 0;
        }

        /// <summary>
        /// Return the timing of the timer (in seconds)
        /// </summary>
        /// <returns>double - timing</returns>
        public float Timing
        {
            
            get
            {
                long longNow;
                QueryPerformanceCounter(out longNow);
                return (float)(longNow - m_longStartTime) / (float)m_longFreq * 1000;
            }
        }
        


        /// <summary>
        /// Phisically sleep the cpu. Warning!!! will cause the whole program idle for the Period Sleep.
        /// </summary>
        /// <param name="iMilliseconds"></param>
        public void dpeSleep(int nMilliseconds)
        {
            if (nMilliseconds < 20)
            {
                long longStartTime;
                long longNowTime;
                double dElapsed;
                bool bIn = true;
                QueryPerformanceCounter(out longStartTime);

                while (bIn)
                {
                    QueryPerformanceCounter(out longNowTime);
                    dElapsed = (double)(longNowTime - longStartTime) / (double)m_longFreq;
                    if (dElapsed * 1000 > nMilliseconds)
                        bIn = false;
                }
            }
        }
    }
}
