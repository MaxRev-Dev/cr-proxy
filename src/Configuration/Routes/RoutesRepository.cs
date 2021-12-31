namespace CRProxy.Configuration.Routes;

public class RoutesRepository
{
    private readonly HashSet<RouteMapping> _mappings;

    public RoutesRepository(IEnumerable<RouteMapping> mappings)
    {
        _mappings = new HashSet<RouteMapping>(mappings);
        DefaultMapping = _mappings.FirstOrDefault(x => x.IsManyToOne);
    }

    public RouteMapping? FirstOrDefault(uint fromId) => _mappings.FirstOrDefault(x => x.Matches(fromId)) ?? DefaultMapping;

    public RouteMapping? DefaultMapping { get; }
}
