using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using PuppeteerSharp.Helpers;
using System.IO;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace PuppeteerSharp
{
    public class Page : IDisposable
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
        private const int DefaultNavigationTimeout = 30000;

        private static Dictionary<string, PaperFormat> _paperFormats = new Dictionary<string, PaperFormat> {
            {"letter", new PaperFormat {Width = 8.5m, Height = 11}},
            {"legal", new PaperFormat {Width = 8.5m, Height = 14}},
            {"tabloid", new PaperFormat {Width = 11, Height = 17}},
            {"ledger", new PaperFormat {Width = 17, Height = 11}},
            {"a0", new PaperFormat {Width = 33.1m, Height = 46.8m }},
            {"a1", new PaperFormat {Width = 23.4m, Height = 33.1m }},
            {"a2", new PaperFormat {Width = 16.5m, Height = 23.4m }},
            {"a3", new PaperFormat {Width = 11.7m, Height = 16.5m }},
            {"a4", new PaperFormat {Width = 8.27m, Height = 11.7m }},
            {"a5", new PaperFormat {Width = 5.83m, Height = 8.27m }},
            {"a6", new PaperFormat {Width = 4.13m, Height = 5.83m }},
        };

        private static Dictionary<string, decimal> _unitToPixels = new Dictionary<string, decimal> {
            {"px", 1},
            {"in", 96},
            {"cm", 37.8m},
            {"mm", 3.78m}
        };
        private Target _target;

        private Page(Session client, Target target, FrameTree frameTree, bool ignoreHTTPSErrors, TaskQueue screenshotTaskQueue)
        {
            _client = client;
            _target = target;
            Keyboard = new Keyboard(client);
            _mouse = new Mouse(client, Keyboard);
            Touchscreen = new Touchscreen(client, Keyboard);
            _frameManager = new FrameManager(client, frameTree, this);
            _networkManager = new NetworkManager(client, _frameManager);
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

            _client.MessageReceived += client_MessageReceived;
        }

        #region Public Properties
        public event EventHandler<EventArgs> Load;
        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<FrameEventArgs> FrameAttached;
        public event EventHandler<FrameEventArgs> FrameDetached;
        public event EventHandler<FrameEventArgs> FrameNavigated;

        public event EventHandler<ResponseCreatedEventArgs> ResponseCreated;
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
            => await MainFrame.GetElementAsync(selector);

        public async Task<IEnumerable<ElementHandle>> GetElementsAsync(string selector)
            => await MainFrame.GetElementsAsync(selector);

        public async Task<JSHandle> EvaluateExpressionHandle(string script)
        {
            var context = await MainFrame.GetExecutionContextAsync();
            return await context.EvaluateExpressionHandleAsync(script);
        }

        public async Task<JSHandle> EvaluateFunctionHandle(string pageFunction, params object[] args)
        {
            var context = await MainFrame.GetExecutionContextAsync();
            return await context.EvaluateFunctionHandleAsync(pageFunction, args);
        }

        public async Task<JSHandle> QueryObjects(JSHandle prototypeHandle)
        {
            var context = await MainFrame.GetExecutionContextAsync();
            return await context.QueryObjects(prototypeHandle);
        }

        public async Task<object> EvalAsync(string selector, Func<object> pageFunction, params object[] args)
            => await MainFrame.Eval(selector, pageFunction, args);

        public async Task<object> EvalAsync(string selector, string pageFunction, params object[] args)
            => await MainFrame.Eval(selector, pageFunction, args);

        public async Task SetRequestInterceptionAsync(bool value)
            => await _networkManager.SetRequestInterceptionAsync(value);

        public async Task SetOfflineModeAsync(bool value) => await _networkManager.SetOfflineModeAsync(value);

        public async Task<object> EvalManyAsync(string selector, Func<object> pageFunction, params object[] args)
            => await MainFrame.EvalMany(selector, pageFunction, args);

        public async Task<object> EvalManyAsync(string selector, string pageFunction, params object[] args)
            => await MainFrame.EvalMany(selector, pageFunction, args);

        public async Task<IEnumerable<CookieParam>> GetCookiesAsync(params string[] urls)
        {
            var response = await _client.SendAsync("Network.getCookies", new Dictionary<string, object>
            {
                { "urls", urls.Length > 0 ? urls : new string[] { Url } }
            });
            return response.cookies.ToObject<CookieParam[]>();
        }

        public async Task SetCookieAsync(params CookieParam[] cookies)
        {
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && Url.StartsWith("http", StringComparison.Ordinal))
                {
                    cookie.Url = Url;
                }
                if (cookie.Url == "about:blank")
                {
                    throw new PuppeteerException($"Blank page can not have cookie \"{cookie.Name}\"");
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

        public async Task<ElementHandle> AddScriptTagAsync(dynamic options) => await MainFrame.AddScriptTag(options);

        public async Task<ElementHandle> AddStyleTagAsync(dynamic options) => await MainFrame.AddStyleTag(options);

        public static async Task<Page> CreateAsync(Session client, Target target, bool ignoreHTTPSErrors, bool appMode,
                                                   TaskQueue screenshotTaskQueue)
        {
            await client.SendAsync("Page.enable", null);
            dynamic result = await client.SendAsync("Page.getFrameTree");
            var page = new Page(client, target, new FrameTree(result.frameTree), ignoreHTTPSErrors, screenshotTaskQueue);

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

        public async Task<string> GetContentAsync() => await _frameManager.MainFrame.GetContentAsync();

        public async Task SetContentAsync(string html) => await _frameManager.MainFrame.SetContentAsync(html);

        public async Task<Response> GoToAsync(string url, NavigationOptions options = null)
        {
            var referrer = _networkManager.ExtraHTTPHeaders?.GetValueOrDefault("referer");
            var requests = new Dictionary<string, Request>();

            EventHandler<RequestEventArgs> createRequestEventListener = (object sender, RequestEventArgs e) =>
            {
                if (!requests.ContainsKey(e.Request.Url))
                {
                    requests.Add(e.Request.Url, e.Request);
                }
            };

            _networkManager.RequestCreated += createRequestEventListener;

            var mainFrame = _frameManager.MainFrame;
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;

            var watcher = new NavigatorWatcher(_frameManager, mainFrame, timeout, options);
            var navigateTask = Navigate(_client, url, referrer);

            await Task.WhenAny(
                navigateTask,
                watcher.NavigationTask
            );

            var exception = navigateTask.Exception;
            if (exception == null)
            {
                await watcher.NavigationTask;
                exception = watcher.NavigationTask.Exception;
            }

            watcher.Cancel();
            _networkManager.RequestCreated -= createRequestEventListener;

            if (exception != null)
            {
                throw new NavigationException(exception.Message, exception);
            }

            Request request = null;

            if (requests.ContainsKey(_frameManager.MainFrame.Url))
            {
                request = requests[_frameManager.MainFrame.Url];
            }

            return request?.Response;
        }

        public async Task PdfAsync(string file) => await PdfAsync(file, new PdfOptions());

        public async Task PdfAsync(string file, PdfOptions options)
        {
            var stream = await PdfStreamAsync(options);

            using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                fs.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }

        public async Task<Stream> PdfStreamAsync() => await PdfStreamAsync(new PdfOptions());

        public async Task<Stream> PdfStreamAsync(PdfOptions options)
        {
            var paperWidth = 8.5m;
            var paperHeight = 11m;

            if (!string.IsNullOrEmpty(options.Format))
            {
                if (!_paperFormats.ContainsKey(options.Format.ToLower()))
                {
                    throw new ArgumentException("Unknown paper format");
                }

                var format = _paperFormats[options.Format.ToLower()];
                paperWidth = format.Width;
                paperHeight = format.Height;
            }
            else
            {
                if (options.Width != null)
                {
                    paperWidth = ConvertPrintParameterToInches(options.Width);
                }
                if (options.Height != null)
                {
                    paperHeight = ConvertPrintParameterToInches(options.Height);
                }
            }

            var marginTop = ConvertPrintParameterToInches(options.MarginOptions.Top);
            var marginLeft = ConvertPrintParameterToInches(options.MarginOptions.Left);
            var marginBottom = ConvertPrintParameterToInches(options.MarginOptions.Bottom);
            var marginRight = ConvertPrintParameterToInches(options.MarginOptions.Right);

            JObject result = await _client.SendAsync("Page.printToPDF", new
            {
                landscape = options.Landscape,
                displayHeaderFooter = options.DisplayHeaderFooter,
                headerTemplate = options.HeaderTemplate,
                footerTemplate = options.FooterTemplate,
                printBackground = options.PrintBackground,
                scale = options.Scale,
                paperWidth,
                paperHeight,
                marginTop,
                marginBottom,
                marginLeft,
                marginRight,
                pageRanges = options.PageRanges
            });

            var buffer = Convert.FromBase64String(result.GetValue("data").Value<string>());
            return new MemoryStream(buffer);
        }

        public async Task SetJavaScriptEnabledAsync(bool enabled)
            => await _client.SendAsync("Emulation.setScriptExecutionDisabled", new { value = !enabled });

        public async Task SetViewport(ViewPortOptions viewport)
        {
            var needsReload = await _emulationManager.EmulateViewport(_client, viewport);
            _viewport = viewport;

            if (needsReload)
            {
                await ReloadAsync();
            }
        }

        public async Task ScreenshotAsync(string file) => await ScreenshotAsync(file, new ScreenshotOptions());

        public async Task ScreenshotAsync(string file, ScreenshotOptions options)
        {
            var fileInfo = new FileInfo(file);
            options.Type = fileInfo.Extension.Replace(".", string.Empty);

            var stream = await ScreenshotStreamAsync(options);

            using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                fs.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }

        public async Task<Stream> ScreenshotStreamAsync() => await ScreenshotStreamAsync(new ScreenshotOptions());

        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
        {
            string screenshotType = null;

            if (!string.IsNullOrEmpty(options.Type))
            {
                if (options.Type != "png" && options.Type != "jpeg")
                {
                    throw new ArgumentException($"Unknown options.type {options.Type}");
                }
                screenshotType = options.Type;
            }

            if (string.IsNullOrEmpty(screenshotType))
            {
                screenshotType = "png";
            }

            if (options.Quality.HasValue)
            {
                if (screenshotType == "jpeg")
                {
                    throw new ArgumentException($"options.Quality is unsupported for the {screenshotType} screenshots");
                }

                if (options.Quality < 0 || options.Quality > 100)
                {
                    throw new ArgumentException($"Expected options.quality to be between 0 and 100 (inclusive), got {options.Quality}");
                }
            }

            if (options.Clip != null && options.FullPage)
            {
                throw new ArgumentException("options.clip and options.fullPage are exclusive");
            }

            return await _screenshotTaskQueue.Enqueue(() => PerformScreenshot(screenshotType, options));
        }

        public Task<string> GetTitleAsync() => MainFrame.GetTitleAsync();

        public async Task CloseAsync()
        {
            if (!(_client?.Connection?.IsClosed ?? true))
            {
                await _client.Connection.SendAsync("Target.closeTarget", new
                {
                    targetId = _target.TargetId
                });
            }
        }

        public Task<dynamic> EvaluateExpressionAsync(string script)
            => _frameManager.MainFrame.EvaluateExpressionAsync(script);

        public Task<T> EvaluateExpressionAsync<T>(string script)
            => _frameManager.MainFrame.EvaluateExpressionAsync<T>(script);

        public Task<dynamic> EvaluateFunctionAsync(string script, params object[] args)
            => _frameManager.MainFrame.EvaluateFunctionAsync(script, args);

        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => _frameManager.MainFrame.EvaluateFunctionAsync<T>(script, args);

        public async Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
            => await _networkManager.SetExtraHTTPHeadersAsync(headers);

        public Task AuthenticateAsync(Credentials credentials) => _networkManager.AuthenticateAsync(credentials);

        public async Task<Response> ReloadAsync(NavigationOptions options = null)
        {
            var navigationTask = WaitForNavigation(options);

            await Task.WhenAll(
              navigationTask,
              _client.SendAsync("Page.reload")
            );

            return navigationTask.Result;
        }

        #endregion

        #region Private Method

        private async Task<Response> WaitForNavigation(NavigationOptions options = null)
        {
            var mainFrame = _frameManager.MainFrame;
            var timeout = options?.Timeout ?? DefaultNavigationTimeout;
            var watcher = new NavigatorWatcher(_frameManager, mainFrame, timeout, options);
            var responses = new Dictionary<string, Response>();

            EventHandler<ResponseCreatedEventArgs> createResponseEventListener = (object sender, ResponseCreatedEventArgs e) =>
                responses.Add(e.Response.Url, e.Response);

            _networkManager.ResponseCreated += createResponseEventListener;

            await watcher.NavigationTask;

            _networkManager.ResponseCreated -= createResponseEventListener;

            var exception = watcher.NavigationTask.Exception;
            if (exception != null)
            {
                throw new NavigationException(exception.Message, exception);
            }

            return responses.GetValueOrDefault(_frameManager.MainFrame.Url);
        }

        private async Task<Stream> PerformScreenshot(string format, ScreenshotOptions options)
        {
            await _client.SendAsync("Target.activateTarget", new
            {
                targetId = _target.TargetId
            });

            var clip = options.Clip != null ? options.Clip.Clone() : null;
            if (clip != null)
            {
                clip.Scale = 1;
            }

            if (options != null && options.FullPage)
            {
                dynamic metrics = await _client.SendAsync("Page.getLayoutMetrics");
                var width = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(metrics.contentSize.width.Value)));
                var height = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(metrics.contentSize.height.Value)));

                // Overwrite clip for full page at all times.
                clip = new Clip
                {
                    X = 0,
                    Y = 0,
                    Width = width,
                    Height = height,
                    Scale = 1
                };

                var mobile = _viewport.IsMobile;
                var deviceScaleFactor = _viewport.DeviceScaleFactor;
                var landscape = _viewport.IsLandscape;
                var screenOrientation = landscape ?
                    new ScreenOrientation
                    {
                        Angle = 90,
                        Type = ScreenOrientationType.LandscapePrimary
                    } :
                    new ScreenOrientation
                    {
                        Angle = 0,
                        Type = ScreenOrientationType.PortraitPrimary
                    };

                await _client.SendAsync("Emulation.setDeviceMetricsOverride", new
                {
                    mobile,
                    width,
                    height,
                    deviceScaleFactor,
                    screenOrientation
                });
            }

            if (options != null && options.OmitBackground)
            {
                await _client.SendAsync("Emulation.setDefaultBackgroundColorOverride", new
                {
                    color = new
                    {
                        r = 0,
                        g = 0,
                        b = 0,
                        a = 0
                    }
                });
            }

            dynamic screenMessage = new ExpandoObject();

            screenMessage.format = format;

            if (options.Quality.HasValue)
            {
                screenMessage.quality = options.Quality.Value;
            }

            if (clip != null)
            {
                screenMessage.clip = clip;
            }

            JObject result = await _client.SendAsync("Page.captureScreenshot", screenMessage);

            if (options != null && options.OmitBackground)
            {
                await _client.SendAsync("Emulation.setDefaultBackgroundColorOverride");
            }

            if (options != null && options.FullPage)
            {
                await SetViewport(_viewport);
            }

            var buffer = Convert.FromBase64String(result.GetValue("data").Value<string>());

            return new MemoryStream(buffer);
        }

        private decimal ConvertPrintParameterToInches(object parameter)
        {
            if (parameter == null)
            {
                return 0;
            }

            var pixels = 0m;

            if (parameter is decimal || parameter is int)
            {
                pixels = Convert.ToDecimal(parameter);
            }
            else
            {
                var text = parameter.ToString();
                var unit = text.Substring(text.Length - 2).ToLower();
                var valueText = "";

                if (_unitToPixels.ContainsKey(unit))
                {
                    valueText = text.Substring(0, text.Length - 2);
                }
                else
                {
                    // In case of unknown unit try to parse the whole parameter as number of pixels.
                    // This is consistent with phantom's paperSize behavior.
                    unit = "px";
                    valueText = text;
                }

                if (Decimal.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out var number))
                {
                    pixels = number * _unitToPixels[unit];
                }
                else
                {
                    throw new ArgumentException($"Failed to parse parameter value: '{text}'", nameof(parameter));
                }
            }

            return pixels / 96;
        }

        private async void client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Page.loadEventFired":
                    Load?.Invoke(this, new EventArgs());
                    break;
                case "Runtime.consoleAPICalled":
                    OnConsoleAPI(e);
                    break;
                case "Page.javascriptDialogOpening":
                    OnDialog(e);
                    break;
                case "Runtime.exceptionThrown":
                    HandleException(e.MessageData.exception.exceptionDetails);
                    break;
                case "Security.certificateError":
                    await OnCertificateError(e);
                    break;
                case "Inspector.targetCrashed":
                    OnTargetCrashed();
                    break;
                case "Performance.metrics":
                    EmitMetrics(e);
                    break;
            }
        }

        private void OnTargetCrashed()
        {
            if (Error == null)
            {
                throw new TargetCrashedException();
            }

            Error.Invoke(this, new ErrorEventArgs("Page crashed!"));
        }

        private void EmitMetrics(MessageEventArgs e)
        {

        }

        private async Task OnCertificateError(MessageEventArgs e)
        {
            if (_ignoreHTTPSErrors)
            {
                //TODO: Puppeteer is silencing an error here, I don't know if that's necessary here
                await _client.SendAsync("Security.handleCertificateError", new Dictionary<string, object>
                {
                    {"eventId", e.MessageData.eventId },
                    {"action", "continue"}
                });

            }
        }

        private void HandleException(string exceptionDetails)
        {
        }

        private void OnDialog(MessageEventArgs e)
        {
        }

        private void OnConsoleAPI(MessageEventArgs e)
        {
        }

        private async Task Navigate(Session client, string url, string referrer)
        {

            dynamic response = await client.SendAsync("Page.navigate", new
            {
                url,
                referrer = referrer ?? string.Empty
            });

            if (response.errorText != null)
            {
                throw new NavigationException(response.errorText.ToString());
            }
        }
        #endregion

        #region IDisposable
        public void Dispose() => CloseAsync().GetAwaiter().GetResult();
        #endregion
    }
}
