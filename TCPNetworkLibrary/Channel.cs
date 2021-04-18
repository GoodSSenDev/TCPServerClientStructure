using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPNetworkModule
{
    public abstract class Channel<TProtocol, TSerializedDataType> : IDisposable, IChannel where TProtocol : Protocol<TSerializedDataType>, new()
    {

        protected bool _isDisposed = false;
        protected bool _isClosed = false;

        readonly TProtocol _protocol = new TProtocol();
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        Func<TSerializedDataType, Task> _messageCallback;
        NetworkStream _networkStream;
        Task _receiveLoopTask;

        public event EventHandler Closed;

        public Guid Id { get; } = Guid.NewGuid();

        public DateTime LastSent { get; protected set; }

        public DateTime LastReceived { get; protected set; }

        public void Attach(Socket socket)
        {
            _networkStream = new NetworkStream(socket, true);
            _receiveLoopTask = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
        }

        public void OnMessage(Func<TSerializedDataType, Task> callbackHandler)
            => _messageCallback = callbackHandler;

        public void Close()
        {
            if (!_isClosed)
            {
                _isClosed = true;
                _cancellationTokenSource.Cancel();
                _networkStream?.Close();
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task SendAsync<TMessage>(TMessage message)
        {
            await _protocol.SendAsync(_networkStream, message).ConfigureAwait(false);
            LastSent = DateTime.UtcNow;
        }

        protected virtual async Task ReceiveLoop()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    //TODO: Pass Cancellation Token to Protocol methods
                    var msg = await _protocol.ReceiveAsync(_networkStream).ConfigureAwait(false);
                    LastReceived = DateTime.UtcNow;
                    await _messageCallback(msg).ConfigureAwait(false);
                }
            }
            catch (System.IO.IOException)
            {
                Close();
            }
            catch (Exception _e)
            {
                Console.WriteLine($"Channel::ReceiveLoop || Exception => {_e}");
                Close();
            }
        }

        ~Channel() => Dispose(false);
        public void Dispose() => Dispose(true);
        protected void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                Close();
                //TODO: Clean up socket, stream, etc.
                _networkStream?.Dispose();

                if (isDisposing)
                    GC.SuppressFinalize(this);
            }
        }
    }
}
