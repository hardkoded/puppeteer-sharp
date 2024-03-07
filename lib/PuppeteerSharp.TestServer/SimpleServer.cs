using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PuppeteerSharp.TestServer
{
    public class SimpleServer
    {
        private readonly IDictionary<string, Action<HttpRequest>> _requestSubscribers;
        private readonly IDictionary<string, RequestDelegate> _routes;
        private readonly IDictionary<string, (string username, string password)> _auths;
        private readonly IDictionary<string, string> _csp;
        private readonly IWebHost _webHost;

        internal IList<string> GzipRoutes { get; }
        public static SimpleServer Create(int port, string contentRoot) => new SimpleServer(port, contentRoot, isHttps: false);
        public static SimpleServer CreateHttps(int port, string contentRoot) => new SimpleServer(port, contentRoot, isHttps: true);

        private SimpleServer(int port, string contentRoot, bool isHttps)
        {
            _requestSubscribers = new ConcurrentDictionary<string, Action<HttpRequest>>();
            _routes = new ConcurrentDictionary<string, RequestDelegate>();
            _auths = new ConcurrentDictionary<string, (string username, string password)>();
            _csp = new ConcurrentDictionary<string, string>();
            GzipRoutes = new List<string>();

            _webHost = new WebHostBuilder()
                .ConfigureAppConfiguration((context, builder) => builder
                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                    .AddEnvironmentVariables()
                )
                .Configure(app => app.Use((context, next) =>
                    {
                        if (_auths.TryGetValue(context.Request.Path, out var auth) && !Authenticate(auth.username, auth.password, context))
                        {
                            context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return context.Response.WriteAsync("HTTP Error 401 Unauthorized: Access is denied");
                        }
                        if (_requestSubscribers.TryGetValue(context.Request.Path, out var subscriber))
                        {
                            subscriber(context.Request);
                        }
                        if (_routes.TryGetValue(context.Request.Path + context.Request.QueryString, out var handler))
                        {
                            return handler(context);
                        }

                        return next();
                    })
                    .UseMiddleware<SimpleCompressionMiddleware>(this)
                    .UseStaticFiles(new StaticFileOptions
                    {
                        OnPrepareResponse = fileResponseContext =>
                        {
                            if (_csp.TryGetValue(fileResponseContext.Context.Request.Path, out var csp))
                            {
                                fileResponseContext.Context.Response.Headers["Content-Security-Policy"] = csp;
                            }

                            if (fileResponseContext.Context.Request.Path.Value != null && !fileResponseContext.Context.Request.Path.Value.StartsWith("/cached/"))
                            {
                                fileResponseContext.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                                fileResponseContext.Context.Response.Headers["Expires"] = "-1";
                            }
                        }
                    }))
                .UseKestrel(options =>
                {
                    options.ConfigureEndpointDefaults(lo => lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);
                    if (isHttps)
                    {
                        options.Listen(IPAddress.Loopback, port, listenOptions => listenOptions.UseHttps("testCert.cer"));
                    }
                    else
                    {
                        options.Listen(IPAddress.Loopback, port);
                    }
                })
                .UseContentRoot(contentRoot)
                .Build();
        }

        public void SetAuth(string path, string username, string password) => _auths.Add(path, (username, password));

        public void SetCSP(string path, string csp) => _csp.Add(path, csp);

        public Task StartAsync() => _webHost.StartAsync();

        public async Task StopAsync()
        {
            Reset();

            await _webHost.StopAsync();
        }

        public void Reset()
        {
            _routes.Clear();
            _auths.Clear();
            _csp.Clear();
            GzipRoutes.Clear();
            foreach (var subscriber in _requestSubscribers.Values)
            {
                subscriber(null);
            }
            _requestSubscribers.Clear();
        }

        public void EnableGzip(string path) => GzipRoutes.Add(path);

        public void SetRoute(string path, RequestDelegate handler) => _routes.Add(path, handler);

        public void SetRedirect(string from, string to) => SetRoute(from, context =>
        {
            context.Response.Redirect(to);
            return Task.CompletedTask;
        });

        public async Task<T> WaitForRequest<T>(string path, Func<HttpRequest, T> selector)
        {
            var taskCompletion = new TaskCompletionSource<T>();
            _requestSubscribers.Add(path, (httpRequest) =>
            {
                taskCompletion.SetResult(selector(httpRequest));
            });

            var request = await taskCompletion.Task;
            _requestSubscribers.Remove(path);

            return request;
        }

        public Task WaitForRequest(string path) => WaitForRequest(path, _ => true);

        private static bool Authenticate(string username, string password, HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic", StringComparison.Ordinal))
            {
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var auth = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                return auth == $"{username}:{password}";
            }
            return false;
        }
    }
}
