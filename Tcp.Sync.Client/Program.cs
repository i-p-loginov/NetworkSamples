using System;
using System.Net;
using System.Net.Sockets;
using NetworkSamples.Model;

namespace Tcp.Sync.Client
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

            //создаём сокет - работающий по протоколу TCP/IP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //подключаемся к серверу
            socket.Connect(targetIp, port);

            //и в цикле осуществляем приём и получение сообщений.
            while (true)
            {
                Console.WriteLine("Type message:");
                string msg = Console.ReadLine();

                //вызываем extension-метод из NetworkSamples.Model.NetworkHelpers
                socket.TcpSendMessage(msg);

                var result = socket.TcpReceiveMessage();

                Console.WriteLine("Response:" + result);
            }

        }
    }
}
