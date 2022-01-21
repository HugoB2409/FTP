using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FTP.Common;

namespace FTP
{
    public class SocketHelper
    {
        private readonly int _port;
        private readonly string _hostname;
        private Socket _client;
        private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);

        public SocketHelper(string hostname, int port)
        {
            _port = port;
            _hostname = hostname;
        }
        
        public SocketCommunication InitializeSocketCommunication()
        {
            try
            {
                var ipHostInfo = Dns.GetHostEntry(_hostname);  
                var ipAddress = ipHostInfo.AddressList[0];
                var remoteEp = new IPEndPoint(ipAddress, _port);
  
                _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _client.BeginConnect( remoteEp, ConnectCallback, _client);  
                _connectDone.WaitOne(); 
                return new SocketCommunication(_client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void ConnectCallback(IAsyncResult ar) {  
            try {  
                var client = (Socket) ar.AsyncState;
                client?.EndConnect(ar);
                _connectDone.Set();  
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }
        
        public void CloseConnection()
        {
            _client.Shutdown(SocketShutdown.Both);  
            _client.Close(); 
            _client = null;
        }
    }
}