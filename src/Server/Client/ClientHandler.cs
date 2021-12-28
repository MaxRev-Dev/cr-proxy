using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace CRProxy.Server
{
    class ClientHandler : IClientHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private TcpMessageToClientIdParser _requestParser;
        private IRequestRouter? _router;

        public ClientHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task AcceptSocketAsync(Socket acceptedSocket)
        {
            _router = _serviceProvider.GetService<IRequestRouter>();
            var client = new Client(acceptedSocket, _serviceProvider);

            var packetPartial = _requestParser.RetrivePacket(client.Socket);

            // TODO: proceed with route negotiation
            return _router != default ? _router.Route(client) : Task.CompletedTask;
        }
    }
}