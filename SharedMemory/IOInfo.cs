using System;

namespace SharedMemory
{

    public struct IOScanInfo
    {
        public byte BytePrev;   // the previous byte of the channel
        public byte ItemBit;   // bits in the channel exist in the input scan
        public int CardNo;   // the IO card number
        public int Channel;   // the channel number
        public int[] ItemNo;   // item number in the listctrl of the bits

        public IOScanInfo(int value)
        {
            BytePrev = 0;
            ItemBit = 0;
            CardNo = 0;
            Channel = value;
            ItemNo = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }

    public struct IOInfo
    {
        public int CardNo;
        public int Channel;
        public int Bit;
        public string Number;
        public string Type;
        public string Description;
    }
}
