using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using NetworkSamples.Model;
using System.Net;
using System.Threading;

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

            bool working = true;

            //предполагаем, что аргументы переданы в корректном формате
            var targetIp = IPAddress.Parse(args[0]);
            var port = int.Parse(args[1]);

            EndPoint endPoint = new IPEndPoint(targetIp, port);
            EndPoint srcEndPoint = new IPEndPoint(targetIp, port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(new IPEndPoint(IPAddress.Any, 0));

            Thread receiverWorker = new Thread(() => {
                while (working)
                {
                    var message = client.UdpReceiveMessage(ref srcEndPoint);

                    if (endPoint.ToString() == srcEndPoint.ToString())
                        Console.WriteLine(message);
                }
            });

            receiverWorker.Start();

            while (working)
            {
                var message = Console.ReadLine();

                client.UdpSendMessage(endPoint, message);
            }

        }
    }
}
