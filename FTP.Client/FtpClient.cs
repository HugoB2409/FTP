using System;
using System.IO;
using System.Threading;
using FTP.Common;

namespace FTP
{
    public class AsynchronousFtpClient { 
        private const int Port = 8000;
        private readonly string _hostname; 
        private readonly string _username;
        private string _password;
        private bool _running;
        private SocketHelper _socketHelper;
        private SocketCommunication _socketCommunication;
        
        public AsynchronousFtpClient(string username, string hostname)
        {
            _hostname = hostname;
            _username = username;
            
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }
    
        public void StartClient() {
            try {
                _socketHelper = new SocketHelper(_hostname, Port);
                _socketCommunication = _socketHelper.InitializeSocketCommunication();
                
                if (IdentifyClient())
                {
                    Console.WriteLine($"Connected to {_hostname}.");
                    _running = true;
                    ListenClient();
                }
                else
                {
                    Console.WriteLine("Invalid credentials.");
                    CleanupRessources();
                }

            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }

        private bool IdentifyClient()
        {
            Console.Write($"{_username}@{_hostname}'s password: ");
            _password = Utils.ReadPassword();
            return _socketCommunication.SendAndReceive($"login {_username}.{_password}").Equals("valid");
        }

        private void ListenClient()
        {
            while (_running)
            {
                Console.Write("sftp> ");
                var command = Console.ReadLine();
                
                if (!string.IsNullOrWhiteSpace(command))
                {
                    if (!command.Equals("exit"))
                    {
                        Execute(command);
                    }
                    else
                    {
                        _running = false;
                        _socketCommunication.Send("exit");
                    }
                }
                Thread.Sleep(10);
            }
            
            CleanupRessources();
        }

        private void Execute(string input)
        {
            var command = input.Split(' ');
            switch (command[0])
            {
                case "pwd":
                    Command.Pwd(_socketCommunication);
                    break;
                case "lpwd":
                    Command.Lpwd();
                    break;
                case "ls":
                    Command.Ls(_socketCommunication);
                    break;
                case "lls":
                    Command.Lls();
                    break;
                case "cd":
                    Command.Cd(_socketCommunication, command[1]);
                    break;
                case "lcd":
                    Command.Lcd(command[1]);
                    break;
                case "get":
                    Command.Get(_socketCommunication, command[1]);
                    break;
                case "put":
                    Command.Put(_socketCommunication, command[1]);
                    break;
                case "help":
                    Command.Help();
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
        
        public void CleanupRessources()
        {
            _socketCommunication.Send("exit");
            _socketHelper.CloseConnection();
            Console.WriteLine("Disconnected.");
        }        
    }
}