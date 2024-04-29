using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.PageAccessibility;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    [DebuggerDisplay("Page {Url}")]
    public abstract class Page : IPage
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

        private readonly TaskQueue _screenshotTaskQueue;
        private readonly ConcurrentSet<Func<IRequest, Task>> _requestInterceptionTask = [];

        internal Page(TaskQueue screenshotTaskQueue)
        {
            _screenshotTaskQueue = screenshotTaskQueue;
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
        public event EventHandler<FrameNavigatedEventArgs> FrameNavigated;

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

        /// <inheritdoc cref="ICDPSession"/>
        ICDPSession IPage.Client => Client;

        /// <inheritdoc cref="CDPSession"/>
        public abstract CDPSession Client { get; }

        /// <inheritdoc/>
        public int DefaultNavigationTimeout
        {
            get => TimeoutSettings.NavigationTimeout;
            set => TimeoutSettings.NavigationTimeout = value;
        }

        /// <inheritdoc/>
        public int DefaultTimeout
        {
            get => TimeoutSettings.Timeout;
            set => TimeoutSettings.Timeout = value;
        }

        /// <inheritdoc/>
        public abstract IFrame MainFrame { get; }

        /// <inheritdoc/>
        public abstract IFrame[] Frames { get; }

        /// <inheritdoc/>
        public abstract WebWorker[] Workers { get; }

        /// <inheritdoc/>
        public bool IsServiceWorkerBypassed { get; protected set; }

        /// <inheritdoc/>
        public string Url => MainFrame.Url;

        /// <inheritdoc/>
        ITarget IPage.Target => Target;

        /// <inheritdoc cref="CDPSession"/>
        public abstract Target Target { get; }

        /// <inheritdoc/>
        IKeyboard IPage.Keyboard => Keyboard;

        /// <inheritdoc/>
        ITouchscreen IPage.Touchscreen => Touchscreen;

        /// <inheritdoc/>
        ICoverage IPage.Coverage => Coverage;

        /// <inheritdoc/>
        ITracing IPage.Tracing => Tracing;

        /// <inheritdoc/>
        IMouse IPage.Mouse => Mouse;

        /// <inheritdoc/>
        public ViewPortOptions Viewport { get; protected set; }

        /// <inheritdoc/>
        IBrowser IPage.Browser => Browser;

        /// <inheritdoc/>
        public abstract IBrowserContext BrowserContext { get; }

        /// <summary>
        /// Get an indication that the page has been closed.
        /// </summary>
        public bool IsClosed { get; protected set; }

        /// <summary>
        /// Gets the accessibility.
        /// </summary>
        IAccessibility IPage.Accessibility => Accessibility;

        /// <inheritdoc/>
        public abstract bool IsJavaScriptEnabled { get; }

        /// <inheritdoc/>
        public bool IsDragInterceptionEnabled { get; protected set; }

        internal Accessibility Accessibility { get; init; }

        internal Keyboard Keyboard { get; init; }

        internal Touchscreen Touchscreen { get; init; }

        internal Coverage Coverage { get; init; }

        internal Tracing Tracing { get; init; }

        internal Mouse Mouse { get; init; }

        internal bool IsDragging { get; set; }

        internal bool HasPopupEventListeners => Popup?.GetInvocationList().Length > 0;

        internal bool HasErrorEventListeners => Error?.GetInvocationList().Length > 0;

        /// <summary>
        /// Timeout settings.
        /// </summary>
        protected TimeoutSettings TimeoutSettings { get; set; } = new();

        /// <summary>
        /// Whether the <see cref="Console"/> event has listeners.
        /// </summary>
        protected bool HasConsoleEventListeners => Console?.GetInvocationList().Length == 0;

        /// <summary>
        /// Browser.
        /// </summary>
        protected abstract Browser Browser { get; }

        /// <summary>
        /// Whether burst mode is on.
        /// </summary>
        protected bool ScreenshotBurstModeOn { get; set; }

        /// <summary>
        /// Screenshot burst mode options.
        /// </summary>
        protected ScreenshotOptions ScreenshotBurstModeOptions { get; set; }

        /// <inheritdoc/>
        public abstract Task SetGeolocationAsync(GeolocationOption options);

        /// <inheritdoc/>
        public abstract Task SetDragInterceptionAsync(bool enabled);

        /// <inheritdoc/>
        public abstract Task<Dictionary<string, decimal>> MetricsAsync();

        /// <inheritdoc/>
        public Task TapAsync(string selector)
            => MainFrame.TapAsync(selector);

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
#pragma warning disable CS0618 // Using obsolete
        public Task<IElementHandle[]> XPathAsync(string expression) => MainFrame.XPathAsync(expression);
#pragma warning restore CS0618

        /// <inheritdoc/>
        public Task<DeviceRequestPrompt> WaitForDevicePromptAsync(
            WaitForOptions options = default(WaitForOptions))
            => MainFrame.WaitForDevicePromptAsync(options);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
            => MainFrame.EvaluateExpressionHandleAsync(script);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
            => MainFrame.EvaluateFunctionHandleAsync(pageFunction, args);

        /// <inheritdoc/>
        public abstract Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(
            string pageFunction,
            params object[] args);

        /// <inheritdoc/>
        public abstract Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier);

        /// <inheritdoc/>
        public abstract Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression);

        /// <inheritdoc/>
        public abstract Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle);

        /// <inheritdoc/>
        public abstract Task SetRequestInterceptionAsync(bool value);

        /// <inheritdoc/>
        public abstract Task SetOfflineModeAsync(bool value);

        /// <inheritdoc/>
        public abstract Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions);

        /// <inheritdoc/>
        public abstract Task<CookieParam[]> GetCookiesAsync(params string[] urls);

        /// <inheritdoc/>
        public abstract Task SetCookieAsync(params CookieParam[] cookies);

        /// <inheritdoc/>
        public abstract Task DeleteCookieAsync(params CookieParam[] cookies);

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
            => MainFrame.AddScriptTagAsync(options);

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(string url)
            => AddScriptTagAsync(new AddTagOptions { Url = url });

        /// <inheritdoc/>
        public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
            => MainFrame.AddStyleTagAsync(options);

        /// <inheritdoc/>
        public Task<IElementHandle> AddStyleTagAsync(string url)
            => AddStyleTagAsync(new AddTagOptions { Url = url });

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
        public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(
            string name,
            Func<T1, T2, T3, T4, TResult> puppeteerFunction)
            => ExposeFunctionAsync(name, (Delegate)puppeteerFunction);

        /// <inheritdoc/>
        public abstract Task RemoveExposedFunctionAsync(string name);

        /// <inheritdoc/>
        public Task<string> GetContentAsync() => MainFrame.GetContentAsync();

        /// <inheritdoc/>
        public Task SetContentAsync(string html, NavigationOptions options = null)
            => MainFrame.SetContentAsync(html, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, NavigationOptions options)
            => MainFrame.GoToAsync(url, options);

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
        public abstract Task SetJavaScriptEnabledAsync(bool enabled);

        /// <inheritdoc/>
        public abstract Task SetBypassCSPAsync(bool enabled);

        /// <inheritdoc/>
        public abstract Task EmulateMediaTypeAsync(MediaType type);

        /// <inheritdoc/>
        public abstract Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features);

        /// <inheritdoc/>
        public abstract Task SetViewportAsync(ViewPortOptions viewport);

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

            using var fs = AsyncFileHelper.CreateStream(file, FileMode.Create);
            await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
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
            => _screenshotTaskQueue.Enqueue(async () =>
            {
                if (options == null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                var screenshotType = options.Type ?? ScreenshotType.Png;

                if (options.Quality.HasValue)
                {
                    if (screenshotType != ScreenshotType.Jpeg)
                    {
                        throw new ArgumentException(
                            $"options.Quality is unsupported for the {screenshotType} screenshots");
                    }

                    if (options.Quality < 0 || options.Quality > 100)
                    {
                        throw new ArgumentException(
                            $"Expected options.quality to be between 0 and 100 (inclusive), got {options.Quality}");
                    }
                }

                if (options.Clip?.Width == 0)
                {
                    throw new PuppeteerException("Expected options.Clip.Width not to be 0.");
                }

                if (options.Clip?.Height == 0)
                {
                    throw new PuppeteerException("Expected options.Clip.Height not to be 0.");
                }

                if (options.Clip != null && options.FullPage)
                {
                    throw new NotSupportedException("options.clip and options.fullPage are exclusive");
                }

                var stack = new DisposableTasksStack();
                await using (stack.ConfigureAwait(false))
                {
                    if (!ScreenshotBurstModeOn)
                    {
                        await BringToFrontAsync().ConfigureAwait(false);
                    }

                    // FromSurface is not supported on Firefox.
                    // It seems that Puppeteer solved this just by ignoring screenshot tests in firefox.
                    if (Browser.BrowserType == SupportedBrowser.Firefox)
                    {
                        if (options.FromSurface != null)
                        {
                            throw new NotSupportedException("Screenshots from surface are not supported on Firefox.");
                        }
                    }
                    else
                    {
                        options.FromSurface ??= true;
                    }

                    if (options.Clip != null && options.FullPage)
                    {
                        throw new ArgumentException("Clip and FullPage are exclusive");
                    }

                    var clip = options.Clip != null ? RoundRectangle(NormalizeRectangle(options.Clip)) : null;
                    var captureBeyondViewport = options.CaptureBeyondViewport;

                    if (!ScreenshotBurstModeOn)
                    {
                        if (options.Clip == null)
                        {
                            if (options.FullPage)
                            {
                                // Overwrite clip for full page at all times.
                                clip = null;

                                if (!captureBeyondViewport)
                                {
                                    var scrollDimensions = await ((Frame)MainFrame).IsolatedRealm
                                        .EvaluateFunctionAsync<BoundingBox>(@"() => {
                                            const element = document.documentElement;
                                            return {
                                                width: element.scrollWidth,
                                                height: element.scrollHeight,
                                            };
                                        }").ConfigureAwait(false);

                                    var viewport = Viewport with { };

                                    await SetViewportAsync(viewport with
                                    {
                                        Width = Convert.ToInt32(scrollDimensions.Width),
                                        Height = Convert.ToInt32(scrollDimensions.Height),
                                    }).ConfigureAwait(false);

                                    stack.Defer(() => SetViewportAsync(viewport));
                                }
                            }
                            else
                            {
                                captureBeyondViewport = false;
                            }
                        }
                    }

                    options.Clip = clip;
                    options.CaptureBeyondViewport = captureBeyondViewport;
                    var result = await PerformScreenshotAsync(screenshotType, options).ConfigureAwait(false);

                    if (options.BurstMode)
                    {
                        ScreenshotBurstModeOptions = options;
                        ScreenshotBurstModeOn = true;
                    }

                    return result;
                }
            });

        /// <inheritdoc/>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => MainFrame.GetTitleAsync();

        /// <inheritdoc/>
        public abstract Task CloseAsync(PageCloseOptions options = null);

        /// <inheritdoc/>
        public abstract Task SetCacheEnabledAsync(bool enabled = true);

        /// <inheritdoc/>
        public Task ClickAsync(string selector, ClickOptions options = null)
            => MainFrame.ClickAsync(selector, options);

        /// <inheritdoc/>
        public Task HoverAsync(string selector) => MainFrame.HoverAsync(selector);

        /// <inheritdoc/>
        public Task FocusAsync(string selector) => MainFrame.FocusAsync(selector);

        /// <inheritdoc/>
        public Task TypeAsync(string selector, string text, TypeOptions options = null)
            => MainFrame.TypeAsync(selector, text, options);

        /// <inheritdoc/>
        public Task<JToken> EvaluateExpressionAsync(string script)
            => MainFrame.EvaluateExpressionAsync<JToken>(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script)
            => MainFrame.EvaluateExpressionAsync<T>(script);

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => MainFrame.EvaluateFunctionAsync<JToken>(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => MainFrame.EvaluateFunctionAsync<T>(script, args);

        /// <inheritdoc/>
        public abstract Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null);

        /// <inheritdoc/>
        public abstract Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers);

        /// <inheritdoc/>
        public abstract Task AuthenticateAsync(Credentials credentials);

        /// <inheritdoc/>
        public abstract Task<IResponse> ReloadAsync(NavigationOptions options);

        /// <inheritdoc/>
        public Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => ReloadAsync(new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(string selector, params string[] values)
            => MainFrame.SelectAsync(selector, values);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
            => MainFrame.WaitForFunctionAsync(script, options ?? new WaitForFunctionOptions(), args);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args) =>
            WaitForFunctionAsync(script, null, args);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null)
            => MainFrame.WaitForExpressionAsync(script, options ?? new WaitForFunctionOptions());

        /// <inheritdoc/>
        public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
            => MainFrame.WaitForSelectorAsync(selector, options ?? new WaitForSelectorOptions());

        /// <inheritdoc/>
