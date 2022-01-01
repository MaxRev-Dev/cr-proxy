using CRProxy.Configuration.Routes;
using System.Net.Sockets;

namespace CRProxy.Server.Routing;

internal class RequestRouter : IRequestRouter
{
    private readonly RoutesRepository _routes;

    public RequestRouter(RoutesRepository routes)
    {
        _routes = routes;
    }

    public RouteMapping? Route(DeviceIdRequestPartial requestPartial)
    {
        if (!requestPartial.IsValid())
        {
            throw new InvalidDataException("Request is not valid");
        }

        return _routes.FirstOrDefault(requestPartial.DeviceId!.Value);
    }
}