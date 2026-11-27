using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.Browsers
{
    /// <summary>
    /// Minimal raw HTTP server. The handler returns a status code: 200 serves the content,
    /// 0 drops the connection, -1 never answers, -2 sends headers and then stalls the body, a redirect function returning a location answers 302 first, anything else is sent as an empty error response.
    /// </summary>
    internal sealed class TestHttpServer : IDisposable
    {
        public static readonly byte[] TestContent = Encoding.UTF8.GetBytes(
            "test browser binary content"
        );

        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly Func<int, int> _handler;
        private readonly Func<string, string> _redirect;
        private int _requests;

        public TestHttpServer(Func<int, int> handler, Func<string, string> redirect = null)
        {
            _handler = handler;
            _redirect = redirect;
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
                var request = new StringBuilder();
                int read;
                while (
                    request.Length < 8192
                    && !request.ToString().Contains("\r\n")
                    && (read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0
                )
                {
                    request.Append(Encoding.ASCII.GetString(buffer, 0, read));
                }

                var requestLine = request.ToString().Split('\r')[0];
                var target = requestLine.Split(' ').ElementAtOrDefault(1) ?? "/";
                var location = _redirect?.Invoke(target);
                if (location != null)
                {
                    var redirectHeader = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 302 Found\r\nLocation: {location}\r\nContent-Length: 0\r\nConnection: close\r\n\r\n"
                    );
                    await stream.WriteAsync(redirectHeader, 0, redirectHeader.Length);
                    return;
                }

                var status = _handler(attempt);
                if (status == -1)
                {
                    // Never answer: lets the client time out.
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return;
                }

                if (status == -2)
                {
                    var stallHeader = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 200 OK\r\nContent-Length: {TestContent.Length}\r\nConnection: close\r\n\r\n"
                    );
                    await stream.WriteAsync(stallHeader, 0, stallHeader.Length);
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