#pragma warning disable CS0618 // WaitForXPathAsync is obsolete
        public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
            => MainFrame.WaitForXPathAsync(xpath, options ?? new WaitForSelectorOptions());
#pragma warning restore CS0618

        /// <inheritdoc/>
        public Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
            => MainFrame.WaitForNavigationAsync(options);

        /// <inheritdoc/>
        public abstract Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null);

        /// <inheritdoc/>
        public Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null)
            => WaitForRequestAsync(request => request.Url == url, options);

        /// <inheritdoc/>
        public abstract Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null);

        /// <inheritdoc/>
        public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions options = null)
            => WaitForFrameAsync((frame) => frame.Url == url, options);

        /// <inheritdoc/>
        public abstract Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null);

        /// <inheritdoc/>
        public Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null)
            => WaitForResponseAsync(response => response.Url == url, options);

        /// <inheritdoc/>
        public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null)
            => WaitForResponseAsync((response) => Task.FromResult(predicate(response)), options);

        /// <inheritdoc/>
        public abstract Task<IResponse> WaitForResponseAsync(
            Func<IResponse, Task<bool>> predicate,
            WaitForOptions options = null);

        /// <inheritdoc/>
        public abstract Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null);

        /// <inheritdoc/>
        public abstract Task<IResponse> GoBackAsync(NavigationOptions options = null);

        /// <inheritdoc/>
        public abstract Task<IResponse> GoForwardAsync(NavigationOptions options = null);

        /// <inheritdoc/>
        public abstract Task SetBurstModeOffAsync();

        /// <inheritdoc/>
        public abstract Task BringToFrontAsync();

        /// <inheritdoc/>
        public abstract Task EmulateVisionDeficiencyAsync(VisionDeficiency type);

        /// <inheritdoc/>
        public abstract Task EmulateTimezoneAsync(string timezoneId);

        /// <inheritdoc/>
        public abstract Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null);

        /// <inheritdoc/>
        public abstract Task EmulateCPUThrottlingAsync(decimal? factor = null);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await CloseAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void AddRequestInterceptor(Func<IRequest, Task> interceptionTask)
            => _requestInterceptionTask.Add(interceptionTask);

        /// <inheritdoc />
        public void RemoveRequestInterceptor(Func<IRequest, Task> interceptionTask)
            => _requestInterceptionTask.Remove(interceptionTask);

        /// <inheritdoc />
        public Task<ICDPSession> CreateCDPSessionAsync() => Target.CreateCDPSessionAsync();

        /// <inheritdoc />
        public abstract Task SetBypassServiceWorkerAsync(bool bypass);

        internal void OnPopup(IPage popupPage) => Popup?.Invoke(this, new PopupEventArgs { PopupPage = popupPage });

        /// <summary>
        /// Raises the <see cref="Dialog"/> event.
        /// </summary>
        /// <param name="message">Dialog message.</param>
        internal void OnDialog(PageJavascriptDialogOpeningResponse message)
        {
            var dialog = new CdpDialog(Client, message.Type, message.Message, message.DefaultPrompt);
            Dialog?.Invoke(this, new DialogEventArgs(dialog));
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            Mouse.Dispose();
            _ = DisposeAsync();
        }

        /// <summary>
        /// Raises the <see cref="FrameAttached"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnFrameAttached(FrameEventArgs e) => FrameAttached?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="FrameNavigated"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnFrameNavigated(FrameNavigatedEventArgs e) => FrameNavigated?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="FrameDetached"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnFrameDetached(FrameEventArgs e) => FrameDetached?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="RequestFailed"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnRequestFailed(RequestEventArgs e) => RequestFailed?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="RequestFinished"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnRequestFinished(RequestEventArgs e) => RequestFinished?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="Response"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnResponse(ResponseCreatedEventArgs e) => Response?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="RequestServedFromCache"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected void OnRequestServedFromCache(RequestEventArgs e) => RequestServedFromCache?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="DOMContentLoaded"/> event.
        /// </summary>
        protected void OnDOMContentLoaded() => DOMContentLoaded?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Raises the <see cref="Load"/> event.
        /// </summary>
        protected void OnLoad() => Load?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Raises the <see cref="Request"/> event.
        /// </summary>
        /// <param name="request">Request object.</param>
        protected void OnRequest(IRequest request)
        {
            if (request == null)
            {
                return;
            }

            // Run tasks one after the other
            foreach (var subscriber in _requestInterceptionTask)
            {
                (request as CdpHttpRequest)?.EnqueueInterceptionAction(subscriber);
            }

            Request?.Invoke(this, new RequestEventArgs(request));
        }

        /// <summary>
        /// Raises the <see cref="WorkerDestroyed"/> event.
        /// </summary>
        /// <param name="worker">Worker.</param>
        protected void OnWorkerDestroyed(WebWorker worker) => WorkerDestroyed?.Invoke(this, new WorkerEventArgs(worker));

        /// <summary>
        /// Raises the <see cref="WorkerCreated"/> event.
        /// </summary>
        /// <param name="worker">Worker.</param>
        protected void OnWorkerCreated(WebWorker worker) => WorkerCreated?.Invoke(this, new WorkerEventArgs(worker));

        /// <summary>
        /// Raises the <see cref="Close"/> event.
        /// </summary>
        protected void OnClose() => Close?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Raises the <see cref="Console"/> event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected void OnConsole(ConsoleEventArgs e) => Console?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="Metrics"/> event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected void OnMetrics(MetricEventArgs e) => Metrics?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="PageError"/> event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected void OnPageError(PageErrorEventArgs e) => PageError?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="Error"/> event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected void OnError(ErrorEventArgs e) => Error?.Invoke(this, e);

        /// <summary>
        /// PDF implementation.
        /// </summary>
        /// <param name="file">File path.</param>
        /// <param name="options">PDF options.</param>
        /// <returns>PDF data.</returns>
        protected abstract Task<byte[]> PdfInternalAsync(string file, PdfOptions options);

        /// <summary>
        /// Screenshot implementation.
        /// </summary>
        /// <param name="type">Screenshot type.</param>
        /// <param name="options">Options.</param>
        /// <returns>The screenshot as a base64 string.</returns>
        protected abstract Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options);

        /// <summary>
        /// Internal add exposed functions.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="puppeteerFunction">Puppeteer function.</param>
        /// <returns>A <see cref="Task"/> that completes when the function has been added.</returns>
        protected abstract Task ExposeFunctionAsync(string name, Delegate puppeteerFunction);

        private Clip RoundRectangle(Clip clip)
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

        private Clip NormalizeRectangle(Clip clip)
        {
            return new Clip()
            {
                Scale = clip.Scale,
                X = clip.Width < 0 ? clip.X + clip.Width : clip.X,
                Y = clip.Height < 0 ? clip.Y + clip.Height : clip.Y,
                Width = Math.Abs(clip.Width),
                Height = Math.Abs(clip.Height),
            };
        }
    }
}
