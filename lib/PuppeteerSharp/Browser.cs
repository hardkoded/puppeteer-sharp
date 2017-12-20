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

            var client = await Connection.CreateSession(targetId);
            var page = await Page.CreateAsync(client, IgnoreHTTPSErrors, AppMode);
            return page;
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

        public void Connect_MessageReceived(object sender, MessageEventArgs args)
        {
            switch (args.MessageID)
            {
                case "Target.targetCreated":
                    TargetCreated(args);
                    return;

                case "Target.targetDestroyed":
                    TargetDestroyed(args);
                    return;

                case "Target.targetInfoChanged":
                    TargetInfoChanged(args);
                    return;
            }
        }

        private void TargetInfoChanged(MessageEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void TargetDestroyed(MessageEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void TargetCreated(MessageEventArgs args)
        {
            var target = new Target(this, args.TargetInfo);
            _targets[args.TargetInfo.TargetId] = target;
            /*
            if (await target.InitializedTask)
            {
                
            }
            */
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
