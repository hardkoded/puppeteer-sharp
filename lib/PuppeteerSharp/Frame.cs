using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class Frame : IFrame
    {
        internal Frame(FrameManager frameManager, string frameId, string parentFrameId, CDPSession client)
        {
            FrameManager = frameManager;
            Id = frameId;
            Client = client;
            ParentId = parentFrameId;

            UpdateClient(client);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IFrame> ChildFrames => FrameManager.FrameTree.GetChildFrames(Id);

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Url { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public bool Detached { get; set; }

        /// <inheritdoc/>
        public IPage Page => FrameManager.Page;

        /// <inheritdoc/>
        public IFrame ParentFrame => FrameManager.FrameTree.GetParentFrame(Id);

        /// <inheritdoc/>
        public bool IsOopFrame => Client != FrameManager.Client;

        /// <inheritdoc/>
        public string Id { get; internal set; }

        internal string ParentId { get; }

        internal FrameManager FrameManager { get; }

        internal string LoaderId { get; set; }

        internal List<string> LifecycleEvents { get; } = new();

        internal IsolatedWorld MainWorld { get; private set; }

        internal IsolatedWorld PuppeteerWorld { get; private set; }

        internal CDPSession Client { get; private set; }

        internal bool HasStartedLoading { get; private set; }

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, NavigationOptions options) => FrameManager.NavigateFrameAsync(this, url, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => GoToAsync(url, new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null) => FrameManager.WaitForFrameNavigationAsync(this, options);

        /// <inheritdoc/>
        public Task<JToken> EvaluateExpressionAsync(string script) => MainWorld.EvaluateExpressionAsync(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script) => MainWorld.EvaluateExpressionAsync<T>(script);

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args) => MainWorld.EvaluateFunctionAsync(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args) => MainWorld.EvaluateFunctionAsync<T>(script, args);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateExpressionHandleAsync(string script) => MainWorld.EvaluateExpressionHandleAsync(script);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string function, params object[] args) => MainWorld.EvaluateFunctionHandleAsync(function, args);

        /// <inheritdoc/>
        public async Task<IExecutionContext> GetExecutionContextAsync()
        {
            return await MainWorld.GetExecutionContextAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            if (string.IsNullOrEmpty(selector))
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var (updatedSelector, queryHandler) = Client.Connection.CustomQuerySelectorRegistry.GetQueryHandlerAndSelector(selector);
            return await queryHandler.WaitForAsync(this, null, updatedSelector, options).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(nameof(xpath));
            }

            if (xpath.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                xpath = $".{xpath}";
            }

            return WaitForSelectorAsync($"xpath/{xpath}", options);
        }

        /// <inheritdoc/>
        public Task WaitForTimeoutAsync(int milliseconds) => Task.Delay(milliseconds);

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return MainWorld.WaitForFunctionAsync(script, options, args);
        }

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return MainWorld.WaitForExpressionAsync(script, options);
        }

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(string selector, params string[] values) => PuppeteerWorld.SelectAsync(selector, values);

        /// <inheritdoc/>
        public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
            => MainWorld.QuerySelectorAllHandleAsync(selector);

        /// <inheritdoc/>
        public Task<IElementHandle> QuerySelectorAsync(string selector) => MainWorld.QuerySelectorAsync(selector);

        /// <inheritdoc/>
        public Task<IElementHandle[]> QuerySelectorAllAsync(string selector) => MainWorld.QuerySelectorAllAsync(selector);

        /// <inheritdoc/>
        public Task<IElementHandle[]> XPathAsync(string expression) => MainWorld.XPathAsync(expression);

        /// <inheritdoc/>
        public async Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.Path) && string.IsNullOrEmpty(options.Content))
            {
                throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
            }

            var content = options.Content;

            if (!string.IsNullOrEmpty(options.Path))
            {
                content = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
                content += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
            }

            var handle = await PuppeteerWorld.EvaluateFunctionHandleAsync(
                @"async (puppeteerUtil, url, id, type, content) => {
                  const createDeferredPromise = puppeteerUtil.createDeferredPromise;
                  const promise = createDeferredPromise();
                  let element;
                  if (!url) {
                    element = document.createElement('style');
                    element.appendChild(document.createTextNode(content));
                  } else {
                    const link = document.createElement('link');
                    link.rel = 'stylesheet';
                    link.href = url;
                    element = link;
                  }
                  element.addEventListener(
                    'load',
                    () => {
                      promise.resolve();
                    },
                    {once: true}
                  );
                  element.addEventListener(
                    'error',
                    event => {
                      promise.reject(
                        new Error(
                          event.message ?? 'Could not load style'
                        )
                      );
                    },
                    {once: true}
                  );
                  document.head.appendChild(element);
                  await promise;
                  return element;
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                options.Url,
                options.Id,
                options.Type,
                content).ConfigureAwait(false);

            return (await MainWorld.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
        }

        /// <inheritdoc/>
        public async Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.Path) && string.IsNullOrEmpty(options.Content))
            {
                throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
            }

            var content = options.Content;

            if (!string.IsNullOrEmpty(options.Path))
            {
                content = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
                content += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
            }

            var handle = await PuppeteerWorld.EvaluateFunctionHandleAsync(
                @"async (puppeteerUtil, url, id, type, content) => {
                  const createDeferredPromise = puppeteerUtil.createDeferredPromise;
                  const promise = createDeferredPromise();
                  const script = document.createElement('script');
                  script.type = type;
                  script.text = content;
                  if (url) {
                    script.src = url;
                    script.addEventListener(
                      'load',
                      () => {
                        return promise.resolve();
                      },
                      {once: true}
                    );
                    script.addEventListener(
                      'error',
                      event => {
                        promise.reject(
                          new Error(event.message ?? 'Could not load script')
                        );
                      },
                      {once: true}
                    );
                  } else {
                    promise.resolve();
                  }
                  if (id) {
                    script.id = id;
                  }
                  document.head.appendChild(script);
                  await promise;
                  return script;
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                options.Url,
                options.Id,
                options.Type,
                content).ConfigureAwait(false);

            return (await MainWorld.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
        }

        /// <inheritdoc/>
        public Task<string> GetContentAsync() => PuppeteerWorld.GetContentAsync();

        /// <inheritdoc/>
        public Task SetContentAsync(string html, NavigationOptions options = null)
            => PuppeteerWorld.SetContentAsync(html, options);

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => PuppeteerWorld.GetTitleAsync();

        /// <inheritdoc/>
        public Task ClickAsync(string selector, ClickOptions options = null)
            => PuppeteerWorld.ClickAsync(selector, options);

        /// <inheritdoc/>
        public Task HoverAsync(string selector) => PuppeteerWorld.HoverAsync(selector);

        /// <inheritdoc/>
        public Task FocusAsync(string selector) => PuppeteerWorld.FocusAsync(selector);

        /// <inheritdoc/>
        public Task TypeAsync(string selector, string text, TypeOptions options = null)
             => PuppeteerWorld.TypeAsync(selector, text, options);

        internal void OnLoadingStarted() => HasStartedLoading = true;

        internal void OnLoadingStopped()
        {
            LifecycleEvents.Add("DOMContentLoaded");
            LifecycleEvents.Add("load");
        }

        internal void OnLifecycleEvent(string loaderId, string name)
        {
            if (name == "init")
            {
                LoaderId = loaderId;
                LifecycleEvents.Clear();
            }

            LifecycleEvents.Add(name);
        }

        internal void Navigated(FramePayload framePayload)
        {
            Name = framePayload.Name ?? string.Empty;
            Url = framePayload.Url + framePayload.UrlFragment;
        }

        internal void NavigatedWithinDocument(string url) => Url = url;

        internal void Detach()
        {
            Detached = true;
            MainWorld.Detach();
            PuppeteerWorld.Detach();
        }

        internal void UpdateClient(CDPSession client)
        {
            Client = client;
            MainWorld = new IsolatedWorld(
              Client,
              FrameManager,
              this,
              FrameManager.TimeoutSettings);

            PuppeteerWorld = new IsolatedWorld(
              Client,
              FrameManager,
              this,
              FrameManager.TimeoutSettings);
        }
    }
}
