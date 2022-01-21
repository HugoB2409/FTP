using System;

namespace FTP
{
    internal static class Program
    {
        private static AsynchronousFtpClient _ftpClient;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += InterruptHandler; 
            var clientInfo = args[0].Split("@");
            
            _ftpClient = new AsynchronousFtpClient(clientInfo[0], clientInfo[1]);
            _ftpClient.StartClient();
        }
        
        private static void InterruptHandler(object sender, ConsoleCancelEventArgs e)
        {
            _ftpClient.CleanupRessources();
        }
    }
}