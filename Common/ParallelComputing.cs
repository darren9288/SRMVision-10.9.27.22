using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Common
{
    public class ParallelComputing
    {
        #region Delegate events

        /// <summary>
        /// Delegate defining for-loop's body.
        /// </summary>
        /// <param name="intIndex">Loop's index.</param>
        public delegate void ForLoopBody(int intIndex);

        #endregion

        #region Member Variables

        private static int m_intThreadCount = Environment.ProcessorCount;  // number of thread on machine
        private static volatile ParallelComputing m_objInstance;  //indicates that a field can be modified in the program by operating system, the hardware, or a concurrently executing thread.

        // loop's body and its current and stop index
        private int m_intCurrentIndex;
        private int m_intStopIndex;
        private ForLoopBody m_eventLoopBody;

        private static object m_objSync = new Object();           // object used for synchronization
        private AutoResetEvent[] m_autoJobAvailable = null;      //this communication concerns a resource to which threads need exclusive access. 
        private ManualResetEvent[] m_manualThreadIdle = null;  //this communication concerns a task which one thread must complete before other threads can proceed. 
        private Thread[] m_thThreads = null;
        
        #endregion

        #region Properties

        /// <summary>
        /// Number of threads used for parallel computations.
        /// By default the property is set to number of CPU's processor in the system
        /// </summary>
        public static int ThreadsCount
        {
            get { return m_intThreadCount; }
            set
            {
                lock (m_objSync)
                {
                    m_intThreadCount = value;
                }
            }
        }
        #endregion



        /// <summary>
        /// Private constructor to avoid class instantiation
        /// </summary> 
        private ParallelComputing() 
        {
        }



        /// <summary>
        ///  Get instace of the ParallelComputing class
        /// </summary>
        private static ParallelComputing Instance
        {
            get
            {
                if (m_objInstance == null)
                {
                    m_objInstance = new ParallelComputing();
                    m_objInstance.Initialize();
                }
                else
                {
                    if (m_objInstance.m_thThreads.Length != m_intThreadCount)
                    {
                        m_objInstance.Terminate();
                        m_objInstance.Initialize();
                    }
                }
                return m_objInstance;
            }
        }


        /// <summary>
        /// Initialize instance by creating required number of threads and synchronization objects
        /// </summary> 
        private void Initialize()
        {
            m_autoJobAvailable = new AutoResetEvent[m_intThreadCount];       // signal about available job
            m_manualThreadIdle = new ManualResetEvent[m_intThreadCount];   // signal about available thread
            m_thThreads = new Thread[m_intThreadCount];

            for (int i = 0; i < m_intThreadCount; i++)
            {
                m_autoJobAvailable[i] = new AutoResetEvent(false);                  //true if the initial state is signaled
                m_manualThreadIdle[i] = new ManualResetEvent(true);

                //List<string> arrThreadNameBF = new List<string>();
                //List<string> arrThreadNameAF = new List<string>();
                //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

                m_thThreads[i] = new Thread(new ParameterizedThreadStart(WorkerThread));
                m_thThreads[i].Priority = ThreadPriority.Lowest;
                m_thThreads[i].IsBackground = true;
                m_thThreads[i].Start(i);

                //Thread.Sleep(500);
                //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
                //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "2_" + i.ToString(), 0x02);
            }
        }
        /// <summary>
        /// Terminate all threads used for parallel computations and close all synchronization objects
        /// </summary> 
        private void Terminate()
        {

            m_eventLoopBody = null;               // finish thread by setting null loop body and signaling about available work

            for (int i = 0, m_intThreadsCount = m_thThreads.Length; i < m_intThreadsCount; i++)
            {
                m_autoJobAvailable[i].Set();      // release a waiting thread
                m_thThreads[i].Join();                  // wait for thread termination

                m_autoJobAvailable[i].Close();
                m_manualThreadIdle[i].Close();
            }

            m_autoJobAvailable = null;
            m_manualThreadIdle = null;
            m_thThreads = null;
        }
        /// <summary>
        /// Threads that perform parallel computations in loop
        /// </summary>
        /// <param name="index">Index of thread</param>
        private void WorkerThread(object intIndex)
        {
            int intThreadIndex = (int)intIndex;
            int intLocalIndex = 0;

            while (true)
            {
                // wait until there is job to do
                m_autoJobAvailable[intThreadIndex].WaitOne();    //If a thread calls WaitOne while the AutoResetEvent is in the signaled state, the thread does not block. The AutoResetEvent releases the thread immediately and returns to the non-signaled state. 

                if (m_eventLoopBody == null)
                    break;

                while (true)
                {
                    // get local index incrementing global loop's current index
                    intLocalIndex = Interlocked.Increment(ref m_intCurrentIndex);

                    if (intLocalIndex >= m_intStopIndex)
                        break;


                    m_eventLoopBody(intLocalIndex);           // run loop's body
                }
                
                m_manualThreadIdle[intThreadIndex].Set();   // This thread complete the task and release all waiting threads
            }
        }


        /// <summary>
        /// Executes a for-loop in which iterations may run in parallel / different thread. The number of iterations is equal to <b>stop - start</b>
        /// </summary>
        /// <param name="start">Loop's start index.</param>
        /// <param name="stop">Loop's stop index.</param>
        /// <param name="loopBody">Loop's body.</param>
        public static void For(int intStart, int intStop, ForLoopBody eventLoop)
        {
            lock (m_objSync)
            {
                // get instance of parallel computation manager
                ParallelComputing m_objInstance = Instance;

                m_objInstance.m_intCurrentIndex = intStart - 1;
                m_objInstance.m_intStopIndex = intStop;
                m_objInstance.m_eventLoopBody = eventLoop;

                // signal about available job for all threads and mark them busy
                for (int i = 0; i < m_intThreadCount; i++)
                {
                    m_objInstance.m_manualThreadIdle[i].Reset();
                    m_objInstance.m_autoJobAvailable[i].Set();
                }

                // wait until all threads become idle
                for (int i = 0; i < m_intThreadCount; i++)
                {
                    m_objInstance.m_manualThreadIdle[i].WaitOne();
                }
            }
        }

    }
}
