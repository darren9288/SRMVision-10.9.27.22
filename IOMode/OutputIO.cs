using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace IOMode
{
    public class OutputIO
    {
        #region Member Variables

        // 2019 06 11 - change default value from 0 to -1 to prevent no exist IO crash with first IO [cardNo 0, channel 0 and bit 0]
        private int m_CardNo = -1;  
        private int m_Channel = -1;
        private int m_Bit = -1;
        private string m_Name = "";

        #endregion


        public OutputIO(string name)
        {
            m_Name = name;
        }


        public int CardNo
        {
            get { return m_CardNo; }
            set { m_CardNo = value; }
        }

        public int Channel
        {
            get { return m_Channel; }
            set { m_Channel = value; }
        }

        public int Bit
        {
            get { return m_Bit; }
            set { m_Bit = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public bool IsOn()
        {

#if(RELEASE || RTXRelease || Release_2_12)
            return (IO.GetOutPortBitStatus(m_CardNo, m_Channel, m_Bit) == 1);
#else
            return false;
#endif
        }

        public bool IsOff()
        {

#if(RELEASE || RTXRelease || Release_2_12)
            return (IO.GetOutPortBitStatus(m_CardNo, m_Channel, m_Bit) == 0);
#else
            return false;
#endif
        }

        public void SetOn()
        {

#if (RELEASE || RTXRelease || Release_2_12)
            IO.OutPort(m_CardNo, m_Channel, m_Bit, 1);
#endif
        }

        public void SetOff()
        {

#if (RELEASE || RTXRelease || Release_2_12)
            IO.OutPort(m_CardNo, m_Channel, m_Bit, 0);
         
#endif
        }

        public void SetOn(string strTrackName)
        {
            //STTrackLog.WriteLine(strTrackName + ", [" + m_CardNo.ToString() + ", " + m_Channel.ToString() + ", " + m_Bit.ToString() + ", ON");

#if (RELEASE || RTXRelease || Release_2_12)
            IO.OutPort(m_CardNo, m_Channel, m_Bit, 1);
#endif
        }

        public void SetOff(string strTrackName)
        {
            //STTrackLog.WriteLine(strTrackName + ", [" + m_CardNo.ToString() + ", " + m_Channel.ToString() + ", " + m_Bit.ToString() + ", OFF");

#if (RELEASE || RTXRelease || Release_2_12)
            IO.OutPort(m_CardNo, m_Channel, m_Bit, 0);

#endif
        }
    }
}
