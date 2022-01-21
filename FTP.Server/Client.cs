using System.Net.Sockets;

namespace FTP.Server
{
    public class Client
    {
        public string Username;
        public string Password;
        public string DefaultRepository;
        public string CurrentRepository;

        public Client(Socket socket)
        {
            var receive = new ReceivePacket(socket, this);
            receive.StartReceiving();
        }
    }
}