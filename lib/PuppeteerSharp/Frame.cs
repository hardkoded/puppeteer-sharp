using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class Frame : IFrame, IEnvironment
    {
        private Task<ElementHandle> _documentTask;

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

        /// <inheritdoc/>
        CDPSession IEnvironment.Client => Client;

        /// <inheritdoc/>
        Realm IEnvironment.MainRealm => MainRealm;

        internal CDPSession Client { get; private set; }

        internal string ParentId { get; }

        internal FrameManager FrameManager { get; }

        internal string LoaderId { get; set; }

        internal List<string> LifecycleEvents { get; } = new();

        internal Realm MainRealm { get; private set; }

        internal Realm IsolatedRealm { get; private set; }

        internal IsolatedWorld MainWorld => MainRealm as IsolatedWorld;

        internal IsolatedWorld PuppeteerWorld => IsolatedRealm as IsolatedWorld;

        internal bool HasStartedLoading { get; private set; }

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, NavigationOptions options) => FrameManager.NavigateFrameAsync(this, url, options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => GoToAsync(url, new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null) => FrameManager.WaitForFrameNavigationAsync(this, options);

        /// <inheritdoc/>
        public Task<JToken> EvaluateExpressionAsync(string script) => MainRealm.EvaluateExpressionAsync(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script) => MainRealm.EvaluateExpressionAsync<T>(script);

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args) => MainRealm.EvaluateFunctionAsync(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args) => MainRealm.EvaluateFunctionAsync<T>(script, args);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateExpressionHandleAsync(string script) => MainRealm.EvaluateExpressionHandleAsync(script);

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string function, params object[] args) => MainRealm.EvaluateFunctionHandleAsync(function, args);

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

            return MainRealm.WaitForFunctionAsync(script, options, args);
        }

        /// <inheritdoc/>
        public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return MainRealm.WaitForExpressionAsync(script, options);
        }

        /// <inheritdoc/>
        public async Task<string[]> SelectAsync(string selector, params string[] values)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            return await handle.SelectAsync(values).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.QuerySelectorAllHandleAsync(selector).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IElementHandle> QuerySelectorAsync(string selector)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.QuerySelectorAsync(selector).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.QuerySelectorAllAsync(selector).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IElementHandle[]> XPathAsync(string expression)
        {
            var document = await GetDocumentAsync().ConfigureAwait(false);
            return await document.XPathAsync(expression).ConfigureAwait(false);
        }

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

            var handle = await IsolatedRealm.EvaluateFunctionHandleAsync(
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

            return (await MainRealm.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
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

            var handle = await IsolatedRealm.EvaluateFunctionHandleAsync(
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

            return (await MainRealm.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
        }

        /// <inheritdoc/>
        public Task<string> GetContentAsync()
            => EvaluateFunctionAsync<string>(@"() => {
                let content = '';
                for (const node of document.childNodes) {
                    switch (node) {
                    case document.documentElement:
                        content += document.documentElement.outerHTML;
                        break;
                    default:
                        content += new XMLSerializer().serializeToString(node);
                        break;
                    }
                }

                return content;
            }");

        /// <inheritdoc/>
        public async Task SetContentAsync(string html, NavigationOptions options = null)
        {
            var waitUntil = options?.WaitUntil ?? new[] { WaitUntilNavigation.Load };
            var timeout = options?.Timeout ?? FrameManager.TimeoutSettings.NavigationTimeout;

            // We rely upon the fact that document.open() will reset frame lifecycle with "init"
            // lifecycle event. @see https://crrev.com/608658
            await IsolatedRealm.EvaluateFunctionAsync(
                @"html => {
                    document.open();
                    document.write(html);
                    document.close();
                }",
                html).ConfigureAwait(false);

            using var watcher = new LifecycleWatcher(FrameManager, this, waitUntil, timeout);
            var watcherTask = await Task.WhenAny(
                watcher.TimeoutOrTerminationTask,
                watcher.LifecycleTask).ConfigureAwait(false);

            await watcherTask.ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => IsolatedRealm.EvaluateExpressionAsync<string>("document.title");

        /// <inheritdoc/>
        public async Task ClickAsync(string selector, ClickOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            await handle.ClickAsync(options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task HoverAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            await handle.HoverAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task FocusAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            await handle.FocusAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task TypeAsync(string selector, string text, TypeOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);
            await handle.TypeAsync(text, options).ConfigureAwait(false);
        }

        internal void ClearContext() => _documentTask = null;

        internal Task<ElementHandle> GetDocumentAsync()
        {
            if (_documentTask != null)
            {
                return _documentTask;
            }

            async Task<ElementHandle> EvaluateDocumentInContext()
            {
                var document = await EvaluateFunctionHandleAsync("() => document").ConfigureAwait(false);

                if (document is not ElementHandle element)
                {
                    throw new PuppeteerException("Document is null");
                }

                return element;
            }

            _documentTask = EvaluateDocumentInContext();

            return _documentTask;
        }

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
