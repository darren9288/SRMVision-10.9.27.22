using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
//using VTComIOMn;


namespace IOMode
{
    //public class VieUSB8IO
    //{
    //    CVTComIOMn mUSB8IO = new CVTComIOMn();
    //    VTComIOStatus mUSB8IO_Status = new VTComIOStatus();
    //    int iRet = 0;  // Initialization

    //    //thread handler
    //    Thread thrInput;

    //    int m_uiOutputData;
    //    int m_uiInputData;

    //    //flag to loop input data
    //    bool bLoopInput = false;

    //    private int m_intSelectedCOM = 0;
    //    private string m_strErrorMessage = "";
    //    private bool[] m_arrInp = new bool[8];
    //    private bool[] m_arrOutp = new bool[8];
    
    //    public VieUSB8IO(int intSelectedCOM)
    //    {
    //        m_intSelectedCOM = intSelectedCOM;
    //    }

    //    public bool Connect()
    //    {
    //        iRet = mUSB8IO.Connect(m_intSelectedCOM);   //start serial com
    //        if (iRet != VTComIOStatus.VT_SUCCESS)
    //        {
    //            m_strErrorMessage = "Failed to connect IO USB Port!";
    //            return false;
    //        }

    //        thrInput = new Thread(ReadInput);
    //        if (!thrInput.IsAlive)  //check if thread is started
    //        {
    //            bLoopInput = true;
    //            thrInput.Start();

    //            if (!thrInput.IsAlive)
    //            {
    //                bLoopInput = false;
    //                m_strErrorMessage = "Failed to create ReadInput thread!";
    //                iRet = mUSB8IO.Disconnect();
    //                return false;
    //            }
    //        }

    //        // reset all output
    //        m_uiOutputData = 0x00;
    //        iRet = mUSB8IO.SetOutput((byte)m_uiOutputData);

    //        return true;
    //    }


    //    public void Disconnect()
    //    {
    //        //set flag to false
    //        bLoopInput = false;

    //        //reset the output
    //        m_uiOutputData = 0x00;

    //        //terminate the thread
    //        thrInput.Abort();

    //        //disconnect the serial COM
    //        iRet = mUSB8IO.Disconnect();
    //    }

    //    public void SetOutput(byte outputByte)
    //    {
    //        m_uiOutputData = Convert.ToInt32(outputByte);

    //        iRet = mUSB8IO.SetOutput(outputByte);
    //    }

    //    public void GetOutput(ref byte outputByte)
    //    {
    //        outputByte = (byte)m_uiOutputData;
    //    }

    //    public void GetInput(ref uint uiInputData)
    //    {
    //        uiInputData = (uint)m_uiInputData;
    //    }

    //    public void OutputSetON(int intBit, bool blnTrue)
    //    {
    //        if (blnTrue)
    //            m_uiOutputData |= (0x01 << intBit);
    //        else
    //            m_uiOutputData &= ~(0x01 << intBit);

    //        iRet = mUSB8IO.SetOutput((byte)m_uiOutputData);
    //    }

    //    public bool OutputIsON(int intBit)
    //    {
    //        return ((m_uiOutputData & (0x01 << intBit)) > 0);
    //    }

    //    public bool InputIsON(int intBit)
    //    {
    //        return ((m_uiInputData & (0x01 << intBit)) > 0);
    //    }

    //    private void ReadInput()
    //    {
    //        //loop until bLoopInput is false
    //        while (bLoopInput)
    //        {
    //            Byte uiInputData = 0;

    //            //Get input
    //            iRet = mUSB8IO.GetInput(ref uiInputData);

    //            m_uiInputData = Convert.ToInt32(uiInputData);

    //            Thread.Sleep(1);
    //        }
    //    }

    //}



    class VTComIOStatus
    {
        //Function response code
        public const Int32 VT_SUCCESS = 0x00;            //Success with no error
        public const Int32 VERR_TIMEOUT = -2;            //Function response time out
        public const Int32 VERR_COM_FAILED = -17;        //Serial Communication Failure
        public const Int32 VERR_BOARD_CONNECTED = -22;   //Target board that attempt to connect was already connected 

    }
}
