using CRProxy.Configuration.Binders;
using CRProxy.Configuration.Routes;
using CRProxy.Server.Routing;
using System.Net;
using System.Net.Sockets;

namespace CRProxy.Server
{
    class ClientHandler : IClientHandler
    {
        private TcpMessageToClientIdParser _requestParser;
        private IRequestRouter _router;

        public ClientHandler()
        {
            // normally this should be injected 
            _requestParser = new TcpMessageToClientIdParser();
            var endpointsMap = new EndpointsConfigurationBinder();
            _router = new RequestRouter(new RoutesRepository(endpointsMap.Endpoints));
        }

        public Task AcceptSocketAsync(Socket acceptedSocket)
        {
            using (acceptedSocket)
            {
                var successRead = _requestParser.RetrivePacket(acceptedSocket, out var packetPartial);
                if (!successRead)
                {
                    acceptedSocket.Close();
                    return Task.CompletedTask;
                }
                var routeResult = _router.Route(packetPartial);

                if (routeResult is null)
                {
                    // we have no mappings for this id
                    acceptedSocket.Close();
                    return Task.CompletedTask;
                }

                return SendResult(acceptedSocket, routeResult.EndPoint!, packetPartial);
            }
        }

        private Task SendResult(Socket acceptedSocket, IPEndPoint endpoint, DeviceIdRequestPartial packet)
        {
            using var clientTarget = new TcpClient();
            clientTarget.Connect(endpoint);

            using var nsSource = new NetworkStream(acceptedSocket);
            var nsTarget = clientTarget.GetStream();
            
            // send buffered header to server
            nsTarget.Write(packet.Buffer!);

            // we will not use Stream.CopyToAsync() or Stream.Read() 
            // as they block this thread until the connection is closed 

            // sending the trailing message body
            CopyData(nsSource, nsTarget);

            // get response from the server
            while (!nsTarget.DataAvailable)
                Thread.Sleep(10);

            // copy response from server to client 
            CopyData(nsTarget, nsSource);

            return Task.CompletedTask;
        }

        private static void CopyData(NetworkStream nsSource, NetworkStream nsTarget, int sendBufferSize = 256)
        {
            var sendBuffer = new byte[sendBufferSize];
            int read;
            while (nsSource.DataAvailable && (read = nsSource.Read(sendBuffer)) > 0)
            {
                nsTarget.Write(sendBuffer, 0, read);
            }
        }
    }
}