using CRProxy.Configuration;
using System.Net.Sockets;

namespace CRProxy.Server
{
    class ProxyServer : IProxyServer
    {
        private readonly ProxyOptions _options;
        private TcpListener? _listener;
        private IClientHandler? _clientHandler;
        private bool _isActive, _isDisposed, _initialized;

        public ProxyServer(ProxyOptions options)
        {
            _options = options;
        }

        private void Initialize()
        {
            CheckIfDisposed();

            _options.Validate();
            // this normally should be injected from DI container
            _clientHandler = new ClientHandler();
            _listener = new TcpListener(_options.Address, (int)_options.Port);
            _initialized = true;
        }

        private Task AcceptSocketsAsync()
        {
            CheckIfDisposed();
            if (!_initialized || _listener is null || _isDisposed)
                throw new InvalidOperationException();
            return Task.Factory.StartNew(() =>
            {
                _isActive = true;
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
                                        acceptedSocket.NoDelay = true;
                                        _clientHandler!.AcceptSocketAsync(acceptedSocket);
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
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        // $"Server {Name} is active or port {Port} is busy";
                        Environment.Exit(-1);
                    }
                    catch (SocketException)
                    {
                        // emergency, listener socket fail??
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        // suspending 
                        break;
                    }
                }
                if (_isActive)
                    throw new TaskCanceledException();
                return Task.CompletedTask;
            }, TaskCreationOptions.LongRunning);
        }

        public Task StartAsync()
        {
            CheckIfDisposed();

            if (!_initialized || _listener is null || _isDisposed)
                Initialize();

            _listener!.Start(_options.MaxNumberOfConnections);
            return AcceptSocketsAsync();
        }

        private void CheckIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("This instance if proxy is already disposed");
        }

        public void Stop()
        {
            CheckIfDisposed();
            if (!_isActive)
                return;
            _isActive = false;
            _listener?.Stop();
        }

        public void Dispose()
        {
            Stop();
            _isDisposed = true;
        }
    }
}