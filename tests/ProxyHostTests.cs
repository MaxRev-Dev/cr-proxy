using CRProxy.Host;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CRProxy.Tests;

public class ProxyHostTests
{
    [Fact]
    public void Proxy_host_disposes_properly()
    {
        var host = new ProxyHost();

        host.Dispose();

        Assert.Throws<ObjectDisposedException>(() => host.Builder);
    }

    [Fact]
    public async Task Host_disposes_server()
    {
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
        {
            var host = new ProxyHost();
            Assert.Null(host.Server);
            host.Run();

            var server = host.Server!;
            Assert.NotNull(server);

            host.Dispose();
            return server.StartAsync();
        });
    }
}
