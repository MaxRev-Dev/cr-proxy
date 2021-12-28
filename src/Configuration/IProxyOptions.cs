using System.Net;

namespace CRProxy.Configuration
{
    public interface IProxyOptions
    {
        int MaxNumberOfConnections { get; set; }
        IPAddress Address { get; set; }
        uint Port { get; set; }
    }
}