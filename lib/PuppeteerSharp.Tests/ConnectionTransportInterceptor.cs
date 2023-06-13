using System;
using System.Threading.Tasks;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests
{
    public sealed class ConnectionTransportInterceptor : IConnectionTransport
    {
        private readonly IConnectionTransport _connectionTransport;

        public ConnectionTransportInterceptor(IConnectionTransport connectionTransport)
        {
            _connectionTransport = connectionTransport;
        }

        public event EventHandler<string> MessageSent;

        public Task SendAsync(string message)
        {
            var task = _connectionTransport.SendAsync(message);
            MessageSent?.Invoke(_connectionTransport, message);
            return task;
        }

        public bool IsClosed => _connectionTransport.IsClosed;

        public event EventHandler<TransportClosedEventArgs> Closed
        {
            add { _connectionTransport.Closed += value; }
            remove { _connectionTransport.Closed -= value; }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { _connectionTransport.MessageReceived += value; }
            remove { _connectionTransport.MessageReceived -= value; }
        }

        public void Dispose() => _connectionTransport.Dispose();

        public void StopReading() => _connectionTransport.StopReading();
    }
}
