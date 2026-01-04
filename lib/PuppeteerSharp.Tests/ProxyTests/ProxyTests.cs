using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ProxyTests
{
    public sealed class ProxyTests : PuppeteerBrowserBaseTest, IDisposable
    {
        private SimpleProxyServer _proxyServer;
        private string _proxyServerUrl;
        private string _hostname;

        [SetUp]
        public void SetupProxy()
        {
            _hostname = GetExternalHostname();
            _proxyServer = new SimpleProxyServer();
            _proxyServer.Start();
            _proxyServerUrl = $"http://{_hostname}:{_proxyServer.Port}";
        }

        [TearDown]
        public async Task TeardownProxy()
        {
            if (_proxyServer != null)
            {
                await _proxyServer.StopAsync();
                _proxyServer.Dispose();
                _proxyServer = null;
            }
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy", "should proxy requests when configured")]
        public async Task ShouldProxyRequestsWhenConfigured()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                .. options.Args ?? [],
                $"--proxy-server={_proxyServerUrl}",
                "--disable-features=NetworkTimeServiceQuerying,AimEnabled"
            ];

            await using var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxyServer.ProxiedRequestUrls, Is.EqualTo(new[] { emptyPageUrl }));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy", "should respect proxy bypass list")]
        public async Task ShouldRespectProxyBypassList()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var host = new Uri(emptyPageUrl).Host;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                .. options.Args ?? [],
                $"--proxy-server={_proxyServerUrl}",
                $"--proxy-bypass-list={host}",
                "--disable-features=NetworkTimeServiceQuerying,AimEnabled"
            ];

            await using var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxyServer.ProxiedRequestUrls, Is.Empty);
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should proxy requests when configured at browser level")]
        public async Task ShouldProxyRequestsWhenConfiguredAtBrowserLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                .. options.Args ?? [],
                $"--proxy-server={_proxyServerUrl}",
                "--disable-features=NetworkTimeServiceQuerying,AimEnabled"
            ];

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxyServer.ProxiedRequestUrls, Is.EqualTo(new[] { emptyPageUrl }));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should respect proxy bypass list when configured at browser level")]
        public async Task ShouldRespectProxyBypassListWhenConfiguredAtBrowserLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var host = new Uri(emptyPageUrl).Host;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                .. options.Args ?? [],
                $"--proxy-server={_proxyServerUrl}",
                $"--proxy-bypass-list={host}",
                "--disable-features=NetworkTimeServiceQuerying,AimEnabled"
            ];

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxyServer.ProxiedRequestUrls, Is.Empty);
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should proxy requests when configured at context level")]
        public async Task ShouldProxyRequestsWhenConfiguredAtContextLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                .. options.Args ?? [],
                "--disable-features=NetworkTimeServiceQuerying,AimEnabled"
            ];

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                ProxyServer = _proxyServerUrl,
            });
            var page = await context.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxyServer.ProxiedRequestUrls, Is.EqualTo(new[] { emptyPageUrl }));
        }

        [Test, PuppeteerTest("proxy.spec", "request proxy in incognito browser context", "should respect proxy bypass list when configured at context level")]
        public async Task ShouldRespectProxyBypassListWhenConfiguredAtContextLevel()
        {
            var emptyPageUrl = GetEmptyPageUrl();
            var host = new Uri(emptyPageUrl).Host;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                .. options.Args ?? [],
                "--disable-features=NetworkTimeServiceQuerying,AimEnabled"
            ];

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync(new BrowserContextOptions
            {
                ProxyServer = _proxyServerUrl,
                ProxyBypassList = [host],
            });
            var page = await context.NewPageAsync();
            var response = await page.GoToAsync(emptyPageUrl);

            Assert.That(response.Ok, Is.True);
            Assert.That(_proxyServer.ProxiedRequestUrls, Is.Empty);
        }

        private string GetEmptyPageUrl()
        {
            // Requests to localhost do not get proxied by default. Create a URL using the hostname instead.
            var emptyPagePath = new Uri(TestConstants.EmptyPage).PathAndQuery;
            return $"http://{_hostname}:{TestConstants.Port}{emptyPagePath}";
        }

        private static string GetExternalHostname()
        {
            // Try to find an external IPv4 address to be used as a hostname in these tests.
            var hostname = Dns.GetHostName();
            try
            {
                var hostEntry = Dns.GetHostEntry(hostname);
                foreach (var address in hostEntry.AddressList)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
                    {
                        return address.ToString();
                    }
                }
            }
            catch
            {
                // Ignore exceptions and fall back to hostname
            }

            return hostname;
        }

        public void Dispose()
        {
            _proxyServer?.Dispose();
        }

        /// <summary>
        /// Simple HTTP proxy server for testing using TcpListener.
        /// </summary>
        private sealed partial class SimpleProxyServer : IDisposable
        {
#pragma warning disable CA2213 // TcpListener's Server socket is disposed in Dispose method
            private TcpListener _listener;
#pragma warning restore CA2213
            private CancellationTokenSource _cts;
            private Task _runTask;
            private bool _disposed;

            public int Port { get; private set; }

            public ConcurrentBag<string> ProxiedRequestUrls { get; } = [];

            public void Start()
            {
                _listener = new TcpListener(IPAddress.Any, 0);
                _listener.Start();
                Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                _cts = new CancellationTokenSource();
                _runTask = Task.Run(() => RunAsync(_cts.Token));
            }

            public async Task StopAsync()
            {
                _cts?.Cancel();
                _listener?.Stop();
                if (_runTask != null)
                {
                    try
                    {
                        await _runTask.ConfigureAwait(false);
                    }
                    catch
                    {
                        // Ignore exceptions during shutdown
                    }
                }
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _cts?.Cancel();

                // Dispose the listener using its server socket
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener.Server?.Dispose();
                }

                _cts?.Dispose();
            }

            private async Task RunAsync(CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var client = await _listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                        _ = ProcessClientAsync(client);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
            }

            private async Task ProcessClientAsync(TcpClient client)
            {
                try
                {
                    using (client)
                    {
                        using var stream = client.GetStream();
                        using var reader = new StreamReader(stream, Encoding.ASCII, false, 8192, true);
                        using var writer = new StreamWriter(stream, new UTF8Encoding(false), 8192, true) { NewLine = "\r\n" };

                        // Read the request line
                        var requestLine = await reader.ReadLineAsync().ConfigureAwait(false);
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
                        var url = parts[1];

                        // Record the proxied URL
                        ProxiedRequestUrls.Add(url);

                        // Read headers
                        var headers = new StringBuilder();
                        string line;
                        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync().ConfigureAwait(false)))
                        {
                            headers.AppendLine(line);
                        }

                        // Parse the target URL
                        if (!Uri.TryCreate(url, UriKind.Absolute, out var targetUri))
                        {
                            await SendErrorResponse(writer, 400, "Bad Request").ConfigureAwait(false);
                            return;
                        }

                        // Forward the request
                        using var httpClient = new HttpClient();
                        using var requestMessage = new HttpRequestMessage(new HttpMethod(method), targetUri);

                        // Parse and add headers
                        foreach (var headerLine in headers.ToString().Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries))
                        {
                            var colonIndex = headerLine.IndexOf(':');
                            if (colonIndex > 0)
                            {
                                var headerName = headerLine[..colonIndex].Trim();
                                var headerValue = headerLine[(colonIndex + 1)..].Trim();
                                if (!headerName.Equals("Host", StringComparison.OrdinalIgnoreCase) &&
                                    !headerName.Equals("Connection", StringComparison.OrdinalIgnoreCase) &&
                                    !headerName.Equals("Proxy-Connection", StringComparison.OrdinalIgnoreCase))
                                {
                                    requestMessage.Headers.TryAddWithoutValidation(headerName, headerValue);
                                }
                            }
                        }

                        var response = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                        // Send the response
                        await writer.WriteLineAsync($"HTTP/1.1 {(int)response.StatusCode} {response.ReasonPhrase}").ConfigureAwait(false);

                        foreach (var header in response.Headers)
                        {
                            await writer.WriteLineAsync($"{header.Key}: {string.Join(", ", header.Value)}").ConfigureAwait(false);
                        }

                        foreach (var header in response.Content.Headers)
                        {
                            await writer.WriteLineAsync($"{header.Key}: {string.Join(", ", header.Value)}").ConfigureAwait(false);
                        }

                        await writer.WriteLineAsync().ConfigureAwait(false);
                        await writer.FlushAsync().ConfigureAwait(false);

                        // Copy the response body
                        await response.Content.CopyToAsync(stream).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // Ignore exceptions during request processing
                }
            }

            private static async Task SendErrorResponse(StreamWriter writer, int statusCode, string reason)
            {
                await writer.WriteLineAsync($"HTTP/1.1 {statusCode} {reason}").ConfigureAwait(false);
                await writer.WriteLineAsync("Content-Length: 0").ConfigureAwait(false);
                await writer.WriteLineAsync().ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
