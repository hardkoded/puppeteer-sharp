using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.TransportTests
{
    [TestFixture]
    public class WebSocketTransportKeepAliveTests
    {
        [Test]
        [Obsolete]
        public void GetEffectiveHeaders_PrefersWsOptionsHeaders()
        {
            var options = new ConnectOptions
            {
                Headers = new Dictionary<string, string> { ["X-Legacy"] = "legacy" },
                WsOptions = new WsOptions
                {
                    Headers = new Dictionary<string, string> { ["X-Modern"] = "modern" },
                },
            };

            var headers = ConnectionOptionsHelper.GetEffectiveHeaders(options);

            Assert.That(headers, Is.Not.Null);
            Assert.That(headers["X-Modern"], Is.EqualTo("modern"));
            Assert.That(headers.ContainsKey("X-Legacy"), Is.False);
        }

        [Test]
        public void ConfigureWebSocketKeepAlive_DisablesKeepAliveByDefault()
        {
            using var client = new ClientWebSocket();
            ConnectionOptionsHelper.ConfigureWebSocketKeepAlive(client, null);

            Assert.That(client.Options.KeepAliveInterval, Is.EqualTo(TimeSpan.Zero));
        }

#if NET9_0_OR_GREATER
        [Test]
        public void ConfigureWebSocketKeepAlive_EnablesPingPongWhenRequested()
        {
            using var client = new ClientWebSocket();
            ConnectionOptionsHelper.ConfigureWebSocketKeepAlive(
                client,
                new WsOptions { KeepAlive = true, KeepAliveIntervalMs = 1234 });

            Assert.That(client.Options.KeepAliveInterval, Is.EqualTo(TimeSpan.FromMilliseconds(1234)));
            Assert.That(client.Options.KeepAliveTimeout, Is.EqualTo(TimeSpan.FromMilliseconds(1234)));
        }

        [Test]
        public async Task KeepAlive_StaysOpenWhilePeerAnswersPings()
        {
            using var listener = new HttpListener();
            var prefix = $"http://127.0.0.1:{GetFreePort()}/";
            listener.Prefixes.Add(prefix);
            listener.Start();

            var serverAccepted = new TaskCompletionSource<WebSocket>(TaskCreationOptions.RunContinuationsAsynchronously);
            _ = Task.Run(async () =>
            {
                var context = await listener.GetContextAsync().ConfigureAwait(false);
                var webSocketContext = await context.AcceptWebSocketAsync(null).ConfigureAwait(false);
                serverAccepted.TrySetResult(webSocketContext.WebSocket);
                var buffer = new byte[128];
                while (webSocketContext.WebSocket.State == WebSocketState.Open)
                {
                    await webSocketContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                }
            });

            var connectOptions = new ConnectOptions
            {
                WsOptions = new WsOptions
                {
                    KeepAlive = true,
                    KeepAliveIntervalMs = 50,
                },
            };

            var transport = await WebSocketTransport.DefaultTransportFactory(
                new Uri(prefix.Replace("http://", "ws://")),
                connectOptions,
                CancellationToken.None).ConfigureAwait(false);

            var closed = false;
            transport.Closed += (_, _) => closed = true;

            await serverAccepted.Task.WithTimeout(5000).ConfigureAwait(false);
            await Task.Delay(300).ConfigureAwait(false);

            Assert.That(closed, Is.False);
            transport.Dispose();
        }
#endif

        private static int GetFreePort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
    }
}
