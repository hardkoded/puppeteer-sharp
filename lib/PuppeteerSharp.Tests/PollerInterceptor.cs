using System;
using System.Threading.Tasks;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests
{
    public sealed class PollerInterceptor(IConnectionTransport connectionTransport) : IConnectionTransport
    {
        public event EventHandler<string> MessageSent;

        public Task SendAsync(string message)
        {
            var task = connectionTransport.SendAsync(message);
            MessageSent?.Invoke(connectionTransport, message);
            return task;
        }

        public bool IsClosed => connectionTransport.IsClosed;

        public event EventHandler<TransportClosedEventArgs> Closed
        {
            add { connectionTransport.Closed += value; }
            remove { connectionTransport.Closed -= value; }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { connectionTransport.MessageReceived += value; }
            remove { connectionTransport.MessageReceived -= value; }
        }

        public void Dispose() => connectionTransport.Dispose();

        public void StopReading() => connectionTransport.StopReading();

        public Task<bool> WaitForStartPollingAsync()
        {
            var startedPolling = new TaskCompletionSource<bool>();

            // Wait for function will release the execution faster than in node.
            // We intercept the poller.start() call to prevent tests from continuing before the polling has started.
            MessageSent += (_, message) =>
            {
                if (message.Contains("poller.start()"))
                {
                    startedPolling.SetResult(true);
                }
            };

            return startedPolling.Task;
        }
    }
}
