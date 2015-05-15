using System.Net;
using System.Net.Sockets;

namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    internal class NetworkHelper
    {
        internal static int GetPort()
        {
            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0)); // Pass 0 here.
                return ((IPEndPoint)sock.LocalEndPoint).Port;
            }
        }
    }
}
