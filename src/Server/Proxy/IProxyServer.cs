
namespace CRProxy.Server
{
    public interface IProxyServer
    {
        Task StartAsync();
        void Stop();
    }
}