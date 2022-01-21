using System;
using System.IO;
using FTP.Common;

namespace FTP
{
    public static class Command
    {
        public static void Pwd(SocketCommunication socketCommunication)
        {
            Console.WriteLine($"Remote working directory: {socketCommunication.SendAndReceive("pwd")}");
        }
        
        public static void Lpwd()
        {
            Console.WriteLine($"local working directory: {Commands.Pwd(Directory.GetCurrentDirectory() + "/")}");
        }
        
        public static void Ls(SocketCommunication socketCommunication)
        {
            Console.WriteLine(socketCommunication.SendAndReceive("ls"));
        }
        
        public static void Lls()
        {
            Console.WriteLine(Commands.Ls(Directory.GetCurrentDirectory() + "/"));
        }
        
        public static void Cd(SocketCommunication socketCommunication, string path)
        {
            socketCommunication.Send($"cd {path}");
        }
        
        public static void Lcd(string path)
        {
            Directory.SetCurrentDirectory(Commands.Cd(path, Directory.GetCurrentDirectory() + "/"));
        }
        
        public static void Get(SocketCommunication socketCommunication, string name)
        {
            socketCommunication.Send($"get {name}");
            socketCommunication.ReceiveFile(Directory.GetCurrentDirectory() + "/" + name);
        }
        
        public static void Put(SocketCommunication socketCommunication, string name)
        {
            Console.WriteLine(name);
            if (socketCommunication.SendAndReceive($"put {name}").Equals("ready"))
            {
                Console.WriteLine($"path : {Directory.GetCurrentDirectory() + "/" + name}");
                socketCommunication.SendFile(Directory.GetCurrentDirectory() + "/" + name);
                Console.WriteLine("File sent.");
            }
            else
            {
                Console.WriteLine("Error while sending file.");
            }
        }
        
        public static void Help() 
        {
            Console.WriteLine("Available commands: \n");
            Console.WriteLine("cd path                            Change remote directory to 'path'");
            Console.WriteLine("exit                               Quit sftp");
            Console.WriteLine("get [-R] remote [local]            Download file or folder");
            Console.WriteLine("help                               Display this help text");
            Console.WriteLine("lcd path                           Change local directory to 'path'");
            Console.WriteLine("lls                                Display local directory listing");
            Console.WriteLine("lpwd                               Print local working directory");
            Console.WriteLine("ls                                 Display remote directory listing");
            Console.WriteLine("put [-R] local                     Upload file");
            Console.WriteLine("pwd                                Display remote working directory");
        }
    }
}