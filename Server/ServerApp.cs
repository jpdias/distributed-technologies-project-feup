using System;
using System.Runtime.Remoting;

namespace Server
{
    internal class ServerApp
    {
        private static void Main(string[] args)
        {
            RemotingConfiguration.Configure("Server.exe.config", false);
            Console.WriteLine("Press Return to terminate.");
            Console.ReadLine();
        }
    }
}