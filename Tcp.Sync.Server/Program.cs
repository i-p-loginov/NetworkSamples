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

            //сокет, который необходим для работы с созданным соединением 
            Socket client = null;

            while (true)
            {
                //обслуживаем лишь одно соединение - если его нет - 
                if (client == null || !client.Connected)
                {
                    //принимаем запрос на входящее соединение
                    client = socket.Accept();

                    Console.WriteLine("Client with ip {0} connected.", client.RemoteEndPoint);
                }

                //получаем сообщение от клиента
                var message = client.TcpReceiveMessage();
                
                //очень простой пример обработки какой-либо команды от клиента
                if (message.StartsWith("#get"))
                {
                    //отправляем ответное сообщение клиенту
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
