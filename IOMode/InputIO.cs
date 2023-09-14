using System;
using System.Collections.Generic;
using System.Text;

namespace IOMode
{
    public class InputIO
    {
        #region Member Variables

        private int m_CardNo = -1;
        private int m_Channel = -1;
        private int m_Bit = -1;
        private string m_Name = "";

        #endregion


        public InputIO(string name)
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
            return (IO.InPort(m_CardNo, m_Channel, m_Bit, 1, false));
#else
            return false;
#endif
        }
        public bool IsOn(bool blnDefault)
        {

#if(RELEASE || RTXRelease || Release_2_12)
            return (IO.InPort(m_CardNo, m_Channel, m_Bit, 1, blnDefault));
#else
            return blnDefault;
#endif
        }
        public bool IsOff()
        {

#if(RELEASE || RTXRelease || Release_2_12)
            return (IO.InPort(m_CardNo, m_Channel, m_Bit, 0, false));
#else
            return false; 
#endif
        }


        public bool IsOff(bool blnDefault)
        {

#if(RELEASE || RTXRelease || Release_2_12)
            return (IO.InPort(m_CardNo, m_Channel, m_Bit, 0, blnDefault));
#else
            return blnDefault;
#endif
        }
    }
}
