using System.Net;

namespace CRProxy.Server;

internal interface IRequestRouter
{
    Task Route(Client source, DeviceIdRequestPartial requestPartial);
}
