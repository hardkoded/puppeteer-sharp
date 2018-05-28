using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    public class Browser : IDisposable
    {
        public Browser(Connection connection, IBrowserOptions options, Process process, Func<Task> closeCallBack)
        {
            Process = process;
            Connection = connection;
            IgnoreHTTPSErrors = options.IgnoreHTTPSErrors;
            AppMode = options.AppMode;
            _targets = new Dictionary<string, Target>();
            ScreenshotTaskQueue = new TaskQueue();

            Connection.Closed += (object sender, EventArgs e) => Disconnected?.Invoke(this, new EventArgs());
            Connection.MessageReceived += Connect_MessageReceived;

            _closeCallBack = closeCallBack;
        }

        #region Private members
        private Dictionary<string, Target> _targets;

        #endregion

        #region Properties

        public event EventHandler Closed;
        public event EventHandler Disconnected;
        public event EventHandler<TargetChangedArgs> TargetChanged;
        public event EventHandler<TargetChangedArgs> TargetCreated;
        public event EventHandler<TargetChangedArgs> TargetDestroyed;
        private event Func<Task> _closeCallBack;

        /// <summary>
        /// Gets the Browser websocket url
        /// </summary>
        /// <remarks>
        /// Browser websocket endpoint which can be used as an argument to <see cref="Puppeteer.ConnectAsync(ConnectOptions)"/>.
        /// The format is <c>ws://${host}:${port}/devtools/browser/[id]</c>
        /// You can find the <c>webSocketDebuggerUrl</c> from <c>http://${host}:${port}/json/version</c>.
        /// Learn more about the devtools protocol <see href="https://chromedevtools.github.io/devtools-protocol"/> 
        /// and the browser endpoint <see href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target"/>
        /// </remarks>
        public string WebSocketEndpoint => Connection.Url;

        /// <summary>
        /// Gets the spawned browser process. Returns <c>null</c> if the browser instance was created with <see cref="Puppeteer.ConnectAsync(ConnectOptions)"/> method.
        /// </summary>
        public Process Process { get; }
        public bool IgnoreHTTPSErrors { get; set; }
        public bool AppMode { get; set; }
        public bool IsClosed { get; internal set; }

        internal TaskQueue ScreenshotTaskQueue { get; set; }
        internal Connection Connection { get; }

        #endregion

        #region Public Methods

        public async Task Initialize()
        {
            dynamic args = new ExpandoObject();
            args.discover = true;
            await Connection.SendAsync("Target.setDiscoverTargets", args);
        }

        public async Task<Page> NewPageAsync()
        {
            string targetId = (await Connection.SendAsync("Target.createTarget", new Dictionary<string, object>(){
                {"url", "about:blank"}
              })).targetId.ToString();

            var target = _targets[targetId];
            await target.InitializedTask;
            return await target.PageAsync();
        }

        /// <summary>
        /// Returns An Array of all active targets
        /// </summary>
        /// <returns>An Array of all active targets</returns>
        public Target[] Targets() => _targets.Values.Where(target => target.IsInitialized).ToArray();

        /// <summary>
        /// Returns a Task which resolves to an array of all open pages.
        /// </summary>
        /// <returns>Task which resolves to an array of all open pages.</returns>
        public async Task<Page[]> PagesAsync()
            => (await Task.WhenAll(Targets().Select(target => target.PageAsync()))).Where(x => x != null).ToArray();

        internal void ChangeTarget(Target target)
        {
            TargetChanged?.Invoke(this, new TargetChangedArgs
            {
                Target = target
            });
        }

        /// <summary>
        /// Gets the browser's version
        /// </summary>
        /// <returns>For headless Chromium, this is similar to <c>HeadlessChrome/61.0.3153.0</c>. For non-headless, this is similar to <c>Chrome/61.0.3153.0</c></returns>
        /// <remarks>
        /// the format of <see cref="GetVersionAsync"/> might change with future releases of Chromium
        /// </remarks>
        public async Task<string> GetVersionAsync()
        {
            dynamic version = await Connection.SendAsync("Browser.getVersion");
            return version.product.ToString();
        }

        /// <summary>
        /// Gets the browser's original user agent
        /// </summary>
        /// <returns>Task which resolves to the browser's original user agent</returns>
        /// <remarks>
        /// Pages can override browser user agent with <see cref="Page.SetUserAgentAsync(string)"/>
        /// </remarks>
        public async Task<string> GetUserAgentAsync()
        {
            dynamic version = await Connection.SendAsync("Browser.getVersion");
            return version.userAgent.ToString();
        }

        /// <summary>
        /// Disconnects Puppeteer from the browser, but leaves the Chromium process running. After calling <see cref="Disconnect"/>, the browser object is considered disposed and cannot be used anymore
        /// </summary>
        public void Disconnect() => Connection.Dispose();

        /// <summary>
        /// Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore
        /// </summary>
        /// <returns>Task</returns>
        public async Task CloseAsync()
        {
            if (IsClosed)
            {
                return;
            }

            IsClosed = true;
            Connection.StopReading();

            var closeTask = _closeCallBack();

            if (closeTask != null)
            {
                await closeTask;
            }

            Disconnect();
            Closed?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Private Methods

        public async void Connect_MessageReceived(object sender, MessageEventArgs args)
        {
            switch (args.MessageID)
            {
                case "Target.targetCreated":
                    await CreateTarget(args);
                    return;

                case "Target.targetDestroyed":
                    DestroyTarget(args);
                    return;

                case "Target.targetInfoChanged":
                    ChangeTargetInfo(args);
                    return;
            }
        }

        private void ChangeTargetInfo(MessageEventArgs args)
        {
            if (!_targets.ContainsKey(args.MessageData.targetInfo.targetId.Value))
            {
                throw new InvalidTargetException("Target should exists before ChangeTargetInfo");
            }

            string targetId = args.MessageData.targetInfo.targetId.Value;
            var target = _targets[targetId];
            target.TargetInfoChanged(new TargetInfo(args.MessageData.targetInfo));
        }

        private void DestroyTarget(MessageEventArgs args)
        {
            if (!_targets.ContainsKey(args.MessageData.targetId.ToString()))
            {
                throw new InvalidTargetException("Target should exists before DestroyTarget");
            }

            var target = _targets[args.MessageData.targetId.ToString()];
            if (!target.InitilizedTaskWrapper.Task.IsCompleted)
            {
                target.InitilizedTaskWrapper.SetResult(false);
            }
            _targets.Remove(args.MessageData.targetId.ToString());

            TargetDestroyed?.Invoke(this, new TargetChangedArgs()
            {
                Target = target
            });
        }

        private async Task CreateTarget(MessageEventArgs args)
        {
            var targetInfo = new TargetInfo(args.MessageData.targetInfo);
            var target = new Target(this, targetInfo);
            _targets[targetInfo.TargetId] = target;

            if (await target.InitializedTask)
            {
                TargetCreated?.Invoke(this, new TargetChangedArgs()
                {
                    Target = target
                });
            }
        }

        internal static async Task<Browser> CreateAsync(
            Connection connection, IBrowserOptions options, Process process, Func<Task> closeCallBack)
        {
            var browser = new Browser(connection, options, process, closeCallBack);
            await connection.SendAsync("Target.setDiscoverTargets", new
            {
                discover = true
            });

            return browser;
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            CloseAsync().GetAwaiter().GetResult();
        }
        #endregion
    }
}
