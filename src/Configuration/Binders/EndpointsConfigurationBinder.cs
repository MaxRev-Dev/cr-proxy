using CRProxy.Configuration.Routes;
using Microsoft.Extensions.Configuration;

namespace CRProxy.Configuration.Binders;

internal class EndpointsConfigurationBinder
{
    private readonly IConfigurationRoot _config;

    public EndpointsConfigurationBinder(string jsonFileName = "mappings.json")
    {
        // simple-dimple configuration
        _config = new ConfigurationBuilder()
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
             .AddJsonFile(jsonFileName)
             .Build();
    }

    public bool IsValidConfig => // this could be done via options validation on start
        Endpoints.Any() && Endpoints.All(x => x is not null);
    public IEnumerable<RouteMapping> Endpoints => _config.GetSection("Mappings").Get<RouteMapping[]>() ?? Array.Empty<RouteMapping>();
}
