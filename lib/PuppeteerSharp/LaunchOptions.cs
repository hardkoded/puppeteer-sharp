using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options for launching the Chrome/ium browser.
    /// </summary>
    public class LaunchOptions : IBrowserOptions, IConnectionOptions
    {
        private string[] _ignoredDefaultArgs;
        private bool _devtools;

        /// <summary>
        /// Chrome Release Channel.
        /// </summary>
        public ChromeReleaseChannel? Channel { get; set; }

        /// <summary>
        /// Whether to ignore HTTPS errors during navigation. Defaults to false.
        /// </summary>
        public bool IgnoreHTTPSErrors { get; set; }

        /// <summary>
        /// Whether to run browser in headless mode. Defaults to true unless the devtools option is true.
        /// If you need to run using the old headless mode, set <see cref="LaunchOptions.HeadlessMode"/> this to <see cref="HeadlessMode.Shell"/>.
        /// </summary>
        public bool Headless
        {
            get => HeadlessMode != HeadlessMode.False;
            set => HeadlessMode = value ? HeadlessMode.True : HeadlessMode.False;
        }

        /// <summary>
        /// Whether to run browser in headless mode. Defaults to true unless the devtools option is true.
        /// </summary>
        public HeadlessMode HeadlessMode { get; set; } = HeadlessMode.True;

        /// <summary>
        /// Path to a Chromium or Chrome executable to run instead of bundled Chromium. If executablePath is a relative path, then it is resolved relative to current working directory.
        /// </summary>
        public string ExecutablePath { get; set; }

        /// <summary>
        /// Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.
        /// </summary>
        public int SlowMo { get; set; }

        /// <summary>
        /// Additional arguments to pass to the browser instance. List of Chromium flags can be found <a href="http://peter.sh/experiments/chromium-command-line-switches/">here</a>.
        /// </summary>
        public string[] Args { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Maximum time in milliseconds to wait for the browser instance to start. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// </summary>
        public int Timeout { get; set; } = Puppeteer.DefaultTimeout;

        /// <summary>
        ///  Whether to pipe browser process stdout and stderr into process.stdout and process.stderr. Defaults to false.
        /// </summary>
        public bool DumpIO { get; set; }

        /// <summary>
        /// Path to a User Data Directory.
        /// </summary>
        public string UserDataDir { get; set; }

        /// <summary>
        /// Specify environment variables that will be visible to browser. Defaults to Environment variables.
        /// </summary>
        public IDictionary<string, string> Env { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Whether to auto-open DevTools panel for each tab. If this option is true, the headless option will be set false.
        /// </summary>
        public bool Devtools
        {
            get => _devtools;
            set
            {
                _devtools = value;
                if (value)
                {
                    HeadlessMode = HeadlessMode.False;
                }
            }
        }

        /// <summary>
        /// Keep alive value.
        /// </summary>
        [Obsolete("Chromium doesn't support pings yet (see: https://bugs.chromium.org/p/chromium/issues/detail?id=865002)")]
        public int KeepAliveInterval { get; set; } = 0;

        /// <summary>
        /// Logs process counts after launching chrome and after exiting.
        /// </summary>
        public bool LogProcess { get; set; }

        /// <summary>
        /// If <c>true</c>, then do not use <see cref="ChromeLauncher.GetDefaultArgs"/>.
        /// Dangerous option; use with care. Defaults to <c>false</c>.
        /// </summary>
        public bool IgnoreDefaultArgs { get; set; }

        /// <summary>
        /// if <see cref="IgnoreDefaultArgs"/> is set to <c>false</c> this list will be used to filter <see cref="ChromeLauncher.GetDefaultArgs"/>.
        /// </summary>
        public string[] IgnoredDefaultArgs
        {
            get => _ignoredDefaultArgs;
            set
            {
                IgnoreDefaultArgs = true;
                _ignoredDefaultArgs = value;
            }
        }

        /// <summary>
        /// Optional factory for <see cref="WebSocket"/> implementations.
        /// If <see cref="Transport"/> is set this property will be ignored.
        /// </summary>
        /// <remarks>
        /// If you need to run Puppeteer-Sharp on Windows 7, you can use <seealso cref="WebSocketFactory"/> to inject <see href="https://www.nuget.org/packages/System.Net.WebSockets.Client.Managed/">System.Net.WebSockets.Client.Managed</see>.
        /// <example>
        /// <![CDATA[
        /// WebSocketFactory = async (uri, socketOptions, cancellationToken) =>
        /// {
        ///     var client = SystemClientWebSocket.CreateClientWebSocket();
        ///     if (client is System.Net.WebSockets.Managed.ClientWebSocket managed)
        ///     {
        ///        managed.Options.KeepAliveInterval = TimeSpan.FromSeconds(0);
        ///         await managed.ConnectAsync(uri, cancellationToken);
        ///     }
        ///     else
        ///     {
        ///         var coreSocket = client as ClientWebSocket;
        ///         coreSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(0);
        ///         await coreSocket.ConnectAsync(uri, cancellationToken);
        ///     }
        ///
        ///     return client;
        /// },
        /// ]]>
        /// </example>
        /// </remarks>
        public WebSocketFactory WebSocketFactory { get; set; }

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
        /// Gets or sets the default Viewport.
        /// </summary>
        /// <value>The default Viewport.</value>
        public ViewPortOptions DefaultViewport { get; set; } = ViewPortOptions.Default;

        /// <summary>
        /// If not <see cref="Transport"/> is set this will be use to determine is the default <see cref="WebSocketTransport"/> will enqueue messages.
        /// </summary>
        /// <remarks>
        /// It's set to <c>true</c> by default because it's the safest way to send commands to Chromium.
        /// Setting this to <c>false</c> proved to work in .NET Core but it tends to fail on .NET Framework.
        /// </remarks>
        public bool EnqueueTransportMessages { get; set; } = true;

        /// <summary>
        /// The browser to be used (Chrome, Chromium, Firefox).
        /// </summary>
        public SupportedBrowser Browser { get; set; } = SupportedBrowser.Chrome;

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
        /// Additional preferences that can be passed when launching with Firefox. <see fref="https://searchfox.org/mozilla-release/source/modules/libpref/init/all.js">See</see>.
        /// </summary>
        public Dictionary<string, object> ExtraPrefsFirefox { get; set; }

        /// <summary>
        /// Callback to decide if Puppeteer should connect to a given target or not.
        /// </summary>
        internal Func<Target, bool> IsPageTarget { get; set; }
    }
}
