
namespace CRProxy.Server
{
    public interface IProxyServer : IDisposable
    {
        Task StartAsync();
        void Stop();
    }
}