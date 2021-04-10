using System;

namespace Server
{
    class Program
    {
        
        static void Main(string[] args)
        {
            TCPSocketServer server = new TCPSocketServer();
            Console.WriteLine("PRESS ENTER TO START THE SERVER");
            Console.ReadLine();
            server.Start();


            Console.ReadLine();
        }
    }
}
