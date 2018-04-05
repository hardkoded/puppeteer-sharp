using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace PuppeteerSharp.TestServer
{
    public class SimpleServer
    {
        private readonly IDictionary<string, TaskCompletionSource<HttpRequest>> _requestSubscribers;
        private readonly IDictionary<string, RequestDelegate> _routes;
        private readonly IDictionary<string, (string username, string password)> _auths;
        private readonly IWebHost _webHost;

        public static SimpleServer Create(int port) => new SimpleServer(port, isHttps: false);
        public static SimpleServer CreateHttps(int port) => new SimpleServer(port, isHttps: true);

        public SimpleServer(int port, bool isHttps)
        {
            _requestSubscribers = new ConcurrentDictionary<string, TaskCompletionSource<HttpRequest>>();
            _routes = new ConcurrentDictionary<string, RequestDelegate>();
            _auths = new ConcurrentDictionary<string, (string username, string password)>();
            _webHost = new WebHostBuilder()
                .ConfigureAppConfiguration((context, builder) => builder
                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                    .AddEnvironmentVariables()
                )
                .Configure(app => app.Use((context, next) =>
                    {
                        if (_auths.TryGetValue(context.Request.Path, out var auth) && Authenticate(auth.username, auth.password, context))
                        {
                            context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return context.Response.WriteAsync("HTTP Error 401 Unauthorized: Access is denied");
                        }
                        if (_requestSubscribers.TryGetValue(context.Request.Path, out var subscriber))
                        {
                            subscriber.SetResult(context.Request);
                        }
                        if (_routes.TryGetValue(context.Request.Path, out var handler))
                        {
                            return handler(context);
                        }
                        return next();
                    })
                    .UseStaticFiles())
                .UseKestrel(options =>
                {
                    if (isHttps)
                    {
                        options.Listen(IPAddress.Loopback, port);
                    }
                    else
                    {
                        options.Listen(IPAddress.Loopback, port, listenOptions => listenOptions.UseHttps("testCert.cer"));
                    }
                })
                .Build();
        }

        public void SetAuth(string path, string username, string password)
        {
            _auths.Add(path, (username, password));
        }

        public async Task Stop()
        {
            Reset();

            await _webHost.StopAsync();
        }

        public void Reset()
        {
            _routes.Clear();
            _auths.Clear();
            var exception = new Exception("Static Server has been reset");
            foreach (var subscriber in _requestSubscribers.Values)
            {
                subscriber.SetException(exception);
            }
            _requestSubscribers.Clear();
        }

        public void SetRoute(string path, RequestDelegate handler)
        {
            _routes.Add(path, handler);
        }

        public void SetRedirect(string from, string to)
        {
            SetRoute(from, context =>
            {
                context.Response.Redirect(to);
                return Task.CompletedTask;
            });
        }

        public async Task<HttpRequest> WaitForRequest(string path)
        {
            var taskCompletion = new TaskCompletionSource<HttpRequest>();
            _requestSubscribers.Add(path, taskCompletion);

            var request = await taskCompletion.Task;
            _requestSubscribers.Remove(path);

            return request;
        }

        private static bool Authenticate(string username, string password, HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader.StartsWith("Basic", StringComparison.Ordinal))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                string auth = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                return auth == $"{username}:{password}";
            }
            return false;
        }
    }
}
