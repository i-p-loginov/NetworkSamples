using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using NetworkSamples.Model;
using System.Net;

namespace Udp.Sync.Client
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

            EndPoint endPoint = new IPEndPoint(targetIp, port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (true)
            {
                var message = Console.ReadLine();

                client.UdpSendMessage(endPoint, message);
            }
           
        }
    }
}
