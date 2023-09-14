using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Common;

namespace Common
{
    public class TCPServer
    {
        #region Delegate Event

        public delegate void ReceivedDataHandle(string strMessage);
        public event ReceivedDataHandle ReceiveEvent;

        #endregion

        #region Member Variables
        char[] m_arrSpecialChars = new char[] { '\r', '\n' }; //2021-10-27 ZJYEOH : Carsem machine will have these special chars in the start or end of the message

        private string m_strDataReceived = "";
    
        private TrackLog m_Log = new TrackLog();
        private ArrayList m_alClients = ArrayList.Synchronized(new ArrayList());
        private Socket m_socServer;
        private IPEndPoint ipepLocal;
        public AsyncCallback m_acbWorker;

        #endregion



        public TCPServer()
        {
        }


        public void AddNewConnection(Socket soc)
        {
            try
            {
                // Program blocks on Accept() until a client connects.
                SocketClient client = new SocketClient(soc);
                m_alClients.Add(client);

                // Convert to byte array and send.
                string strSend = "Client joined";
                if (ReceiveEvent != null) ReceiveEvent(strSend);

                client.SetupReceiveCallback(this);
            }
            catch (Exception ex)
            {
                m_Log.WriteLine("TCPServer AddNewConnection: " + ex.ToString());
            }
        }

        public void CloseConnection()
        {
            STTrackLog.WriteLine("CloseConnection");
            if (m_socServer != null)
            {
                STTrackLog.WriteLine("m_socServer != null");
                if (m_alClients != null && m_alClients.Count > 0)
                {
                    foreach (SocketClient socClient in m_alClients)
                    {
                        try
                        {
                            STTrackLog.WriteLine(socClient.ClientSocket.ToString());
                            socClient.ClientSocket.Close();
                            STTrackLog.WriteLine("ClientSocket.Close");
                        }
                        catch
                        {
                            m_Log.WriteLine("TCPServer Unable to Close Client Connection");
                        }
                    }
                }

                try
                {
                    if (m_alClients != null && m_alClients.Count > 0)
                        m_alClients.Clear();
                    STTrackLog.WriteLine("m_alClients.Clear");
                    if (m_socServer.Connected)
                        m_socServer.Shutdown(SocketShutdown.Both);
                    m_socServer.Close();
                    STTrackLog.WriteLine("m_socServer.Close");
                    m_socServer = null;
                }
                catch (Exception ex)
                {
                    STTrackLog.WriteLine("TCP Server CloseConnection: " + ex.ToString());
                    m_Log.WriteLine("TCP Server CloseConnection: " + ex.ToString());
                }
            }
        }

        public void ConnectToClient()
        {
            if (ipepLocal == null)
                return;

            m_socServer.BeginConnect(IPAddress.Any, ipepLocal.Port, new AsyncCallback(ConnectCallBack), m_socServer);
        }

        public void Init(int intPort, int intTimeOut)
        {
            try
            {
                STTrackLog.WriteLine("Init");

                if (m_socServer == null)
                    STTrackLog.WriteLine("A socServer is NULL");
                else
                {
                    STTrackLog.WriteLine("A socServer is Not NULL. SendTimeOout = " + m_socServer.SendTimeout.ToString() + ", " + intTimeOut.ToString());

                }

                if (ipepLocal == null)
                    STTrackLog.WriteLine("A ipepLocal is NULL");
                else
                {
                    STTrackLog.WriteLine("A ipepLocal is NOT NULL. ipepLocal = " + ipepLocal.Port.ToString() + ", " + intPort);
                }

                if (m_socServer == null || m_socServer.SendTimeout != intTimeOut || ipepLocal.Port != intPort)
                {
                    STTrackLog.WriteLine("Port " + intPort.ToString());
                    CloseConnection();
                    STTrackLog.WriteLine("After Close Connection");
                    m_socServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //IPAddress ipa = IPAddress.Parse("10.0.0.4");
                    ipepLocal = new IPEndPoint(IPAddress.Any, intPort);//IPAddress.Any
                    STTrackLog.WriteLine("IP " + ipepLocal.Address.ToString());
                    STTrackLog.WriteLine("After Declare Socket and IPEndPoint");
                    m_socServer.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                    STTrackLog.WriteLine("SetSocketOption");
                    m_socServer.Bind(ipepLocal);
                    STTrackLog.WriteLine("Bind IP");
                    m_socServer.Listen(100);
                    STTrackLog.WriteLine("Listen");
                    m_socServer.BeginAccept(new AsyncCallback(OnConnect), m_socServer);
                    STTrackLog.WriteLine("BeginAccept");
                    m_socServer.SendTimeout = intTimeOut;
                }
            }
            catch (SocketException se)
            {
                STTrackLog.WriteLine("TCP Server Init Connection Error: " + se.ToString());
                m_Log.WriteLine("TCP Server Init Connection Error: " + se.ToString());
            }          
        }
        public void Init(int intPort, int intTimeOut, string IPAdd)
        {
            try
            {
                STTrackLog.WriteLine("Init");

                if (m_socServer == null)
                    STTrackLog.WriteLine("B socServer is NULL");
                else
                {
                    STTrackLog.WriteLine("B socServer is Not NULL. SendTimeOout = " + m_socServer.SendTimeout.ToString() + ", " + intTimeOut.ToString());

                }

                if (ipepLocal == null)
                    STTrackLog.WriteLine("B ipepLocal is NULL");
                else
                {
                    STTrackLog.WriteLine("B ipepLocal is NOT NULL. ipepLocal = " + ipepLocal.Port.ToString() + ", " + intPort);
                }

                if (m_socServer == null || m_socServer.SendTimeout != intTimeOut || ipepLocal.Port != intPort)
                {
                    STTrackLog.WriteLine("Port " + intPort.ToString());
                    CloseConnection();
                    STTrackLog.WriteLine("After Close Connection");
                    m_socServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress ipa = IPAddress.Parse(IPAdd);
                    ipepLocal = new IPEndPoint(ipa, intPort);//IPAddress.Any
                    STTrackLog.WriteLine("IP " + ipepLocal.Address.ToString());
                    STTrackLog.WriteLine("After Declare Socket and IPEndPoint");
                    m_socServer.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                    STTrackLog.WriteLine("SetSocketOption");
                    m_socServer.Bind(ipepLocal);
                    STTrackLog.WriteLine("Bind IP");
                    m_socServer.Listen(100);
                    STTrackLog.WriteLine("Listen");
                    m_socServer.BeginAccept(new AsyncCallback(OnConnect), m_socServer);
                    STTrackLog.WriteLine("BeginAccept");
                    m_socServer.SendTimeout = intTimeOut;
                }
            }
            catch (SocketException se)
            {
                STTrackLog.WriteLine("TCP Server Init Connection Error: " + se.ToString());
                m_Log.WriteLine("TCP Server Init Connection Error: " + se.ToString());
            }
        }

