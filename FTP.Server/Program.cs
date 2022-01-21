using System;

namespace FTP.Server
{
    class Program
    {
        private static FtpServer _ftpServer;
        
        static void Main(string[] args)
        {
            Console.CancelKeyPress += InterruptHandler;
            _ftpServer = new FtpServer();
            _ftpServer.StartListening();
        }

        private static void InterruptHandler(object sender, ConsoleCancelEventArgs e)
        {
            _ftpServer.Shutdown();
        }
    }
}