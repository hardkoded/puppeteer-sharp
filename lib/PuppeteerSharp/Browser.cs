using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Browser : IDisposable
    {
        public Browser(Connection connection, IBrowserOptions options, Func<Task> closeCallBack)
        {
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
        public bool IsClosed { get; internal set; }
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
            return await target.Page();
        }

        public async Task<IEnumerable<Page>> Pages()
            => (await Task.WhenAll(this._targets.Select(x => x.Value.Page()))).Where(x => x != null);

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

        public void Disconnect() => Connection.Dispose();

        public async Task CloseAsync()
        {
            if (IsClosed)
            {
                return;
            }

            IsClosed = true;

            await _closeCallBack();
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

            var target = _targets[args.MessageData.targetInfo.targetId.Value];
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

        internal static async Task<Browser> CreateAsync(Connection connection, IBrowserOptions options,
                                                        Func<Task> closeCallBack)
        {
            var browser = new Browser(connection, options, closeCallBack);
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
