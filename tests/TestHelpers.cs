using CRProxy.Server;
using System;
using System.Runtime.InteropServices;

namespace CRProxy.Tests
{
    public static class TestHelpers
    {
        public static void CopyIdToBuffer(uint deviceId, byte[] buffer)
        {
            var sliceOfId = buffer.AsSpan()
                .Slice(TcpMessageToClientIdParser.HeaderStart, TcpMessageToClientIdParser.DeviceIdSize);
            MemoryMarshal.Write(sliceOfId, ref deviceId);
        }
    }
}