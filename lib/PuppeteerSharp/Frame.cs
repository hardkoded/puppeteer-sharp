using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using PuppeteerSharp.QueryHandlers;

namespace PuppeteerSharp
{
    /// <inheritdoc cref="PuppeteerSharp.IFrame" />
    public abstract class Frame : IFrame, IEnvironment
    {
        private Task<ElementHandle> _documentTask;

        /// <inheritdoc />
        public event EventHandler FrameSwappedByActivation;

        internal event EventHandler FrameDetached;

        internal event EventHandler<FrameNavigatedEventArgs> FrameNavigated;

        internal event EventHandler FrameNavigatedWithinDocument;

        internal event EventHandler LifecycleEvent;

        internal event EventHandler FrameSwapped;

        /// <inheritdoc/>
        public abstract IReadOnlyCollection<IFrame> ChildFrames { get; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public virtual string Url { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public virtual bool Detached { get; private set; }

        /// <inheritdoc/>
        public abstract IPage Page { get; }

        /// <inheritdoc/>
        IFrame IFrame.ParentFrame => ParentFrame;

        /// <inheritdoc/>
        public string Id { get; internal set; }

        /// <inheritdoc/>
        public abstract ICDPSession Client { get; protected set; }

        /// <inheritdoc/>
        Realm IEnvironment.MainRealm => MainRealm;

        internal string ParentId { get; init; }

        internal string LoaderId { get; private set; } = string.Empty;

        internal List<string> LifecycleEvents { get; } = new();

        internal virtual Realm MainRealm { get; set; }

        internal virtual Realm IsolatedRealm { get; set; }

        internal IsolatedWorld MainWorld => MainRealm as IsolatedWorld;

        internal IsolatedWorld PuppeteerWorld => IsolatedRealm as IsolatedWorld;

        internal bool HasStartedLoading { get; private set; }

        internal abstract Frame ParentFrame { get; }

        /// <summary>
        /// Logger.
        /// </summary>
        protected ILogger Logger { get; init; }

        /// <inheritdoc/>
        public abstract Task<IResponse> GoToAsync(string url, NavigationOptions options);

        /// <inheritdoc/>
        public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
            => GoToAsync(url, new NavigationOptions { Timeout = timeout, WaitUntil = waitUntil });

        /// <inheritdoc/>
        public abstract Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null);

        /// <inheritdoc/>
        public Task EvaluateExpressionAsync(string script) => MainRealm.EvaluateExpressionAsync(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script) => MainRealm.EvaluateExpressionAsync<T>(script);

        /// <inheritdoc/>
        public Task EvaluateFunctionAsync(string script, params object[] args) => MainRealm.EvaluateFunctionAsync(script, args);

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

            var (updatedSelector, queryHandler) = CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector(selector);
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

            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

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
        public Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions options = null)
            => GetDeviceRequestPromptManager().WaitForDevicePromptAsync(options);

        /// <inheritdoc/>
        public abstract Task<IElementHandle> AddStyleTagAsync(AddTagOptions options);

        /// <inheritdoc/>
        public async Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.Path) &&
                string.IsNullOrEmpty(options.Content))
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
        public Task<string> GetContentAsync(GetContentOptions options = null)
            => EvaluateFunctionAsync<string>(
                @"(replaceLoneSurrogates) => {
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

                    return replaceLoneSurrogates ? content.toWellFormed() : content;
                }",
                options?.ReplaceLoneSurrogates ?? false);

        /// <inheritdoc/>
        public abstract Task SetContentAsync(string html, NavigationOptions options = null);

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => IsolatedRealm.EvaluateExpressionAsync<string>("document.title");

        /// <inheritdoc/>
        public async Task ClickAsync(string selector, ClickOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);

            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

            await handle.ClickAsync(options).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task HoverAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);

            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

            await handle.HoverAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task FocusAsync(string selector)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);

            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

            await handle.FocusAsync().ConfigureAwait(false);
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
        public async Task TypeAsync(string selector, string text, TypeOptions options = null)
        {
            var handle = await QuerySelectorAsync(selector).ConfigureAwait(false);

            if (handle == null)
            {
                throw new SelectorException($"No node found for selector: {selector}", selector);
            }

            await handle.TypeAsync(text, options).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<ElementHandle> FrameElementAsync()
        {
            var parentFrame = ParentFrame;
            if (parentFrame == null)
            {
                return null;
            }

            var list = await parentFrame.IsolatedRealm.EvaluateFunctionHandleAsync(@"() => {
                return document.querySelectorAll('iframe, frame');
            }").ConfigureAwait(false);

            await foreach (var iframe in list.TransposeIterableHandleAsync().ConfigureAwait(false))
            {
                var frame = await iframe.ContentFrameAsync().ConfigureAwait(false);
                if (frame?.Id == Id)
                {
                    return iframe as ElementHandle;
                }

                try
                {
                    await iframe.DisposeAsync().ConfigureAwait(false);
                }
                catch
                {
                    Logger.LogWarning("FrameElementAsync: Error disposing iframe");
                }
            }

            return null;
        }

        internal void ClearDocumentHandle() => _documentTask = null;

        internal void OnLoadingStarted() => HasStartedLoading = true;

        internal void OnLoadingStopped()
        {
            LifecycleEvents.Add("DOMContentLoaded");
            LifecycleEvents.Add("load");
            LifecycleEvent?.Invoke(this, EventArgs.Empty);
        }

        internal void OnLifecycleEvent(string loaderId, string name)
        {
            if (name == "init")
            {
                LoaderId = loaderId;
                LifecycleEvents.Clear();
            }

            LifecycleEvents.Add(name);
            LifecycleEvent?.Invoke(this, EventArgs.Empty);
        }

        internal void Navigated(FramePayload framePayload)
        {
            Name = framePayload.Name ?? string.Empty;
            Url = framePayload.Url + framePayload.UrlFragment;
        }

        internal void OnFrameNavigated(FrameNavigatedEventArgs e) => FrameNavigated?.Invoke(this, e);

        internal void OnSwapped() => FrameSwapped?.Invoke(this, EventArgs.Empty);

        internal void NavigatedWithinDocument(string url)
        {
            Url = url;
            FrameNavigatedWithinDocument?.Invoke(this, EventArgs.Empty);
        }

        internal void Detach()
        {
            Detached = true;
            MainWorld.Detach();
            PuppeteerWorld.Detach();
            FrameDetached?.Invoke(this, EventArgs.Empty);
        }

        internal void OnFrameSwappedByActivation()
            => FrameSwappedByActivation?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Gets the prompts manager for the current client.
        /// </summary>
        /// <returns>The <see cref="DeviceRequestPromptManager"/>.</returns>
        protected internal abstract DeviceRequestPromptManager GetDeviceRequestPromptManager();

        private Task<ElementHandle> GetDocumentAsync()
        {
            if (_documentTask != null)
            {
                return _documentTask;
            }

            async Task<ElementHandle> EvaluateDocumentInContext()
            {
                var document = await IsolatedRealm.EvaluateFunctionHandleAsync("() => document").ConfigureAwait(false);

                if (document is not ElementHandle)
                {
                    throw new PuppeteerException("Document is null");
                }

                return await MainRealm.TransferHandleAsync(document).ConfigureAwait(false) as ElementHandle;
            }

            _documentTask = EvaluateDocumentInContext();

            return _documentTask;
        }
    }
}
