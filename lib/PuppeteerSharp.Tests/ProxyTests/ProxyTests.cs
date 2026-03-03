using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ProxyTests
{
    public sealed class ProxyTests : PuppeteerBaseTest, IDisposable
    {
        private TcpListener _proxyListener;
        private ConcurrentBag<string> _proxiedRequestUrls;
        private string _proxyServerUrl;
        private string _hostname;
        private CancellationTokenSource _cts;

        private static int GetAvailablePort()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)socket.LocalEndPoint).Port;
        }

        private static string GetExternalIPv4Address()
        {
            var hostname = Dns.GetHostName();

            foreach (var address in Dns.GetHostAddresses(hostname))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(address))
                {
                    return address.ToString();
                }
            }

            // Fallback to hostname
            return hostname;
        }

        /// <summary>
        /// Gets the empty page URL using the external hostname instead of localhost,
        /// since requests to localhost/127.0.0.1 are not proxied by default in Chrome.
        /// </summary>
        private string GetEmptyPageUrl()
        {
            var emptyPagePath = new Uri(TestConstants.EmptyPage).AbsolutePath;
            return $"http://{_hostname}:{TestConstants.Port}{emptyPagePath}";
        }

        [SetUp]
        public void ProxySetUp()
        {
            _hostname = GetExternalIPv4Address();
            _proxiedRequestUrls = new ConcurrentBag<string>();
            _cts = new CancellationTokenSource();

            _proxyListener = new TcpListener(IPAddress.Loopback, 0);
            _proxyListener.Start();

            var proxyPort = ((IPEndPoint)_proxyListener.LocalEndpoint).Port;
            _proxyServerUrl = $"http://127.0.0.1:{proxyPort}";

            _ = RunProxyAsync(_cts.Token);
        }

        private async Task RunProxyAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await _proxyListener.AcceptTcpClientAsync(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException)
                {
                    break;
                }

                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII, false, 4096, leaveOpen: true);

                // Read the request line: "GET http://host:port/path HTTP/1.1"
                var requestLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(requestLine))
                {
                    return;
                }

                var parts = requestLine.Split(' ');
                if (parts.Length < 3)
                {
                    return;
                }

                var method = parts[0];
                var requestUrl = parts[1];

                // Read headers until empty line
                while (true)
                {
                    var headerLine = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        break;
                    }
                }

                // Handle CONNECT method (HTTPS tunneling) - just reject it
                // since we only need to test HTTP proxy functionality.
                if (string.Equals(method, "CONNECT", StringComparison.OrdinalIgnoreCase))
                {
                    var connectResponse = Encoding.ASCII.GetBytes("HTTP/1.1 403 Forbidden\r\nContent-Length: 0\r\n\r\n");
                    await stream.WriteAsync(connectResponse, 0, connectResponse.Length);
                    return;
                }

                _proxiedRequestUrls.Add(requestUrl);

                // Rewrite the URL to point to localhost since the test server
                // only listens on 127.0.0.1.
                var uri = new Uri(requestUrl);
                var forwardUrl = $"http://127.0.0.1:{uri.Port}{uri.PathAndQuery}";

                try
                {
                    using var httpClient = new HttpClient();
                    using var proxyRequest = new HttpRequestMessage(new HttpMethod(method), forwardUrl);
                    using var response = await httpClient.SendAsync(proxyRequest);

                    var body = await response.Content.ReadAsByteArrayAsync();

                    var responseBuilder = new StringBuilder();
                    responseBuilder.Append($"HTTP/1.1 {(int)response.StatusCode} {response.ReasonPhrase}\r\n");
                    responseBuilder.Append($"Content-Length: {body.Length}\r\n");

                    foreach (var header in response.Content.Headers)
                    {
                        if (string.Equals(header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        responseBuilder.Append($"{header.Key}: {string.Join(", ", header.Value)}\r\n");
                    }

                    foreach (var header in response.Headers)
                    {
                        responseBuilder.Append($"{header.Key}: {string.Join(", ", header.Value)}\r\n");
                    }

                    responseBuilder.Append("\r\n");

                    var headerBytes = Encoding.ASCII.GetBytes(responseBuilder.ToString());
                    await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                    await stream.WriteAsync(body, 0, body.Length);
                }
                catch
                {
                    var errorResponse = Encoding.ASCII.GetBytes("HTTP/1.1 502 Bad Gateway\r\nContent-Length: 0\r\n\r\n");
                    await stream.WriteAsync(errorResponse, 0, errorResponse.Length);
                }
            }
        }

        [TearDown]
        public void ProxyTearDown()
        {
            try
            {
                _cts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed.
            }

            _proxyListener?.Stop();
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy", "should proxy requests when configured")]
        public async Task ShouldProxyRequestsWhenConfigured()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Append("--disable-features=NetworkTimeServiceQuerying,AimEnabled")
                .Append($"--proxy-server={_proxyServerUrl}")
                .ToArray();

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxiedRequestUrls, Does.Contain(emptyPageUrl));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy", "should respect proxy bypass list")]
        public async Task ShouldRespectProxyBypassList()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var host = new Uri(emptyPageUrl).Host;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Append("--disable-features=NetworkTimeServiceQuerying,AimEnabled")
                .Append($"--proxy-server={_proxyServerUrl}")
                .Append($"--proxy-bypass-list={host}")
                .ToArray();

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var page = await browser.NewPageAsync();

            // When the proxy is bypassed, the browser connects directly.
            // The test server only listens on localhost, so the connection
            // to the external IP will fail, but the proxy should not be used.
            try
            {
                await page.GoToAsync(emptyPageUrl);
            }
            catch (NavigationException)
            {
                // Expected when connecting directly to external IP
                // since test server only listens on localhost.
            }

            Assert.That(_proxiedRequestUrls, Does.Not.Contain(emptyPageUrl));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should proxy requests when configured at browser level")]
        public async Task ShouldProxyRequestsWhenConfiguredAtBrowserLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Append("--disable-features=NetworkTimeServiceQuerying,AimEnabled")
                .Append($"--proxy-server={_proxyServerUrl}")
                .ToArray();

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var context = await browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxiedRequestUrls, Does.Contain(emptyPageUrl));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should respect proxy bypass list when configured at browser level")]
        public async Task ShouldRespectProxyBypassListWhenConfiguredAtBrowserLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var host = new Uri(emptyPageUrl).Host;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Append("--disable-features=NetworkTimeServiceQuerying,AimEnabled")
                .Append($"--proxy-server={_proxyServerUrl}")
                .Append($"--proxy-bypass-list={host}")
                .ToArray();

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var context = await browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            // When the proxy is bypassed, the browser connects directly.
            // The test server only listens on localhost, so the connection
            // to the external IP will fail, but the proxy should not be used.
            try
            {
                await page.GoToAsync(emptyPageUrl);
            }
            catch (NavigationException)
            {
                // Expected when connecting directly to external IP
                // since test server only listens on localhost.
            }

            Assert.That(_proxiedRequestUrls, Does.Not.Contain(emptyPageUrl));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should proxy requests when configured at context level")]
        public async Task ShouldProxyRequestsWhenConfiguredAtContextLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Append("--disable-features=NetworkTimeServiceQuerying,AimEnabled")
                .ToArray();

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var context = await browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                ProxyServer = _proxyServerUrl,
            });
            var page = await context.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxiedRequestUrls, Does.Contain(emptyPageUrl));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should respect proxy bypass list when configured at context level")]
        public async Task ShouldRespectProxyBypassListWhenConfiguredAtContextLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var host = new Uri(emptyPageUrl).Host;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Append("--disable-features=NetworkTimeServiceQuerying,AimEnabled")
                .ToArray();

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var context = await browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                ProxyServer = _proxyServerUrl,
                ProxyBypassList = new[] { host },
            });
            var page = await context.NewPageAsync();

            // When the proxy is bypassed, the browser connects directly.
            // The test server only listens on localhost, so the connection
            // to the external IP will fail, but the proxy should not be used.
            try
            {
                await page.GoToAsync(emptyPageUrl);
            }
            catch (NavigationException)
            {
                // Expected when connecting directly to external IP
                // since test server only listens on localhost.
            }

            Assert.That(_proxiedRequestUrls, Does.Not.Contain(emptyPageUrl));
        }

        public void Dispose()
        {
            try
            {
                _cts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed.
            }

            _proxyListener?.Stop();
            _proxyListener?.Dispose();
            _cts?.Dispose();
        }
    }
}
