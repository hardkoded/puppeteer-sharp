using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url}")]
    public class Target
    {
        #region Private members
        private TargetInfo _targetInfo;
        private readonly Func<Task<CDPSession>> _sessionFactory;
        private readonly TaskCompletionSource<bool> _initializedTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private Task<Worker> _workerTask;
        #endregion

        internal bool IsInitialized;

        internal Target(
            TargetInfo targetInfo,
            Func<Task<CDPSession>> sessionFactory,
            BrowserContext browserContext)
        {
            _targetInfo = targetInfo;
            _sessionFactory = sessionFactory;
            BrowserContext = browserContext;
            PageTask = null;

            _ = _initializedTaskWrapper.Task.ContinueWith(async initializedTask =>
            {
                var success = initializedTask.Result;
                if (!success)
                {
                    return;
                }

                var openerPageTask = Opener?.PageTask;
                if (openerPageTask == null || Type != TargetType.Page)
                {
                    return;
                }
                var openerPage = await openerPageTask.ConfigureAwait(false);
                if (!openerPage.HasPopupEventListeners)
                {
                    return;
                }
                var popupPage = await PageAsync().ConfigureAwait(false);
                openerPage.OnPopup(popupPage);
            });

            CloseTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            IsInitialized = _targetInfo.Type != TargetType.Page || _targetInfo.Url != string.Empty;

            if (IsInitialized)
            {
                _initializedTaskWrapper.TrySetResult(true);
            }
        }

        #region Properties
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url => _targetInfo.Url;
        /// <summary>
        /// Gets the type. It will be <see cref="TargetInfo.Type"/>.
        /// Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.
        /// </summary>
        /// <value>The type.</value>
        public TargetType Type => _targetInfo.Type;

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId => _targetInfo.TargetId;

        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        /// <remarks>
        /// Top-level targets return <c>null</c>.
        /// </remarks>
        public Target Opener => _targetInfo.OpenerId != null ?
            Browser.TargetsMap.GetValueOrDefault(_targetInfo.OpenerId) : null;

        /// <summary>
        /// Get the browser the target belongs to.
        /// </summary>
        public Browser Browser => BrowserContext.Browser;

        /// <summary>
        /// Get the browser context the target belongs to.
        /// </summary>
        public BrowserContext BrowserContext { get; }

        internal Task<bool> InitializedTask => _initializedTaskWrapper.Task;
        internal Task CloseTask => CloseTaskWrapper.Task;
        internal TaskCompletionSource<bool> CloseTaskWrapper { get; }
        internal Task<Page> PageTask { get; set; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Page"/>. If the target is not <c>"page"</c> or <c>"background_page"</c> returns <c>null</c>
        /// </summary>
        /// <returns>a task that returns a new <see cref="Page"/></returns>
        public Task<Page> PageAsync()
        {
            if ((_targetInfo.Type == TargetType.Page || _targetInfo.Type == TargetType.BackgroundPage) && PageTask == null)
            {
                PageTask = CreatePageAsync();
            }

            return PageTask ?? Task.FromResult<Page>(null);
        }

        /// <summary>
        /// If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.
        /// </summary>
        /// <returns>A task that returns a <see cref="Worker"/></returns>
        public Task<Worker> WorkerAsync()
        {
            if (_targetInfo.Type != TargetType.ServiceWorker && _targetInfo.Type != TargetType.SharedWorker)
            {
                return null;
            }
            if (_workerTask == null)
            {
                _workerTask = WorkerInternalAsync();
            }

            return _workerTask;
        }

        private async Task<Worker> WorkerInternalAsync()
        {
            var client = await _sessionFactory().ConfigureAwait(false);
            var targetAttachedWrapper = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            void MessageReceived(object sender, MessageEventArgs e)
            {
                if (e.MessageID == "Target.attachedToTarget")
                {
                    targetAttachedWrapper.TrySetResult(e.MessageData.ToObject<TargetAttachedToTargetResponse>(true).SessionId);
                    client.MessageReceived -= MessageReceived;
                }
            }
            client.MessageReceived += MessageReceived;

            await Task.WhenAll(
                targetAttachedWrapper.Task,
                client.SendAsync("Target.setAutoAttach", new TargetSetAutoAttachRequest
                {
                    AutoAttach = true,
                    WaitForDebuggerOnStart = false,
                    Flatten = true
                })).ConfigureAwait(false);
            var session = Connection.FromSession(client).GetSession(targetAttachedWrapper.Task.Result);
            return new Worker(
                session,
                _targetInfo.Url,
                (consoleType, handles, stackTrace) => Task.CompletedTask,
                (e) => { });
        }

        private async Task<Page> CreatePageAsync()
        {
            var session = await _sessionFactory().ConfigureAwait(false);
            return await Page.CreateAsync(session, this, Browser.IgnoreHTTPSErrors, Browser.DefaultViewport, Browser.ScreenshotTaskQueue).ConfigureAwait(false);
        }

        internal void TargetInfoChanged(TargetInfo targetInfo)
        {
            var previousUrl = _targetInfo.Url;
            _targetInfo = targetInfo;

            if (!IsInitialized && (_targetInfo.Type != TargetType.Page || _targetInfo.Url != string.Empty))
            {
                IsInitialized = true;
                _initializedTaskWrapper.TrySetResult(true);
                return;
            }

            if (previousUrl != targetInfo.Url)
            {
                Browser.ChangeTarget(this);
            }
        }

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="CDPSession"/></returns>
        public Task<CDPSession> CreateCDPSessionAsync() => Browser.Connection.CreateSessionAsync(_targetInfo);
    }
}