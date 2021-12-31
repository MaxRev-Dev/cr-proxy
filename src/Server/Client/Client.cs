using System.Net.Sockets;

namespace CRProxy.Server
{
    internal class Client : IDisposable
    {
        private Socket _acceptedSocket;

        public Socket Socket { get => _acceptedSocket; set => _acceptedSocket = value; }

        public Client(Socket acceptedSocket)
        {
            _acceptedSocket = acceptedSocket;
        }

        internal void CloseConnection()
        {
            // this won't really check if client is connected. 
            // we shoold try to peek some bytes to make sure
            if (_acceptedSocket.Connected)
            {
                _acceptedSocket.Close();
            }
        }

        public void Dispose()
        {
            CloseConnection();

            _acceptedSocket.Dispose();
        }
    }
}