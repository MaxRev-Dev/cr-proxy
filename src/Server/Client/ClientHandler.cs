using CRProxy.Configuration.Binders;
using CRProxy.Configuration.Routes;
using Microsoft.Extensions.DependencyInjection;
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

        public async Task AcceptSocketAsync(Socket acceptedSocket)
        {
            using var client = new Client(acceptedSocket);

            var successRead = _requestParser.RetrivePacket(client.Socket, out var packetPartial);
            if (!successRead)
            {
                client.CloseConnection(); 
                return;
            }
            await _router.Route(client, packetPartial);
        }
    }
}