using System;
using System.Net;
using System.Net.Sockets;
using NetworkSamples.Model;

namespace Tcp.Sync.Server
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
            var ip = IPAddress.Parse(args[0]);
            int port = int.Parse(args[1]);

            const int backlog = 4;
            
            //создаём сокет - работающий по протоколу TCP/IP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            var currentEndPoint = new IPEndPoint(IPAddress.Loopback, port);

            socket.Bind(currentEndPoint);
            socket.Listen(backlog);

            Console.WriteLine("Listening...");

            Socket client = null;

            while (true)
            {
                if (client == null || !client.Connected)
                {
                    client = socket.Accept();

                    Console.WriteLine("Client with ip {0} connected.", client.RemoteEndPoint);
                }

                var message = client.TcpReceiveMessage();
                
                if (message.StartsWith("#get"))
                {
                    client.TcpSendMessage("#get command executed.");
                }
                else
                {
                    client.TcpSendMessage("Simple message received.");
                }
                Console.WriteLine(message);
            }
        }

       
       
    }
}
