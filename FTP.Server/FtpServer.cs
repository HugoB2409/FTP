using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FTP.Server
{
    public class FtpServer
    {
        private bool _running;
        private const int Port = 8000;
        private readonly IPEndPoint _localEndPoint;
        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        private readonly Socket _listener;
        
        public FtpServer()
        {
            var ipHostInfo = Dns.GetHostEntry("localhost");  
            var ipAddress = ipHostInfo.AddressList[0];  
            _localEndPoint = new IPEndPoint(ipAddress, Port);  
  
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp );  
        }

        public void StartListening()
        { 
            try {
                _listener.Bind(_localEndPoint);  
                _listener.Listen(100);
                _running = true;
                
                while (_running) {
                    _allDone.Reset();  
  
                    Console.WriteLine("Waiting for a connection...");  
                    _listener.BeginAccept(AcceptCallback, _listener);  
  
                    _allDone.WaitOne();  
                }
                
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        { 
            _allDone.Set();
            var socket = _listener.EndAccept(ar);
            Console.WriteLine($"New connection from {socket.RemoteEndPoint}.");
            var client = new Client(socket);
        }

        public void Shutdown()
        {
            _running = false;
            Console.WriteLine("The server will shutdown...");
        }
    }
}
