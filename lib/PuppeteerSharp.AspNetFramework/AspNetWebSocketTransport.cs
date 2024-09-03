using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.AspNetFramework
{
    public class AspNetWebSocketTransport : WebSocketTransport
    {
        /// <summary>
        /// Gets a <see cref="TransportFactory"/> that creates <see cref="AspNetWebSocketTransport"/> instances.
        /// </summary>
        public static readonly TransportFactory AspNetTransportFactory = CreateAspNetTransport;

        /// <summary>
        /// Gets a <see cref="TransportTaskScheduler"/> that uses ASP.NET <see cref="HostingEnvironment.QueueBackgroundWorkItem(Func{CancellationToken,Task})"/>
        /// for scheduling of tasks.
        /// </summary>
        public static readonly TransportTaskScheduler AspNetTransportScheduler = ScheduleBackgroundTask;

        private static async Task<IConnectionTransport> CreateAspNetTransport(Uri url, IConnectionOptions connectionOptions, CancellationToken cancellationToken)
        {
            var webSocketFactory = connectionOptions.WebSocketFactory ?? DefaultWebSocketFactory;
            var webSocket = await webSocketFactory(url, connectionOptions, cancellationToken);
            return new AspNetWebSocketTransport(webSocket, connectionOptions.EnqueueTransportMessages);
        }

        private static void ScheduleBackgroundTask(Func<CancellationToken, Task> taskFactory, CancellationToken cancellationToken)
        {
            Task ExecuteAsync(CancellationToken hostingCancellationToken)
                => taskFactory(CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, hostingCancellationToken).Token);
            HostingEnvironment.QueueBackgroundWorkItem(ExecuteAsync);
        }

        /// <inheritdoc />
        public AspNetWebSocketTransport(WebSocket client, bool queueRequests)
            : base(client, AspNetTransportScheduler, queueRequests)
        { }
    }
}
