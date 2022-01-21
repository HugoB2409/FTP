using System;
using System.Net.Sockets;
using FTP.Common;

namespace FTP.Server
{
    public class ReceivePacket
    {
        private readonly Client _client;
        private readonly Socket _receiveSocket;
        private readonly SocketCommunication _socketCommunication;
        private bool _connected;

        public ReceivePacket(Socket receiveSocket, Client client)
        {
            _client = client;
            _receiveSocket = receiveSocket;
            _socketCommunication = new SocketCommunication(_receiveSocket);
            _connected = true;
        }

        public void StartReceiving()
        {
            while (_connected)
            {
                var command = _socketCommunication.Receive();
                if (command != null)
                {
                    Execute(command);
                }
            }
        }

        private void Execute(string message)
        {
            var command = message.Split(' ');
            Console.WriteLine($"Message receive: {message} from {_client.Username}");
            switch (command[0])
            {
                case "login":
                    Command.Login(_socketCommunication, _client, command[1]);
                    break;
                case "pwd":
                    Command.Pwd(_socketCommunication, _client.CurrentRepository);
                    break;
                case "ls":
                    Command.Ls(_socketCommunication, _client.CurrentRepository);
                    break;
                case "cd":
                    Command.Cd(command[1], _client);
                    break;
                case "get":
                    Command.Get(_socketCommunication, command[1]);
                    break;
                case "put":
                    Command.Put(_socketCommunication, command[1], _client);
                    break;
                case "exit":
                    Disconnect();
                    break;
                default:
                    Console.WriteLine($"Invalid message: {message}");
                    break;
            }
        }

        private void Disconnect()
        {
            _connected = false;
            _receiveSocket.Disconnect(true);
            Console.WriteLine($"Connection close with {_client.Username}.");
        }
    }
}