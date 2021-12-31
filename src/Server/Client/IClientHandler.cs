using System.Net.Sockets;

namespace CRProxy.Server
{
    internal interface IClientHandler
    {
        Task AcceptSocketAsync(Socket acceptedSocket);
    }
}