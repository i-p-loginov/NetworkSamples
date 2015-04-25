using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkSamples.Model;

namespace Udp.Server.Threading
{
    class Program
    {
        private static Dictionary<string, UserInfo> _activeUsers = new Dictionary<string, UserInfo>();

        private static Queue<MessageInfo> _incomingDgrams = new Queue<MessageInfo>();
        private static AutoResetEvent _dispatchEvent = new AutoResetEvent(false);

        private static bool _working = true;

        private static object _sendLock = new object();

        static void Main(string[] args)
        {           
            var targetIp = IPAddress.Any;
            var port = 54545;

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            EndPoint endPoint = new IPEndPoint(targetIp, port);
            server.Bind(endPoint);

            var threads = Enumerable.Range(0, 10).Select(n => new Thread(HandlerProc) { IsBackground = true }).ToList();
            threads.ForEach(th => th.Start(server));

            while (_working)
            {
                var messageText = server.UdpReceiveMessage(ref endPoint);

                lock (_incomingDgrams)
                {
                    var messageInfo = new MessageInfo(messageText, endPoint);
                    _incomingDgrams.Enqueue(messageInfo);
                }

                _dispatchEvent.Set();
            }
        }

        static void HandlerProc(object obj)
        {
            var server = (Socket)obj;

            while (_working)
            {
                _dispatchEvent.WaitOne();

                MessageInfo messageInfo;
                UserInfo user;

                List<UserInfo> users;

                lock (_incomingDgrams)
                {
                    messageInfo = _incomingDgrams.Dequeue();
                }

                lock (_activeUsers)
                {
                    if (!_activeUsers.TryGetValue(messageInfo.Source.ToString(), out user))
                    {
                        _activeUsers.Add(messageInfo.Source.ToString(), new UserInfo(messageInfo.Content, messageInfo.Source));
                    }

                    users = _activeUsers.Values.ToList();
                }

                string newMessage;

                if (user != null)
                {
                    newMessage = user.Name + ": " + messageInfo.Content;
                }
                else
                {
                    newMessage = string.Format("User {0} entered", messageInfo.Content);
                }

                lock (_sendLock)
                {
                    users.ForEach(usr => server.UdpSendMessage(usr.EndPoint, newMessage));
                }
                
                Console.WriteLine("[TID:{0}] Message: {1}", Thread.CurrentThread.ManagedThreadId, newMessage);
            }
        }
    }
}