        public void Send(string strData, bool blnOnTracking)
        {
            // Check connection
            //if (!m_socServer.Connected)
            //{
            //    ConnectToClient();
            //}

            byte[] bySend = new byte[0];
            try
            {
                bySend = Encoding.ASCII.GetBytes(strData);

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Send: " + ex.ToString());
            }

            // Send the recieved data to all clients (including sender for echo)
            foreach (SocketClient socClient in m_alClients)
            {
                try
                {
                    if (blnOnTracking) STTrackLog.WriteLine("Sent : " + strData);
                    socClient.ClientSocket.Send(bySend);
                }
                catch
                {
                    // If the send fails the close the connection
                    string strMessage = "Send to client failed.";
                    if (ReceiveEvent != null)
                        ReceiveEvent(strMessage);
                    else
                        m_Log.WriteLine("TCP Server Sending Error: " + strMessage);

                    socClient.ClientSocket.Close();
                    m_alClients.Remove(socClient);
                    return;                
                }
            }

        }



        //function to check if the port sent is in use
        //returns false if the port is in use or else return true
        private bool PortAvailable(int port)
        {
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();
                while (myEnum.MoveNext())
                {
                    TcpConnectionInformation tcpi = (TcpConnectionInformation)myEnum.Current;

                    if (tcpi.LocalEndPoint.Port == port)
                        return false;
                }
            }
            catch (Exception ex)
            {
                m_Log.WriteLine("TCPServer PortAvailable: " + ex.ToString());
            }

           return true;
        }


        private void OnConnect(IAsyncResult asyn)
        {
            try
            {
                // Socket was the passed in object
                Socket sock = (Socket)asyn.AsyncState;
                sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                AddNewConnection(sock.EndAccept(asyn));
                sock.BeginAccept(new AsyncCallback(OnConnect), sock);
            }
            catch (ObjectDisposedException)
            {                
                if (ReceiveEvent != null)
                    ReceiveEvent("OnConnect: Socket has been closed");
                else
                    m_Log.WriteLine("TCPServer OnConnect: Socket has been closed");
            }
            catch (SocketException se)
            {
                m_Log.WriteLine("TCPServer Connection Error:" + se.ToString());
            }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {

        }

        public void OnReceivedData(IAsyncResult iar)
        {
            try
            {
                SocketClient client = (SocketClient)iar.AsyncState;
                byte[] byData = client.GetReceivedData(iar);
     
                // If no data was recieved then the connection is probably dead
                if (byData.Length < 1)
                {
                    string strMessage = "Client disconnected.";
                    if (ReceiveEvent != null)
                        ReceiveEvent(strMessage);
                    else
                        m_Log.WriteLine("TCPServer Receiving Error:" + strMessage);

                    client.ClientSocket.Close();
                    m_alClients.Remove(client);

                    return;
                }
                
                char[] chars = new char[byData.Length];
                Decoder d = Encoding.ASCII.GetDecoder();
                int charLen = d.GetChars(byData, 0, byData.Length, chars, 0);
                m_strDataReceived += new String(chars);

                //2021-10-27 ZJYEOH : need trim special chars in the start and end of the message
                m_strDataReceived = m_strDataReceived.TrimStart(m_arrSpecialChars);
                m_strDataReceived = m_strDataReceived.TrimEnd(m_arrSpecialChars);

                if ((m_strDataReceived.StartsWith("<") & m_strDataReceived.EndsWith(">")) || m_strDataReceived.IndexOf("<") < 0)
                {
                    if (ReceiveEvent != null) ReceiveEvent(m_strDataReceived);
                    m_strDataReceived = "";
                }

                client.SetupReceiveCallback(this);
            }
            catch (Exception ex)
            {
                m_Log.WriteLine("TCPServer SRM TCP/IP Error:" + ex.ToString());
            }
        }
    }
}
