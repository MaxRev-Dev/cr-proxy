using CRProxy.Configuration.Routes;
using System.Net.Sockets;

namespace CRProxy.Server;

internal class RequestRouter : IRequestRouter
{
    private readonly RoutesRepository _routes;
    public RequestRouter(RoutesRepository routes)
    {
        _routes = routes;
    }

    public Task Route(Client source, DeviceIdRequestPartial requestPartial)
    {
        if (!requestPartial.IsValid())
        {
            return Task.FromException<InvalidDataException>(new InvalidDataException("Request is not valid"));
        }

        var match = _routes.FirstOrDefault(requestPartial.DeviceId!.Value);

        if (match is null)
        {
            // we have no mappings for this id
            source.CloseConnection();
            return Task.CompletedTask;
        }

        var endpoint = match.EndPoint!; 

        using var clientTarget = new TcpClient();
        clientTarget.Connect(endpoint);
        using var nsSource = new NetworkStream(source.Socket, false);
        using var nsTarget = new NetworkStream(clientTarget.Client);

        return nsSource.CopyToAsync(nsTarget);

    }
}
