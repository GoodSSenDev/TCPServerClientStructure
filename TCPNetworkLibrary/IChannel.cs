using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCPNetworkModule
{
    public interface IChannel
    {
        Guid Id { get; }

        DateTime LastSent { get; }
        DateTime LastReceived { get; }

        event EventHandler Closed;

        void Attach(Socket socket);
        void Close();
        void Dispose();
        Task SendAsync<TMessage>(TMessage message);
    }
}
