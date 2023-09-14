using System;
using System.Collections.Generic;
using System.Text;

namespace SharedMemory
{
    public class LightSource
    {
        public string ref_strCommPort;
        public string ref_strType;
        public int ref_intChannel;
        public int ref_intValue;
        public int ref_intPortNo;
        public int ref_intSeqNo;
        public List<int> ref_arrImageNo;
        public List<int> ref_arrValue;
        public int ref_PHValue;
        public int ref_EmptyValue;
    }
}
