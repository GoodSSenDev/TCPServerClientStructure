﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPNetworkModule
{
    public abstract class Channel<TProtocol, TSerializedDataType> : IDisposable
          where TProtocol : Protocol<TSerializedDataType>, new()
    {

        protected bool _isDisposed = false;

        readonly TProtocol _protocol = new TProtocol();
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        Func<TSerializedDataType, Task> _messageCallback;
        NetworkStream _networkStream;
        Task _receiveLoopTask;


        public void Attach(Socket socket)
        {
            _networkStream = new NetworkStream(socket, true);
            _receiveLoopTask = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
        }

        public void OnMessage(Func<TSerializedDataType, Task> callbackHandler)
            => _messageCallback = callbackHandler;

        public void Close()
        {
            _cancellationTokenSource.Cancel();
            _networkStream?.Close();
        }

        public async Task SendAsync<T>(T message)
            => await _protocol.SendAsync(_networkStream, message).ConfigureAwait(false);

        protected virtual async Task ReceiveLoop()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                //TODO: Pass Cancellation Token to Protocol methods
                var msg = await _protocol.ReceiveAsync(_networkStream).ConfigureAwait(false);
                await _messageCallback(msg).ConfigureAwait(false);
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
                //TODO: Clean up socket, stream, etc...
                _networkStream?.Dispose();

                if (isDisposing)
                    GC.SuppressFinalize(this);
            }
        }
    }
}
