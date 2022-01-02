using CRProxy.Configuration.Binders;
using CRProxy.Host;
using CRProxy.Server;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CRProxy.Tests;

public class ServerTests
{
    /// <summary>
    /// Ensures the client-proxy-service request cycle works as espected. 
    /// </summary>
    [Fact]
    public async Task Server_routes_device_to_listener()
    {
        var port = 10000;
        const string requestMessage = "Request";
        const string responseMessage = "Some awesome test";
        var binder = new EndpointsConfigurationBinder();

        var routeInfo = binder.Endpoints.First();
        var remoteEndpoint = routeInfo.EndPoint!;

        var host = new ProxyHost();
        // run proxy
        var proxy = Task.Run(async () =>
        {
            host.Builder.WithOptions(x =>
            {
                x.Address = System.Net.IPAddress.Loopback;
                x.Port = (uint)port;
            });
            await host.Run();
        });

        // run a simple stub for this request
        var serviceStub = Task.Run(() =>
        {
            var serviceListener = new TcpListener(remoteEndpoint);
            serviceListener.Start();

            while (!serviceListener.Pending())
                Thread.Sleep(10);

            // read message
            var clientSocket = serviceListener.AcceptSocket();
            var serverStream = new NetworkStream(clientSocket, false);

            var buffer = new byte[TcpMessageToClientIdParser.HeaderSize];
            var received = serverStream.Read(buffer);
            Assert.Equal(received, TcpMessageToClientIdParser.HeaderSize);
            buffer = new byte[100];
            received = serverStream.Read(buffer);
            Assert.True(received > 0);
            Assert.Equal(requestMessage, Encoding.UTF8.GetString(buffer.AsSpan(0, received)));
            // write server response
            serverStream.Write(Encoding.UTF8.GetBytes(responseMessage));
            serverStream.Flush();
        });

        /// allow socket to start
        await Task.Delay(100);

        using var client = new TcpClient();
        client.Connect(System.Net.IPAddress.Loopback, port);

        uint? deviceId = routeInfo.FromValue.HasValue ?
            routeInfo.FromValue :  // get mapping or wildcard defaults
            (uint?)Random.Shared.Next();

        using var outStream = client.GetStream();

        // create payload
        var buffer = new byte[TcpMessageToClientIdParser.HeaderSize];
        TestHelpers.CopyIdToBuffer(deviceId!.Value, buffer);

        // send packet
        outStream.Write(buffer);
        outStream.Write(Encoding.UTF8.GetBytes(requestMessage));
        outStream.Flush();


        // read server response 
        buffer = new byte[100];
        int messageSize = outStream.Read(buffer);
        Assert.True(messageSize > 0);

        var result = Encoding.UTF8.GetString(buffer.AsSpan(0, messageSize));
        Assert.Equal(responseMessage, result);

        await serviceStub;
        host.Server!.Stop();
        await proxy;
    }
}
