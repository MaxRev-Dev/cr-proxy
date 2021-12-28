using System.Net.Sockets;

namespace CRProxy.Server
{
    internal class Client
    {
        private Socket _acceptedSocket;
        private readonly IServiceProvider _serviceProvider;

        public Socket Socket { get => _acceptedSocket; set => _acceptedSocket = value; }

        public Client(Socket acceptedSocket, IServiceProvider serviceProvider)
        {
            _acceptedSocket = acceptedSocket;
            _serviceProvider = serviceProvider;
        }
    }
}