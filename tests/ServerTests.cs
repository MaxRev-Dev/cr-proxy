using CRProxy.Configuration.Binders;
using CRProxy.Host;
using CRProxy.Server;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace CRProxy.Tests;

public class ServerTests
{
    [Fact]
    public async Task Server_routes_devices_to_listeners()
    {
        using var host = new ProxyHost();
        var port = 10000;
        host.Builder.WithOptions(x =>
        {
            x.Address = System.Net.IPAddress.Loopback;
            x.Port = (uint)port;
        });
        _ = host.Run();

        await Task.Yield();

        var binder = new EndpointsConfigurationBinder();

        var entities = binder.Endpoints.ToList();
        var listeners = entities.Select(x => x.EndPoint!).Distinct()
            .Select(x => new TcpListener(x)).ToList();
        foreach (var listener in listeners)
            listener.Start();// start stub servers
        for (int i = 0; i < entities.Count; i++)
        {
            using var client = new TcpClient();
            client.Connect(System.Net.IPAddress.Loopback, port);

            uint? deviceId = entities[i].FromValue.HasValue ?
                entities[i].FromValue :  // get mapping or wildcard defaults
                (uint?)Random.Shared.Next();

            using var outStream
                = client.GetStream();

            // create payload
            var buffer = new byte[TcpMessageToClientIdParser.HeaderSize];
            TestHelpers.CopyIdToBuffer(deviceId!.Value, buffer);

            // send packet
            outStream.Write(buffer);
            outStream.Flush(); 


            // receive on some of servers
            foreach (var listener in listeners)
            {
                if (listener.Pending())
                {
                    var socket = listener.AcceptSocket();
                    var stream = new NetworkStream(socket);
                    var buff = new byte[TcpMessageToClientIdParser.HeaderSize];
                    var received = stream.Read(buff);
                    Assert.Equal(TcpMessageToClientIdParser.HeaderSize, received);
                }
            }
        }

        foreach (var listener in listeners) listener.Stop();
    }

}
