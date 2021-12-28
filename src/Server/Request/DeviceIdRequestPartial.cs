namespace CRProxy.Server
{
    internal struct DeviceIdRequestPartial
    {
        private readonly long? deviceId;
        private readonly byte[]? buffer;
        private readonly int received;

        public DeviceIdRequestPartial(int received, long? deviceId, byte[]? buffer)
        {
            this.deviceId = deviceId;
            this.buffer = buffer;
            this.received = received;
        }
    }
}