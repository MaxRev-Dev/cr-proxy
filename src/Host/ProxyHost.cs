
using Microsoft.Extensions.DependencyInjection;

namespace CRProxy.Host;

internal class ProxyHost : IDisposable
{
    private IServiceCollection _services;
    private HostBuilder _builder;
    private bool _isDisposed;

    public ProxyHost()
    {

    }

    public IServiceCollection Services => _services ??= new ServiceCollection();

    internal Task Run()
    {
        var server = _builder.Build(Services);
        return server.StartAsync();
    }

    public IHostBuilder Builder
    {
        get
        {
            return _isDisposed ?
                throw new InvalidOperationException("The host is already disposed") :
                  _builder ??= new HostBuilder();
        }
    }

    public void Dispose()
    {
        _isDisposed = true;
    }
}
