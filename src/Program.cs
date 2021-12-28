using CRProxy.Host;
using CRProxy.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CRProxy.Tests")]

using (var host = new ProxyHost())
{
    var builder = host.Builder;

    builder.WithOptions(x =>
    {
        x.Port = 3000;
    });

    host.Services.AddSingleton<RequestLogger>();

    await host.Run();
}
