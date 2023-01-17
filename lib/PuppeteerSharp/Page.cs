using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.PageCoverage;
using Timer = System.Timers.Timer;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    [DebuggerDisplay("Page {Url}")]
    public class Page : IPage
    {
        /// <summary>
        /// List of supported metrics.
        /// </summary>
        public static readonly IEnumerable<string> SupportedMetrics = new List<string>
        {
            "Timestamp",
            "Documents",
            "Frames",
            "JSEventListeners",
            "Nodes",
            "LayoutCount",
            "RecalcStyleCount",
            "LayoutDuration",
            "RecalcStyleDuration",
            "ScriptDuration",
            "TaskDuration",
            "JSHeapUsedSize",
            "JSHeapTotalSize",
        };

        private static readonly Dictionary<string, decimal> _unitToPixels = new()
        {
            { "px", 1 },
            { "in", 96 },
            { "cm", 37.8m },
            { "mm", 3.78m },
        };

        private readonly TaskQueue _screenshotTaskQueue;
        private readonly EmulationManager _emulationManager;
        private readonly ConcurrentDictionary<string, Delegate> _pageBindings;
        private readonly ConcurrentDictionary<string, Worker> _workers;
        private readonly ILogger _logger;
        private readonly TaskCompletionSource<bool> _closeCompletedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TimeoutSettings _timeoutSettings;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<FileChooser>> _fileChooserInterceptors;
        private PageGetLayoutMetricsResponse _burstModeMetrics;
        private bool _screenshotBurstModeOn;
        private ScreenshotOptions _screenshotBurstModeOptions;
        private TaskCompletionSource<bool> _sessionClosedTcs;

        private Page(
            CDPSession client,
            Target target,
            TaskQueue screenshotTaskQueue,
            bool ignoreHTTPSErrors)
        {
            Client = client;
            Target = target;
            Keyboard = new Keyboard(client);
            Mouse = new Mouse(client, (Keyboard)Keyboard);
            Touchscreen = new Touchscreen(client, (Keyboard)Keyboard);
            Tracing = new Tracing(client);
            Coverage = new Coverage(client);

            _fileChooserInterceptors = new ConcurrentDictionary<Guid, TaskCompletionSource<FileChooser>>();
            _timeoutSettings = new TimeoutSettings();
            _emulationManager = new EmulationManager(client);
            _pageBindings = new ConcurrentDictionary<string, Delegate>();
            _workers = new ConcurrentDictionary<string, Worker>();
            _logger = Client.Connection.LoggerFactory.CreateLogger<Page>();
            FrameManager = new FrameManager(client, this, ignoreHTTPSErrors, _timeoutSettings);
            Accessibility = new Accessibility(client);

            _screenshotTaskQueue = screenshotTaskQueue;

            target.TargetManager.AddTargetInterceptor(Client, OnAttachedToTarget);

            target.TargetManager.TargetGone += OnDetachedFromTarget;

            _ = target.CloseTask.ContinueWith(
                _ =>
                {
                    try
                    {
                        target.TargetManager.RemoveTargetInterceptor(Client, OnAttachedToTarget);
                        target.TargetManager.TargetGone -= OnDetachedFromTarget;
                        Close?.Invoke(this, EventArgs.Empty);
                    }
                    finally
                    {
                        IsClosed = true;
                        _closeCompletedTcs.TrySetResult(true);
                    }
                },
                TaskScheduler.Default);
        }

        /// <inheritdoc/>
        public event EventHandler Load;

        /// <inheritdoc/>
        public event EventHandler<ErrorEventArgs> Error;

        /// <inheritdoc/>
        public event EventHandler<MetricEventArgs> Metrics;

        /// <inheritdoc/>
        public event EventHandler<DialogEventArgs> Dialog;

        /// <inheritdoc/>
        public event EventHandler DOMContentLoaded;

        /// <inheritdoc/>
        public event EventHandler<ConsoleEventArgs> Console;

        /// <inheritdoc/>
        public event EventHandler<FrameEventArgs> FrameAttached;

        /// <inheritdoc/>
        public event EventHandler<FrameEventArgs> FrameDetached;

        /// <inheritdoc/>
        public event EventHandler<FrameEventArgs> FrameNavigated;

        /// <inheritdoc/>
        public event EventHandler<ResponseCreatedEventArgs> Response;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> Request;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> RequestFinished;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> RequestFailed;

        /// <inheritdoc/>
        public event EventHandler<RequestEventArgs> RequestServedFromCache;

        /// <inheritdoc/>
        public event EventHandler<PageErrorEventArgs> PageError;

        /// <inheritdoc/>
        public event EventHandler<WorkerEventArgs> WorkerCreated;

        /// <inheritdoc/>
        public event EventHandler<WorkerEventArgs> WorkerDestroyed;

        /// <inheritdoc/>
        public event EventHandler Close;

        /// <inheritdoc/>
        public event EventHandler<PopupEventArgs> Popup;

        /// <inheritdoc cref="CDPSession"/>
        public CDPSession Client { get; }

        /// <inheritdoc cref="ICDPSession"/>
        ICDPSession IPage.Client => Client;

        /// <inheritdoc/>
        public int DefaultNavigationTimeout
        {
            get => _timeoutSettings.NavigationTimeout;
            set => _timeoutSettings.NavigationTimeout = value;
        }

        /// <inheritdoc/>
        public int DefaultTimeout
        {
            get => _timeoutSettings.Timeout;
            set => _timeoutSettings.Timeout = value;
        }

        /// <inheritdoc/>
        public IFrame MainFrame => FrameManager.MainFrame;

        /// <inheritdoc/>
        public IFrame[] Frames => FrameManager.GetFrames();

        /// <inheritdoc/>
        public Worker[] Workers => _workers.Values.ToArray();

        /// <inheritdoc/>
        public string Url => MainFrame.Url;

        /// <inheritdoc/>
        public ITarget Target { get; }

        /// <inheritdoc/>
        public IKeyboard Keyboard { get; }

        /// <inheritdoc/>
        public ITouchscreen Touchscreen { get; }

        /// <inheritdoc/>
        public ICoverage Coverage { get; }

        /// <inheritdoc/>
        public ITracing Tracing { get; }

        /// <inheritdoc/>
        public IMouse Mouse { get; }

        /// <inheritdoc/>
        public ViewPortOptions Viewport { get; private set; }

        /// <inheritdoc/>
        public IBrowser Browser => Target.Browser;

        /// <inheritdoc/>
        public IBrowserContext BrowserContext => Target.BrowserContext;

        /// <summary>
        /// Get an indication that the page has been closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the accessibility.
        /// </summary>
        public IAccessibility Accessibility { get; }

        /// <inheritdoc/>
        public bool IsDragInterceptionEnabled { get; private set; }

        internal bool JavascriptEnabled { get; set; } = true;

        internal bool HasPopupEventListeners => Popup?.GetInvocationList().Any() == true;

        internal FrameManager FrameManager { get; private set; }

        private Task SessionClosedTask
        {
            get
            {
                if (_sessionClosedTcs == null)
                {
                    _sessionClosedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    Client.Disconnected += ClientDisconnected;

                    void ClientDisconnected(object sender, EventArgs e)
                    {
                        _sessionClosedTcs.TrySetException(new TargetClosedException("Target closed", "Session closed"));
                        Client.Disconnected -= ClientDisconnected;
                    }
                }

                return _sessionClosedTcs.Task;
            }
        }

        /// <inheritdoc/>
        public Task SetGeolocationAsync(GeolocationOption options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Longitude < -180 || options.Longitude > 180)
            {
                throw new ArgumentException($"Invalid longitude '{options.Longitude}': precondition - 180 <= LONGITUDE <= 180 failed.");
            }

            if (options.Latitude < -90 || options.Latitude > 90)
            {
                throw new ArgumentException($"Invalid latitude '{options.Latitude}': precondition - 90 <= LATITUDE <= 90 failed.");
            }

            if (options.Accuracy < 0)
            {
                throw new ArgumentException($"Invalid accuracy '{options.Accuracy}': precondition 0 <= ACCURACY failed.");
            }

            return Client.SendAsync("Emulation.setGeolocationOverride", options);
        }

        /// <inheritdoc/>
        public Task SetDragInterceptionAsync(bool enabled)
        {
            IsDragInterceptionEnabled = enabled;
            return Client.SendAsync("Input.setInterceptDrags", new InputSetInterceptDragsRequest { Enabled = enabled });
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, decimal>> MetricsAsync()
        {
            var response = await Client.SendAsync<PerformanceGetMetricsResponse>("Performance.getMetrics").ConfigureAwait(false);
            return BuildMetricsObject(response.Metrics);
        }

        /// <inheritdoc/>
        public async Task TapAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

            await handle.TapAsync().ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IElementHandle> QuerySelectorAsync(string selector)
            => MainFrame.QuerySelectorAsync(selector);

        /// <inheritdoc/>
        public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
            => MainFrame.QuerySelectorAllAsync(selector);

        /// <inheritdoc/>
        public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
            => MainFrame.QuerySelectorAllHandleAsync(selector);

        /// <inheritdoc/>
        public Task<IElementHandle[]> XPathAsync(string expression) => MainFrame.XPathAsync(expression);

        /// <inheritdoc/>
        public async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
        {
            var context = await MainFrame.GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            var context = await MainFrame.GetExecutionContextAsync().ConfigureAwait(false);
            return await context.EvaluateFunctionHandleAsync(pageFunction, args).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
        {
            var source = BindingUtils.EvaluationString(pageFunction, args);
            return Client.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = source,
            });
        }

        /// <inheritdoc/>
        public Task EvaluateExpressionOnNewDocumentAsync(string expression)
            => Client.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = expression,
            });

        /// <inheritdoc/>
        public async Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
        {
            var context = await MainFrame.GetExecutionContextAsync().ConfigureAwait(false);
            return await context.QueryObjectsAsync(prototypeHandle).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task SetRequestInterceptionAsync(bool value)
            => FrameManager.NetworkManager.SetRequestInterceptionAsync(value);

        /// <inheritdoc/>
        public Task SetOfflineModeAsync(bool value) => FrameManager.NetworkManager.SetOfflineModeAsync(value);

        /// <inheritdoc/>
        public Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions) => FrameManager.NetworkManager.EmulateNetworkConditionsAsync(networkConditions);

        /// <inheritdoc/>
        public async Task<CookieParam[]> GetCookiesAsync(params string[] urls)
            => (await Client.SendAsync<NetworkGetCookiesResponse>("Network.getCookies", new NetworkGetCookiesRequest
            {
                Urls = urls.Length > 0 ? urls : new string[] { Url },
            }).ConfigureAwait(false)).Cookies;

        /// <inheritdoc/>
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

            await DeleteCookieAsync(cookies).ConfigureAwait(false);

            if (cookies.Length > 0)
            {
                await Client.SendAsync("Network.setCookies", new NetworkSetCookiesRequest
                {
                    Cookies = cookies,
                }).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteCookieAsync(params CookieParam[] cookies)
        {
            var pageURL = Url;
            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Url) && pageURL.StartsWith("http", StringComparison.Ordinal))
                {
                    cookie.Url = pageURL;
                }

                await Client.SendAsync("Network.deleteCookies", cookie).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options) => MainFrame.AddScriptTagAsync(options);

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(string url) => AddScriptTagAsync(new AddTagOptions { Url = url });

        /// <inheritdoc/>
        public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options) => MainFrame.AddStyleTagAsync(options);

        /// <inheritdoc/>
        public Task<IElementHandle> AddStyleTagAsync(string url) => AddStyleTagAsync(new AddTagOptions { Url = url });

        /// <inheritdoc/>
        public Task ExposeFunctionAsync(string name, Action puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public Task<string> GetContentAsync() => FrameManager.MainFrame.GetContentAsync();

        /// <inheritdoc/>
        public Task SetContentAsync(string html, NavigationOptions options = null) => FrameManager.MainFrame.SetContentAsync(html, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, NavigationOptions options) => FrameManager.MainFrame.GoToAsync(url, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => GoToAsync(url, new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil)
            => GoToAsync(url, new NavigationOptions { WaitUntil = new[] { waitUntil } });

        /// <inheritdoc/>
        public Task PdfAsync(string file) => PdfAsync(file, new PdfOptions());

        /// <inheritdoc/>
        public async Task PdfAsync(string file, PdfOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            await PdfInternalAsync(file, options).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<Stream> PdfStreamAsync() => PdfStreamAsync(new PdfOptions());

        /// <inheritdoc/>
        public async Task<Stream> PdfStreamAsync(PdfOptions options)
            => new MemoryStream(await PdfDataAsync(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<byte[]> PdfDataAsync() => PdfDataAsync(new PdfOptions());

        /// <inheritdoc/>
        public Task<byte[]> PdfDataAsync(PdfOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return PdfInternalAsync(null, options);
        }

        /// <inheritdoc/>
        public Task SetJavaScriptEnabledAsync(bool enabled)
        {
            if (enabled == JavascriptEnabled)
            {
                return Task.CompletedTask;
            }

            JavascriptEnabled = enabled;
            return Client.SendAsync("Emulation.setScriptExecutionDisabled", new EmulationSetScriptExecutionDisabledRequest
            {
                Value = !enabled,
            });
        }

        /// <inheritdoc/>
        public Task SetBypassCSPAsync(bool enabled) => Client.SendAsync("Page.setBypassCSP", new PageSetBypassCSPRequest
        {
            Enabled = enabled,
        });

        /// <inheritdoc/>
        public Task EmulateMediaTypeAsync(MediaType type)
            => Client.SendAsync("Emulation.setEmulatedMedia", new EmulationSetEmulatedMediaTypeRequest { Media = type });

        /// <inheritdoc/>
        public Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
            => Client.SendAsync("Emulation.setEmulatedMedia", new EmulationSetEmulatedMediaFeatureRequest { Features = features });

        /// <inheritdoc/>
        public async Task SetViewportAsync(ViewPortOptions viewport)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException(nameof(viewport));
            }

            var needsReload = await _emulationManager.EmulateViewportAsync(viewport).ConfigureAwait(false);
            Viewport = viewport;

            if (needsReload)
            {
                await ReloadAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task EmulateAsync(DeviceDescriptor options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return Task.WhenAll(
                SetViewportAsync(options.ViewPort),
                SetUserAgentAsync(options.UserAgent));
        }

        /// <inheritdoc/>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task ScreenshotAsync(string file, ScreenshotOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.Type.HasValue)
            {
                options.Type = ScreenshotOptions.GetScreenshotTypeFromFile(file);

                if (options.Type == ScreenshotType.Jpeg && !options.Quality.HasValue)
                {
                    options.Quality = 90;
                }
            }

            var data = await ScreenshotDataAsync(options).ConfigureAwait(false);

            using (var fs = AsyncFileHelper.CreateStream(file, FileMode.Create))
            {
                await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ScreenshotOptions());

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async(ScreenshotOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var screenshotType = options.Type;

            if (!screenshotType.HasValue)
            {
                screenshotType = ScreenshotType.Png;
            }

            if (options.Quality.HasValue)
            {
                if (screenshotType != ScreenshotType.Jpeg)
                {
                    throw new ArgumentException($"options.Quality is unsupported for the {screenshotType} screenshots");
                }

                if (options.Quality < 0 || options.Quality > 100)
                {
                    throw new ArgumentException($"Expected options.quality to be between 0 and 100 (inclusive), got {options.Quality}");
                }
            }

            if (options?.Clip?.Width == 0)
            {
                throw new PuppeteerException("Expected options.Clip.Width not to be 0.");
            }

            if (options?.Clip?.Height == 0)
            {
                throw new PuppeteerException("Expected options.Clip.Height not to be 0.");
            }

            if (options.Clip != null && options.FullPage)
            {
                throw new ArgumentException("options.clip and options.fullPage are exclusive");
            }

            return _screenshotTaskQueue.Enqueue(() => PerformScreenshot(screenshotType.Value, options));
        }

        /// <inheritdoc/>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => MainFrame.GetTitleAsync();

        /// <inheritdoc/>
        public Task CloseAsync(PageCloseOptions options = null)
        {
            if (!(Client?.Connection?.IsClosed ?? true))
            {
                var runBeforeUnload = options?.RunBeforeUnload ?? false;

                if (runBeforeUnload)
                {
                    return Client.SendAsync("Page.close");
                }

                return Client.Connection.SendAsync("Target.closeTarget", new TargetCloseTargetRequest
                {
                    TargetId = Target.TargetId,
                }).ContinueWith(task => ((Target)Target).CloseTask, TaskScheduler.Default);
            }

            _logger.LogWarning("Protocol error: Connection closed. Most likely the page has been closed.");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SetCacheEnabledAsync(bool enabled = true)
            => FrameManager.NetworkManager.SetCacheEnabledAsync(enabled);

        /// <inheritdoc/>
        public Task ClickAsync(string selector, ClickOptions options = null) => FrameManager.MainFrame.ClickAsync(selector, options);

        /// <inheritdoc/>
        public Task HoverAsync(string selector) => FrameManager.MainFrame.HoverAsync(selector);

        /// <inheritdoc/>
        public Task FocusAsync(string selector) => FrameManager.MainFrame.FocusAsync(selector);

        /// <inheritdoc/>
        public Task TypeAsync(string selector, string text, TypeOptions options = null)
            => FrameManager.MainFrame.TypeAsync(selector, text, options);

        /// <inheritdoc/>
        public Task<JToken> EvaluateExpressionAsync(string script)
            => FrameManager.MainFrame.EvaluateExpressionAsync<JToken>(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script)
            => FrameManager.MainFrame.EvaluateExpressionAsync<T>(script);

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => FrameManager.MainFrame.EvaluateFunctionAsync<JToken>(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => FrameManager.MainFrame.EvaluateFunctionAsync<T>(script, args);

        /// <inheritdoc/>
        public Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null)
            => FrameManager.NetworkManager.SetUserAgentAsync(userAgent, userAgentData);

        /// <inheritdoc/>
        public Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            return FrameManager.NetworkManager.SetExtraHTTPHeadersAsync(headers);
        }

        /// <inheritdoc/>
        public Task AuthenticateAsync(Credentials credentials) => FrameManager.NetworkManager.AuthenticateAsync(credentials);

        /// <inheritdoc/>
        public async Task<IResponse> ReloadAsync(NavigationOptions options)
        {
            var navigationTask = WaitForNavigationAsync(options);

            await Task.WhenAll(
              navigationTask,
              Client.SendAsync("Page.reload", new PageReloadRequest { FrameId = MainFrame.Id })).ConfigureAwait(false);

            return navigationTask.Result;
        }

        /// <inheritdoc/>
        public Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => ReloadAsync(new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(string selector, params string[] values)
            => MainFrame.SelectAsync(selector, values);

        /// <inheritdoc/>
        public Task WaitForTimeoutAsync(int milliseconds)
            => MainFrame.WaitForTimeoutAsync(milliseconds);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
            => MainFrame.WaitForFunctionAsync(script, options ?? new WaitForFunctionOptions(), args);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args) => WaitForFunctionAsync(script, null, args);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null)
            => MainFrame.WaitForExpressionAsync(script, options ?? new WaitForFunctionOptions());

        /// <inheritdoc/>
        public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
            => MainFrame.WaitForSelectorAsync(selector, options ?? new WaitForSelectorOptions());

        /// <inheritdoc/>
        public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => MainFrame.WaitForXPathAsync(xpath, options ?? new WaitForSelectorOptions());

        /// <inheritdoc/>
        public Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
            => FrameManager.WaitForFrameNavigationAsync(FrameManager.MainFrame, options);

        /// <inheritdoc/>
        public async Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var idleTime = options?.IdleTime ?? 500;

            var networkIdleTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var idleTimer = new Timer
            {
                Interval = idleTime,
            };

            idleTimer.Elapsed += (sender, args) =>
            {
                networkIdleTcs.TrySetResult(true);
            };

            var networkManager = FrameManager.NetworkManager;

            void Evaluate()
            {
                idleTimer.Stop();

                if (networkManager.NumRequestsInProgress == 0)
                {
                    idleTimer.Start();
                }
            }

            void RequestEventListener(object sender, RequestEventArgs e) => Evaluate();
            void ResponseEventListener(object sender, ResponseCreatedEventArgs e) => Evaluate();

            void Cleanup()
            {
                idleTimer.Stop();
                idleTimer.Dispose();

                networkManager.Request -= RequestEventListener;
                networkManager.Response -= ResponseEventListener;
            }

            networkManager.Request += RequestEventListener;
            networkManager.Response += ResponseEventListener;

            Evaluate();

            await Task.WhenAny(networkIdleTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
            {
                Cleanup();

                return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
            }).ConfigureAwait(false);

            Cleanup();

            if (SessionClosedTask.IsFaulted)
            {
                await SessionClosedTask.ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null)
            => WaitForRequestAsync(request => request.Url == url, options);

        /// <inheritdoc/>
        public async Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            void RequestEventListener(object sender, RequestEventArgs e)
            {
                if (predicate(e.Request))
                {
                    requestTcs.TrySetResult(e.Request);
                    FrameManager.NetworkManager.Request -= RequestEventListener;
                }
            }

            FrameManager.NetworkManager.Request += RequestEventListener;

            await Task.WhenAny(requestTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
            {
                FrameManager.NetworkManager.Request -= RequestEventListener;
                return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
            }).ConfigureAwait(false);

            if (SessionClosedTask.IsFaulted)
            {
                await SessionClosedTask.ConfigureAwait(false);
            }

            return await requestTcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions options = null)
            => WaitForFrameAsync((frame) => frame.Url == url, options);

        /// <inheritdoc/>
        public async Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var frameTcs = new TaskCompletionSource<IFrame>(TaskCreationOptions.RunContinuationsAsynchronously);

            void FrameEventListener(object sender, FrameEventArgs e)
            {
                if (predicate(e.Frame))
                {
                    frameTcs.TrySetResult(e.Frame);
                    FrameManager.FrameAttached -= FrameEventListener;
                    FrameManager.FrameNavigated -= FrameEventListener;
                }
            }

            FrameManager.FrameAttached += FrameEventListener;
            FrameManager.FrameNavigated += FrameEventListener;

            var eventRace = Task.WhenAny(frameTcs.Task, SessionClosedTask).WithTimeout(timeout, t =>
            {
                FrameManager.FrameAttached -= FrameEventListener;
                FrameManager.FrameNavigated -= FrameEventListener;
                return new TimeoutException($"Timeout of {t.TotalMilliseconds} ms exceeded");
            });

            foreach (var frame in Frames)
            {
                if (predicate(frame))
                {
                    return frame;
                }
            }

            await eventRace.ConfigureAwait(false);

            if (SessionClosedTask.IsFaulted)
            {
                await SessionClosedTask.ConfigureAwait(false);
            }

            return await frameTcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null)
            => WaitForResponseAsync(response => response.Url == url, options);

        /// <inheritdoc/>
        public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null)
            => WaitForResponseAsync((response) => Task.FromResult(predicate(response)), options);

        /// <inheritdoc/>
        public async Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null)
        {
            var timeout = options?.Timeout ?? DefaultTimeout;
            var responseTcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void ResponseEventListener(object sender, ResponseCreatedEventArgs e)
            {
                try
                {
                    if (await predicate(e.Response).ConfigureAwait(false))
                    {
                        responseTcs.TrySetResult(e.Response);
                        FrameManager.NetworkManager.Response -= ResponseEventListener;
                    }
                }
                catch (Exception ex)
                {
                    responseTcs.TrySetException(new Exception("Predicated failed", ex));
                }
            }

            FrameManager.NetworkManager.Response += ResponseEventListener;

            await Task.WhenAny(responseTcs.Task, SessionClosedTask).WithTimeout(timeout).ConfigureAwait(false);

            if (SessionClosedTask.IsFaulted)
            {
                await SessionClosedTask.ConfigureAwait(false);
            }

            return await responseTcs.Task.ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<FileChooser> WaitForFileChooserAsync(WaitForFileChooserOptions options = null)
        {
            if (!_fileChooserInterceptors.Any())
            {
                await Client.SendAsync("Page.setInterceptFileChooserDialog", new PageSetInterceptFileChooserDialog
                {
                    Enabled = true,
                }).ConfigureAwait(false);
            }

            var timeout = options?.Timeout ?? _timeoutSettings.Timeout;
            var tcs = new TaskCompletionSource<FileChooser>(TaskCreationOptions.RunContinuationsAsynchronously);
            var guid = Guid.NewGuid();
            _fileChooserInterceptors.TryAdd(guid, tcs);

            try
            {
                return await tcs.Task.WithTimeout(timeout).ConfigureAwait(false);
            }
            catch (Exception)
            {
                _fileChooserInterceptors.TryRemove(guid, out _);
                throw;
            }
        }

        /// <inheritdoc/>
        public Task<IResponse> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

        /// <inheritdoc/>
        public Task<IResponse> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

        /// <inheritdoc/>
        public Task SetBurstModeOffAsync()
        {
            _screenshotBurstModeOn = false;
            if (_screenshotBurstModeOptions != null)
            {
                ResetBackgroundColorAndViewportAsync(_screenshotBurstModeOptions);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task BringToFrontAsync() => Client.SendAsync("Page.bringToFront");

        /// <inheritdoc/>
        public Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
            => Client.SendAsync("Emulation.setEmulatedVisionDeficiency", new EmulationSetEmulatedVisionDeficiencyRequest
            {
                Type = type,
            });

        /// <inheritdoc/>
        public async Task EmulateTimezoneAsync(string timezoneId)
        {
            try
            {
                await Client.SendAsync("Emulation.setTimezoneOverride", new EmulateTimezoneRequest
                {
                    TimezoneId = timezoneId ?? string.Empty,
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.Message.Contains("Invalid timezone"))
            {
                throw new PuppeteerException($"Invalid timezone ID: {timezoneId}");
            }
        }

        /// <inheritdoc/>
        public async Task EmulateIdleStateAsync(EmulateIdleOverrides overrides = null)
        {
            if (overrides != null)
            {
                await Client.SendAsync(
                    "Emulation.setIdleOverride",
                    new EmulationSetIdleOverrideRequest
                    {
                        IsUserActive = overrides.IsUserActive,
                        IsScreenUnlocked = overrides.IsScreenUnlocked,
                    }).ConfigureAwait(false);
            }
            else
            {
                await Client.SendAsync("Emulation.clearIdleOverride").ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task EmulateCPUThrottlingAsync(decimal? factor = null)
        {
            if (factor != null && factor < 1)
            {
                throw new ArgumentException("Throttling rate should be greater or equal to 1", nameof(factor));
            }

            return Client.SendAsync("Emulation.setCPUThrottlingRate", new EmulationSetCPUThrottlingRateRequest
            {
                Rate = factor ?? 1,
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync() => new(CloseAsync());

        internal static async Task<Page> CreateAsync(
            CDPSession client,
            Target target,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewPort,
            TaskQueue screenshotTaskQueue)
        {
            var page = new Page(client, target, screenshotTaskQueue, ignoreHTTPSErrors);
            await page.InitializeAsync().ConfigureAwait(false);

            if (defaultViewPort != null)
            {
                await page.SetViewportAsync(defaultViewPort).ConfigureAwait(false);
            }

            return page;
        }

        internal async Task<byte[]> PdfInternalAsync(string file, PdfOptions options)
        {
            var paperWidth = PaperFormat.Letter.Width;
            var paperHeight = PaperFormat.Letter.Height;

            if (options.Format != null)
            {
                paperWidth = options.Format.Width;
                paperHeight = options.Format.Height;
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

            if (options.OmitBackground)
            {
                await SetTransparentBackgroundColorAsync().ConfigureAwait(false);
            }

            var result = await Client.SendAsync<PagePrintToPDFResponse>("Page.printToPDF", new PagePrintToPDFRequest
            {
                TransferMode = "ReturnAsStream",
                Landscape = options.Landscape,
                DisplayHeaderFooter = options.DisplayHeaderFooter,
                HeaderTemplate = options.HeaderTemplate,
                FooterTemplate = options.FooterTemplate,
                PrintBackground = options.PrintBackground,
                Scale = options.Scale,
                PaperWidth = paperWidth,
                PaperHeight = paperHeight,
                MarginTop = marginTop,
                MarginBottom = marginBottom,
                MarginLeft = marginLeft,
                MarginRight = marginRight,
                PageRanges = options.PageRanges,
                PreferCSSPageSize = options.PreferCSSPageSize,
            }).ConfigureAwait(false);

            if (options.OmitBackground)
            {
                await ResetDefaultBackgroundColorAsync().ConfigureAwait(false);
            }

            return await ProtocolStreamReader.ReadProtocolStreamByteAsync(Client, result.Stream, file).ConfigureAwait(false);
        }

        internal void OnPopup(IPage popupPage) => Popup?.Invoke(this, new PopupEventArgs { PopupPage = popupPage });

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
            => _ = DisposeAsync();

        private async Task InitializeAsync()
        {
            await FrameManager.InitializeAsync().ConfigureAwait(false);
            var networkManager = FrameManager.NetworkManager;

            Client.MessageReceived += Client_MessageReceived;
            FrameManager.FrameAttached += (_, e) => FrameAttached?.Invoke(this, e);
            FrameManager.FrameDetached += (_, e) => FrameDetached?.Invoke(this, e);
            FrameManager.FrameNavigated += (_, e) => FrameNavigated?.Invoke(this, e);

            networkManager.Request += (_, e) => Request?.Invoke(this, e);
            networkManager.RequestFailed += (_, e) => RequestFailed?.Invoke(this, e);
            networkManager.Response += (_, e) => Response?.Invoke(this, e);
            networkManager.RequestFinished += (_, e) => RequestFinished?.Invoke(this, e);
            networkManager.RequestServedFromCache += (_, e) => RequestServedFromCache?.Invoke(this, e);

            await Task.WhenAll(
               Client.SendAsync("Performance.enable", null),
               Client.SendAsync("Log.enable", null)).ConfigureAwait(false);
        }

        private async Task<IResponse> GoAsync(int delta, NavigationOptions options)
        {
            var history = await Client.SendAsync<PageGetNavigationHistoryResponse>("Page.getNavigationHistory").ConfigureAwait(false);

            if (history.Entries.Count <= history.CurrentIndex + delta || history.CurrentIndex + delta < 0)
            {
                return null;
            }

            var entry = history.Entries[history.CurrentIndex + delta];
            var waitTask = WaitForNavigationAsync(options);

            await Task.WhenAll(
                waitTask,
                Client.SendAsync("Page.navigateToHistoryEntry", new PageNavigateToHistoryEntryRequest
                {
                    EntryId = entry.Id,
                })).ConfigureAwait(false);

            return waitTask.Result;
        }

        private Dictionary<string, decimal> BuildMetricsObject(List<Metric> metrics)
        {
            var result = new Dictionary<string, decimal>();

            foreach (var item in metrics)
            {
                if (SupportedMetrics.Contains(item.Name))
                {
                    result.Add(item.Name, item.Value);
                }
            }

            return result;
        }

        private async Task<string> PerformScreenshot(ScreenshotType type, ScreenshotOptions options)
        {
            if (!_screenshotBurstModeOn)
            {
                await Client.SendAsync("Target.activateTarget", new TargetActivateTargetRequest
                {
                    TargetId = Target.TargetId,
                }).ConfigureAwait(false);
            }

            var clip = options.Clip != null ? ProcessClip(options.Clip) : null;

            if (!_screenshotBurstModeOn)
            {
                if (options != null && options.FullPage)
                {
                    var metrics = _screenshotBurstModeOn
                        ? _burstModeMetrics :
                        await Client.SendAsync<PageGetLayoutMetricsResponse>("Page.getLayoutMetrics").ConfigureAwait(false);

                    if (options.BurstMode)
                    {
                        _burstModeMetrics = metrics;
                    }

                    var contentSize = metrics.CssContentSize ?? metrics.ContentSize;

                    var width = Convert.ToInt32(Math.Ceiling(contentSize.Width));
                    var height = Convert.ToInt32(Math.Ceiling(contentSize.Height));

                    // Overwrite clip for full page at all times.
                    clip = new Clip
                    {
                        X = 0,
                        Y = 0,
                        Width = width,
                        Height = height,
                        Scale = 1,
                    };

                    var isMobile = Viewport?.IsMobile ?? false;
                    var deviceScaleFactor = Viewport?.DeviceScaleFactor ?? 1;
                    var isLandscape = Viewport?.IsLandscape ?? false;
                    var screenOrientation = isLandscape
                        ? new ScreenOrientation
                        {
                            Angle = 90,
                            Type = ScreenOrientationType.LandscapePrimary,
                        }
                        : new ScreenOrientation
                        {
                            Angle = 0,
                            Type = ScreenOrientationType.PortraitPrimary,
                        };

                    await Client.SendAsync("Emulation.setDeviceMetricsOverride", new EmulationSetDeviceMetricsOverrideRequest
                    {
                        Mobile = isMobile,
                        Width = width,
                        Height = height,
                        DeviceScaleFactor = deviceScaleFactor,
                        ScreenOrientation = screenOrientation,
                    }).ConfigureAwait(false);
                }

                if (options?.OmitBackground == true && type == ScreenshotType.Png)
                {
                    await SetTransparentBackgroundColorAsync().ConfigureAwait(false);
                }
            }

            var screenMessage = new PageCaptureScreenshotRequest
            {
                Format = type.ToString().ToLower(CultureInfo.CurrentCulture),
            };

            if (options.Quality.HasValue)
            {
                screenMessage.Quality = options.Quality.Value;
            }

            if (clip != null)
            {
                screenMessage.Clip = clip;
            }

            var result = await Client.SendAsync<PageCaptureScreenshotResponse>("Page.captureScreenshot", screenMessage).ConfigureAwait(false);

            if (options.BurstMode)
            {
                _screenshotBurstModeOptions = options;
                _screenshotBurstModeOn = true;
            }
            else
            {
                await ResetBackgroundColorAndViewportAsync(options).ConfigureAwait(false);
            }

            return result.Data;
        }

        private Clip ProcessClip(Clip clip)
        {
            var x = Math.Round(clip.X);
            var y = Math.Round(clip.Y);

            return new Clip
            {
                X = x,
                Y = y,
                Width = Math.Round(clip.Width + clip.X - x, MidpointRounding.AwayFromZero),
                Height = Math.Round(clip.Height + clip.Y - y, MidpointRounding.AwayFromZero),
                Scale = clip.Scale,
            };
        }

        private Task ResetBackgroundColorAndViewportAsync(ScreenshotOptions options)
        {
            var omitBackgroundTask = options?.OmitBackground == true && options.Type == ScreenshotType.Png ?
                ResetDefaultBackgroundColorAsync() : Task.CompletedTask;
            var setViewPortTask = (options?.FullPage == true && Viewport != null) ?
                SetViewportAsync(Viewport) : Task.CompletedTask;
            return Task.WhenAll(omitBackgroundTask, setViewPortTask);
        }

        private Task ResetDefaultBackgroundColorAsync()
            => Client.SendAsync("Emulation.setDefaultBackgroundColorOverride");

        private Task SetTransparentBackgroundColorAsync()
            => Client.SendAsync("Emulation.setDefaultBackgroundColorOverride", new EmulationSetDefaultBackgroundColorOverrideRequest
            {
                Color = new EmulationSetDefaultBackgroundColorOverrideColor
                {
                    R = 0,
                    G = 0,
                    B = 0,
                    A = 0,
                },
            });

        private decimal ConvertPrintParameterToInches(object parameter)
        {
            if (parameter == null)
            {
                return 0;
            }

            decimal pixels;
            if (parameter is decimal || parameter is int)
            {
                pixels = Convert.ToDecimal(parameter, CultureInfo.CurrentCulture);
            }
            else
            {
                var text = parameter.ToString();
                var unit = text.Substring(text.Length - 2).ToLower(CultureInfo.CurrentCulture);
                string valueText;
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

                if (decimal.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out var number))
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

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Page.domContentEventFired":
                        DOMContentLoaded?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Page.loadEventFired":
                        Load?.Invoke(this, EventArgs.Empty);
                        break;
                    case "Runtime.consoleAPICalled":
                        await OnConsoleAPIAsync(e.MessageData.ToObject<PageConsoleResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Page.javascriptDialogOpening":
                        OnDialog(e.MessageData.ToObject<PageJavascriptDialogOpeningResponse>(true));
                        break;
                    case "Runtime.exceptionThrown":
                        HandleException(e.MessageData.ToObject<RuntimeExceptionThrownResponse>(true).ExceptionDetails);
                        break;
                    case "Inspector.targetCrashed":
                        OnTargetCrashed();
                        break;
                    case "Performance.metrics":
                        EmitMetrics(e.MessageData.ToObject<PerformanceMetricsResponse>(true));
                        break;
                    case "Log.entryAdded":
                        await OnLogEntryAddedAsync(e.MessageData.ToObject<LogEntryAddedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Runtime.bindingCalled":
                        await OnBindingCalled(e.MessageData.ToObject<BindingCalledResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Page.fileChooserOpened":
                        await OnFileChooserAsync(e.MessageData.ToObject<PageFileChooserOpenedResponse>(true)).ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"Page failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                Client.Close(message);
            }
        }

        private async Task OnFileChooserAsync(PageFileChooserOpenedResponse e)
        {
            if (_fileChooserInterceptors.Count == 0)
            {
                try
                {
                    await Client.SendAsync("Page.handleFileChooser", new PageHandleFileChooserRequest
                    {
                        Action = FileChooserAction.Fallback,
                    }).ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.ToString());
                }
            }

            var frame = await FrameManager.GetFrameAsync(e.FrameId).ConfigureAwait(false);
            var context = await frame.GetExecutionContextAsync().ConfigureAwait(false) as ExecutionContext;
            var world = context.World;
            var element = await world.AdoptBackendNodeAsync(e.BackendNodeId).ConfigureAwait(false);
            var fileChooser = new FileChooser(element, e);
            while (_fileChooserInterceptors.Count > 0)
            {
                var key = _fileChooserInterceptors.FirstOrDefault().Key;

                if (_fileChooserInterceptors.TryRemove(key, out var tcs))
                {
                    tcs.TrySetResult(fileChooser);
                }
            }
        }

        private async Task OnBindingCalled(BindingCalledResponse e)
        {
            string expression;
            try
            {
                if (e.BindingPayload.Type != "exposedFun" || !_pageBindings.ContainsKey(e.BindingPayload.Name))
                {
                    return;
                }

                var result = await BindingUtils.ExecuteBindingAsync(e, _pageBindings).ConfigureAwait(false);

                expression = BindingUtils.EvaluationString(
                    @"function deliverResult(name, seq, result) {
                        window[name]['callbacks'].get(seq).resolve(result);
                        window[name]['callbacks'].delete(seq);
                    }",
                    e.BindingPayload.Name,
                    e.BindingPayload.Seq,
                    result);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                expression = BindingUtils.EvaluationString(
                    @"function deliverError(name, seq, message, stack) {
                        const error = new Error(message);
                        error.stack = stack;
                        window[name]['callbacks'].get(seq).reject(error);
                        window[name]['callbacks'].delete(seq);
                    }",
                    e.BindingPayload.Name,
                    e.BindingPayload.Seq,
                    ex.Message,
                    ex.StackTrace);
            }

            Client.Send("Runtime.evaluate", new
            {
                expression,
                contextId = e.ExecutionContextId,
            });
        }

        private void OnDetachedFromTarget(object sender, TargetChangedArgs e)
        {
            var sessionId = e.Target.Session?.Id;
            if (sessionId != null && _workers.TryRemove(sessionId, out var worker))
            {
                WorkerDestroyed?.Invoke(this, new WorkerEventArgs(worker));
            }
        }

        private void OnAttachedToTarget(Target target, Target parentTarget)
        {
            FrameManager.OnAttachedToTarget(new TargetChangedArgs { Target = target });
            var targetInfo = target.TargetInfo;
            var sessionId = target.Session.Id;

            if (targetInfo.Type == TargetType.Worker)
            {
                var session = target.Session;
                var worker = new Worker(session, targetInfo.Url, AddConsoleMessageAsync, HandleException);
                _workers[sessionId] = worker;
                WorkerCreated?.Invoke(this, new WorkerEventArgs(worker));
            }

            if (target.Session != null)
            {
                target.TargetManager.AddTargetInterceptor(target.Session, OnAttachedToTarget);
            }
        }

        private async Task OnLogEntryAddedAsync(LogEntryAddedResponse e)
        {
            if (e.Entry.Args != null)
            {
                foreach (var arg in e.Entry?.Args)
                {
                    await RemoteObjectHelper.ReleaseObjectAsync(Client, arg, _logger).ConfigureAwait(false);
                }
            }

            if (e.Entry.Source != TargetType.Worker)
            {
                Console?.Invoke(this, new ConsoleEventArgs(new ConsoleMessage(
                    e.Entry.Level,
                    e.Entry.Text,
                    null,
                    new ConsoleMessageLocation
                    {
                        URL = e.Entry.URL,
                        LineNumber = e.Entry.LineNumber,
                    })));
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

        private void EmitMetrics(PerformanceMetricsResponse metrics)
            => Metrics?.Invoke(this, new MetricEventArgs(metrics.Title, BuildMetricsObject(metrics.Metrics)));

        private void HandleException(EvaluateExceptionResponseDetails exceptionDetails)
            => PageError?.Invoke(this, new PageErrorEventArgs(GetExceptionMessage(exceptionDetails)));

        private string GetExceptionMessage(EvaluateExceptionResponseDetails exceptionDetails)
        {
            if (exceptionDetails.Exception != null)
            {
                return exceptionDetails.Exception.Description;
            }

            var message = exceptionDetails.Text;
            if (exceptionDetails.StackTrace != null)
            {
                foreach (var callframe in exceptionDetails.StackTrace.CallFrames)
                {
                    var location = $"{callframe.Url}:{callframe.LineNumber}:{callframe.ColumnNumber}";
                    var functionName = callframe.FunctionName ?? "<anonymous>";
                    message += $"\n at {functionName} ({location})";
                }
            }

            return message;
        }

        private void OnDialog(PageJavascriptDialogOpeningResponse message)
        {
            var dialog = new Dialog(Client, message.Type, message.Message, message.DefaultPrompt);
            Dialog?.Invoke(this, new DialogEventArgs(dialog));
        }

        private Task OnConsoleAPIAsync(PageConsoleResponse message)
        {
            if (message.ExecutionContextId == 0)
            {
                return Task.CompletedTask;
            }

            var ctx = FrameManager.ExecutionContextById(message.ExecutionContextId, Client);
            var values = message.Args.Select(ctx.CreateJSHandle).ToArray();

            return AddConsoleMessageAsync(message.Type, values, message.StackTrace);
        }

        private async Task AddConsoleMessageAsync(ConsoleType type, IJSHandle[] values, Messaging.StackTrace stackTrace)
        {
            if (Console?.GetInvocationList().Length == 0)
            {
                await Task.WhenAll(values.Select(v => RemoteObjectHelper.ReleaseObjectAsync(Client, v.RemoteObject, _logger))).ConfigureAwait(false);
                return;
            }

            var tokens = values.Select(i => i.RemoteObject.ObjectId != null || i.RemoteObject.Type == RemoteObjectType.Object
                ? i.ToString()
                : RemoteObjectHelper.ValueFromRemoteObject<string>(i.RemoteObject));

            var location = new ConsoleMessageLocation();
            if (stackTrace?.CallFrames?.Length > 0)
            {
                var callFrame = stackTrace.CallFrames[0];
                location.URL = callFrame.URL;
                location.LineNumber = callFrame.LineNumber;
                location.ColumnNumber = callFrame.ColumnNumber;
            }

            var consoleMessage = new ConsoleMessage(type, string.Join(" ", tokens), values, location);
            Console?.Invoke(this, new ConsoleEventArgs(consoleMessage));
        }

        private async Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
        {
            if (_pageBindings.ContainsKey(name))
            {
                throw new PuppeteerException($"Failed to add page binding with name {name}: window['{name}'] already exists!");
            }

            _pageBindings.TryAdd(name, puppeteerFunction);

            const string addPageBinding = @"function addPageBinding(type, bindingName) {
              const binding = window[bindingName];
              window[bindingName] = (...args) => {
                const me = window[bindingName];
                let callbacks = me['callbacks'];
                if (!callbacks) {
                  callbacks = new Map();
                  me['callbacks'] = callbacks;
                }
                const seq = (me['lastSeq'] || 0) + 1;
                me['lastSeq'] = seq;
                const promise = new Promise((resolve, reject) => callbacks.set(seq, {resolve, reject}));
                binding(JSON.stringify({type, name: bindingName, seq, args}));
                return promise;
              };
            }";
            var expression = BindingUtils.EvaluationString(addPageBinding, "exposedFun", name);
            await Client.SendAsync("Runtime.addBinding", new RuntimeAddBindingRequest { Name = name }).ConfigureAwait(false);
            await Client.SendAsync("Page.addScriptToEvaluateOnNewDocument", new PageAddScriptToEvaluateOnNewDocumentRequest
            {
                Source = expression,
            }).ConfigureAwait(false);

            await Task.WhenAll(Frames.Select(
                frame => frame
                    .EvaluateExpressionAsync(expression)
                    .ContinueWith(
                        task =>
                        {
                            if (task.IsFaulted)
                            {
                                _logger.LogError(task.Exception.ToString());
                            }
                        },
                        TaskScheduler.Default)))
                .ConfigureAwait(false);
        }
    }
}
