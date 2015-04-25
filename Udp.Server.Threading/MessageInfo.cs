using System.Net;

namespace Udp.Server.Threading
{
    public struct MessageInfo
    {
        public MessageInfo(string content, EndPoint source)
        {
            Content = content;
            Source = source;
        }

        public readonly string Content;

        public readonly EndPoint Source;
    }
}
