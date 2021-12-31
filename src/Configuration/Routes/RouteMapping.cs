using System.Globalization;
using System.Net;

namespace CRProxy.Configuration.Routes;

public class RouteMapping
{
    private string? _from;
    private string? _to;

    public RouteMapping()
    {
        // this ctor mainly for configuration binder
    }

    public RouteMapping(string from, string to)
    {
        From = from;
        To = to;
    }


    public bool Matches(uint otherId) => otherId.Equals(FromValue);

    public uint? FromValue { get; private set; }


    public string? From
    {
        get => _from;
        set
        {
            if (value != null && value != "*")
                FromValue = uint.Parse(value!);
            _from = value;
        }
    }
    public string? To
    {
        get => _to;
        set
        {
            if (value is null)
                return;
            var source = value;
            if (!source.Contains(':'))
                source = source + ":80";

            var partials = source.Split(':');
            if (partials.Length != 2)
                throw new FormatException("Invalid endpoint format");
            var address = Dns.GetHostAddresses(partials[0]).FirstOrDefault(); 
            if (!int.TryParse(partials[1], out var port))
                throw new FormatException("Invalid port");
            EndPoint = new IPEndPoint(address, port);
            _to = value;
        }
    }
    public bool IsManyToOne => From == "*";

    public IPEndPoint? EndPoint { get; private set; }
}