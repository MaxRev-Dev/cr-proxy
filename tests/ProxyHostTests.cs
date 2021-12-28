using CRProxy.Host;
using System;
using Xunit;

namespace CRProxy.Tests;

public class ProxyHostTests
{
    [Fact]
    public void Proxy_host_disposes_properly()
    {
        var host = new ProxyHost();

        host.Dispose();

        Assert.Throws<InvalidOperationException>(() => host.Builder);
    }
}