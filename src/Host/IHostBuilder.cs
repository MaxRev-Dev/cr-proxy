using CRProxy.Configuration;
using CRProxy.Server;

namespace CRProxy.Host
{
    public interface IHostBuilder
    {
        IHostBuilder WithOptions(Action<IProxyOptions> options);
    }
}