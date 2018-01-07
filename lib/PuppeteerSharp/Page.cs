using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    public class Page
    {
        private Session _client;
        private bool _ignoreHTTPSErrors;
        private NetworkManager _networkManager;
        private FrameManager _frameManager;
        private TaskQueue _screenshotTaskQueue;
        private EmulationManager _emulationManager;
        private ViewPortOptions _viewport;
        private Keyboard _keyboard;
        private Mouse _mouse;
        private Touchscreen _touchscreen;
        private Tracing _tracing;
        private Dictionary<string, Action> _pageBindings;

        private Page(Session client, FrameTree frameTree, bool ignoreHTTPSErrors, TaskQueue screenshotTaskQueue)
        {
            _client = client;

            _keyboard = new Keyboard(client);
            _mouse = new Mouse(client, _keyboard);
            _touchscreen = new Touchscreen(client, _keyboard);
            _frameManager = new FrameManager(client, frameTree, this);
            _networkManager = new NetworkManager(client);
            _emulationManager = new EmulationManager(client);
            _tracing = new Tracing(client);
            _pageBindings = new Dictionary<string, Action>();

            _ignoreHTTPSErrors = ignoreHTTPSErrors;

            _screenshotTaskQueue = screenshotTaskQueue;

            //TODO: Do we need this bubble?
            _frameManager.FrameAttached += (sender, e) => FrameAttached?.Invoke(this, e);
            _frameManager.FrameDetached += (sender, e) => FrameDetached?.Invoke(this, e);
            _frameManager.FrameNavigated += (sender, e) => FrameNavigated?.Invoke(this, e);

            _networkManager.RequestCreated += (sender, e) => RequestCreated?.Invoke(this, e);
            _networkManager.RequestFailed += (sender, e) => RequestFailed?.Invoke(this, e);
            _networkManager.ResponseCreated += (sender, e) => ResponseCreated?.Invoke(this, e);
            _networkManager.RequestFinished += (sender, e) => RequestFinished?.Invoke(this, e);

            _client.MessageReceived += _client_MessageReceived;
        }

        #region Public Properties
        public event EventHandler<EventArgs> Load;
        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<EventArgs> FrameAttached;
        public event EventHandler<EventArgs> FrameDetached;
        public event EventHandler<EventArgs> FrameNavigated;

        public event EventHandler<ResponseCreatedArgs> ResponseCreated;
        public event EventHandler<RequestEventArgs> RequestCreated;
        public event EventHandler<RequestEventArgs> RequestFinished;
        public event EventHandler<RequestEventArgs> RequestFailed;
        #endregion

        internal static async Task<Page> CreateAsync(Session client, bool ignoreHTTPSErrors, bool appMode,
                                                     TaskQueue screenshotTaskQueue)
        {
            await client.SendAsync("Page.enable", null);
            dynamic frameTree = await client.SendAsync<FrameTree>("Page.getFrameTree", null);
            var page = new Page(client, frameTree, ignoreHTTPSErrors, screenshotTaskQueue);

            await Task.WhenAll(
                client.SendAsync("Page.setLifecycleEventsEnabled", new Dictionary<string, object>
                {
                    {"enabled", true }
                }),
                client.SendAsync("Network.enable", null),
                client.SendAsync("Runtime.enable", null),
                client.SendAsync("Security.enable", null),
                client.SendAsync("Performance.enable", null)
            );

            if (ignoreHTTPSErrors)
            {
                await client.SendAsync("Security.setOverrideCertificateErrors", new Dictionary<string, object>
                {
                    {"override", true}
                });
            }

            // Initialize default page size.
            if (!appMode)
            {
                await page.SetViewport(new ViewPortOptions
                {
                    Width = 800,
                    Height = 600
                });
            }
            return page;
        }

        void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Page.loadEventFired":
                    Load(this, new EventArgs());
                    break;
                case "Runtime.consoleAPICalled":
                    OnConsoleAPI(e);
                    break;
                case "Page.javascriptDialogOpening":
                    OnDialog(e);
                    break;
                case "Runtime.exceptionThrown":
                    HandleException(e.Exception.ExceptionDetails);
                    break;
                case "Security.certificateError":
                    OnCertificateError(e);
                    break;
                case "Inspector.targetCrashed":
                    OnTargetCrashed(e);
                    break;
                case "Performance.metrics":
                    EmitMetrics(e);
                    break;
            }
        }

        private void OnTargetCrashed(MessageEventArgs e)
        {

        }

        private void EmitMetrics(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCertificateError(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleException(string exceptionDetails)
        {
            throw new NotImplementedException();
        }

        private void OnDialog(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnConsoleAPI(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task SetViewport(ViewPortOptions viewport)
        {
            var needsReload = await _emulationManager.EmulateViewport(_client, viewport);
            _viewport = viewport;
            if (needsReload)
            {
                await Reload();
            }
        }

        private Task Reload()
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GoToAsync(string url, Dictionary<string, string> options)
        {
            var referrer = _networkManager.ExtraHTTPHeaders["referer"];
            var requests = new Dictionary<string, Request>();

            EventHandler<RequestEventArgs> createRequestEventListener = (object sender, RequestEventArgs e) =>
                requests.Add(e.Request.Url, e.Request);

            _networkManager.RequestCreated += createRequestEventListener;

            var mainFrame = _frameManager.MainFrame();
            var watcher = new NavigationWatcher(_frameManager, mainFrame, options);

            var navigateTask = Navigate(_client, url, referrer);

            await Task.WhenAll(
                navigateTask,
                watcher.NavigationTask
            );

            var error = !string.IsNullOrEmpty(navigateTask.Result) ? navigateTask.Result : watcher.NavigationTask.Result;

            watcher.Cancel();
            _networkManager.RequestCreated -= createRequestEventListener;

            if (!string.IsNullOrEmpty(error))
            {
                throw new NavigationException(error);
            }

            Request request = null;

            if (requests.ContainsKey(_frameManager.MainFrame().Url))
            {
                request = requests[_frameManager.MainFrame().Url];
            }

            return request?.Response;
        }

        private async Task<string> Navigate(Session client, string url, string referrer)
        {
            try
            {
                dynamic response = await client.SendAsync("Page.navigate", new Dictionary<string, object>
                {
                    { "url", url},
                    {"referrer", referrer}
                });

                return response.ErrorText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
