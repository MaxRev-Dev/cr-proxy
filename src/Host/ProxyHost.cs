
using CRProxy.Server;

namespace CRProxy.Host;

internal class ProxyHost : IDisposable
{ 
    private HostBuilder? _builder;
    private bool _isDisposed;
    private IProxyServer? _server;

    public ProxyHost()
    {

    } 


    public Task Run()
    {
        _server = ((HostBuilder)Builder).Build();
        return _server.StartAsync();
    }

    private readonly ObjectDisposedException _disposedEx = new("The host is already disposed");

    public IHostBuilder Builder => _isDisposed ?
                throw _disposedEx :
                  _builder ??= new HostBuilder();

    public IProxyServer? Server => _isDisposed ? throw _disposedEx : _server;

    public void Dispose()
    {
        _server?.Dispose();
        _isDisposed = true;
    }
}
