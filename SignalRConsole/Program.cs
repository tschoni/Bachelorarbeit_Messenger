using System;

namespace SignalRConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SignalRClient signalRClient = new SignalRClient();
            Console.WriteLine("Connecting...");
            signalRClient.StartAsync().GetAwaiter().GetResult();
            if (signalRClient.IsConnected)
            {
                Console.WriteLine("Connected.");
            }
            while (signalRClient.IsConnected)
            {

            }

        }
    }
}
