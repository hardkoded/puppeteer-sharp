using System;
using System.Collections.Generic;
using System.Net;
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
        private Mouse _mouse;
        private Dictionary<string, Func<object>> _pageBindings;


        private Page(Session client, FrameTree frameTree, bool ignoreHTTPSErrors, TaskQueue screenshotTaskQueue)
        {
            _client = client;

            Keyboard = new Keyboard(client);
            _mouse = new Mouse(client, Keyboard);
            Touchscreen = new Touchscreen(client, Keyboard);
            _frameManager = new FrameManager(client, frameTree, this);
            _networkManager = new NetworkManager(client);
            _emulationManager = new EmulationManager(client);
            Tracing = new Tracing(client);
            _pageBindings = new Dictionary<string, Func<object>>();

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

        public event EventHandler<FrameEventArgs> FrameAttached;
        public event EventHandler<EventArgs> FrameDetached;
        public event EventHandler<FrameEventArgs> FrameNavigated;

        public event EventHandler<ResponseCreatedArgs> ResponseCreated;
        public event EventHandler<RequestEventArgs> RequestCreated;
        public event EventHandler<RequestEventArgs> RequestFinished;
        public event EventHandler<RequestEventArgs> RequestFailed;

        public Frame MainFrame => _frameManager.MainFrame;
        public IEnumerable<Frame> Frames => _frameManager.Frames.Values;
        public string Url => MainFrame.Url;

        public Keyboard Keyboard { get; internal set; }
        public Touchscreen Touchscreen { get; internal set; }
        public Tracing Tracing { get; internal set; }


        #endregion

        #region Public Methods

        public async Task TapAsync(string selector)
        {
            var handle = await GetElementAsync(selector);

            if (handle != null)
            {
                await handle.TapAsync();
                await handle.DisposeAsync();
            }
        }

        public async Task<ElementHandle> GetElementAsync(string selector)
        {
            return await MainFrame.GetElementAsync(selector);
        }

        public async Task<IEnumerable<ElementHandle>> GetElementsAsync(string selector)
        {
            return await MainFrame.GetElementsAsync(selector);
        }

        public async Task<JSHandle> EvaluateHandle(Func<object> pageFunction, params object[] args)
        {
            var context = await MainFrame.GetExecutionContext();
            return await context.EvaluateHandle(pageFunction, args);
        }

        public async Task<JSHandle> EvaluateHandle(string pageFunction, params object[] args)
        {
            var context = await MainFrame.GetExecutionContext();
            return await context.EvaluateHandle(pageFunction, args);
        }

        public async Task<JSHandle> QueryObjects(JSHandle prototypeHandle)
        {
            var context = await MainFrame.GetExecutionContext();
            return await context.QueryObjects(prototypeHandle);
        }


        public async Task<object> EvalAsync(string selector, Func<object> pageFunction, params object[] args)
        {
            return await MainFrame.Eval(selector, pageFunction, args);
        }

        public async Task<object> EvalAsync(string selector, string pageFunction, params object[] args)
        {
            return await MainFrame.Eval(selector, pageFunction, args);
        }

        public async Task SetRequestInterceptionAsync(bool value)
        {
            await _networkManager.SetRequestInterceptionAsync(value);
        }

        public async Task SetOfflineModeAsync(bool value)
        {
            await _networkManager.SetOfflineModeAsync(value);
        }

        public async Task<object> EvalManyAsync(string selector, Func<object> pageFunction, params object[] args)
        {
            return await MainFrame.EvalMany(selector, pageFunction, args);
        }

        public async Task<object> EvalManyAsync(string selector, string pageFunction, params object[] args)
        {
            return await MainFrame.EvalMany(selector, pageFunction, args);
        }

        public async Task<IEnumerable<CookieParam>> GetCookiesAsync(params string[] urls)
        {
            return await _client.SendAsync<List<CookieParam>>("Network.getCookies", new Dictionary<string, object>
            {
                { "urls", urls.Length > 0 ? urls : (object)Url}
            });
        }

        public async Task SetCookieAsync(params CookieParam[] cookies)
        {
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && Url.StartsWith("http"))
                {
                    cookie.Url = Url;
                }
            }

            await DeleteCookieAsync(cookies);

            if (cookies.Length > 0)
            {
                await _client.SendAsync("Network.setCookies", new Dictionary<string, object>
                {
                    { "cookies", cookies}
                });
            }
        }

        public async Task DeleteCookieAsync(params CookieParam[] cookies)
        {
            var pageURL = Url;
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && pageURL.StartsWith("http", StringComparison.Ordinal))
                {
                    cookie.Url = pageURL;
                }
                await _client.SendAsync("Network.deleteCookies", cookie);
            }
        }

        public async Task<ElementHandle> AddScriptTagAsync(dynamic options)
        {
            return await MainFrame.AddScriptTag(options);
        }

        public async Task<ElementHandle> AddStyleTagAsync(dynamic options)
        {
            return await MainFrame.AddStyleTag(options);
        }

        public async Task ExposeFunction(string name, Func<object> puppeteerFunction)
        {
            //TODO: We won't implement this yet
        }

        public static async Task<Page> CreateAsync(Session client, bool ignoreHTTPSErrors, bool appMode,
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

        public async Task<dynamic> GoToAsync(string url, Dictionary<string, string> options)
        {
            var referrer = _networkManager.ExtraHTTPHeaders["referer"];
            var requests = new Dictionary<string, Request>();

            EventHandler<RequestEventArgs> createRequestEventListener = (object sender, RequestEventArgs e) =>
                requests.Add(e.Request.Url, e.Request);

            _networkManager.RequestCreated += createRequestEventListener;

            var mainFrame = _frameManager.MainFrame;
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

            if (requests.ContainsKey(_frameManager.MainFrame.Url))
            {
                request = requests[_frameManager.MainFrame.Url];
            }

            return request?.Response;
        }

        #endregion

        #region Private Method

        private async void _client_MessageReceived(object sender, MessageEventArgs e)
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
                    await OnCertificateError(e);
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
            if (Error == null)
            {
                throw new TargetCrashedException();
            }

            Error.Invoke(this, new ErrorEventArgs());
        }

        private void EmitMetrics(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task OnCertificateError(MessageEventArgs e)
        {
            if (_ignoreHTTPSErrors)
            {
                //TODO: Puppeteer is silencing an error here, I don't know if that's necessary here
                await _client.SendAsync("Security.handleCertificateError", new Dictionary<string, object>
                {
                    {"eventId", e.eventId },
                    {"action", "continue"}
                });

            }
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

        #endregion

    }
}
