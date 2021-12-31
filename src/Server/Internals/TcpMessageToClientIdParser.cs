using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace CRProxy.Server
{
    internal class TcpMessageToClientIdParser
    {
        public const int HeaderStart = 3;
        public const int DeviceIdSize = 4;
        public const int HeaderSize = HeaderStart + DeviceIdSize;

        public bool RetrivePacket(Socket socket, out DeviceIdRequestPartial result, int? bufferSize = default)
        {
            var buffer = new byte[bufferSize ?? HeaderSize]; // we can use shared array pool here
            int received = 0;
            try
            {
                // this code does not handle HTTP requests
                // so we will just close connection on other requests
                received = socket.Receive(buffer);
                var span = buffer.AsSpan();
                var deviceId = MemoryMarshal.Read<uint>(span.Slice(HeaderStart, DeviceIdSize));

                result = new DeviceIdRequestPartial(received, deviceId, buffer);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                // format error in memory marshal
            }
            catch (ArgumentException)
            {
                // format error in memory marshal
            }
            catch (ObjectDisposedException)
            {
                // should be a closed connection
            }
            catch (SocketException)
            {
                // some problems with socket connection
            }
            // this 
            result = new(received, null, null);
            return false;
        }
    }
}