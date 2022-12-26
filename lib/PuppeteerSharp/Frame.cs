using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class Frame : IFrame
    {
        private readonly List<IFrame> _childFrames = new();

        internal Frame(FrameManager frameManager, Frame parentFrame, string frameId, CDPSession client)
        {
            FrameManager = frameManager;
            ParentFrame = parentFrame;
            Id = frameId;
            Client = client;

            LifecycleEvents = new List<string>();

            if (parentFrame != null)
            {
                parentFrame.AddChildFrame(this);
            }

            UpdateClient(client);
        }

        /// <inheritdoc/>
        public List<IFrame> ChildFrames
        {
            get
            {
                lock (_childFrames)
                {
                    return _childFrames.ToList();
                }
            }
        }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string Url { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public bool Detached { get; set; }

        /// <inheritdoc/>
        public IFrame ParentFrame { get; private set; }

        /// <inheritdoc/>
        public bool IsOopFrame => Client != FrameManager.Client;

        /// <inheritdoc/>
        public string Id { get; internal set; }

        internal FrameManager FrameManager { get; }

        internal string LoaderId { get; set; }

        internal List<string> LifecycleEvents { get; }

        internal string NavigationURL { get; private set; }

        internal DOMWorld MainWorld { get; private set; }

        internal DOMWorld SecondaryWorld { get; private set; }

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
            var handle = await SecondaryWorld.WaitForSelectorAsync(selector, options).ConfigureAwait(false);
            if (handle == null)
            {
                return null;
            }

            var mainExecutionContext = await MainWorld.GetExecutionContextAsync().ConfigureAwait(false);
            var result = await mainExecutionContext.AdoptElementHandleAsync(handle).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        public async Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
        {
            var handle = await SecondaryWorld.WaitForXPathAsync(xpath, options).ConfigureAwait(false);
            if (handle == null)
            {
                return null;
            }

            var mainExecutionContext = await MainWorld.GetExecutionContextAsync().ConfigureAwait(false);
            var result = await mainExecutionContext.AdoptElementHandleAsync(handle).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
            return result;
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
        public Task<string[]> SelectAsync(string selector, params string[] values) => SecondaryWorld.SelectAsync(selector, values);

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
        public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return MainWorld.AddStyleTagAsync(options);
        }

        /// <inheritdoc/>
        public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return MainWorld.AddScriptTagAsync(options);
        }

        /// <inheritdoc/>
        public Task<string> GetContentAsync() => SecondaryWorld.GetContentAsync();

        /// <inheritdoc/>
        public Task SetContentAsync(string html, NavigationOptions options = null)
            => SecondaryWorld.SetContentAsync(html, options);

        /// <inheritdoc/>
        public Task<string> GetTitleAsync() => SecondaryWorld.GetTitleAsync();

        /// <inheritdoc/>
        public Task ClickAsync(string selector, ClickOptions options = null)
            => SecondaryWorld.ClickAsync(selector, options);

        /// <inheritdoc/>
        public Task HoverAsync(string selector) => SecondaryWorld.HoverAsync(selector);

        /// <inheritdoc/>
        public Task FocusAsync(string selector) => SecondaryWorld.FocusAsync(selector);

        /// <inheritdoc/>
        public Task TypeAsync(string selector, string text, TypeOptions options = null)
             => SecondaryWorld.TypeAsync(selector, text, options);

        internal void AddChildFrame(Frame frame)
        {
            lock (_childFrames)
            {
                _childFrames.Add(frame);
            }
        }

        internal void RemoveChildFrame(Frame frame)
        {
            lock (_childFrames)
            {
                _childFrames.Remove(frame);
            }
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
            NavigationURL = framePayload.Url + framePayload.UrlFragment;
            Url = framePayload.Url + framePayload.UrlFragment;
        }

        internal void NavigatedWithinDocument(string url) => Url = url;

        internal void Detach()
        {
            Detached = true;
            MainWorld.Detach();
            SecondaryWorld.Detach();
            if (ParentFrame != null)
            {
                ((Frame)ParentFrame).RemoveChildFrame(this);
            }

            ParentFrame = null;
        }

        internal void UpdateClient(CDPSession client)
        {
            Client = client;
            MainWorld = new DOMWorld(
              Client,
              FrameManager,
              this,
              FrameManager.TimeoutSettings);

            SecondaryWorld = new DOMWorld(
              Client,
              FrameManager,
              this,
              FrameManager.TimeoutSettings);
        }
    }
}
