using System;
using System.Net.WebSockets;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for connecting to an existing browser.
    /// </summary>
    public class ConnectOptions : IBrowserOptions, IConnectionOptions
    {
        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// If set to true, sets Headless = false, otherwise, enables automation.
        /// </summary>
        [Obsolete("No longer required and usages should be removed")]
        public bool AppMode { get; set; }

        /// <summary>
        /// A browser websocket endpoint to connect to.
        /// </summary>
        public string BrowserWSEndpoint { get; set; }

        /// <summary>
        /// A browser url to connect to, in format `http://${host}:${port}`.
        /// Use interchangeably with `browserWSEndpoint` to let Puppeteer fetch it from <see href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target">metadata endpoin</see>.
        /// </summary>
        public string BrowserURL { get; set; }

        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        public int SlowMo { get; set; }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        [Obsolete("Chromium doesn't support pings yet (see: https://bugs.chromium.org/p/chromium/issues/detail?id=865002)")]
        public int KeepAliveInterval { get; set; } = 0;

        /// <summary>
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// If <see cref="Transport"/> is set this property will be ignored.
        /// </summary>
        public WebSocketFactory WebSocketFactory { get; set; }

        /// <summary>
        /// Gets or sets the default Viewport.
        /// </summary>
        /// <value>The default Viewport.</value>
        public ViewPortOptions DefaultViewport { get; set; } = ViewPortOptions.Default;

        /// <summary>
        /// Optional connection transport.
        /// </summary>
        [Obsolete("Use " + nameof(TransportFactory) + " instead")]
        public IConnectionTransport Transport { get; set; }

        /// <summary>
        /// Optional factory for <see cref="IConnectionTransport"/> implementations.
        /// </summary>
        public TransportFactory TransportFactory { get; set; }

        /// <summary>
        /// If not <see cref="Transport"/> is set this will be use to determine is the default <see cref="WebSocketTransport"/> will enqueue messages.
        /// </summary>
        /// <remarks>
        /// It's set to <c>true</c> by default because it's the safest way to send commands to Chromium.
        /// Setting this to <c>false</c> proved to work in .NET Core but it tends to fail on .NET Framework.
        /// </remarks>
        public bool EnqueueTransportMessages { get; set; } = true;

        /// <summary>
        /// Affects how responses to <see cref="CDPSession.SendAsync"/> are returned to the caller. If <c>true</c> (default), the
        /// response is delivered to the caller on its own thread; otherwise, the response is delivered the same way <see cref="CDPSession.MessageReceived"/>
        /// events are raised.
        /// </summary>
        /// <remarks>
        /// This should normally be set to <c>true</c> to support applications that aren't <c>async</c> "all the way up"; i.e., the application
        /// has legacy code that is not async which makes calls into PuppeteerSharp. If you experience issues, or your application is not mixed sync/async use, you
        /// can set this to <c>false</c> (default).
        /// </remarks>
        public bool EnqueueAsyncMessages { get; set; }

        /// <summary>
        /// Callback to decide if Puppeteer should connect to a given target or not.
        /// </summary>
        public Func<Target, bool> TargetFilter { get; set; }

        /// <inheritdoc />
        public int ProtocolTimeout { get; set; } = Connection.DefaultCommandTimeout;

        /// <summary>
        /// Optional callback to initialize properties as soon as the <see cref="IBrowser"/> instance is created, i.e., set up event handlers.
        /// </summary>
        public Action<IBrowser> InitAction { get; set; }

        /// <summary>
        /// Callback to decide if Puppeteer should connect to a given target or not.
        /// </summary>
        internal Func<Target, bool> IsPageTarget { get; set; }
    }
}
