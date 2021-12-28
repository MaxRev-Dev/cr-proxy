using CRProxy.Configuration;
using System.Net.Sockets;

namespace CRProxy.Server
{
    class ProxyServer : IProxyServer
    {
        private readonly ProxyOptions _options;
        private bool _initialized;
        private TcpListener? _listener;
        private IClientHandler _clientHandler;
        private bool _isActive;

        public ProxyServer(ProxyOptions options)
        {
            _options = options;
        }

        private void Initialize()
        {
            _options.Validate();
            _initialized = true;
            _listener = new TcpListener(_options.Address, (int)_options.Port);

            // this normally should be injected from DI container
            _clientHandler = _options.ClientHandler;
        }

        private Task AcceptSocketsAsync()
        {
            if (!_initialized || _listener is null)
                throw new InvalidOperationException();

            bool _passiveModeAccept = false;
            while (true)
            {
                try
                {
                    if (_passiveModeAccept)
                        Thread.Sleep(10);
                    if (!_isActive)
                        break;
                    if (!_listener.Pending())
                    {
                        _passiveModeAccept = true;
                        continue;
                    }
                    _passiveModeAccept = false;

                    while (_listener.Pending())
                        try
                        {
                            _listener.BeginAcceptSocket(ar =>
                            {
                                try
                                {
                                    var acceptedSocket = _listener.EndAcceptSocket(ar);
                                    _clientHandler.AcceptSocketAsync(acceptedSocket);
                                }
                                catch
                                {
                                    // client acceptation fail 
                                    // maybe connection was closed by client
                                }
                            }, default);
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }
                }
                catch (SocketException)
                {
                    // emergency, listener socket fail??
                    break;
                }
            }
            if (_isActive)
                throw new TaskCanceledException();
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            if (!_initialized || _listener is null)
                Initialize();

            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    _listener!.Start(_options.MaxNumberOfConnections);
                    while (_isActive)
                    {
                        await AcceptSocketsAsync();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // suspending
                }
                catch (InvalidOperationException)
                {
                    // suspending
                }
                catch (AggregateException)
                {
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    // $"Server {Name} is active or port {Port} is busy";
                    Environment.Exit(-1);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            if (!_isActive)
                return;
            _isActive = false;
        }
    }
}