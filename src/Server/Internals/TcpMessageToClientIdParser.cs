using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace CRProxy.Server
{
    internal class TcpMessageToClientIdParser
    {
        public const int HeaderStart = 3;
        public const int DeviceIdSize = 4;
        public const int HeaderSize = HeaderStart + DeviceIdSize;
        internal DeviceIdRequestPartial RetrivePacket(Socket socket, int? bufferSize = default)
        {
            var buffer = new byte[bufferSize ?? HeaderSize]; // we can use shared array pool here
            int received = 0;
            try
            {
                while (received < HeaderSize)
                    received = socket.Receive(buffer);
                var span = buffer.AsSpan();
                var deviceId = MemoryMarshal.Read<long>(span.Slice(HeaderStart, DeviceIdSize));

                return new DeviceIdRequestPartial(received, deviceId, buffer);
            }
            catch (ObjectDisposedException) { }
            catch (SocketException) { }
            return new DeviceIdRequestPartial(received, -1, default);
        }
    }
}