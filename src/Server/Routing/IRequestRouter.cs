using CRProxy.Configuration.Routes;

namespace CRProxy.Server.Routing;

internal interface IRequestRouter
{
    RouteMapping? Route(DeviceIdRequestPartial requestPartial);
}
