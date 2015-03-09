using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NetworkSamples.Model;
using System.Net.Sockets;
using System.Text;

namespace Udp.Sync.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //проверка аргументов командной строки - 
            //args[0] - IP
            //args[1] - порт
            if (args == null || args.Length != 2)
            {
                Console.WriteLine("Invalid arguments.");
                return;
            }

            //предполагаем, что аргументы переданы в корректном формате
            var targetIp = IPAddress.Parse(args[0]);
            var port = int.Parse(args[1]);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            EndPoint endPoint = new IPEndPoint(targetIp, port);
            server.Bind(endPoint);

            while (true)
            {
                var message = server.UdpReceiveMessage(ref endPoint);

                Console.WriteLine("Message: {0}", message);

                if (message == "exit")
                {
                    Environment.Exit(0);
                }
            }

        }
    }
}
