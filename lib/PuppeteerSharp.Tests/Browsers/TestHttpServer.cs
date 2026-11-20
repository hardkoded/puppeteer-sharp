using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.Browsers
{
    /// <summary>
    /// Minimal raw HTTP server. The handler returns a status code: 200 serves the content,
    /// 0 drops the connection, -1 never answers, anything else is sent as an empty error response.
    /// </summary>
    internal sealed class TestHttpServer : IDisposable
    {
        public static readonly byte[] TestContent = Encoding.UTF8.GetBytes(
            "test browser binary content"
        );

        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly Func<int, int> _handler;
        private int _requests;

        public TestHttpServer(Func<int, int> handler)
        {
            _handler = handler;
            _listener.Start();
            _ = AcceptLoopAsync();
        }

        public int Requests => Volatile.Read(ref _requests);

        public string Url => $"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/test";

        public void Dispose()
        {
            _listener.Stop();
            _listener.Dispose();
        }

        private async Task AcceptLoopAsync()
        {
            try
            {
                while (true)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleAsync(client);
                }
            }
            catch (Exception) when (!_listener.Server.IsBound) { }
        }

        private async Task HandleAsync(TcpClient client)
        {
            using (client)
            {
                var attempt = Interlocked.Increment(ref _requests);
                var stream = client.GetStream();
                var buffer = new byte[4096];
                _ = await stream.ReadAsync(buffer, 0, buffer.Length);
                var status = _handler(attempt);
                if (status == -1)
                {
                    // Never answer: lets the client time out.
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return;
                }

                if (status == 0)
                {
                    client.Client.LingerState = new LingerOption(true, 0);
                    return;
                }

                var body = status == 200 ? TestContent : Encoding.UTF8.GetBytes("fail");
                var header = Encoding.ASCII.GetBytes(
                    $"HTTP/1.1 {status} X\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n"
                );
                await stream.WriteAsync(header, 0, header.Length);
                await stream.WriteAsync(body, 0, body.Length);
            }
        }
    }
}
