using CRProxy.Configuration.Binders;
using CRProxy.Configuration.Routes;
using CRProxy.Host;
using CRProxy.Services;
using Microsoft.Extensions.DependencyInjection;

using (var host = new ProxyHost())
{
    var builder = host.Builder;

    builder.WithOptions(x =>
    {
        x.Port = 3000;

        var mappings = new EndpointsConfigurationBinder();
        if (mappings.IsValidConfig)
        { 
            x.Routes = new RoutesRepository(mappings.Endpoints);
        }
    });


    await host.Run();
}
