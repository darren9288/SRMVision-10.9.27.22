using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    internal class SocketClient
    {
        private Socket m_socClient;
        private byte[] m_byBuffer = new byte[1024];

        public SocketClient(Socket sock)
        {
            m_socClient = sock;
        }


        public Socket ClientSocket
        {
            get { return m_socClient; }
        }

        public void SetupReceiveCallback(TCPServer server)
        {
            try
            {
                AsyncCallback receiveData = new AsyncCallback(server.OnReceivedData);
                m_socClient.BeginReceive(m_byBuffer, 0, m_byBuffer.Length, SocketFlags.None, receiveData, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Receive callback setup failed! {0}", ex.Message);
            }
        }

        public byte[] GetReceivedData(IAsyncResult iar)
        {
            int iBytesRec = 0;
            try
            {
                iBytesRec = m_socClient.EndReceive(iar);
            }
            catch { }

            byte[] byReturn = new byte[iBytesRec];
            Array.Copy(m_byBuffer, byReturn, iBytesRec);

            return byReturn;
        }
    }
}
