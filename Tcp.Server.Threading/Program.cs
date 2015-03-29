using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkSamples.Model;

namespace Tcp.Server.Threading
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

            //размер очереди ожидаемых соединений
            const int backlog = 4;

            //создаём сокет - работающий по протоколу TCP/IP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //пара IP + порт, для которой будет выполняться обслуживание входящих подключений
            var currentEndPoint = new IPEndPoint(IPAddress.Loopback, port);

            //"привязка" сокета к нужной паре ip/port
            socket.Bind(currentEndPoint);
            //стартуем ожидаение входящих подключений - прослушиваем интерфейс/порт
            socket.Listen(backlog);

            Console.WriteLine("Listening...");

            //!!
            //сокет, который необходим для работы с созданным соединением 
            //Socket client = null;

            while (true)
            {

                var client = socket.Accept();
                Console.WriteLine("Connection accepted.");

                Thread thread = new Thread(ConnectionHandler);
                thread.Start(client);
            }
        }

        static void ConnectionHandler(object obj)
        {
            var clientSocket = obj as Socket;

            if (clientSocket == null)
            {
                throw new ArgumentException();
            }

            Console.WriteLine("Thread started.");
            while (true)
            {
                string message = "";

                try
                {
                    message = clientSocket.TcpReceiveMessage();
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }

                Console.WriteLine(message);

                try
                {
                    clientSocket.TcpSendMessage(string.Format("Message {0} printed.", message));
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            Console.WriteLine("Thread stopped.");
        }
    }
}
