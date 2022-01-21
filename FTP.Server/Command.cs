using System;
using System.IO;
using FTP.Common;

namespace FTP.Server
{
    public static class Command
    {
        public static void Login(SocketCommunication socketCommunication, Client client, string credentials)
        {
            var credential = credentials.Split('.');
            if (Utils.ValidateUser(credential[0], credential[1]))
            {
                client.Username = credential[0];
                client.Password = credential[1];
                client.DefaultRepository = Utils.GetClientDefaultDirectory(credential[0], credential[1]);
                client.CurrentRepository = client.DefaultRepository;
                socketCommunication.Send("valid");
            }
            else
            {
                socketCommunication.Send("invalid");
            }
        }
        
        public static void Pwd(SocketCommunication socketCommunication, string path)
        {
            socketCommunication.Send(Commands.Pwd(path));
        }
        
        public static void Ls(SocketCommunication socketCommunication, string path)
        {
            socketCommunication.Send(Commands.Ls(path));
        }
        
        public static void Cd(string path, Client client)
        {
            client.CurrentRepository = Commands.CdWithRestriction(client.CurrentRepository + path, client.DefaultRepository);
        }

        public static void Get(SocketCommunication socketCommunication, string name)
        {
            socketCommunication.SendFile(name);
        }
        
        public static void Put(SocketCommunication socketCommunication, string name, Client client)
        {
            socketCommunication.Send("ready");
            Console.WriteLine(socketCommunication.ReceiveFile(client.CurrentRepository + name));
        }
    }
}