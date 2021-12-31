namespace CRProxy.Server
{
    internal struct DeviceIdRequestPartial
    {
        public DeviceIdRequestPartial(int received, uint? deviceId, byte[]? buffer)
        {
            DeviceId = deviceId;
            Buffer = buffer;
            Received = received;
        }

        public uint? DeviceId { get; }
        public byte[]? Buffer { get; }
        public int Received { get; }

        public bool IsValid() => Buffer != default && DeviceId.HasValue;
    }
}