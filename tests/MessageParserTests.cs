using CRProxy.Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xunit;

namespace CRProxy.Tests;

public class MessageParserTests
{
    [Fact]
    public void Parser_correctly_accepts_packets_from_socket()
    {
        var targetIP = IPAddress.Loopback;
        uint targetPort = 45454;
        int packetsCount = 10;

        var tcpListener = new TcpListener(targetIP, (int)targetPort);
        tcpListener.Start();

        using var tcpClient = new TcpClient();
        tcpClient.Connect(targetIP, (int)targetPort);
        var stream = tcpClient.GetStream();
        var incomming = tcpListener.AcceptSocket();
        var receiver = new TcpMessageToClientIdParser();

        for (int i = 0; i < packetsCount; i++)
        {
            // create payload
            var sendBuffer = new byte[TcpMessageToClientIdParser.HeaderSize];

            uint deviceId = (uint)Random.Shared.Next(); // some uint32 device id (4 bytes) 
            TestHelpers.CopyIdToBuffer(deviceId, sendBuffer);

            // send packet
            stream.Write(sendBuffer);

            // receive packet
            var isSuccess = receiver.RetrivePacket(incomming, out var packet);

            Assert.True(isSuccess);
            Assert.Equal(deviceId, packet.DeviceId);

            // check default buffer
            Assert.Equal(TcpMessageToClientIdParser.HeaderSize, packet.Received);
        }
    }
}
