using CRProxy.Configuration.Binders;
using CRProxy.Configuration.Routes;
using System.IO;
using Xunit;

namespace CRProxy.Tests;

public class ConfigurationTests
{
    [Fact]
    public void Configuration_accepts_array_of_endpoint_mappings()
    {
        var mappingsSource = "mappings.json";
        var mappings = new EndpointsConfigurationBinder(mappingsSource);
        Assert.True(File.Exists(mappingsSource));
        Assert.True(mappings.IsValidConfig);
        Assert.NotEmpty(mappings.Endpoints);
        var repo = new RoutesRepository(mappings.Endpoints);
        Assert.NotNull(repo.DefaultMapping);
        Assert.All(mappings.Endpoints, x => Assert.NotNull(x));
    }
}