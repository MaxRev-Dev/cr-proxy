using CRProxy.Configuration.Routes;
using System.Net;

namespace CRProxy.Configuration
{
    class ProxyOptions : IProxyOptions
    {
        public int MaxNumberOfConnections { get; set; }
        public IPAddress Address { get; set; }
        public uint Port { get; set; }
        public RoutesRepository? Routes { get; set; }

        public ProxyOptions()
        {
            MaxNumberOfConnections = 10000;
            Address = IPAddress.Any;
        }

        public void Validate()
        {
            if (MaxNumberOfConnections < 1) MaxNumberOfConnections = 1;
            if (Address is null) Address = IPAddress.Any;
            if (Port < 1024) Port = 3000; // lower ports on linux may fail without admin rights
        }
    }
}