
using CRProxy.Configuration;
using CRProxy.Server;
using Microsoft.Extensions.DependencyInjection;

namespace CRProxy.Host;

internal class HostBuilder : IHostBuilder
{
    public HostBuilder()
    {
        OptionsContext = new ProxyOptions();
    }

    /// <summary>
    /// Creates builder from user input arguments 
    /// </summary>
    /// <param name="args"></param>
    private HostBuilder(string[] args) : this()
    {
        // TODO: parse console args   
    }

    private readonly ProxyOptions OptionsContext;

    /// <summary>
    /// Builds server instance for this host
    /// </summary>
    /// <returns></returns>
    public IProxyServer Build(IServiceCollection services)
    {
        return new ProxyServer(OptionsContext);
    }

    /// <summary>
    /// Configures options for current servers
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public IHostBuilder WithOptions(Action<IProxyOptions> options)
    {
        options(OptionsContext);
        return this;
    }
}
