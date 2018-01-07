using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Browser
    {
        public Browser(Connection connection, Dictionary<string, object> options, Func<Task> closeCallBack)
        {
            Connection = connection;
            IgnoreHTTPSErrors = options.ContainsKey("ignoreHTTPSErrors") && (bool)options["ignoreHTTPSErrors"];
            AppMode = options.ContainsKey("appMode") && (bool)options["appMode"];
            _targets = new Dictionary<string, Target>();

            Connection.Closed += (object sender, EventArgs e) => Disconnected?.Invoke(this, new EventArgs());
            Connection.MessageReceived += Connect_MessageReceived;

            _closeCallBack = closeCallBack;
        }

        #region Private members
        private Dictionary<string, Target> _targets;
        #endregion

        #region Properties
        public Connection Connection { get; set; }
        public event EventHandler Closed;
        public event EventHandler Disconnected;
        public event EventHandler<TargetChangedArgs> TargetChanged;
        public event EventHandler<TargetChangedArgs> TargetCreated;
        public event EventHandler<TargetChangedArgs> TargetDestroyed;
        private event Func<Task> _closeCallBack;

        public string WebSocketEndpoint
        {
            get
            {
                return Connection.Url;
            }
        }

        public bool IgnoreHTTPSErrors { get; set; }
        public bool AppMode { get; set; }
        internal TaskQueue ScreenshotTaskQueue { get; set; }

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
            var targetId = (await Connection.SendAsync("Target.createTarget", new Dictionary<string, object>(){
                {"url", "about:blank"}
              })).ToString();

            var target = _targets[targetId];
            return await target.Page();
        }

        internal void ChangeTarget(TargetInfo targetInfo)
        {
            TargetChanged?.Invoke(this, new TargetChangedArgs()
            {
                TargetInfo = targetInfo
            });
        }

        public async Task<string> GetVersionAsync()
        {
            dynamic version = await Connection.SendAsync("Browser.getVersion");
            return version.product.ToString();
        }

        public void Close()
        {
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
            if (!_targets.ContainsKey(args.TargetInfo.TargetId))
            {
                throw new InvalidTargetException("Target should exists before ChangeTargetInfo");
            }

            var target = _targets[args.TargetInfo.TargetId];
            target.TargetInfoChanged(args.TargetInfo);
        }

        private void DestroyTarget(MessageEventArgs args)
        {
            if (!_targets.ContainsKey(args.TargetInfo.TargetId))
            {
                throw new InvalidTargetException("Target should exists before DestroyTarget");
            }

            var target = _targets[args.TargetInfo.TargetId];
            target.InitilizedTaskWrapper.SetResult(false);
            _targets.Remove(args.TargetInfo.TargetId);

            TargetDestroyed(this, new TargetChangedArgs()
            {
                Target = target
            });
        }

        private async Task CreateTarget(MessageEventArgs args)
        {
            var target = new Target(this, args.TargetInfo);
            _targets[args.TargetInfo.TargetId] = target;

            if (await target.InitializedTask)
            {
                TargetCreated(this, new TargetChangedArgs()
                {
                    Target = target
                });
            }

        }

        internal static async Task<Browser> CreateAsync(Connection connection, Dictionary<string, object> options,
                                                        Func<Task> closeCallBack)
        {
            var browser = new Browser(connection, options, closeCallBack);
            await connection.SendAsync("Target.setDiscoverTargets", new Dictionary<string, object>()
            {
                { "discover", false }
            });

            return browser;
        }

        #endregion
    }
}
