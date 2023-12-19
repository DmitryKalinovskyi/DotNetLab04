using ChatLibrary;
using System;
using System.ServiceModel;

namespace ChatServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(ChatService)))
            {
                host.Open();
                Console.WriteLine("Host started!");
                Console.ReadLine();
            }
        }
    }
}
