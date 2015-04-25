using System.Net;

namespace Udp.Server.Threading
{
    class UserInfo
    {
        public UserInfo(string name, EndPoint endPoint)
        {
            Name = name;
            EndPoint = endPoint;
        }

        public string Name { get; private set; }

        public EndPoint EndPoint { get; private set; }
    }
}
