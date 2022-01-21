using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FTP.Common
{
    public class SocketCommunication
    {
        private readonly Socket _socket;
        private byte[] _buffer;
        private readonly StringBuilder _sb = new StringBuilder();
        private string _response;
        private string _fileName;

        private readonly ManualResetEvent _receiveDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _sendDone = new ManualResetEvent(false); 
        
        public SocketCommunication(Socket socket)
        {
            _socket = socket;
        }
        
        public string SendAndReceive(string message)
        {
            Send(message);
            _sendDone.WaitOne();
            return Receive();
        }
        
        public void Send(string data)
        {
            var byteData = Encoding.ASCII.GetBytes(data);
            _socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, _socket);  
        }  
  
        private void SendCallback(IAsyncResult ar) {  
            try {
                var bytesSent = _socket?.EndSend(ar);
                _sendDone.Set();  
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }

        public void SendFile(string path)
        {
            _socket.BeginSendFile(path, null,null, 0, AsynchronousFileSendCallback, _socket);
        }

        private void AsynchronousFileSendCallback(IAsyncResult ar)
        {
            _socket.EndSendFile(ar);
            _sendDone.Set();  
        }
        
        public string Receive() {  
            _buffer = new byte[1024];
            try {
                if (_socket != null)
                {
                    _receiveDone.Reset();
                    _socket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
                    _receiveDone.WaitOne();
                    return _response;
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());  
            }
            return string.Empty;
        }

        private void ReceiveCallback( IAsyncResult ar ) {
            try {
                var bytesRead = _socket.EndReceive(ar);
                _sb.Append(Encoding.ASCII.GetString(_buffer,0,bytesRead));
                
                if (_socket.Available > 0) {
                    _socket.BeginReceive(_buffer,0,_buffer.Length,0, ReceiveCallback, null);  
                } else {
                    if (_sb.Length > 1) {
                        _response = _sb.ToString();
                        _sb.Clear();
                    }
                    _receiveDone.Set();
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());  
            }
        }
        
        public string ReceiveFile(string name) {  
            _buffer = new byte[1024];
            _fileName = name;
            Console.WriteLine($"FILENAME: {_fileName}");
            try {
                if (_socket != null)
                {
                    _receiveDone.Reset();
                    _socket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveFileCallback, null);
                    _receiveDone.WaitOne();
                    return "file download succesful.";
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());  
            }

            return "file download not succesful.";
        }
        
        private void ReceiveFileCallback(IAsyncResult ar) 
        { 
            BinaryWriter writer;
            var bytesRead = _socket.EndReceive(ar);
            Console.WriteLine($"byte read {bytesRead}");
            Console.WriteLine($"byte available {_socket.Available}");
            
            if (!File.Exists(_fileName)) 
            {
                Console.WriteLine($"file dont exist {_fileName}");
                writer = new BinaryWriter(File.Open(_fileName, FileMode.Create)); 
            } 
            else 
            {
                Console.WriteLine($"file exist {_fileName}");
                writer = new BinaryWriter(File.Open(_fileName, FileMode.Append)); 
            }

            writer.Write(_buffer, 0, bytesRead); 
            writer.Flush();
            writer.Close();

            if (_socket.Available > 0)
            {
                try 
                { 
                    _socket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveFileCallback, null); 
                }
                catch
                {
                    // ignored
                }
            }
            else 
            {
                _receiveDone.Set(); 
            } 
        }
    }
}